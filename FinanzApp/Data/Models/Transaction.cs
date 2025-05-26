namespace FinanzApp.Data.Models;

public class Transaction
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Amount { get; set; }
    public DateTime Date { get; set; }
    public User User { get; set; } = null!;
}
