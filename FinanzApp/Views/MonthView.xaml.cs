using FinanzApp.Data;
using System.Collections.Generic;
using System.Linq;

namespace FinanzApp;

public partial class MonthView : ContentPage
{
    readonly FinanceService _service = new();
    readonly List<FinanceEntry> _allEntries = new();
    int _currentMonth;
    int _currentYear;

    public MonthView()
    {
        InitializeComponent();
        _currentMonth = DateTime.Now.Month;
        _currentYear = DateTime.Now.Year;
        UpdateTitle();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _allEntries.Clear();
        _allEntries.AddRange(await _service.GetEntriesAsync(App.LoggedInUser));
        FilterEntries();
    }

    void OnPrevMonthClicked(object sender, EventArgs e)
    {
        if (_currentMonth == 1)
        {
            _currentMonth = 12;
            _currentYear--;
        }
        else
        {
            _currentMonth--;
        }
        UpdateTitle();
        FilterEntries();
    }

    void OnNextMonthClicked(object sender, EventArgs e)
    {
        if (_currentMonth == 12)
        {
            _currentMonth = 1;
            _currentYear++;
        }
        else
        {
            _currentMonth++;
        }
        UpdateTitle();
        FilterEntries();
    }

    void UpdateTitle()
    {
        var date = new DateTime(_currentYear, _currentMonth, 1);
        TitleLabel.Text = $"Monatsübersicht {date:MMMM yyyy}";
    }

    void FilterEntries()
    {
        var filtered = _allEntries
            .Where(e => e.Datum.Month == _currentMonth && e.Datum.Year == _currentYear)
            .ToList();
        EntriesView.ItemsSource = filtered;
    }

    async void OnNewEntryClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Info", "Neuer Eintrag", "OK");
    }

    async void OnEditEntryClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Info", "Eintrag \u00e4ndern", "OK");
    }
}
