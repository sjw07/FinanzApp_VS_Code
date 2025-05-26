using FinanzApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace FinanzApp.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(FinanzAppContext ctx)
    {
        await ctx.Database.MigrateAsync();

        if (await ctx.Users.AnyAsync())
            return;

        using var sha = SHA256.Create();
        var hash = Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes("1234")));

        var users = new List<User>
        {
            new() { UserName = "Stefan",  PasswordHash = hash },
            new() { UserName = "Stefan2", PasswordHash = hash },
            new() { UserName = "Stefan3", PasswordHash = hash },
            new() { UserName = "Stefan4", PasswordHash = hash },
        };

        ctx.Users.AddRange(users);
        await ctx.SaveChangesAsync();

        var names = new[] { "Gehalt", "Tanken", "Restaurant", "\u00D6PNV", "Einkaufen", "Kaffee" };
        var rnd = new Random();

        foreach (var user in users)
        {
            for (int month = 1; month <= 12; month++)
            {
                int days = DateTime.DaysInMonth(2025, month);
                for (int i = 0; i < 30; i++)
                {
                    var name = names[rnd.Next(names.Length)];
                    double amount = name == "Gehalt"
                        ? rnd.NextDouble() * 700 + 2800
                        : -(rnd.NextDouble() * 115 + 5);
                    var date = new DateTime(2025, month, rnd.Next(1, days + 1));
                    ctx.Transactions.Add(new Transaction
                    {
                        UserId = user.Id,
                        Name = name,
                        Amount = Math.Round(amount, 2),
                        Date = date
                    });
                }
            }
        }

        await ctx.SaveChangesAsync();
        Debug.WriteLine($"Datenbank erstellt: {users.Count} Benutzer, {users.Count * 30 * 12} Transaktionen (Jahr 2025)");
    }
}
