using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using money_management.Models;

namespace money_management.Pages.Reports
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context) => _context = context;

        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public List<CategoryReport> CategoryReports { get; set; }

        public void OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) RedirectToPage("/Login");

            TotalIncome = _context.Transactions
                .Where(t => t.UserId == userId && t.Category.Type == "income")
                .Sum(t => (decimal?)t.Amount) ?? 0;

            TotalExpense = _context.Transactions
                .Where(t => t.UserId == userId && t.Category.Type == "expense")
                .Sum(t => (decimal?)t.Amount) ?? 0;

            CategoryReports = _context.Transactions
                .Where(t => t.UserId == userId)
                .GroupBy(t => t.Category.Name)
                .Select(g => new CategoryReport
                {
                    CategoryName = g.Key,
                    Total = g.Sum(t => t.Amount)
                })
                .ToList();
        }

        public class CategoryReport
        {
            public string CategoryName { get; set; }
            public decimal Total { get; set; }
        }
    }
}
