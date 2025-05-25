using System.Windows;
using FinanzApp.WPF.Data;

namespace FinanzApp.WPF.Views
{
    public partial class HomeView : Window
    {
        private readonly User _currentUser;
        public HomeView(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void Month_Click(object sender, RoutedEventArgs e)
        {
            var mv = new MonthView(_currentUser);
            mv.Show();
            this.Close();
        }

        private void Year_Click(object sender, RoutedEventArgs e)
        {
            var yv = new YearView(_currentUser);
            yv.Show();
            this.Close();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var start = new StartView();
            start.Show();
            this.Close();
        }
    }
}
