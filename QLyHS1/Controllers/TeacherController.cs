using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLyHS1.Data;
using QLyHS1.Models;

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
                            join cl in _context.Classrooms on te.Id equals cl.TeacherId
                            join ass in _context.Assignments on te.Id equals ass.TeacherId
                            join su in _context.Subjects on ass.SubjectId equals su.Id
                            
                            select new TeacherViewModel
                            {
                                Name = te.Name,
                                Subject = su.Name,
                                ClassName = cl.Name,
                                StartTime = ass.StartDate.ToString("dd/MM/yyyy"),
                            };

            if (!string.IsNullOrEmpty(searchString))
            {
                teacherVM = teacherVM.Where(s => s.Name.Contains(searchString));
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
                Token = "111",
                Role = false,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                Status = true
            };

            _context.Teachers.Add(teacher);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
