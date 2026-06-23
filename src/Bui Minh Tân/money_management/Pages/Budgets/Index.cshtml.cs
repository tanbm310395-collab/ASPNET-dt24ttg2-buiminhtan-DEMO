using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using money_management.Models;

namespace money_management.Pages.Budgets
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context) => _context = context;

        public List<Budget> Budgets { get; set; } = new();
        public List<SelectListItem> Categories { get; set; } = new();

        [BindProperty]
        public Budget NewBudget { get; set; } = new();

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                RedirectToPage("/Login");
                return;
            }

            Budgets = _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.Year)
                .ThenByDescending(b => b.Month)
                .ToList();

            Categories = _context.Categories
                .Where(c => c.UserId == userId)
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Type.ToUpper()} - {c.Name}"
                })
                .ToList();
        }

        public IActionResult OnPostAdd()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            // ✅ Xử lý nếu Month, Year, hoặc Amount không được nhập (0 hoặc null)
            if (NewBudget.Month <= 0 || NewBudget.Month > 12) NewBudget.Month = DateTime.Now.Month;
            if (NewBudget.Year <= 0) NewBudget.Year = DateTime.Now.Year;
            if (NewBudget.Amount <= 0) NewBudget.Amount = 0;

            // ✅ Gán UserId
            NewBudget.UserId = userId.Value;

            _context.Budgets.Add(NewBudget);
            _context.SaveChanges();

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var budget = _context.Budgets.Find(id);
            if (budget != null)
            {
                _context.Budgets.Remove(budget);
                _context.SaveChanges();
            }
            return RedirectToPage();
        }
    }
}
