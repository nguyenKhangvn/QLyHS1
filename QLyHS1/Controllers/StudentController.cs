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
                                orderby st.Name ascending
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
                Conduct = "Chưa có",
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

            try
            {
               
                ViewBag.GenderOptions = new List<SelectListItem>
                {
                    new SelectListItem { Value = "true", Text = "Nam" },
                    new SelectListItem { Value = "false", Text = "Nữ" }
                };

                 var student = await _context.Students
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.ClassId,
                    Name = s.Name ?? "N/A",
                    s.Gender,
                    Email = s.Email ?? "N/A",
                    DateOfBirth = s.DateOfBirth == DateTime.MinValue ? DateTime.Now : s.DateOfBirth,
                    Phone = s.Phone ?? "N/A",
                    PhoneParent = s.PhoneParent ?? "N/A",
                    Address = s.Address ?? "N/A",
                    Conduct = s.Conduct ?? "N/A",
                    s.Status,
                    s.CreateAt,
                    s.UpdateAt
                })
                .FirstOrDefaultAsync();

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
            catch (Exception ex)
            {
                
                Console.WriteLine(ex.Message);
            }
            return View();
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
                     var student = await _context.Students
                         .Where(s => s.Id == id)
                         .FirstOrDefaultAsync();


                    if (student == null)
                    {
                        return NotFound();
                    }


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
                                ClassName = cl.Name,
                                Birthday = st.DateOfBirth.ToString("dd/MM/yyyy"),
                               
                    /*            Address = st.Address,
                                ParentPhone = st.PhoneParent,
                                Conduct = st.Conduct ?? "Không có thông tin"*/
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
                                ClassName = cl.Name,
                                Birthday = st.DateOfBirth.ToString("dd/MM/yyyy"),
                              /*  Address = st.Address,
                                ParentPhone = st.PhoneParent,
                                Conduct = st.Conduct ?? "Không có thông tin"
                            */};
            }

            var data = studentVM.ToList();

            
            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Students");
                worksheet.Cells[1, 1].Value = "Trường: THPT chuyên Lương Văn Chánh";
               /* worksheet.Cells[2, 1].Value = $"Danh sách lớp: {(string.IsNullOrEmpty(className) ? "Tất cả" : className)}";*/

                int headerCell = 4;
                worksheet.Cells[headerCell, 1].Value = "STT";
                worksheet.Cells[headerCell, 2].Value = "Họ và tên";
                worksheet.Cells[headerCell, 3].Value = "Giới tính";
                worksheet.Cells[headerCell, 5].Value = "Lớp";
                worksheet.Cells[headerCell, 4].Value = "Ngày sinh";
                /*                worksheet.Cells[headerCell, 6].Value = "Địa chỉ";
                                worksheet.Cells[headerCell, 7].Value = "Liên hệ phụ huynh";
                                worksheet.Cells[headerCell, 8].Value = "Hạnh kiểm";*/

                int stt = 1;
                int row = headerCell + 1;
                foreach (var student in data)
                {
                    worksheet.Cells[row, 1].Value = stt;
                    worksheet.Cells[row, 2].Value = student.Name;
                    worksheet.Cells[row, 3].Value = student.Gender;
                    worksheet.Cells[row, 5].Value = student.ClassName;
                    worksheet.Cells[row, 4].Value = student.Birthday;
                    /* worksheet.Cells[row, 6].Value = student.Address;
                   worksheet.Cells[row, 7].Value = student.ParentPhone;
                   worksheet.Cells[row, 8].Value = student.Conduct ?? "Không có thông tin";*/
                   
                    stt++;
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

        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Vui lòng chọn một tệp Excel.";
                return RedirectToAction("Index");
            }
            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    using (var package = new OfficeOpenXml.ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                        {
                            TempData["Error"] = "Tệp Excel không có dữ liệu.";
                            return RedirectToAction("Index");
                        }

                        var rowCount = worksheet.Dimension.Rows;
                        var students = new List<Student>();

                        for (int row = 5; row <= rowCount; row++)
                        {                       
                            var name = worksheet.Cells[row, 2].Value?.ToString();
                            var genderString = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                            var dateOfBirth = worksheet.Cells[row, 4].Value as DateTime? ?? DateTime.Now;
                            var className = worksheet.Cells[row, 5].Value?.ToString();
                            var phone = worksheet.Cells[row, 6].Value?.ToString();
                            var parentPhone = worksheet.Cells[row, 7].Value?.ToString();
                            var address = worksheet.Cells[row, 8].Value?.ToString();
                            var email = worksheet.Cells[row, 9].Value?.ToString();

                            var classroom = _context.Classrooms.FirstOrDefault(c => c.Name == className);
                            if (classroom == null)
                            {
                                TempData["Error"] = $"Lớp học '{className}' không tồn tại. Hãy thêm lớp học trước.";
                                continue;
                            }

                            var student = new Student
                            {
                                Name = name,
                                Gender = genderString,
                                Email = email,
                                DateOfBirth = dateOfBirth,
                                ClassId = classroom.Id,
                                Phone = phone,
                                Address = address,
                                PhoneParent = parentPhone,
                                Conduct = "",
                                Status = true,
                                CreateAt = DateTime.Now,
                                UpdateAt = DateTime.Now
                            };

                            students.Add(student);
                            //_context.Students.AddRange(students);
                           
                        }

                        _context.Students.AddRange(students);
                        await _context.SaveChangesAsync();

                        TempData["Success"] = "Nhập dữ liệu thành công.";
                    }
                }
            
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
            }

            return RedirectToAction("Index");
        }


    }
}
