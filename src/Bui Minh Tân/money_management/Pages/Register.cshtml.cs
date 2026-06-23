using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using money_management.Models;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace money_management.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RegisterModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public User User { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            if (_context.Users.Any(u => u.Email == User.Email))
            {
                ModelState.AddModelError(string.Empty, "Email đã tồn tại.");
                return Page();
            }


            User.Password = HashPassword(Password);
            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu không được để trống.");
                return Page();
            }

            User.CreateAt = DateTime.Now;


            _context.Users.Add(User);
            _context.SaveChanges();

            return RedirectToPage("/Login");
        }

        private string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                password = ""; // hoặc throw nếu muốn bắt buộc nhập

            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

    }
}
