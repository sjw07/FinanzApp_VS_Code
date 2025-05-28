using FinanzApp.Data;
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
    readonly FinanceService _service = new();

    public LoginView()
    {
        InitializeComponent();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        double fieldWidth = width / 3;
        usernameEntry.WidthRequest = fieldWidth;
        passwordEntry.WidthRequest = fieldWidth;
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
            var entries = await _service.GetEntriesAsync(username);
            var balances = _service.CalculateMonthlyBalances(entries);
            App.MonthlyBalances.Clear();
            foreach (var kv in balances)
                App.MonthlyBalances[kv.Key] = kv.Value;
            await Shell.Current.GoToAsync(nameof(HomeView));
        }
        else
        {
            await DisplayAlert("Fehler", "Ung\u00fcltige Anmeldedaten", "OK");
        }
    }

    private async void OnRegisterClicked(object? sender, EventArgs e)
    {
        await DisplayAlert("Info", "Registrierung", "OK");
    }
}
