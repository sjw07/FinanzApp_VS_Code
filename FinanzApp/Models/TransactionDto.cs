namespace FinanzApp.Models;

public class TransactionDto
{
    public DateTime Date { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Amount { get; set; }
}
