using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLyHS1.Data;
using QLyHS1.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace QLyHS1.Controllers
{
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


                            select new TeacherViewModel
                            {
                                Id = te.Id,
                                Name = te.Name,
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
                           

                            select new TeacherViewModel
                            {
                                Id = te.Id,
                                Name = te.Name,
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
                        // Log or inspect the error messages
                        Console.WriteLine(error.ErrorMessage);
                    }
                }
                return View(model);
            }
            var teacher = new Teacher
            {
                
                Name = model.Name,
                UserName = model.UserName,
                Password = model.Password,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                Phone = model.Phone,
                Address = model.Address,
                Token = "",
                Role = false,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                Status = true
            };

            _context.Teachers.Add(teacher);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Details(int? id)
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
            var student = await _context.Teachers.FindAsync(id);
            if (student != null)
            {
                _context.Teachers.Remove(student);
            }

            await _context.SaveChangesAsync();
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
