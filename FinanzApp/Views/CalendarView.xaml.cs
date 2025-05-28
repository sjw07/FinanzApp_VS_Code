using FinanzApp.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FinanzApp;

public partial class CalendarView : ContentPage
{
    readonly FinanceService _service = new();
    int _year;
    int _month;
    List<FinanceEntry> _entries = new();

    public CalendarView()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (App.NavigateToCalendar is (int year, int month))
        {
            _year = year;
            _month = month;
        }
        else
        {
            _year = DateTime.Now.Year;
            _month = DateTime.Now.Month;
        }
        var dt = new DateTime(_year, _month, 1);
        TitleLabel.Text = dt.ToString("MMMM yyyy");
        _entries = await _service.GetEntriesAsync(App.LoggedInUser);
        BuildCalendar();
    }

    void BuildCalendar()
    {
        CalendarGrid.RowDefinitions.Clear();
        CalendarGrid.ColumnDefinitions.Clear();
        CalendarGrid.Children.Clear();

        for (int i = 0; i < 7; i++)
            CalendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

        string[] week = new[] { "Mo", "Di", "Mi", "Do", "Fr", "Sa", "So" };
        CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        for (int i = 0; i < 7; i++)
        {
            var lbl = new Label
            {
                Text = week[i],
                FontAttributes = FontAttributes.Bold,
                HorizontalTextAlignment = TextAlignment.Center,
                TextColor = Colors.White
            };
            CalendarGrid.Children.Add(lbl);
            Grid.SetRow(lbl, 0);
            Grid.SetColumn(lbl, i);
        }

        DateTime first = new DateTime(_year, _month, 1);
        int startCol = ((int)first.DayOfWeek + 6) % 7;
        int daysInMonth = DateTime.DaysInMonth(_year, _month);
        int row = 1;
        int col = startCol;
        CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        for (int day = 1; day <= daysInMonth; day++)
        {
            if (col == 7)
            {
                col = 0;
                row++;
                CalendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            var stack = new VerticalStackLayout { Spacing = 2, Padding = 2 };
            stack.Children.Add(new Label
            {
                Text = day.ToString(),
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center
            });

            foreach (var entry in _entries.Where(e => e.Datum.Year == _year && e.Datum.Month == _month && e.Datum.Day == day))
            {
                stack.Children.Add(new Label
                {
                    Text = $"{entry.Name} {entry.Betrag:C}",
                    FontSize = 12,
                    TextColor = entry.Betrag < 0 ? Colors.Red : Colors.White
                });
            }

            var border = new Border
            {
                Stroke = Colors.Black,
                StrokeThickness = 1,
                BackgroundColor = Color.FromArgb("#0DD3D3D3"),
                Content = stack
            };

            CalendarGrid.Children.Add(border);
            Grid.SetRow(border, row);
            Grid.SetColumn(border, col);
            col++;
        }
    }

    async void OnBackClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(MonthView));
    }
}

