using FinanzApp.Data;

namespace FinanzApp;

public partial class MonthView : ContentPage
{
    readonly FinanceService _service = new FinanceService();

    public MonthView()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        EntriesView.ItemsSource = await _service.GetEntriesAsync();
    }
}
