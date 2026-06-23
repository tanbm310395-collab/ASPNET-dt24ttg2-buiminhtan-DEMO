using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using money_management.Models;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace money_management.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public LoginModel(ApplicationDbContext context) => _context = context;

        [BindProperty] public string Email { get; set; }
        [BindProperty] public string Password { get; set; }

        [TempData]
        public string SuccessMessage { get; set; }

        public IActionResult OnPost()
        {
            var hashedPassword = HashPassword(Password);
            var user = _context.Users.FirstOrDefault(u => u.Email == Email && u.Password == hashedPassword);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Sai email hoặc mật khẩu");
                return Page();
            }

            // Lưu user ID vào session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserRole", user.Role);

            return RedirectToPage("/Dashboard");
        }

        private string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return string.Empty;

            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

    }
}
