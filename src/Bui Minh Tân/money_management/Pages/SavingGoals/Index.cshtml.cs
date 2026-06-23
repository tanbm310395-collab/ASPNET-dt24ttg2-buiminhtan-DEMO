using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using money_management.Models;

namespace money_management.Pages.SavingGoals
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        public IndexModel(ApplicationDbContext context) => _context = context;

        public List<SavingGoal> Goals { get; set; } = new();

        [BindProperty]
        public SavingGoal NewGoal { get; set; } = new();

        [BindProperty]
        public SavingGoal Goal { get; set; } = new();

        public IActionResult OnGet()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            Goals = _context.SavingGoals
                .Where(g => g.UserId == userId)
                .ToList();

            return Page();
        }

        public IActionResult OnPostAdd()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToPage("/Login");

            // Kiểm tra null cho từng trường nếu cần
            NewGoal.UserId = userId.Value;
            NewGoal.Title = NewGoal.Title?.Trim() ?? ""; // Tránh null
            if (NewGoal.TargetAmount <= 0)
                NewGoal.TargetAmount = 0;
            if (NewGoal.CurrentAmount < 0)
                NewGoal.CurrentAmount = 0;

            _context.SavingGoals.Add(NewGoal);
            _context.SaveChanges();
            return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
            var existing = _context.SavingGoals.Find(Goal.Id);
            if (existing != null)
            {
                existing.Title = Goal.Title?.Trim() ?? "";
                existing.TargetAmount = Goal.TargetAmount <= 0 ? 0 : Goal.TargetAmount;
                existing.CurrentAmount = Goal.CurrentAmount < 0 ? 0 : Goal.CurrentAmount;

                _context.SaveChanges();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var existing = _context.SavingGoals.Find(id);
            if (existing != null)
            {
                _context.SavingGoals.Remove(existing);
                _context.SaveChanges();
            }

            return RedirectToPage();
        }
    }
}
