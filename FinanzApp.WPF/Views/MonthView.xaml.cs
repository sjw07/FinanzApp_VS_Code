using System;
using System.Linq;
using System.Windows;
using FinanzApp.WPF.Data;

namespace FinanzApp.WPF.Views
{
    public partial class MonthView : Window
    {
        private readonly User _user;
        private DateTime _current;

        public MonthView(User user)
        {
            InitializeComponent();
            _user = user;
            _current = new DateTime(2025, 1, 1);
            LoadData();
        }

        private void LoadData()
        {
            Header.Text = _current.ToString("MMMM yyyy");
            using var db = new ApplicationDbContext();
            var entries = db.FinanceEntries
                .Where(e => e.UserId == _user.UserId && e.EntryDate.Month == _current.Month)
                .ToList();
            EntriesGrid.ItemsSource = entries;
            var balance = entries.Sum(e => e.Type == EntryType.Income ? e.Amount : -e.Amount);
            BalanceText.Text = $"Bilanz: {balance:C}";
        }

        private void PrevMonth_Click(object sender, RoutedEventArgs e)
        {
            _current = _current.AddMonths(-1);
            LoadData();
        }

        private void NextMonth_Click(object sender, RoutedEventArgs e)
        {
            _current = _current.AddMonths(1);
            LoadData();
        }
    }
}
