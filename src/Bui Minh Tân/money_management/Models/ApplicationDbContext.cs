using Microsoft.EntityFrameworkCore;

namespace money_management.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<SavingGoal> SavingGoals { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.CreateAt)
                .HasColumnName("create_at");

            // Nếu muốn map thêm các cột khác, làm tương tự
            modelBuilder.Entity<User>()
                .Property(u => u.FirstName)
                .HasColumnName("first_name");

            modelBuilder.Entity<User>()
                .Property(u => u.LastName)
                .HasColumnName("last_name");

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .HasColumnName("password");

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasColumnName("email");
        }
    }
}
