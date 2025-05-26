using System.Windows;

namespace FinanceApp.Views;

public partial class MonthView : Window
{
    public MonthView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is ViewModels.MonthViewModel vm)
                await vm.OnAppearingAsync();
        };
    }
}
