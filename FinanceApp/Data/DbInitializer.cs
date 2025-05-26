using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FinanceApp.Data.Models;

namespace FinanceApp.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<FinanzAppContext>();
        await context.Database.MigrateAsync();

        if (context.Users.Any()) return;

        string password = "1234";
        using var sha = SHA256.Create();
        var hash = Convert.ToHexString(sha.ComputeHash(Encoding.UTF8.GetBytes(password)));

        var users = Enumerable.Range(1,4).Select(i => new User{UserName=$"Stefan{i==1?string.Empty:i}".TrimEnd(),PasswordHash=hash}).ToList();
        context.Users.AddRange(users);
        var names = new[]{"Gehalt","Tanken","Restaurant","ÖPNV","Einkaufen","Kaffee"};
        var rnd = new Random();
        foreach(var user in users)
        {
            for(int m=1;m<=12;m++)
            {
                var days = DateTime.DaysInMonth(2025,m);
                for(int j=0;j<30;j++)
                {
                    var name = names[rnd.Next(names.Length)];
                    double amount = name=="Gehalt" ? rnd.Next(2800,3501) : -rnd.Next(5,121);
                    var date = new DateTime(2025,m,rnd.Next(1,days+1));
                    context.Transactions.Add(new Transaction{User=user,Amount=amount,Name=name,Date=date});
                }
            }
        }
        await context.SaveChangesAsync();
        Console.WriteLine("Datenbank erstellt: 4 Benutzer, 1440 Transaktionen (Jahr 2025)");
    }
}
