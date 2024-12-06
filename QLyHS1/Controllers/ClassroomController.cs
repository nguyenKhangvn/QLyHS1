using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using QLyHS1.Data;
using QLyHS1.Models;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace QLyHS1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ClassroomController : Controller
    {
        private readonly QlyHs1Context _context;

        public ClassroomController(QlyHs1Context context)
        {
            _context = context;
        }
        public IActionResult Index(String? teacherName)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "User");
            }

            ViewBag.GrandLevel = _context.GrandLevels
               .Select(c => new SelectListItem
               {
                   Value = c.Name,
                   Text = c.Name
               }).ToList();

            if (role == "Admin")
            {
                ViewBag.Classrooms = _context.Classrooms
                            .Select(c => new SelectListItem
                            {
                                Value = c.Name,
                                Text = c.Name
                            }).ToList();

                var schedules = from cl in _context.Classrooms
                                join t in _context.Teachers on cl.TeacherId equals t.Id
                                join gl in _context.GrandLevels on cl.GrandLevelId equals gl.Id
                                where string.IsNullOrEmpty(teacherName) || t.Name == teacherName
                                select new ClassroomViewModel
                                {
                                    Id = cl.Id,
                                    Name = cl.Name,
                                    Quantity = cl.Quantity,
                                    GrandLevelName = gl.Name,
                                    TeacherName = t.Name,

                                };

                return View(schedules.ToList());
            } else
            {
                ViewBag.Classrooms = _context.Classrooms
                            .Where(t => t.TeacherId == userId)
                           .Select(c => new SelectListItem
                           {
                               Value = c.Name,
                               Text = c.Name
                           }).ToList();

                var schedules = from cl in _context.Classrooms
                                join t in _context.Teachers on cl.TeacherId equals t.Id
                                join gl in _context.GrandLevels on cl.GrandLevelId equals gl.Id
                                where cl.TeacherId == userId && string.IsNullOrEmpty(teacherName) || gl.Name == teacherName
                                select new ClassroomViewModel
                                {
                                    Id = cl.Id,
                                    Name = cl.Name,
                                    Quantity = cl.Quantity,
                                    GrandLevelName = gl.Name,
                                    TeacherName = t.Name,

                                };

                return View(schedules.ToList());
            }
        }

        [Route("Classroom/Search")]
        public IActionResult Search(string? teacherName)
        {
            var stu = _context.Classrooms.AsQueryable();
            if (teacherName == null)
            {
                return NotFound();
            }

            var students = _context.Classrooms
                .Include(t => t.Teacher)
                .Include(gl => gl.GrandLevel)   
                .Where(t => t.Name.Contains(teacherName))
                .Select(cl => new ClassroomViewModel
                {
                    Id = cl.Id,
                    Name = cl.Name,
                    Quantity = cl.Quantity,
                    GrandLevelName = cl.GrandLevel.Name,
                    TeacherName = cl.Teacher.Name,
                }).ToList();

            if (!students.Any())
            {
                return NotFound();
            }

            return View(students);
        }
        [HttpGet]
        public IActionResult Add()
        {
            var GrandLevel = _context.GrandLevels
                             .Select(s => new { s.Id, s.Name })
                             .ToList();

            ViewBag.GrandLevel = new SelectList(GrandLevel, "Id", "Name");

            var Teacher = _context.Teachers
                             .Where(s => s.Status == true)
                             .Select(s => new { s.Id, s.Name })
                             .ToList();

            ViewBag.Teacher = new SelectList(Teacher, "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Add(ClassroomDetailViewModel model)
        {
            var GrandLevel = _context.GrandLevels
                           .Select(s => new { s.Id, s.Name })
                           .ToList();

            ViewBag.GrandLevel = new SelectList(GrandLevel, "Id", "Name");

            var Teacher = _context.Teachers
                             .Where(s => s.Status == true)
                             .Select(s => new { s.Id, s.Name })
                             .ToList();

            ViewBag.Teacher = new SelectList(Teacher, "Id", "Name");

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (!_context.Teachers.Any(t => t.Id == model.TeacherID) ||
                !_context.GrandLevels.Any(gl => gl.Id == model.GrandLevelID))
            {
                ModelState.AddModelError("", "Giáo viên hoặc cấp lớp không hợp lệ.");
                return View(model);
            }
            var classroom = new Classroom
            {
                TeacherId = model.TeacherID,
                GrandLevelId = model.GrandLevelID,
                Name = model.Name,
                Room = model.Room,
                Quantity = model.Quantity,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                Status = true
            };

            _context.Classrooms.Add(classroom);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }



        // GET: Student/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Classrooms.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var studentViewModel = new ClassroomDetailToEditViewModel
            {
                Id = student.Id,
                TeacherID = student.TeacherId,
                GrandLevelID = student.GrandLevelId,
                Name = student.Name,
                Room = student.Room,
                Quantity = student.Quantity,
                Status = student.Status,
                CreateAt = student.CreateAt,
                UpdateAt = student.UpdateAt
            };

            ViewData["TeacherId"] = new SelectList(_context.Teachers, "Id", "Name", student.TeacherId);
            ViewData["GrandLevelId"] = new SelectList(_context.GrandLevels, "Id", "Name", student.GrandLevelId);
            return View(studentViewModel);
        }


        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TeacherID,GrandLevelID,Name,Room,Quantity,Status")] ClassroomDetailToEditViewModel studentViewModel)
        {
            if (id != studentViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var student = await _context.Classrooms.FindAsync(id);
                    if (student == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các thuộc tính cần thiết
                    student.TeacherId = studentViewModel.TeacherID;
                    student.GrandLevelId = studentViewModel.GrandLevelID;
                    student.Name = studentViewModel.Name;
                    student.Room = studentViewModel.Room;
                    student.Quantity = studentViewModel.Quantity;
                    student.Status = studentViewModel.Status;
                    student.CreateAt = studentViewModel.CreateAt;
                    student.UpdateAt = DateTime.Now;

                    _context.Update(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewData["TeacherID"] = new SelectList(_context.Teachers, "Id", "Name", studentViewModel.TeacherID);
            ViewData["GrandLevelID"] = new SelectList(_context.GrandLevels, "Id", "Name", studentViewModel.GrandLevelID);
            return View(studentViewModel);
        }



        private bool StudentExists(int id)
        {
            return _context.Classrooms.Any(e => e.Id == id);
        }

        //detail
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var classroms = await _context.Classrooms
                .Include(t => t.Teacher)
                .Include(gl => gl.GrandLevel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (classroms == null)
            {
                return NotFound();
            }

            return View(classroms);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Classrooms
                 .Include(t => t.Teacher)
                .Include(gl => gl.GrandLevel)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Classrooms.FindAsync(id);
            if (student != null)
            {
                _context.Classrooms.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
