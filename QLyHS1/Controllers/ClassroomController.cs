using Microsoft.AspNetCore.Mvc;
using QLyHS1.Data;

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
            return View();
        }
    }
}
