using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using money_management.Models;

namespace money_management.Pages.Categories
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context) => _context = context;

        public List<Category> Categories { get; set; } = new();

        [BindProperty]
        public Category NewCategory { get; set; } = new();

        [BindProperty]
        public Category Category { get; set; } = new();

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                RedirectToPage("/Login");
                return;
            }

            Categories = _context.Categories
                .Where(c => c.UserId == userId)
                .ToList();
        }

        public IActionResult OnPostAdd()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            // ✅ Check null & gán giá trị mặc định
            NewCategory.Name = string.IsNullOrWhiteSpace(NewCategory.Name) ? "" : NewCategory.Name;
            NewCategory.Type = string.IsNullOrWhiteSpace(NewCategory.Type) ? "expense" : NewCategory.Type;

            NewCategory.UserId = userId.Value;
            _context.Categories.Add(NewCategory);
            _context.SaveChanges();

            return RedirectToPage();
        }



        public IActionResult OnPostEdit()
        {
            var existing = _context.Categories.Find(Category.Id);
            if (existing != null)
            {
                existing.Name = string.IsNullOrWhiteSpace(Category.Name) ? "" : Category.Name;
                existing.Type = string.IsNullOrWhiteSpace(Category.Type) ? "expense" : Category.Type;

                _context.SaveChanges();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                // ✅ Kiểm tra xem có giao dịch nào đang dùng danh mục này không
                bool hasTransaction = _context.Transactions.Any(t => t.CategoryId == id);

                if (hasTransaction)
                {
                    ModelState.AddModelError(string.Empty, "Không thể xóa danh mục đang được sử dụng.");
                    // ✨ Cập nhật lại danh sách để hiển thị lại trang
                    var userId = HttpContext.Session.GetInt32("UserId");
                    Categories = _context.Categories
                        .Where(c => c.UserId == userId)
                        .ToList();
                    return Page(); // ⚠ Trả lại trang hiện tại kèm lỗi
                }

                _context.Categories.Remove(category);
                _context.SaveChanges();
            }

            return RedirectToPage(); // ✅ Chỉ khi xóa thành công mới reload trang
        }

    }
}
