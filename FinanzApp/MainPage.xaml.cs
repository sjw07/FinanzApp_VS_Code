using FinanzApp.Views;

namespace FinanzApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnMonthClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(Views.MonthView));
    }
}
