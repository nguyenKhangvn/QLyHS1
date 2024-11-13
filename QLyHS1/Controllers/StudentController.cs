using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLyHS1.Models;
using QLyHS1.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace QLyHS1.Controllers
{
    public class StudentController : Controller
    {
        private readonly QlyHs1Context _context;

        public StudentController(QlyHs1Context context)
        {
            _context = context;
        }

        // Danh sách học sinh và tìm kiếm
        public IActionResult Index()
        {
            var studentVM = from st in _context.Students
                            join cl in _context.Classrooms on st.ClassId equals cl.Id
                            select new StudentViewModel
                            {
                                Id = st.Id,
                                Name = st.Name,
                                Gender = st.Gender,
                                Birthday = st.DateOfBirth.ToString("dd/MM/yyyy"),
                                ClassName = cl.Name,
                                Address = st.Address,
                                ParentPhone = st.PhoneParent
                            };

            return View(studentVM.ToList());
        }


        // Xem chi tiết học sinh
        [Route("Student/Search")]
        public IActionResult Search(string? query)
        {
            var students = _context.Students
                .Include(s => s.Class)
                .Where(m => m.Name.Contains(query))
                .Select(student => new StudentViewModel
                {
                    Id = student.Id,
                    Name = student.Name,
                    Gender = student.Gender,
                    Birthday = student.DateOfBirth.ToString("dd/MM/yyyy"),
                    ClassName = student.Class.Name,
                    Address = student.Address,
                    ParentPhone = student.PhoneParent
                }).ToList();

            if (!students.Any())
            {
                return Index();
            }
            return View(students);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(StudentDetailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var student = new Student
            {
                ClassId = model.ClassID,
                Name = model.Name,
                Gender = model.Gender,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                Phone = model.Phone,
                PhoneParent = model.PhoneParent,
                Address = model.Address,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                Status = true
            };

            _context.Students.Add(student);
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

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            // Ánh xạ dữ liệu từ `Student` sang `StudentDetailToEditViewModel`
            var studentViewModel = new StudentDetailToEditViewModel
            {
                Id = student.Id,
                ClassId = student.ClassId,
                Name = student.Name,
                Gender = student.Gender,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth,
                Phone = student.Phone,
                PhoneParent = student.PhoneParent,
                Address = student.Address,
                Status = student.Status,
                CreateAt = student.CreateAt,
                UpdateAt = student.UpdateAt
            };

            ViewData["ClassId"] = new SelectList(_context.Classrooms, "Id", "Name", student.ClassId);
            return View(studentViewModel);
        }


        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClassId,Name,Gender,Email,DateOfBirth,Phone,PhoneParent,Address,Status")] StudentDetailToEditViewModel studentViewModel)
        {
            if (id != studentViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var student = await _context.Students.FindAsync(id);
                    if (student == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các thuộc tính cần thiết
                    student.ClassId = studentViewModel.ClassId;
                    student.Name = studentViewModel.Name;
                    student.Gender = studentViewModel.Gender;
                    student.Email = studentViewModel.Email;
                    student.DateOfBirth = studentViewModel.DateOfBirth;
                    student.Phone = studentViewModel.Phone;
                    student.PhoneParent = studentViewModel.PhoneParent;
                    student.Address = studentViewModel.Address;
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

            ViewData["ClassId"] = new SelectList(_context.Classrooms, "Id", "Name", studentViewModel.ClassId);
            return View(studentViewModel);
        }



        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }

        //detail
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Class)
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

            var student = await _context.Students
                .Include(s => s.Class)
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
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
