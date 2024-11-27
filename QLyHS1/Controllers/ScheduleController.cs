using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLyHS1.Data;
using QLyHS1.Models;
using System;
using System.Linq;
using System.Security.Claims;
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

        public async Task<IActionResult> Index(string? Semester)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "User");
            }

           /* ViewBag.Classrooms = _context.Classrooms
               .Where(c => c.TeacherId == userId)
               .Select(c => new SelectListItem
               {
                   Value = c.Name,
                   Text = c.Name
               }).ToList();

            ViewBag.Semester = _context.Semesters
             .Select(c => new SelectListItem
             {
                 Value = c.Name,
                 Text = c.Name
             }).ToList();*/

            if (role == "Admin")
            {
                var schedules = from sch in _context.Schedules
                                join s in _context.Subjects on sch.TeacherId equals s.Id
                                join t in _context.Teachers on sch.SubjectId equals t.Id
                                select new ScheduleViewModel
                                {
                                    Id = sch.Id,
                                    SubjectName = s.Name,
                                    TeacherName = t.Name,
                                   /* ClassRoom = sch.ClassRoom,*/
                                    DayOfWeek = sch.DayOfWeek,
                                    Infomation = sch.Infomation ?? "Không có thông tin",
                                    StartTime = sch.StartTime,
                                    EndTime = sch.EndTime
                                };

                return View(schedules.ToList());
            }
            else
            {
                var schedules = from sch in _context.Schedules
                                join t in _context.Teachers on sch.TeacherId equals t.Id
                                join s in _context.Subjects on sch.SubjectId equals s.Id
                                where (sch.TeacherId == userId)
                                select new ScheduleViewModel
                                {
                                    Id = sch.Id,
                                    SubjectName = s.Name,
                                    TeacherName = t.Name,
                                   /* ClassRoom = sch.ClassRoom,*/
                                    DayOfWeek = sch.DayOfWeek,
                                    Infomation = sch.Infomation ?? "Không có thông tin",
                                    StartTime = sch.StartTime,
                                    EndTime = sch.EndTime
                                };

                return View(schedules.ToList());
            }  
        }


        [HttpGet]
        public async Task<IActionResult> Search(string? query)
        {
            var schedules = _context.Schedules
                .Include(s => s.Subject)
                 .Include(s => s.Teacher)
                .Where(s => s.Subject.Name.Contains(query) || s.ClassRoom.Contains(query))
                .Select(s => new ScheduleViewModel
                {
                    Id = s.Id,
                    SubjectName = s.Subject.Name,
                    TeacherName = s.Teacher.Name,
                  /*  ClassRoom = s.ClassRoom,*/
                    DayOfWeek = s.DayOfWeek,
                    Infomation = s.Infomation ?? "Không có thông tin",
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                });

            if (!schedules.Any())
            {
                return NotFound();
            }

            return View(await schedules.ToListAsync());
        }


        public IActionResult Add()
        {
            var Subject = _context.Subjects
                            .Where(s => s.Status == true)
                            .Select(s => new { s.Id, s.Name })
                            .ToList();

            ViewBag.Subject = new SelectList(Subject, "Id", "Name");

            var Teacher = _context.Teachers
                           .Where(s => s.Status == true)
                           .Select(s => new { s.Id, s.Name })
                           .ToList();

            ViewBag.Teacher = new SelectList(Teacher, "Id", "Name");
            //ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name");
            return View();
        }

        // Add a new schedule
        [HttpPost]
        public IActionResult Add(ScheduleDetailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var schedule = new Schedule
            {
                SubjectId = model.SubjectId,
                TeacherId = model.TeacherId,
                ClassRoom = model.ClassRoom,
                DayOfWeek = model.DayOfWeek,
                Infomation = model.Infomation,
                StartTime = model.StartTime,
                EndTime = model.EndTime
            };

            _context.Schedules.Add(schedule);
            _context.SaveChanges();

            return RedirectToAction("Index");
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

            var model = new ScheduleToEditViewModel
            {
                Id = schedule.Id,
                SubjectId = schedule.SubjectId,
                TeacherId = schedule.TeacherId,
                ClassRoom = schedule.ClassRoom,
                DayOfWeek = schedule.DayOfWeek,
                Infomation = schedule.Infomation,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime
            };

            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", schedule.SubjectId);

            ViewData["TeacherId"] = new SelectList(_context.Teachers, "Id", "Name", schedule.TeacherId);
            return View(model);
        }

        // Edit a schedule
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ScheduleToEditViewModel model)
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
                schedule.TeacherId = model.TeacherId;
               /* schedule.ClassRoom = model.ClassRoom;*/
                schedule.DayOfWeek = model.DayOfWeek;
                schedule.Infomation = model.Infomation;
                schedule.StartTime = model.StartTime;
                schedule.EndTime = model.EndTime;

                _context.Update(schedule);
                await _context.SaveChangesAsync();

                ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", schedule.SubjectId);

                ViewData["TeacherId"] = new SelectList(_context.Teachers, "Id", "Name", schedule.TeacherId);
                return RedirectToAction(nameof(Index));
            }
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
