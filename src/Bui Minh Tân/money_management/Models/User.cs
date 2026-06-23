using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    [Column("create_at")]
    public DateTime CreateAt { get; set; }

    public string Role { get; set; } = "user";
}
