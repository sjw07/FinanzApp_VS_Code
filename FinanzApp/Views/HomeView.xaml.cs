namespace FinanzApp;

public partial class HomeView : ContentPage
{
    public HomeView()
    {
        InitializeComponent();
        SizeChanged += OnPageSizeChanged;
    }

    private void OnPageSizeChanged(object? sender, EventArgs e)
    {
        double buttonWidth = Width / 4;
        MonthButton.WidthRequest = buttonWidth;
        YearButton.WidthRequest = buttonWidth;
        LogoutButton.WidthRequest = buttonWidth;
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
        App.LoggedInUser = null;
        await Shell.Current.GoToAsync("//StartView");
    }
}
