using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using FinanzApp.Data;
using FinanzApp.Services;
using FinanzApp.ViewModels;

namespace FinanzApp.Wpf;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                var dbPath = Path.Combine(AppContext.BaseDirectory, "Data", "finanzapp.db");
                services.AddDbContext<FinanzAppContext>(o => o.UseSqlite($"Data Source={dbPath}"));

                services.AddSingleton<ICurrentUserService, DummyCurrentUserService>();
                services.AddTransient<MonthViewModel>();
                services.AddTransient<MainWindow>();
            })
            .Build();

        using (var scope = host.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<FinanzAppContext>();
            DbInitializer.SeedAsync(ctx).Wait();
        }

        var app = new Application();
        var window = host.Services.GetRequiredService<MainWindow>();
        app.Run(window);
    }
}
