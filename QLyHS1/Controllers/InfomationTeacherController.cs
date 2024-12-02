using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QLyHS1.Data;
using System.Security.Claims;

namespace QLyHS1.Controllers
{
    public class InfomationTeacherController : Controller
    {
        private readonly QlyHs1Context _context;

        public InfomationTeacherController(QlyHs1Context context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "User");
            }


            var subject = (from sub in _context.Subjects
                            join sch in _context.Assignments on sub.Id equals sch.SubjectId
                            join tea in _context.Teachers on sch.TeacherId equals tea.Id
                            where tea.Id == userId
                            select sub).FirstOrDefault();

            ViewBag.SubjectName = subject?.Name ?? "Chưa có";

            var teacher = _context.Teachers
                .FirstOrDefault(m => m.Id == userId);

            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
            
          
        }
    }
}
