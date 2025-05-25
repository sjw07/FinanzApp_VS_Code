namespace FinanzApp;

public partial class StartView : ContentPage
{
    public StartView()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(LoginView));
    }

    private void OnExitClicked(object? sender, EventArgs e)
    {
        Application.Current?.Quit();
    }
}
