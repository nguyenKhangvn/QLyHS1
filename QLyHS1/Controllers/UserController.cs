using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using QLyHS1.Data;
using QLyHS1.Models;
using System.Security.Claims;

namespace QLyHS1.Controllers
{
    public class UserController : Controller
    {
        private readonly QlyHs1Context _context;

        public UserController(QlyHs1Context context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Login(UserViewModel model)
        {
            var teacher = await _context.Teachers.SingleOrDefaultAsync(u => u.UserName == model.username && u.Password == model.password);

            if (teacher != null)
            {
                // Thêm thông tin vào Claims
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, teacher.Id.ToString()), 
            new Claim(ClaimTypes.Name, teacher.UserName),
             new Claim(ClaimTypes.Role, teacher.Role ? "Admin" : "User"),
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);


                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(model);
        }



        [Authorize]
        public async Task<IActionResult> Signout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "User");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


    }
}
