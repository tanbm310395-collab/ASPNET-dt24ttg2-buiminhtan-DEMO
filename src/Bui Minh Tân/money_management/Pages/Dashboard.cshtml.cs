using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using money_management.Models;
using System.Linq;

namespace money_management.Pages
{
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public List<decimal> MonthlyIncome { get; set; } = new();
        public List<decimal> MonthlyExpense { get; set; } = new();
        public List<string> Months { get; set; } = new();

        public Dictionary<string, decimal> ExpenseByCategory { get; set; } = new();

        public decimal Balance => TotalIncome - TotalExpense;

        public List<SavingGoal> SavingGoals { get; set; } = new();
        public List<string> BudgetAlerts { get; set; } = new();

        public List<Transaction> RecentTransactions { get; set; }

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            TotalIncome = _context.Transactions
                .Where(t => t.UserId == userId && t.Category.Type == "income")
                .Sum(t => (decimal?)t.Amount) ?? 0;

            TotalExpense = _context.Transactions
                .Where(t => t.UserId == userId && t.Category.Type == "expense")
                .Sum(t => (decimal?)t.Amount) ?? 0;

            var now = DateTime.Now;
            for (int i = 5; i >= 0; i--)
            {
                var month = now.AddMonths(-i);
                Months.Add(month.ToString("MM/yyyy"));

                MonthlyIncome.Add(_context.Transactions
                    .Where(t => t.UserId == userId && t.Date.Month == month.Month && t.Date.Year == month.Year && t.Category.Type == "income")
                    .Sum(t => (decimal?)t.Amount) ?? 0);

                MonthlyExpense.Add(_context.Transactions
                    .Where(t => t.UserId == userId && t.Date.Month == month.Month && t.Date.Year == month.Year && t.Category.Type == "expense")
                    .Sum(t => (decimal?)t.Amount) ?? 0);
            }

            ExpenseByCategory = _context.Transactions
                .Where(t => t.UserId == userId && t.Category.Type == "expense")
                .GroupBy(t => t.Category.Name)
                .Select(g => new { Category = g.Key, Total = g.Sum(t => t.Amount) })
                .ToDictionary(x => x.Category, x => x.Total);

            // Lấy mục tiêu tiết kiệm
            SavingGoals = _context.SavingGoals
                .Where(g => g.UserId == userId)
                .ToList();

            // Cảnh báo ngân sách
            BudgetAlerts = _context.Budgets
                .Include(b => b.Category)
                .Where(b => b.UserId == userId)
                .Select(b => new
                {
                    b.Category.Name,
                    b.Month,
                    b.Year,
                    b.Amount,
                    Spent = _context.Transactions
                                .Where(t => t.CategoryId == b.CategoryId &&
                                            t.Date.Month == b.Month &&
                                            t.Date.Year == b.Year &&
                                            t.UserId == userId)
                                .Sum(t => t.Amount)
                })
                .Where(x => x.Spent > x.Amount)
                .Select(x => $"Vượt ngân sách {x.Name} ({x.Month}/{x.Year})")
                .ToList();



            //RecentTransactions = _context.Transactions
            //    .Include(t => t.Category)
            //    .Where(t => t.UserId == userId && t.Category != null)
            //    .OrderByDescending(t => t.Date)
            //    .Take(5)
            //    .ToList();


            return Page();
        }

    }
}
