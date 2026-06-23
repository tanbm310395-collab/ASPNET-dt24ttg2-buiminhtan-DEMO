using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using money_management.Models;

namespace money_management.Pages.Transactions
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public IndexModel(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }


        public List<Transaction> Transactions { get; set; } = new();
        public List<SelectListItem> CategoryOptions { get; set; } = new();

        [BindProperty]
        public Transaction NewTransaction { get; set; } = new();

        [BindProperty]
        public Transaction EditTransaction { get; set; } = new();

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                RedirectToPage("/Login");
                return;
            }

            Transactions = _context.Transactions
                .Include(t => t.Category)
                .Where(t => t.UserId == userId && t.Category != null)
                .ToList();

            CategoryOptions = _context.Categories
                .Where(c => c.UserId == userId)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Type.ToUpper()} - {c.Name}"
                }).ToList();
        }

        //public IActionResult OnPostAdd()
        //{
        //    var userId = HttpContext.Session.GetInt32("UserId");
        //    if (userId == null) return RedirectToPage("/Login");

        //    if (NewTransaction.Date == DateTime.MinValue)
        //        NewTransaction.Date = DateTime.Now;

        //    if (string.IsNullOrWhiteSpace(NewTransaction.Note))
        //        NewTransaction.Note = "";

        //    NewTransaction.UserId = userId.Value;

        //    _context.Transactions.Add(NewTransaction);
        //    _context.SaveChanges();

        //    return RedirectToPage();
        //}

        public async Task<IActionResult> OnPostAddAsync(IFormFile ImageFile)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            // ✅ Kiểm tra định dạng file ảnh
            if (ImageFile != null)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("ImageFile", "Chỉ cho phép file ảnh JPG, PNG hoặc GIF.");
                    return Page();
                }

                // ✅ Lưu ảnh
                var fileName = Guid.NewGuid().ToString() + ext;
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                NewTransaction.ImagePath = fileName;
            }

            if (NewTransaction.Date == DateTime.MinValue)
                NewTransaction.Date = DateTime.Now;

            if (string.IsNullOrWhiteSpace(NewTransaction.Note))
                NewTransaction.Note = "";

            NewTransaction.UserId = userId.Value;

            _context.Transactions.Add(NewTransaction);
            _context.SaveChanges();

            return RedirectToPage();
        }






        public IActionResult OnPostEdit()
        {
            var existing = _context.Transactions.Find(EditTransaction.Id);
            if (existing != null)
            {
                existing.CategoryId = EditTransaction.CategoryId;
                existing.Amount = EditTransaction.Amount;
                existing.Note = string.IsNullOrWhiteSpace(EditTransaction.Note) ? "" : EditTransaction.Note;
                existing.Date = EditTransaction.Date == DateTime.MinValue ? DateTime.Now : EditTransaction.Date;

                _context.SaveChanges();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var existing = _context.Transactions.Find(id);
            if (existing != null)
            {
                _context.Transactions.Remove(existing);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }
    }
}
