using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Windows;
using FinanzApp.Data;
using FinanzApp.Services;
using FinanzApp.ViewModels;

namespace FinanzApp.Wpf;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var services = new ServiceCollection();
        var dbPath = Path.Combine(AppContext.BaseDirectory, "Data", "finanzapp.db");
        services.AddDbContext<FinanzAppContext>(o => o.UseSqlite($"Data Source={dbPath}"));
        services.AddSingleton<ICurrentUserService, DummyCurrentUserService>();
        services.AddTransient<MonthViewModel>();
        var provider = services.BuildServiceProvider();
        using (var scope = provider.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<FinanzAppContext>();
            DbInitializer.SeedAsync(ctx).Wait();
        }
        var main = new MainWindow { DataContext = provider.GetRequiredService<MonthViewModel>() };
        main.Show();
    }
}
