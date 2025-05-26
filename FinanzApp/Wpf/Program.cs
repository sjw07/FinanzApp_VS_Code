using System;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FinanzApp.Data;
using FinanzApp.Services;
using FinanzApp.ViewModels;

namespace FinanzApp.Wpf;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        var services = new ServiceCollection();
        var dbPath = System.IO.Path.Combine(AppContext.BaseDirectory, "Data", "finanzapp.db");
        services.AddDbContext<FinanzAppContext>(o => o.UseSqlite($"Data Source={dbPath}"));
        services.AddSingleton<ICurrentUserService, DummyCurrentUserService>();
        services.AddTransient<MonthViewModel>();

        var provider = services.BuildServiceProvider();
        using (var scope = provider.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<FinanzAppContext>();
            DbInitializer.SeedAsync(ctx).Wait();
        }

        var app = new App();
        var vm = provider.GetRequiredService<MonthViewModel>();
        var mainWindow = new MainWindow(vm);
        app.Run(mainWindow);
    }
}
