using System.Collections.Generic;

namespace FinanzApp;

public partial class App : Application
{
    public static string? LoggedInUser { get; set; }
    public static Dictionary<(int Year, int Month), decimal> MonthlyBalances { get; } = new();
    public static (int Year, int Month)? NavigateToMonth { get; set; }
    public static (int Year, int Month)? NavigateToCalendar { get; set; }

    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
