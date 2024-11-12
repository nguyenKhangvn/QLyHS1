﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLyHS1.Data;
using QLyHS1.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace QLyHS1.Controllers
{
    public class ClassroomController : Controller
    {
        private readonly QlyHs1Context _context;

        public ClassroomController(QlyHs1Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var schedules = from cl in _context.Classrooms
                            join t in _context.Teachers on cl.TeacherId equals t.Id
                            join gl in _context.GrandLevels on cl.GradeLevelId equals gl.Id
                            select new ClassroomViewModel
                            {
                                Id = cl.Id,
                                Name = cl.Name,
                                Quantity = cl.Quantity,
                                GradeLevelName= gl.Name,
                                TeacherName = t.Name,
                                
                            };

            return View(schedules.ToList());
        }

        [Route("Classroom/Search")]
        public IActionResult Search(string? query)
        {
            var stu = _context.Classrooms.AsQueryable();
            if (query == null)
            {
                return NotFound();
            }

            var students = _context.Classrooms
                .Include(t => t.Teacher)
                .Include(gl => gl.GradeLevel)
                .Where(m => m.Name.Contains(query))
                .Select(cl => new ClassroomViewModel
                {
                    Id = cl.Id,
                    Name = cl.Name,
                    Quantity = cl.Quantity,
                    GradeLevelName = cl.GradeLevel.Name,
                    TeacherName = cl.Teacher.Name,
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
        public IActionResult Add(ClassroomDetailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var student = new Classroom
            {
               
               TeacherId = model.TeacherID,
                GradeLevelId = model.GradeLevelID,
                Name = model.Name,
               Quantity = model.Quantity,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                Status = true
            };

            _context.Classrooms.Add(student);
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
                GradeLevelID = student.GradeLevelId,
                Name = student.Name,
                Quantity = student.Quantity,
                Status = student.Status,
                CreateAt = student.CreateAt,
                UpdateAt = student.UpdateAt
            };

            ViewData["TeacherId"] = new SelectList(_context.Teachers, "Id", "Name", student.TeacherId);
            ViewData["GradeLevelId"] = new SelectList(_context.GrandLevels, "Id", "Name", student.GradeLevelId);
            return View(studentViewModel);
        }


        // POST: Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TeacherID,GrandLevelID,Name,Quantity,Status")] ClassroomDetailToEditViewModel studentViewModel)
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
                    student.GradeLevelId = studentViewModel.GradeLevelID;
                    student.Name = studentViewModel.Name;
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
            ViewData["ClassId"] = new SelectList(_context.Teachers, "Id", "Name", studentViewModel.TeacherID);
            ViewData["ClassId"] = new SelectList(_context.GrandLevels, "Id", "Name", studentViewModel.GradeLevelID);
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
                .Include(gl => gl.GradeLevel)
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
                .Include(gl => gl.GradeLevel)
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
