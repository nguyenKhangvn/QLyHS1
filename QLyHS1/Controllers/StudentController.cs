using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLyHS1.Models;
using QLyHS1.Data;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace QLyHS1.Controllers
{
    public class StudentController : Controller
    {
        private readonly QlyHs1Context _context;

        public StudentController(QlyHs1Context context)
        {
            _context = context;
        }

        public IActionResult Index(string? className)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "User");
            }

            if (role == "Admin")
            {
                ViewBag.Classrooms = _context.Classrooms
                .Select(c => new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name
                }).ToList();


                var studentVM = from st in _context.Students
                                join cl in _context.Classrooms on st.ClassId equals cl.Id
                                where (string.IsNullOrEmpty(className) || cl.Name == className)
                                select new StudentViewModel
                                {
                                    Id = st.Id,
                                    Name = st.Name,
                                    Gender = st.Gender,
                                    Birthday = st.DateOfBirth.ToString("dd/MM/yyyy"),
                                    ClassName = cl.Name,
                                    Address = st.Address,
                                    ParentPhone = st.PhoneParent,
                                    Conduct = st.Conduct ?? "Không có thông tin",
                                    Status = st.Status ? "Còn học" : "Nghỉ học"
                                };

                return View(studentVM.ToList());
            }
            else
            {
                ViewBag.Classrooms = _context.Classrooms
                .Where(c => c.TeacherId == userId)
                .Select(c => new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name
                }).ToList();


                var studentVM = from st in _context.Students
                                join cl in _context.Classrooms on st.ClassId equals cl.Id
                                where cl.TeacherId == userId &&
                                      (string.IsNullOrEmpty(className) || cl.Name == className)
                                select new StudentViewModel
                                {
                                    Id = st.Id,
                                    Name = st.Name,
                                    Gender = st.Gender,
                                    Birthday = st.DateOfBirth.ToString("dd/MM/yyyy"),
                                    ClassName = cl.Name,
                                    Address = st.Address,
                                    ParentPhone = st.PhoneParent,
                                    Conduct = st.Conduct ?? "Không có thông tin",
                                    Status = st.Status ? "Còn học" : "Nghỉ học"
                                };

                return View(studentVM.ToList());
            }
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
                    ParentPhone = student.PhoneParent,
                    Conduct = student.Conduct ?? "Không có thông tin",
                    Status = student.Status ? "Còn học" : "Nghỉ học"
                }).ToList();          
            return View(students);
        }

        public IActionResult Add()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "User");
            }

            if (role == "Admin")
            {
                var classroom = _context.Classrooms
                            .Where(s => s.Status == true)
                            .Select(s => new { s.Id, s.Name })
                            .ToList();

                ViewBag.Classroom = new SelectList(classroom, "Id", "Name");
                return View();
            }
            else
            {
                var classroom = _context.Classrooms
                            .Where(s => s.Status == true && s.TeacherId == userId)
                            .Select(s => new { s.Id, s.Name })
                            .ToList();

                ViewBag.Classroom = new SelectList(classroom, "Id", "Name");
                return View();
            }

           
        }

        [HttpPost]
        public IActionResult Add(StudentDetailViewModel model)
        {
            var classroom = _context.Classrooms
                             .Where(s => s.Status == true)
                             .Select(s => new { s.Id, s.Name })
                             .ToList();

            ViewBag.Classroom = new SelectList(classroom, "Id", "Name");

            if (!ModelState.IsValid)
            {
                return View(model);
            }
           

            if (!_context.Classrooms.Any(t => t.Id == model.ClassID))
            {
                ModelState.AddModelError("", "Lớp học không tồn tại.");
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
                Conduct = model.Conduct,
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
                Conduct = student.Conduct,
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClassId,Name,Gender,Email,DateOfBirth,Phone,PhoneParent,Address,Status,Conduct")] StudentDetailToEditViewModel studentViewModel)
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
                    student.Conduct = studentViewModel.Conduct ?? "Không có thông tin";
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

        [HttpGet]
        public IActionResult ExportToExcel(string? className)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "User");
            }

            // Lấy dữ liệu
            IQueryable<StudentViewModel> studentVM;

            if (role == "Admin")
            {
                studentVM = from st in _context.Students
                            join cl in _context.Classrooms on st.ClassId equals cl.Id
                            where (string.IsNullOrEmpty(className) || cl.Name == className)
                            select new StudentViewModel
                            {
                                Id = st.Id,
                                Name = st.Name,
                                Gender = st.Gender,
                                Birthday = st.DateOfBirth.ToString("dd/MM/yyyy"),
                                ClassName = cl.Name,
                                Address = st.Address,
                                ParentPhone = st.PhoneParent,
                                Conduct = st.Conduct ?? "Không có thông tin"
                            };
            }
            else
            {
                studentVM = from st in _context.Students
                            join cl in _context.Classrooms on st.ClassId equals cl.Id
                            where cl.TeacherId == userId &&
                                  (string.IsNullOrEmpty(className) || cl.Name == className)
                            select new StudentViewModel
                            {
                                Id = st.Id,
                                Name = st.Name,
                                Gender = st.Gender,
                                Birthday = st.DateOfBirth.ToString("dd/MM/yyyy"),
                                ClassName = cl.Name,
                                Address = st.Address,
                                ParentPhone = st.PhoneParent,
                                Conduct = st.Conduct ?? "Không có thông tin"
                            };
            }

            var data = studentVM.ToList();

            
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Students");
                worksheet.Cells[1, 1].Value = "Trường: Đại học Quy Nhơn";
                worksheet.Cells[2, 1].Value = $"Danh sách lớp: {(string.IsNullOrEmpty(className) ? "Tất cả" : className)}";

                int headerCell = 4;
                worksheet.Cells[headerCell, 1].Value = "ID";
                worksheet.Cells[headerCell, 2].Value = "Họ và tên";
                worksheet.Cells[headerCell, 3].Value = "Giới tính";
                worksheet.Cells[headerCell, 4].Value = "Sinh nhật";
                worksheet.Cells[headerCell, 5].Value = "Lớp";
                worksheet.Cells[headerCell, 6].Value = "Địa chỉ";
                worksheet.Cells[headerCell, 7].Value = "Liên hệ phụ huynh";
                worksheet.Cells[headerCell, 8].Value = "Hạnh kiểm";


                int row = headerCell + 1;
                foreach (var student in data)
                {
                    worksheet.Cells[row, 1].Value = student.Id;
                    worksheet.Cells[row, 2].Value = student.Name;
                    worksheet.Cells[row, 3].Value = student.Gender;
                    worksheet.Cells[row, 4].Value = student.Birthday;
                    worksheet.Cells[row, 5].Value = student.ClassName;
                    worksheet.Cells[row, 6].Value = student.Address;
                    worksheet.Cells[row, 7].Value = student.ParentPhone;
                    worksheet.Cells[row, 8].Value = student.Conduct ?? "Không có thông tin";
                    row++;
                }
                
                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = "Students.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }
        }

    }
}
