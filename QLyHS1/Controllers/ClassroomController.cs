using Microsoft.AspNetCore.Mvc;
using QLyHS1.Data;
using QLyHS1.Models;

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
                                GrandLevelName = gl.Name,
                                TeacherName = t.Name,
                                
                            };

            return View(schedules.ToList());
        }
    }
}
