using System.Windows;
using FinanzApp.ViewModels;

namespace FinanzApp.Wpf;

public partial class MainWindow : Window
{
    private readonly MonthViewModel _vm;

    public MainWindow(MonthViewModel vm)
    {
        InitializeComponent();
        _vm = vm;
        DataContext = vm;
    }

    protected override async void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        await _vm.OnAppearingAsync();
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
