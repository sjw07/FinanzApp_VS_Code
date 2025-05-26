using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using FinanceApp.Data;

namespace FinanceApp;

public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        using (var scope = host.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            DbInitializer.SeedAsync(services).Wait();
        }
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }

    static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                var dbPath = Path.Combine(AppContext.BaseDirectory, "Data", "finanzapp.db");
                services.AddDbContext<FinanzAppContext>(o => o.UseSqlite($"Data Source={dbPath}"));
                services.AddSingleton<MainWindow>();
                services.AddSingleton<CurrentUserService>();
                services.AddSingleton<ViewModels.MonthViewModel>();
            });
}
