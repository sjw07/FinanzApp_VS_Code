using System.Linq;
using System.Windows;
using FinanzApp.WPF.Data;

namespace FinanzApp.WPF.Views
{
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            using var db = new ApplicationDbContext();
            var user = db.Users.FirstOrDefault(u => u.Username == UsernameBox.Text && u.PasswordHash == PasswordBox.Password);
            if (user != null)
            {
                var home = new HomeView(user);
                home.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Anmeldung fehlgeschlagen");
            }
        }
    }
}
