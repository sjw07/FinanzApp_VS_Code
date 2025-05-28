using FinanzApp.Data;
using FinanzApp.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace FinanzApp;

public partial class YearView : ContentPage
{
    readonly FinanceService _service = new();
    List<FinanceEntry> _entries = new();
    int? _selectedYear;
    readonly YearGraphDrawable _graph = new();
    List<DateTime> _months = new();
    decimal[] _incomes = Array.Empty<decimal>();
    decimal[] _expenses = Array.Empty<decimal>();
    decimal[] _balances = Array.Empty<decimal>();

    void OnGraphPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_months.Count == 0)
            return;
        var pos = e.GetPosition(YearGraph);
        double stepX = YearGraph.Width / (_months.Count - 1);
        int index = (int)Math.Round(pos.X / stepX);
        if (index < 0 || index >= _months.Count)
        {
            GraphTooltip.IsVisible = false;
            return;
        }
        string month = _months[index].ToString("MMMM yyyy");
        string text = $"{month}\nEingänge: {_incomes[index]:F1}\nAusgänge: {_expenses[index]:F1}\nBilanz: {_balances[index]:F1}";
        GraphTooltip.Text = text;
        GraphTooltip.IsVisible = true;
        GraphTooltip.TranslationX = pos.X + 10;
        GraphTooltip.TranslationY = pos.Y + 10;
    }

    void OnGraphPointerExited(object? sender, PointerEventArgs e)
    {
        GraphTooltip.IsVisible = false;
    }

    public YearView()
    {
        InitializeComponent();
        var pointer = new PointerGestureRecognizer();
        pointer.PointerMoved += OnGraphPointerMoved;
        pointer.PointerExited += OnGraphPointerExited;
        YearGraph.GestureRecognizers.Add(pointer);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        FinanceService.EntriesChanged += OnEntriesChanged;
        await RefreshAsync();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        FinanceService.EntriesChanged -= OnEntriesChanged;
    }

    async Task RefreshAsync()
    {
        _entries = await _service.GetEntriesAsync(App.LoggedInUser);
        var dict = _service.CalculateMonthlyBalances(_entries);
        App.MonthlyBalances.Clear();
        foreach (var kv in dict)
            App.MonthlyBalances[kv.Key] = kv.Value;
        BuildGrid();
        _graph.Entries = _entries;
        PrepareGraphData();
        YearGraph.Drawable = _graph;
        YearGraph.Invalidate();
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
            Margin = new Thickness(0),
            BackgroundColor = Colors.White
        };
        if (isYear)
        {
            var tap = new TapGestureRecognizer();
            tap.Tapped += (_, __) => { _selectedYear = year; BuildGrid(); };
            label.GestureRecognizers.Add(tap);
            if (_selectedYear == year)
            {
                label.BackgroundColor = Colors.LightBlue;
                label.TextColor = Colors.Orange;
            }
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
        {
            label.BackgroundColor = Colors.LightBlue;
            label.TextColor = Colors.Orange;
        }
        var tap = new TapGestureRecognizer();
        tap.Tapped += (_, __) => { _selectedYear = year; BuildGrid(); };
        label.GestureRecognizers.Add(tap);
        YearGrid.Children.Add(label);
        Grid.SetRow(label, row);
        Grid.SetColumn(label, column);
    }

    void PrepareGraphData()
    {
        _months.Clear();
        const int startYear = 2020;
        const int endYear = 2030;
        for (int year = startYear; year <= endYear; year++)
            for (int month = 1; month <= 12; month++)
                _months.Add(new DateTime(year, month, 1));

        _incomes = new decimal[_months.Count];
        _expenses = new decimal[_months.Count];
        _balances = new decimal[_months.Count];

        decimal running = 0;
        for (int i = 0; i < _months.Count; i++)
        {
            var dt = _months[i];
            var inc = _entries.Where(e => e.Datum.Year == dt.Year && e.Datum.Month == dt.Month && e.Betrag > 0)
                               .Sum(e => e.Betrag);
            var exp = _entries.Where(e => e.Datum.Year == dt.Year && e.Datum.Month == dt.Month && e.Betrag < 0)
                               .Sum(e => e.Betrag);
            running += inc + exp;
            _incomes[i] = inc;
            _expenses[i] = -exp;
            _balances[i] = running;
        }
    }

    async void OnEntriesChanged(object? sender, EventArgs e)
    {
        await RefreshAsync();
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
