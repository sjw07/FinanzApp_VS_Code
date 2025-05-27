using FinanzApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinanzApp;

public partial class YearView : ContentPage
{
    readonly FinanceService _service = new();

    public YearView()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var entries = await _service.GetEntriesAsync(App.LoggedInUser);
        BuildGrid(entries);
    }

    void BuildGrid(List<FinanceEntry> entries)
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
            AddHeader(year.ToString(), year - 2020 + 1, 0);

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
                decimal sum = entries
                    .Where(e => e.Datum.Year == year && e.Datum.Month == i + 1)
                    .Sum(e => e.Betrag);
                AddValue(sum, year - 2020 + 1, i + 1);
            }
        }
    }

    void AddHeader(string text, int column, int row, bool bold = true)
    {
        var label = new Label
        {
            Text = text,
            FontSize = 14,
            FontAttributes = bold ? FontAttributes.Bold : FontAttributes.None,
            TextColor = Colors.Black,
            Margin = new Thickness(2)
        };
        YearGrid.Children.Add(label);
        Grid.SetRow(label, row);
        Grid.SetColumn(label, column);
    }

    void AddValue(decimal value, int column, int row)
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
        YearGrid.Children.Add(label);
        Grid.SetRow(label, row);
        Grid.SetColumn(label, column);
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
