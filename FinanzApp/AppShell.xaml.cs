namespace FinanzApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(LoginView), typeof(LoginView));
        Routing.RegisterRoute(nameof(HomeView), typeof(HomeView));
        Routing.RegisterRoute(nameof(MonthView), typeof(MonthView));
        Routing.RegisterRoute(nameof(YearView), typeof(YearView));
        Routing.RegisterRoute(nameof(NewEntryPage), typeof(NewEntryPage));
        Routing.RegisterRoute(nameof(EditEntryPage), typeof(EditEntryPage));
    }
}
