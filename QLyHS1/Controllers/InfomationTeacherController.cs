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

        public async Task<IActionResult> Index()
        {
            // Lấy UserId từ Claims
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Kiểm tra và chuyển đổi userId
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login", "User");
            }

            // Lấy danh sách môn học (Subjects) mà giáo viên này giảng dạy
            var subjects = await _context.Subjects
    .Join(_context.Assignments, s => s.Id, sch => sch.SubjectId, (s, sch) => new { s, sch })
    .Where(x => x.sch.TeacherId == userId) // Giả sử bảng Assignments có cột TeacherId liên kết với userId
    .Select(x => new { x.s.Id, x.s.Name })
    .ToListAsync();


            // Đưa danh sách môn học vào ViewBag để hiển thị trong SelectList
            ViewBag.Subjects = new SelectList(subjects, "Id", "Name");

            // Lấy thông tin giáo viên
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(m => m.Id == userId);

            if (teacher == null)
            {
                return NotFound(); // Nếu không tìm thấy giáo viên
            }

            return View(teacher); // Trả lại view với thông tin giáo viên
        }
    }
}
