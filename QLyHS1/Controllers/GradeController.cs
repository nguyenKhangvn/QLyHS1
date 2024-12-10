using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using QLyHS1.Data;
using QLyHS1.Models;
using System.Security.Claims;
using System.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace QLyHS1.Controllers
{
    public class GradeController : Controller
    {
        private readonly QlyHs1Context _context;
        public GradeController(QlyHs1Context context)
        {
            _context = context;
        }

        public static double CalculateGPA(double gradeI, double gradeI1, double gradeII, double gradeSemester)
        {
            double gpa = ((gradeI * 1) + (gradeI1 * 1) + (gradeII * 2) + (gradeSemester * 3)) / 7;
            return Math.Round(gpa, 3);
        }

        public static string rangeGPA(double GradeAverage)
        {
            if(GradeAverage < 5)
            {
                return "Yếu";
            }
            else if (GradeAverage >= 5 && GradeAverage < 7)
            {
                return "Trung bình";
            }
            else if (GradeAverage >= 7 && GradeAverage < 8)
            {
                return "Khá";
            }
            else if (GradeAverage >= 8 && GradeAverage < 9)
            {
                return "Giỏi";
            }
            else
            {
                return "Xuất sắc";
            }
        }
        public IActionResult Index(String? className, String? subName)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "User");
            }


          /*  ViewBag.Classrooms = _context.Classrooms
                .Where(c => c.TeacherId == userId)
                .Select(c => new SelectListItem
                {
                    Value = c.Name,
                    Text = c.Name
                }).ToList();*/
            
            if (role == "Admin")
            {
                    ViewBag.Classrooms = _context.Classrooms
                                        .Select(c => new SelectListItem
                                        {
                                            Value = c.Name,
                                            Text = c.Name
                                        }).ToList();


                 ViewBag.SubjectsForSpecificTeacher = _context.Assignments
                    .Join(_context.Subjects,
                          assignment => assignment.SubjectId,
                          subject => subject.Id,
                          (assignment, subject) => new SelectListItem
                          {
                              Value = subject.Name,
                              Text = subject.Name
                          })
                .ToList();

                var schedules = from g in _context.Grades
                                join s in _context.Students on g.StudentId equals s.Id
                                join sm in _context.Semesters on g.SemesterId equals sm.Id
                                join sc in _context.SchoolYears on g.SchoolYearId equals sc.Id
                                join su in _context.Subjects on g.SubjectId equals su.Id
                                join cl in _context.Classrooms on g.ClassNameID equals cl.Id
                                where (string.IsNullOrEmpty(className) || cl.Name == className) 
                                            && (string.IsNullOrEmpty(subName) || su.Name == subName)
                                select new GradeViewModel
                                {
                                    Id = g.Id,
                                    StudentName = s.Name,
                                    SemesterName = sm.Name,
                                    SubjectName = su.Name,
                                    SchoolYearName = sc.Year,
                                    className = cl.Name,
                                    GradeI = g.GradeI ?? 0.0,
                                    GradeI1 = g.GradeI1 ?? 0.0,
                                    GradeII = g.GradeII ?? 0.0,
                                    GradeIII = g.GradeSemester ?? 0.0,
                                    GradeAverage = (g.GradeI.HasValue && g.GradeI1.HasValue && g.GradeII.HasValue && g.GradeSemester.HasValue)
                                           ? CalculateGPA(g.GradeI.Value, g.GradeI1.Value, g.GradeII.Value, g.GradeSemester.Value)
                                           : 0.0,
                                    RangeGrade = (g.GradeI.HasValue && g.GradeI1.HasValue && g.GradeII.HasValue && g.GradeSemester.HasValue)
                                         ? rangeGPA(CalculateGPA(g.GradeI.Value, g.GradeI1.Value, g.GradeII.Value, g.GradeSemester.Value))
                                         : "Chưa có"

                                };

                return View(schedules.ToList());
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

                ViewBag.SubjectsForSpecificTeacher = _context.Assignments
                        .Where(a => a.TeacherId == userId) 
                        .Join(_context.Subjects, assignment => assignment.SubjectId, subject => subject.Id, (assignment, subject) => new SelectListItem
                        {
                            Value = subject.Name,
                            Text = subject.Name
                        })
                        .ToList();
                var schedules = from g in _context.Grades
                                join s in _context.Students on g.StudentId equals s.Id
                                join sm in _context.Semesters on g.SemesterId equals sm.Id
                                join sc in _context.SchoolYears on g.SchoolYearId equals sc.Id
                                join su in _context.Subjects on g.SubjectId equals su.Id
                                join cl in _context.Classrooms on g.ClassNameID equals cl.Id
                                join te in _context.Teachers on cl.TeacherId equals te.Id
                                where te.Id == userId
                       && (string.IsNullOrEmpty(className) || cl.Name == className)
                        && (string.IsNullOrEmpty(subName) || su.Name == subName)
                                select new GradeViewModel
                                {
                                    Id = g.Id,
                                    StudentName = s.Name,
                                    SemesterName = sm.Name,
                                    SubjectName = su.Name,
                                    SchoolYearName = sc.Year,
                                    className = cl.Name,
                                    GradeI = g.GradeI ?? 0.0,
                                    GradeI1 = g.GradeI ?? 0.0,
                                    GradeII = g.GradeII ?? 0.0,
                                    GradeIII = g.GradeSemester ?? 0.0,
                                    GradeAverage = (g.GradeI.HasValue && g.GradeI1.HasValue && g.GradeII.HasValue && g.GradeSemester.HasValue)
                                           ? CalculateGPA(g.GradeI.Value, g.GradeI1.Value, g.GradeII.Value, g.GradeSemester.Value)
                                           : 0.0,
                                    RangeGrade = (g.GradeI.HasValue && g.GradeI1.HasValue && g.GradeII.HasValue && g.GradeSemester.HasValue)
                                         ? rangeGPA(CalculateGPA(g.GradeI.Value, g.GradeI1.Value, g.GradeII.Value, g.GradeSemester.Value))
                                         : "Chưa có"
                                };

                return View(schedules.ToList());
            }
            
        }

        [Route("Grade/Search")]
        public IActionResult Search(string? query)
        {
            var stu = _context.Grades.AsQueryable();
            if (query == null)
            {
                return NotFound();
            }

            var students = _context.Grades
                .Include(s => s.Student)
                .Include(sm => sm.Semester)
                .Include(su => su.Subject)
                .Include(sc => sc.SchoolYear)
                .Where(m => m.Student.Name.Contains(query))
                .Select(g => new GradeViewModel
                {
                    Id = g.Id,
                    StudentName = g.Student.Name,
                    SemesterName = g.Semester.Name,
                    SubjectName = g.Subject.Name,
                    SchoolYearName = g.SchoolYear.Year,
                    className = g.Classroom.Name,
                    GradeI = g.GradeI ?? 0.0,
                    GradeI1 = g.GradeI ?? 0.0,
                    GradeII = g.GradeII ?? 0.0,
                    GradeIII = g.GradeSemester ?? 0.0,
                    GradeAverage = g.GradeAverage ?? 0.0,
                    RangeGrade = g.RangeGrade
                }).ToList();

            if (!students.Any())
            {
                return NotFound();
            }

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
                var Classroom = from st in _context.Classrooms
                                where st.Status == true
                                select new
                                {
                                    st.Id,
                                    st.Name,
                                };
                ViewBag.Classroom = new SelectList(Classroom, "Id", "Name");

                var Student = from st in _context.Students
                              join cl in _context.Classrooms on st.ClassId equals cl.Id
                              where st.Status == true
                              select new
                              {
                                  st.Id,
                                  st.Name,
                                  st.ClassId
                              };
                ViewBag.Student = new SelectList(Student, "Id", "Name");
            } 
            else
            {
                var Classroom = from st in _context.Classrooms
                                where st.Status == true && st.TeacherId == userId
                                select new
                                {
                                    st.Id,
                                    st.Name,
                                };
                ViewBag.Classroom = new SelectList(Classroom, "Id", "Name");

                var Student = from st in _context.Students
                              join cl in _context.Classrooms on st.ClassId equals cl.Id
                              where st.Status == true && cl.TeacherId == userId
                              select new
                              {
                                  st.Id,
                                  st.Name,
                                  st.ClassId
                              };
                ViewBag.Student = new SelectList(Student, "Id", "Name");
            }

            var Semester = _context.Semesters
                     .Select(s => new { s.Id, s.Name })
                     .ToList();

            ViewBag.Semester = new SelectList(Semester, "Id", "Name");
            var Subject = _context.Subjects
                     .Where(s => s.Status == true)
                     .Select(s => new { s.Id, s.Name })
                     .ToList();

            ViewBag.Subject = new SelectList(Subject, "Id", "Name");
            var SchoolYear = _context.SchoolYears
                     .Select(s => new { s.Id, s.Year })
                     .ToList();

            ViewBag.SchoolYear = new SelectList(SchoolYear, "Id", "Year");
            return View();
        }

        [HttpPost]
        public IActionResult Add(GradeDetailViewModel model)
        {
            var Student = _context.Students
                          .Select(s => new { s.Id, s.Name })
                          .ToList();

            ViewBag.Student = new SelectList(Student, "Id", "Name");
            var Semester = _context.Semesters
                     .Select(s => new { s.Id, s.Name })
                     .ToList();

            ViewBag.Semester = new SelectList(Semester, "Id", "Name");
            var Subject = _context.Subjects
                     .Select(s => new { s.Id, s.Name })
                     .ToList();

            ViewBag.Subject = new SelectList(Subject, "Id", "Name");
            var SchoolYear = _context.SchoolYears
                     .Select(s => new { s.Id, s.Year })
                     .ToList();

            ViewBag.SchoolYear = new SelectList(SchoolYear, "Id", "Year");


            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var student = new Grade
            {
                StudentId = model.StudentId,
                SemesterId = model.SemesterId,
                SubjectId = model.SubjectId,
                SchoolYearId = model.SchoolYearId,
                ClassNameID = model.ClassNameId,
                GradeI = model.GradeI ?? 0,
                GradeII = model.GradeII ?? 0,
                GradeSemester = model.GradeIII ?? 0,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                Status = true
            };

            _context.Grades.Add(student);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public IActionResult AddGrade(int gradeType)
        {
            
            ViewBag.GradeType = gradeType; 
            return View();
        }
        [HttpPost]
        public IActionResult AddGrade(GradeDetailToEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var classroom = _context.Classrooms.FirstOrDefault(c => c.Id == model.ClassNameId);
            if (classroom == null)
            {
                ModelState.AddModelError("ClassroomId", "Lớp học không hợp lệ.");
                return View(model);
            }

            var student = new Grade
            {
               
                StudentId = model.StudentId,
                SemesterId = model.SemesterId,
                SubjectId = model.SubjectId,
                SchoolYearId = model.SchoolYearId,
                ClassNameID = model.ClassNameId,
                GradeI = model.GradeI ?? 0.0,
                GradeII = model.GradeII ?? 0.0,
                GradeSemester = model.GradeSemester ?? 0.0,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                Status = true,
                
            };

            _context.Grades.Add(student);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Grades.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            var studentViewModel = new GradeDetailToEditViewModel
            {
                Id = student.Id,
                StudentId = student.StudentId,
                SemesterId = student.SemesterId,
                SubjectId = student.SubjectId,
                SchoolYearId = student.SchoolYearId,
                ClassNameId = student.ClassNameID,
                GradeI = student.GradeI ?? 0.0,
                GradeII = student.GradeII ?? 0.0,
                GradeSemester = student.GradeSemester ?? 0.0,
                CreateAt = student.CreateAt,
                UpdateAt = student.UpdateAt,
                Status = student.Status ?? true
            };

            // Populating dropdown data
            ViewData["StudentId"] = new SelectList(_context.Students.Where(s => s.Status == true), "Id", "Name", studentViewModel.StudentId);
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name", studentViewModel.SemesterId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(s => s.Status == true), "Id", "Name", studentViewModel.SubjectId);
            ViewData["SchoolYearId"] = new SelectList(_context.SchoolYears, "Id", "Year", studentViewModel.SchoolYearId);
            ViewData["ClassNameId"] = new SelectList(_context.Classrooms.Where(c => c.Status == true), "Id", "Name", studentViewModel.ClassNameId);

            return View(studentViewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,SemesterId,SubjectId,SchoolYearId,ClassNameId,GradeI,GradeII,GradeSemester,Status")] GradeDetailToEditViewModel studentViewModel)
        {
            if (id != studentViewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var student = await _context.Grades.FindAsync(id);
                    if (student == null)
                    {
                        return NotFound();
                    }

                    // Update the entity with new values
                    student.StudentId = studentViewModel.StudentId;
                    student.SemesterId = studentViewModel.SemesterId;
                    student.SubjectId = studentViewModel.SubjectId;
                    student.SchoolYearId = studentViewModel.SchoolYearId;
                    student.ClassNameID = studentViewModel.ClassNameId;
                    student.GradeI = studentViewModel.GradeI;
                    student.GradeII = studentViewModel.GradeII;
                    student.GradeSemester = studentViewModel.GradeSemester;
                    student.UpdateAt = DateTime.Now;
                    student.Status = studentViewModel.Status;

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

            // Repopulate dropdown data for invalid model state
            ViewData["StudentId"] = new SelectList(_context.Students.Where(s => s.Status == true), "Id", "Name", studentViewModel.StudentId);
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name", studentViewModel.SemesterId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects.Where(s => s.Status == true), "Id", "Name", studentViewModel.SubjectId);
            ViewData["SchoolYearId"] = new SelectList(_context.SchoolYears, "Id", "Year", studentViewModel.SchoolYearId);
            ViewData["ClassNameId"] = new SelectList(_context.Classrooms.Where(c => c.Status == true), "Id", "Name", studentViewModel.ClassNameId);

            return View(studentViewModel);
        }


        private bool StudentExists(int id)
        {
            return _context.Grades.Any(e => e.Id == id);
        }

        //detail
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var classroms = await _context.Grades
                .Include(c => c.Classroom)
                .Include(s => s.Student)
                .Include(sm => sm.Semester)
                .Include(su => su.Subject)
                .Include(sc => sc.SchoolYear)
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

            var student = await _context.Grades
                .Include(c => c.Classroom)
                .Include(s => s.Student)
                .Include(sm => sm.Semester)
                .Include(su => su.Subject)
                .Include(sc => sc.SchoolYear)
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
            var student = await _context.Grades.FindAsync(id);
            if (student != null)
            {
                _context.Grades.Remove(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult ExportToExcel(string? className, string? Year, [FromQuery] string subName)
        {
          /*  className = Request.Query["className"];
            string subName1 = HttpContext.Request.Query["subName"];
            Console.WriteLine($"Subject Name: {subName}");*/

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "User");
            }

            IQueryable<GradeViewModel> studentVM;

            if (role == "Admin")
            {
                studentVM = from st in _context.Students
                            join cl in _context.Classrooms on st.ClassId equals cl.Id
                            join g in _context.Grades on st.Id equals g.StudentId
                            join sem in _context.Semesters on g.SemesterId equals sem.Id
                            join sub in _context.Subjects on g.SubjectId equals sub.Id 
                            join sy in _context.SchoolYears on g.SchoolYearId equals sy.Id 
                            where (string.IsNullOrEmpty(className) || cl.Name == className)
                                  && (string.IsNullOrEmpty(subName) || sub.Name == subName)
                            select new GradeViewModel
                            {
                                StudentName = st.Name,
                                SemesterName = sem.Name,
                                SubjectName = sub.Name,
                                SchoolYearName = sy.Year,
                                className = cl.Name,
                                GradeI = g.GradeI ?? 0.0,
                                GradeI1 = g.GradeI1 ?? 0.0,
                                GradeII = g.GradeII ?? 0.0,
                                GradeIII = g.GradeSemester ?? 0.0,
                                GradeAverage = g.GradeAverage ?? 0.0,
                                RangeGrade = g.RangeGrade ?? "Chưa có"
                            };
            }
            else
            {
                studentVM = from st in _context.Students
                            join cl in _context.Classrooms on st.ClassId equals cl.Id
                            join g in _context.Grades on st.Id equals g.StudentId
                            join sem in _context.Semesters on g.SemesterId equals sem.Id
                            join sub in _context.Subjects on g.SubjectId equals sub.Id
                            join sy in _context.SchoolYears on g.SchoolYearId equals sy.Id
                            where cl.TeacherId == userId &&
                                  (string.IsNullOrEmpty(className) || cl.Name == className)
                                  && (string.IsNullOrEmpty(subName) || sub.Name == subName)
                            select new GradeViewModel
                            {
                                StudentName = st.Name,
                                SemesterName = sem.Name,
                                SubjectName = sub.Name,
                                SchoolYearName = sy.Year,
                                className = cl.Name,
                                GradeI = g.GradeI ?? 0.0,
                                GradeI1 = g.GradeI1 ?? 0.0,
                                GradeII = g.GradeII ?? 0.0,
                                GradeIII = g.GradeSemester ?? 0.0,
                                GradeAverage = g.GradeAverage ?? 0.0,
                                RangeGrade = g.RangeGrade ?? "Chưa có"
                            };
            }

            var data = studentVM.ToList();

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Students");
                worksheet.Cells[1, 1].Value = "Trường: THPT Chuyên Lương Văn Chánh";
             /*   worksheet.Cells[2, 1].Value = $"Danh sách lớp: {(string.IsNullOrEmpty(className) ? "Tất cả" : className)}";
                worksheet.Cells[3, 1].Value = $"Học kì: {(string.IsNullOrEmpty(className) ? "---" : className)}";
                worksheet.Cells[4, 1].Value = $"Học kì: {(string.IsNullOrEmpty(className) ? "---" : className)}";*/
                
                int headerCell = 5;
                worksheet.Cells[headerCell, 1].Value = "STT";
                worksheet.Cells[headerCell, 2].Value = "Họ và tên";
                worksheet.Cells[headerCell, 3].Value = "Học kì";
                worksheet.Cells[headerCell, 4].Value = "Lớp";

                worksheet.Cells[headerCell, 5].Value = "Tên môn";
                worksheet.Cells[headerCell, 6].Value = "Năm học";
                worksheet.Cells[headerCell, 7].Value = "Điểm KT thường xuyên";
                worksheet.Cells[headerCell, 8].Value = "Điểm hệ số 1";
                worksheet.Cells[headerCell, 9].Value = "Điểm hệ số 2";
                worksheet.Cells[headerCell, 10].Value = "Điểm hệ số 3";
                worksheet.Cells[headerCell, 11].Value = "Điểm TB";
                worksheet.Cells[headerCell, 12].Value = "Xếp loại";

                int row = headerCell + 1;
                int stt = 1;
                foreach (var student in data)
                {
                    worksheet.Cells[row, 1].Value = stt;
                    worksheet.Cells[row, 2].Value = student.StudentName;
                    worksheet.Cells[row, 3].Value = student.SemesterName;
                    worksheet.Cells[row, 4].Value = student.className;
                    worksheet.Cells[row, 5].Value = student.SubjectName;
                    worksheet.Cells[row, 6].Value = student.SchoolYearName;
                    worksheet.Cells[row, 7].Value = student.GradeI;
                    worksheet.Cells[row, 8].Value = student.GradeI1;
                    worksheet.Cells[row, 9].Value = student.GradeII;
                    worksheet.Cells[row, 10].Value = student.GradeIII;
                    worksheet.Cells[row, 11].Value = student.GradeAverage;
                    worksheet.Cells[row, 12].Value = student.RangeGrade;
                    row++;
                    stt++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = "Grade.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportFromExcel(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng chọn file Excel.");
                return RedirectToAction("Index");
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var worksheet = package.Workbook.Worksheets[0];
                        int rowCount = worksheet.Dimension.Rows;

                        for (int row = 5; row <= rowCount; row++)
                        {
                            var studentName = worksheet.Cells[row, 2].Value?.ToString();
                            var semesterName = worksheet.Cells[row, 3].Value?.ToString();
                            var subjectName = worksheet.Cells[row, 4].Value?.ToString();
                            var schoolYearName = int.TryParse(worksheet.Cells[row, 5].Value?.ToString(), out var y) ? y : (int?)null;
                            var className = worksheet.Cells[row, 6].Value?.ToString();
                            var gradeI = double.TryParse(worksheet.Cells[row, 7].Value?.ToString(), out var g1) ? g1 : (double?)null;
                            var gradeI1 = double.TryParse(worksheet.Cells[row, 8].Value?.ToString(), out var g11) ? g11 : (double?)null;
                            var gradeII = double.TryParse(worksheet.Cells[row, 9].Value?.ToString(), out var g2) ? g2 : (double?)null;
                            var gradeIII = double.TryParse(worksheet.Cells[row, 10].Value?.ToString(), out var g3) ? g3 : (double?)null;
                            var GradeAverage = double.TryParse(worksheet.Cells[row, 11].Value?.ToString(), out var g) ? g : (double?)null;
                            var RangeGrade = worksheet.Cells[row, 12].Value?.ToString();

                            // Tìm các bản ghi tương ứng trong database
                            var student = _context.Students.FirstOrDefault(s => s.Name == studentName);
                            var semester = _context.Semesters.FirstOrDefault(s => s.Name == semesterName);
                            var subject = _context.Subjects.FirstOrDefault(s => s.Name == subjectName);
                            var schoolYear = _context.SchoolYears.FirstOrDefault(sy => sy.Year == schoolYearName);
                            var classObj = _context.Classrooms.FirstOrDefault(c => c.Name == className);

                            if (semester != null && subject != null && schoolYear != null && classObj != null)
                            {/*
                                if (student == null)
                                {
                                    student = new grade
                                    {
                                        Name = studentName,
                                        ClassId = classObj.Id,
                                        CreateAt = DateTime.Now,
                                        UpdateAt = DateTime.Now,
                                        Status = true
                                    };
                                    _context.Students.Add(student);
                                    await _context.SaveChangesAsync(); 
                                }*/

                      
                                var grade = _context.Grades.FirstOrDefault(g =>
                                    g.StudentId == student.Id &&
                                    g.SemesterId == semester.Id &&
                                    g.SubjectId == subject.Id &&
                                    g.SchoolYearId == schoolYear.Id);

                                var isStudent = _context.Grades.FirstOrDefault(g => g.StudentId == student.Id);
                                var isSub = _context.Grades
                                            .FirstOrDefault(g => g.StudentId == student.Id 
                                                && g.SubjectId == subject.Id 
                                                && g.SemesterId == semester.Id 
                                                && g.SchoolYearId == schoolYear.Id);

                                if (isStudent == null)
                                {
                                    
                                    grade = new Grade
                                    {
                                        StudentId = student.Id,
                                        SemesterId = semester.Id,
                                        SubjectId = subject.Id,
                                        SchoolYearId = schoolYear.Id,
                                        ClassNameID = classObj.Id,
                                        GradeI = gradeI,
                                        GradeI1 = gradeI1,
                                        GradeII = gradeII,
                                        GradeSemester = gradeIII,
                                        CreateAt = DateTime.Now,
                                        UpdateAt = DateTime.Now,
                                        Status = true
                                    };
                                    _context.Grades.Add(grade);
                                }
                                else if (isStudent != null && isSub == null)
                                {
                                    grade = new Grade
                                    {
                                        StudentId = student.Id,
                                        SemesterId = semester.Id,
                                        SubjectId = subject.Id,
                                        SchoolYearId = schoolYear.Id,
                                        ClassNameID = classObj.Id,
                                        GradeI = gradeI,
                                        GradeI1 = gradeI1,
                                        GradeII = gradeII,
                                        GradeSemester = gradeIII,
                                        CreateAt = DateTime.Now,
                                        UpdateAt = DateTime.Now,
                                        Status = true
                                    };
                                    _context.Grades.Add(grade);
                                }
                                else
                                {
                                    grade = isSub;
                                    if (grade != null)
                                    {
                                        grade.GradeI = gradeI ?? grade.GradeI;   
                                        grade.GradeI1 = gradeI1 ?? grade.GradeI1;
                                        grade.GradeII = gradeII ?? grade.GradeII;
                                        grade.GradeSemester = gradeIII ?? grade.GradeSemester;
                                        grade.GradeAverage = (grade.GradeI.HasValue && grade.GradeI1.HasValue && grade.GradeII.HasValue && grade.GradeSemester.HasValue)
                                           ? CalculateGPA(grade.GradeI.Value, grade.GradeI1.Value, grade.GradeII.Value, grade.GradeSemester.Value)
                                           : 0.0;
                                        grade.RangeGrade = (grade.GradeI.HasValue && grade.GradeI1.HasValue && grade.GradeII.HasValue && grade.GradeSemester.HasValue)
                                             ? rangeGPA(CalculateGPA(grade.GradeI.Value, grade.GradeI1.Value, grade.GradeII.Value, grade.GradeSemester.Value))
                                             : "Chưa có";
                                       grade.UpdateAt = DateTime.Now;
                                    }
                                    _context.Grades.Update(grade);
                                }
                            }
                        }

                        // Lưu thay đổi vào database
                        await _context.SaveChangesAsync();
                    }
                }

                TempData["SuccessMessage"] = "Nhập dữ liệu từ file Excel thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
