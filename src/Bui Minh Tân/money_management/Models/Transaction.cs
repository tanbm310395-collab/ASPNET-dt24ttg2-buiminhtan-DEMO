using System.ComponentModel.DataAnnotations.Schema;

namespace money_management.Models
{
    public class Transaction
    {
        public int Id { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        public decimal Amount { get; set; } = 0;
        public string Note { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Now;
        public string ImagePath { get; set; } = "";

        public Category Category { get; set; }
        public User? User { get; set; }

    }
}
