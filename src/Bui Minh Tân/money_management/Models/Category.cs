using System.ComponentModel.DataAnnotations.Schema;

namespace money_management.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty; // "income" hoặc "expense"

        [Column("user_id")]
        public int UserId { get; set; }

        public User User { get; set; }
    }
}
