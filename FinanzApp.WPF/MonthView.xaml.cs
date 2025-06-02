using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FinanzApp.Data;

namespace FinanzApp.WPF;

public partial class MonthView : Window
{
    readonly FinanceService _service = new();
    readonly ObservableCollection<FinanceEntry> _allEntries = new();
    readonly ObservableCollection<FinanceEntry> _filteredEntries = new();

    public ObservableCollection<FinanceEntry> FilteredEntries => _filteredEntries;

    public MonthView()
    {
        InitializeComponent();
        DataContext = this;
        Loaded += MonthView_Loaded;
    }

    async void MonthView_Loaded(object sender, RoutedEventArgs e)
    {
        var entries = await _service.GetEntriesAsync(null);
        foreach (var entry in entries)
            _allEntries.Add(entry);
        ApplyFilter();
    }

    void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        ApplyFilter();
    }

    void ApplyFilter()
    {
        var text = SearchBox.Text?.Trim() ?? string.Empty;
        _filteredEntries.Clear();
        var filtered = _allEntries.Where(x =>
            x.Name.Contains(text, System.StringComparison.OrdinalIgnoreCase) ||
            x.Datum.ToString("dd.MM.yyyy").Contains(text));
        foreach (var entry in filtered)
            _filteredEntries.Add(entry);
    }
}
