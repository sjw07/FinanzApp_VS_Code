using FinanzApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinanzApp;

public partial class MonthView : ContentPage
{
    readonly FinanceService _service = new();
    readonly List<FinanceEntry> _allEntries = new();
    int _currentMonth;
    int _currentYear;
    string _sortColumn = "Datum";
    bool _sortAscending = true;

    void UpdateSortIcons()
    {
        DateSortIcon.Text = "-";
        AmountSortIcon.Text = "-";
        NameSortIcon.Text = "-";

        var arrow = _sortAscending ? "\u2191" : "\u2193"; // up or down arrow
        switch (_sortColumn)
        {
            case "Datum":
                DateSortIcon.Text = arrow;
                break;
            case "Betrag":
                AmountSortIcon.Text = arrow;
                break;
            case "Name":
                NameSortIcon.Text = arrow;
                break;
        }
    }

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
        var monthEntries = _allEntries
            .Where(e => e.Datum.Month == _currentMonth && e.Datum.Year == _currentYear)
            .ToList();

        var monthStart = new DateTime(_currentYear, _currentMonth, 1);
        var carryBalance = _allEntries
            .Where(e => e.Datum < monthStart)
            .Sum(e => e.Betrag);

        var carry = new FinanceEntry
        {
            Datum = monthStart,
            Betrag = carryBalance,
            Name = "\u00dcbertrag"
        };

        monthEntries.Insert(0, carry);

        var monthBalance = monthEntries.Sum(e => e.Betrag);
        BalanceLabel.Text = $"Bilanz: {monthBalance:C}";

        ApplySort(monthEntries);
        UpdateSortIcons();
    }

    void ApplySort(List<FinanceEntry> entries)
    {
        IOrderedEnumerable<FinanceEntry>? ordered = _sortColumn switch
        {
            "Betrag" => _sortAscending ? entries.OrderBy(e => e.Betrag) : entries.OrderByDescending(e => e.Betrag),
            "Name" => _sortAscending ? entries.OrderBy(e => e.Name) : entries.OrderByDescending(e => e.Name),
            _ => _sortAscending ? entries.OrderBy(e => e.Datum) : entries.OrderByDescending(e => e.Datum)
        };
        EntriesView.SelectedItem = null;
        foreach (var entry in _allEntries)
            entry.IsSelected = false;
        EntriesView.ItemsSource = ordered.ToList();
    }

    void OnSortByDate(object? sender, EventArgs e)
    {
        if (_sortColumn == "Datum")
            _sortAscending = !_sortAscending;
        else
        {
            _sortColumn = "Datum";
            _sortAscending = true;
        }
        FilterEntries();
    }

    void OnSortByAmount(object? sender, EventArgs e)
    {
        if (_sortColumn == "Betrag")
            _sortAscending = !_sortAscending;
        else
        {
            _sortColumn = "Betrag";
            _sortAscending = true;
        }
        FilterEntries();
    }

    void OnSortByName(object? sender, EventArgs e)
    {
        if (_sortColumn == "Name")
            _sortAscending = !_sortAscending;
        else
        {
            _sortColumn = "Name";
            _sortAscending = true;
        }
        FilterEntries();
    }

    async void OnNewEntryClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Info", "Neuer Eintrag", "OK");
    }

    async void OnEditEntryClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Info", "Eintrag \u00e4ndern", "OK");
    }

    void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        foreach (FinanceEntry item in e.PreviousSelection.OfType<FinanceEntry>())
            item.IsSelected = false;

        foreach (FinanceEntry item in e.CurrentSelection.OfType<FinanceEntry>())
            item.IsSelected = true;
    }

    async void OnHomeClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(HomeView));
    }

    async void OnLogoutClicked(object? sender, EventArgs e)
    {
        App.LoggedInUser = null;
        await Shell.Current.GoToAsync("//StartView");
    }
}
