namespace FinanzApp;

public partial class App : Application
{
    public static string? LoggedInUser { get; set; }

    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
