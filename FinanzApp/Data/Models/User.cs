namespace FinanzApp.Data.Models;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty; // UNIQUE
    public string PasswordHash { get; set; } = string.Empty;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
