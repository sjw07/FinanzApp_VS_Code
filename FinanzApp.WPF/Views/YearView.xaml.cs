using System.Linq;
using System.Windows;
using FinanzApp.WPF.Data;

namespace FinanzApp.WPF.Views
{
    public partial class YearView : Window
    {
        private readonly User _user;
        public YearView(User user)
        {
            InitializeComponent();
            _user = user;
            LoadData();
        }

        private void LoadData()
        {
            using var db = new ApplicationDbContext();
            var query = db.FinanceEntries
                .Where(e => e.UserId == _user.UserId)
                .GroupBy(e => e.EntryDate.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    Balance = g.Sum(e => e.Type == EntryType.Income ? e.Amount : -e.Amount)
                })
                .OrderBy(x => x.Month)
                .ToList();
            YearGrid.ItemsSource = query;
        }
    }
}
