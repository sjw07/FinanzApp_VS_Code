using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using FinanzApp.Data;
using FinanzApp.Services;
using System.IO;
using FinanzApp.ViewModels;

namespace FinanzApp;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        var builder = MauiApp.CreateBuilder();
        var dbPath = Path.Combine(AppContext.BaseDirectory, "Data", "finanzapp.db");
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Services.AddDbContext<FinanzAppContext>(o => o.UseSqlite($"Data Source={dbPath}"));
        builder.Services.AddSingleton<ICurrentUserService, DummyCurrentUserService>();
        builder.Services.AddTransient<MonthViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var ctx = scope.ServiceProvider.GetRequiredService<FinanzAppContext>();
            DbInitializer.SeedAsync(ctx).Wait();
        }

        return app;
    }
}
