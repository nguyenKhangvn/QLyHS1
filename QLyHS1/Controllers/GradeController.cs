using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLyHS1.Data;
using QLyHS1.Models;
using System.Security.Claims;
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
        public IActionResult Index(String? className)
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "User");
            }

            ViewBag.Classrooms = _context.Classrooms
                .Where(c => c.TeacherId == userId)
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
                var schedules = from g in _context.Grades
                                join s in _context.Students on g.StudentId equals s.Id
                                join sm in _context.Semesters on g.SemesterId equals sm.Id
                                join sc in _context.SchoolYears on g.SchoolYearId equals sc.Id
                                join su in _context.Subjects on g.SubjectId equals su.Id
                                join cl in _context.Classrooms on g.SubjectId equals cl.Id
                                where string.IsNullOrEmpty(className) || cl.Name == className
                                select new GradeViewModel
                                {
                                    Id = g.Id,
                                    StudentName = s.Name,
                                    SemesterName = sm.Name,
                                    SubjectName = su.Name,
                                    SchoolYearName = sc.Year,
                                    GradeI = g.GradeI ?? 0.0,
                                    GradeII = g.GradeII ?? 0.0,
                                    GradeIII = g.GradeSemester ?? 0.0

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
                var schedules = from g in _context.Grades
                                join s in _context.Students on g.StudentId equals s.Id
                                join sm in _context.Semesters on g.SemesterId equals sm.Id
                                join sc in _context.SchoolYears on g.SchoolYearId equals sc.Id
                                join su in _context.Subjects on g.SubjectId equals su.Id
                                join cl in _context.Classrooms on g.ClassNameID equals cl.Id
                                join te in _context.Teachers on cl.TeacherId equals te.Id
                                where te.Id == userId && string.IsNullOrEmpty(className) || cl.Name == className
                                select new GradeViewModel
                                {
                                    Id = g.Id,
                                    StudentName = s.Name,
                                    SemesterName = sm.Name,
                                    SubjectName = su.Name,
                                    SchoolYearName = sc.Year,
                                    GradeI = g.GradeI ?? 0.0,
                                    GradeII = g.GradeII ?? 0.0,
                                    GradeIII = g.GradeSemester ?? 0.0
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
                    GradeI = g.GradeI,
                    GradeII = g.GradeII,
                    GradeIII = g.GradeSemester
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
                ClassNameID = model.ClassNameId,
                StudentId = model.StudentId,
                SemesterId = model.SemesterId,
                SubjectId = model.SubjectId,
                SchoolYearId = model.SchoolYearId,
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
                GradeI = model.GradeI ?? 0.0,
                GradeII = model.GradeII ?? 0.0,
                GradeSemester = model.GradeSemester ?? 0.0,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                Status = true,
                ClassNameID = model.ClassNameId,
                StudentId = model.StudentId,
                SubjectId = model.SubjectId 
            };

            _context.Grades.Add(student);
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
                GradeI = student.GradeI ?? 0.0,
                GradeII = student.GradeII ?? 0.0,
                GradeSemester = student.GradeSemester ?? 0.0,
                CreateAt = student.CreateAt,
                UpdateAt = student.UpdateAt,
                Status = true
            };
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Name", studentViewModel.StudentId);
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name", studentViewModel.SemesterId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", studentViewModel.SubjectId);
            ViewData["SchoolYearId"] = new SelectList(_context.SchoolYears, "Id", "Year", studentViewModel.SchoolYearId);
            return View(studentViewModel);
        }


        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,SemesterId,SubjectId,SchoolYearId,GradeI,GradeII,GradeSemester,Status")] GradeDetailToEditViewModel studentViewModel)
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

                    student.StudentId = studentViewModel.StudentId;
                    student.SemesterId = studentViewModel.SemesterId;
                    student.SubjectId = studentViewModel.SubjectId;
                    student.SchoolYearId = studentViewModel.SchoolYearId;
                    student.GradeI = studentViewModel.GradeI;
                    student.GradeII = studentViewModel.GradeII;
                    student.GradeSemester = studentViewModel.GradeSemester;
                    student.CreateAt = studentViewModel.CreateAt;
                    student.UpdateAt = DateTime.Now;
                    student.Status = true;


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
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Name", studentViewModel.StudentId);
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name", studentViewModel.SemesterId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", studentViewModel.SubjectId);
            ViewData["SchoolYearId"] = new SelectList(_context.SchoolYears, "Id", "Name", studentViewModel.SchoolYearId);
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
        public IActionResult ExportToExcel(string? className, string? Year)
        {
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
                            select new GradeViewModel
                            {
                                StudentName = st.Name,
                                SemesterName = sem.Name,
                                SubjectName = sub.Name,
                                SchoolYearName = sy.Year,
                                GradeI = g.GradeI ?? 0.0,
                                GradeII = g.GradeII ?? 0.0,
                                GradeIII = g.GradeSemester ?? 0.0
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
                            select new GradeViewModel
                            {
                                StudentName = st.Name,
                                SemesterName = sem.Name,
                                SubjectName = sub.Name,
                                SchoolYearName = sy.Year,
                                GradeI = g.GradeI ?? 0.0,
                                GradeII = g.GradeII ?? 0.0,
                                GradeIII = g.GradeSemester ?? 0.0
                            };
            }

            var data = studentVM.ToList();

            using (var package = new OfficeOpenXml.ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Students");
                worksheet.Cells[1, 1].Value = "Trường: Đại học Quy Nhơn";
                worksheet.Cells[2, 1].Value = $"Danh sách lớp: {(string.IsNullOrEmpty(className) ? "Tất cả" : className)}";
                worksheet.Cells[3, 1].Value = $"Học kì: {(string.IsNullOrEmpty(className) ? "---" : className)}";
                worksheet.Cells[4, 1].Value = $"Học kì: {(string.IsNullOrEmpty(className) ? "---" : className)}";
                
                int headerCell = 5;
                worksheet.Cells[headerCell, 2].Value = "Học kì";
                worksheet.Cells[headerCell, 3].Value = "Tên môn";
                worksheet.Cells[headerCell, 4].Value = "Năm học";
                worksheet.Cells[headerCell, 5].Value = "Điểm hệ số 1";
                worksheet.Cells[headerCell, 6].Value = "Điểm hệ số 2";
                worksheet.Cells[headerCell, 7].Value = "Điểm hệ số 3";

                int row = headerCell + 1;
                foreach (var student in data)
                {
                    worksheet.Cells[row, 1].Value = student.StudentName;
                    worksheet.Cells[row, 2].Value = student.SemesterName;
                    worksheet.Cells[row, 3].Value = student.SubjectName;
                    worksheet.Cells[row, 4].Value = student.SchoolYearName;
                    worksheet.Cells[row, 5].Value = student.GradeI;
                    worksheet.Cells[row, 6].Value = student.GradeII;
                    worksheet.Cells[row, 7].Value = student.GradeIII;
                    row++;
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = "Grade.xlsx";
                var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                return File(stream, contentType, fileName);
            }
        }

    }
}
