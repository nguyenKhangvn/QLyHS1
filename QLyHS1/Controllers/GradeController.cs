using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLyHS1.Data;
using QLyHS1.Models;
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
        public IActionResult Index()
        {
            var schedules = from g in _context.Grades
                            join s in _context.Students on g.StudentId equals s.Id
                            join sm in _context.Semesters on g.SemesterId equals sm.Id
                            join sc in _context.SchoolYears on g.SchoolYearId equals sc.Id
                            join su in _context.Subjects on g.SubjectId equals su.Id
                            select new GradeViewModel
                            {
                                Id = g.Id,
                                StudentName = s.Name, 
                                SemesterName = sm.Name,
                                SubjectName = su.Name,
                                SchoolYearName = sc.Year,
                                GradeI = g.GradeI,
                                GradeII = g.GradeII,
                                GradeIII = g.GradeSemester
                            };

            return View(schedules.ToList());
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
            return View();
        }

        [HttpPost]
        public IActionResult Add(GradeDetailViewModel model)
        {
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
                GradeI = model.GradeI,
                GradeII = model.GradeII,
                GradeSemester = model.GradeIII,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                Status = true
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
                StudentId = student.StudentId,
                SemesterId = student.SemesterId,
                SubjectId = student.SubjectId,
                SchoolYearId = student.SchoolYearId,
                GradeI = student.GradeI,
                GradeII = student.GradeII,
                GradeSemester = student.GradeSemester,
                CreateAt = student.CreateAt,
                UpdateAt = student.UpdateAt,
                Status = true
            };
            ViewData["StudentId"] = new SelectList(_context.Students, "Id", "Name", studentViewModel.StudentId);
            ViewData["SemesterId"] = new SelectList(_context.Semesters, "Id", "Name", studentViewModel.SemesterId);
            ViewData["SubjectId"] = new SelectList(_context.Subjects, "Id", "Name", studentViewModel.SubjectId);
            ViewData["SchoolYearId"] = new SelectList(_context.SchoolYears, "Id", "Name", studentViewModel.SchoolYearId);
            return View(studentViewModel);
        }


        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StudentId,SemesterId,SubjectId,SchoolYearId,GradeI,GradeII,GradeIII,Status")] GradeDetailToEditViewModel studentViewModel)
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
    }
}
