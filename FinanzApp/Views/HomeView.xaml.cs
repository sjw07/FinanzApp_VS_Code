namespace FinanzApp;

public partial class HomeView : ContentPage
{
    public HomeView()
    {
        InitializeComponent();
    }

    private async void OnMonthClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MonthView));
    }

    private async void OnYearClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(YearView));
    }

    private async void OnLogoutClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//StartView");
    }
}
