using System.Windows;

namespace FinanceApp;

public partial class MainWindow : Window
{
    private readonly IServiceProvider _provider;
    public MainWindow(IServiceProvider provider)
    {
        _provider = provider;
        InitializeComponent();
    }

    private void OnMonthView(object sender, RoutedEventArgs e)
    {
        var vm = _provider.GetService(typeof(ViewModels.MonthViewModel)) as ViewModels.MonthViewModel;
        var view = new Views.MonthView { DataContext = vm };
        view.Show();
    }
}
