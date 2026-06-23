using Microsoft.Extensions.Configuration.UserSecrets;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace money_management.Models
{
    public class Budget
    {
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Required]
        public int Month { get; set; } = 0;

        [Required]
        public int Year { get; set; } = 0;

        [Required]
        public decimal Amount { get; set; } = 0;

        public Category Category { get; set; }
        public User User { get; set; }
    }
}
