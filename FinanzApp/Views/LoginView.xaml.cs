using System.Collections.Generic;

namespace FinanzApp;

public partial class LoginView : ContentPage
{
    readonly Dictionary<string, string> _users = new()
    {
        ["Stefan"] = "1234",
        ["Stefan2"] = "1234",
        ["Stefan3"] = "1234",
        ["Stefan4"] = "1234"
    };

    public LoginView()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        var username = usernameEntry.Text?.Trim();
        var password = passwordEntry.Text;

        if (!string.IsNullOrEmpty(username) &&
            _users.TryGetValue(username, out var validPass) &&
            password == validPass)
        {
            App.LoggedInUser = username;
            await Shell.Current.GoToAsync(nameof(HomeView));
        }
        else
        {
            await DisplayAlert("Fehler", "Ung\u00fcltige Anmeldedaten", "OK");
        }
    }
}
