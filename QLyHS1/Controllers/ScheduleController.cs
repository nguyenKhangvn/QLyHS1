using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLyHS1.Data;
using QLyHS1.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace QLyHS1.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly QlyHs1Context _context;

        public ScheduleController(QlyHs1Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var schedules = from sch in _context.Schedules
                            join s in _context.Subjects on sch.SubjectId equals s.Id
                            select new ScheduleViewModel
                            {
                                Id = sch.Id,
                                SubjectName = s.Name,
                                ClassRoom = sch.ClassRoom,
                                DayOfWeek = sch.DayOfWeek,
                                StartTime = sch.StartTime,
                                EndTime = sch.EndTime
                            };

            return View(schedules.ToList());
        }

        // Search schedules based on subject name or class room
        [HttpGet]
        public async Task<IActionResult> Search(string? query)
        {
            var schedules = _context.Schedules
                .Include(s => s.Subject)
                .Where(s => s.Subject.Name.Contains(query) || s.ClassRoom.Contains(query))
                .Select(s => new ScheduleViewModel
                {
                    Id = s.Id,
                    SubjectName = s.Subject.Name,
                    ClassRoom = s.ClassRoom,
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                });

            if (!schedules.Any())
            {
                return NotFound();
            }

            return View(await schedules.ToListAsync());
        }

        // Display the add schedule form
        public IActionResult Add()
        {
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name");
            return View();
        }

        // Add a new schedule
        [HttpPost]
        public async Task<IActionResult> Add(ScheduleDetailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var schedule = new Schedule
                {
                    SubjectId = model.SubjectId,
                    ClassRoom = model.ClassRoom,
                    DayOfWeek = model.DayOfWeek,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime
                };

                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", model.SubjectId);
            return View(model);
        }

        // Display edit form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }

            var model = new ScheduleDetailViewModel
            {
                Id = schedule.Id,
                SubjectId = schedule.SubjectId,
                ClassRoom = schedule.ClassRoom,
                DayOfWeek = schedule.DayOfWeek,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime
            };

            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", schedule.SubjectId);
            return View(model);
        }

        // Edit a schedule
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ScheduleDetailViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var schedule = await _context.Schedules.FindAsync(id);
                if (schedule == null)
                {
                    return NotFound();
                }

                schedule.SubjectId = model.SubjectId;
                schedule.ClassRoom = model.ClassRoom;
                schedule.DayOfWeek = model.DayOfWeek;
                schedule.StartTime = model.StartTime;
                schedule.EndTime = model.EndTime;

                _context.Update(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", model.SubjectId);
            return View(model);
        }

        // Delete a schedule
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.Subject)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
