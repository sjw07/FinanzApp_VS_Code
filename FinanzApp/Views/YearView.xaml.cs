using FinanzApp.Data;
using FinanzApp.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinanzApp;

public partial class YearView : ContentPage
{
    readonly FinanceService _service = new();
    List<FinanceEntry> _entries = new();
    int? _selectedYear;
    readonly YearGraphDrawable _graph = new();

    public YearView()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        FinanceService.EntriesChanged += OnEntriesChanged;
        _entries = await _service.GetEntriesAsync(App.LoggedInUser);
        if (App.MonthlyBalances.Count == 0)
        {
            var dict = _service.CalculateMonthlyBalances(_entries);
            foreach (var kv in dict)
                App.MonthlyBalances[kv.Key] = kv.Value;
        }
        BuildGrid();
        _graph.Entries = _entries;
        YearGraph.Drawable = _graph;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        FinanceService.EntriesChanged -= OnEntriesChanged;
    }

    void BuildGrid()
    {
        YearGrid.RowDefinitions.Clear();
        YearGrid.ColumnDefinitions.Clear();
        YearGrid.Children.Clear();

        YearGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        for (int year = 2020; year <= 2030; year++)
            YearGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        YearGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        AddHeader("Monat", 0, 0);
        for (int year = 2020; year <= 2030; year++)
            AddHeader(year.ToString(), year - 2020 + 1, 0, true, true, year);

        string[] months = new[]
        {
            "Januar", "Februar", "März", "April", "Mai", "Juni",
            "Juli", "August", "September", "Oktober", "November", "Dezember"
        };

        for (int i = 0; i < months.Length; i++)
        {
            YearGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            AddHeader(months[i], 0, i + 1, false);

            for (int year = 2020; year <= 2030; year++)
            {
                if (!App.MonthlyBalances.TryGetValue((year, i + 1), out var sum))
                {
                    sum = _entries
                        .Where(e => e.Datum.Year == year && e.Datum.Month == i + 1)
                        .Sum(e => e.Betrag);
                }
                AddValue(sum, year - 2020 + 1, i + 1, year);
            }
        }

        YearGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        AddHeader("Gesamt", 0, months.Length + 1, true);
        for (int year = 2020; year <= 2030; year++)
        {
            if (!App.MonthlyBalances.TryGetValue((year, 12), out var total))
            {
                total = _entries
                    .Where(e => e.Datum.Year == year)
                    .Sum(e => e.Betrag);
            }
            AddValue(total, year - 2020 + 1, months.Length + 1, year);
        }
    }

    void AddHeader(string text, int column, int row, bool bold = true, bool isYear = false, int year = 0)
    {
        var label = new Label
        {
            Text = text,
            FontSize = 14,
            FontAttributes = bold ? FontAttributes.Bold : FontAttributes.None,
            TextColor = Colors.Black,
            Margin = new Thickness(2),
            BackgroundColor = Colors.White
        };
        if (isYear)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, __) => { _selectedYear = year; BuildGrid(); };
            label.GestureRecognizers.Add(tap);
            if (_selectedYear == year)
                label.BackgroundColor = Colors.LightBlue;
        }
        YearGrid.Children.Add(label);
        Grid.SetRow(label, row);
        Grid.SetColumn(label, column);
    }

    void AddValue(decimal value, int column, int row, int year)
    {
        var label = new Label
        {
            Text = value.ToString("C"),
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            TextColor = value < 0 ? Colors.Red : Colors.White,
            Margin = new Thickness(2),
            HorizontalTextAlignment = TextAlignment.End
        };
        if (_selectedYear == year)
            label.BackgroundColor = Colors.LightBlue;
        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, __) => { _selectedYear = year; BuildGrid(); };
        label.GestureRecognizers.Add(tap);
        YearGrid.Children.Add(label);
        Grid.SetRow(label, row);
        Grid.SetColumn(label, column);
    }

    async void OnEntriesChanged(object? sender, EventArgs e)
    {
        _entries = await _service.GetEntriesAsync(App.LoggedInUser);
        var dict = _service.CalculateMonthlyBalances(_entries);
        App.MonthlyBalances.Clear();
        foreach (var kv in dict)
            App.MonthlyBalances[kv.Key] = kv.Value;
        BuildGrid();
        _graph.Entries = _entries;
        YearGraph.Invalidate();
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
