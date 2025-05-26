using FinanzApp.ViewModels;

namespace FinanzApp.Views;

public partial class MonthView : ContentPage
{
    private MonthViewModel Vm => (MonthViewModel)BindingContext;

    public MonthView()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Vm.OnAppearingAsync();
    }
}
