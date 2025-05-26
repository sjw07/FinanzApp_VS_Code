using System.Windows;
using FinanzApp.ViewModels;

namespace FinanzApp.Wpf;

public partial class MainWindow : Window
{
    private readonly MonthViewModel _viewModel;

    public MainWindow(MonthViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        DataContext = _viewModel;
    }

    protected override async void OnContentRendered(System.EventArgs e)
    {
        base.OnContentRendered(e);
        await _viewModel.OnAppearingAsync();
    }
}
