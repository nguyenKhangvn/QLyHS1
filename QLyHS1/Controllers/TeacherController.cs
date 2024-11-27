using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLyHS1.Data;
using QLyHS1.Models;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace QLyHS1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TeacherController : Controller
    {
        private readonly QlyHs1Context _context;

        public TeacherController(QlyHs1Context context)
        {
            _context = context;
        }

        public IActionResult Index(string searchString)
        {
            var teacherVM = from te in _context.Teachers
                            join ass in _context.Assignments on te.Id equals ass.TeacherId into assGroup
                            from ass in assGroup.DefaultIfEmpty()
                            join su in _context.Subjects on ass.SubjectId equals su.Id into suGroup
                            from su in suGroup.DefaultIfEmpty()
                            select new TeacherViewModel
                            {
                                Id = te.Id,
                                Name = te.Name,
                                SubjectName = su != null ? su.Name : "N/A",
                                Email = te.Email,
                                Address = te.Address,
                                DateOfBirth = te.DateOfBirth.ToString("dd/MM/yyyy"),
                                Phone = te.Phone
                            };



            if (!string.IsNullOrEmpty(searchString))
            {
                teacherVM = teacherVM.Where(s => s.Name.Contains(searchString));
            }

            return View(teacherVM.ToList());
        }

        [Route("Teacher/Search")]
        public IActionResult Search(string? query)
        {
            var stu = _context.Teachers.AsQueryable();
            var teacherVM = from te in _context.Teachers
                            join ass in _context.Assignments on te.Id equals ass.TeacherId into assGroup
                            from ass in assGroup.DefaultIfEmpty()
                            join su in _context.Subjects on ass.SubjectId equals su.Id into suGroup
                            from su in suGroup.DefaultIfEmpty()
                            select new TeacherViewModel
                            {
                                Id = te.Id,
                                Name = te.Name,
                                SubjectName = su != null ? su.Name : "N/A", 
                                Email = te.Email,
                                Address = te.Address,
                                DateOfBirth = te.DateOfBirth.ToString("dd/MM/yyyy"),
                                Phone = te.Phone
                            };


            if (!string.IsNullOrEmpty(query))
            {
                teacherVM = teacherVM.Where(s => s.Name.Contains(query));
            }

            return View(teacherVM.ToList());
        }


        public IActionResult Add()
        {
            var subjects = _context.Subjects
                              .Where(s => s.Status)
                              .Select(s => new { s.Id, s.Name })
                              .ToList();

            ViewBag.Subjects = new SelectList(subjects, "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult Add(TeacherDetailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                
                {
                    foreach (var error in state.Value.Errors)
                    {     
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
                return View(model);
            }
            if (ModelState.IsValid)
            {
                var teacher = new Teacher
                {
                    Name = model.Name,
                    UserName = model.UserName,
                    Password = model.Password,
                    Email = model.Email,
                    DateOfBirth = model.DateOfBirth,
                    Phone = model.Phone,
                    Address = model.Address,
                    Token = Util.GenerateRamdomKey(),
                    Role = false,
                    CreateAt = DateTime.Now,
                    UpdateAt = DateTime.Now,
                    Status = true
                };

                _context.Teachers.Add(teacher);
                _context.SaveChanges();

                int teacherId = teacher.Id;
                var assignment = new Assignment
                {
                    TeacherId = teacherId,
                    SubjectId = model.SubjectId,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddMonths(6) 
                };

                _context.Assignments.Add(assignment);
                _context.SaveChanges();

                var subjects = _context.Subjects
                       .Where(s => s.Status)
                       .Select(s => new { s.Id, s.Name })
                       .ToList();
                ViewBag.Subjects = new SelectList(subjects, "Id", "Name");
                /*return View(model);*/

                return RedirectToAction("Index");
            }


            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int? id)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;  

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "User");
            }
            if (id == null)
            {
                return NotFound();
            }

            var subject = (from sub in _context.Subjects
                           join sch in _context.Assignments on sub.Id equals sch.SubjectId
                           join tea in _context.Teachers on sch.TeacherId equals tea.Id
                           where tea.Id == userId
                           select sub).FirstOrDefault();

            ViewBag.SubjectName = subject?.Name ?? "Chưa có";

            var student = await _context.Teachers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Teachers
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
            try
            {
                var teacher = await _context.Teachers.FindAsync(id);
                if (teacher != null)
                {
                    _context.Teachers.Remove(teacher);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Xóa giáo viên thành công!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Không tìm thấy giáo viên cần xóa.";
                }
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = "Không thể xóa giáo viên. Giáo viên này có dữ liệu liên quan trong hệ thống.";
                Console.WriteLine(ex.Message);
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: Teacher/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            var subjects = _context.Subjects
                             .Where(s => s.Status)
                             .Select(s => new { s.Id, s.Name })
                             .ToList();

            ViewBag.Subjects = new SelectList(subjects, "Id", "Name");
            // Map data from `Teacher` to `TeacherDetailViewModel`
            var teacherViewModel = new TeacherDetailViewModel
            {
                Id = teacher.Id,
                Name = teacher.Name,
                UserName = teacher.UserName,
                Password = teacher.Password,
                Email = teacher.Email,
                DateOfBirth = teacher.DateOfBirth,
                Phone = teacher.Phone,
                Address = teacher.Address,
                Role = teacher.Role,
                CreateAt = teacher.CreateAt,
                UpdateAt = teacher.UpdateAt,
                Status = teacher.Status
            };

            return View(teacherViewModel);
        }

        // POST: Teacher/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,UserName,Password,Email,DateOfBirth,Phone,Address,Token,Role,Status")] TeacherDetailViewModel teacherViewModel)
        {
            if (id != teacherViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var teacher = await _context.Teachers.FindAsync(id);
                    if (teacher == null)
                    {
                        return NotFound();
                    }

                    // Update the necessary properties
                    teacher.Name = teacherViewModel.Name;
                    teacher.UserName = teacherViewModel.UserName;
                    teacher.Password = teacherViewModel.Password;
                    teacher.Email = teacherViewModel.Email;
                    teacher.DateOfBirth = teacherViewModel.DateOfBirth;
                    teacher.Phone = teacherViewModel.Phone;
                    teacher.Address = teacherViewModel.Address;
                    teacher.Role = teacherViewModel.Role;
                    teacher.Status = teacherViewModel.Status;
                    teacher.UpdateAt = DateTime.Now;

                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(teacherViewModel);
        }

        private bool TeacherExists(int id)
        {
            return _context.Teachers.Any(e => e.Id == id);
        }

    }
}
