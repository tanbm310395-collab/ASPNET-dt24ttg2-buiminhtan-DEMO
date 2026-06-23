using System.ComponentModel.DataAnnotations.Schema;

public class SavingGoal
{
    public int Id { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [Column("title")]
    public string? Title { get; set; }

    [Column("target_amount")]
    public decimal TargetAmount { get; set; }

    [Column("current_amount")]
    public decimal CurrentAmount { get; set; }

    [Column("deadline")]
    public DateTime? Deadline { get; set; }

    // Optional navigation property (nếu có)
    // public User User { get; set; }
}
