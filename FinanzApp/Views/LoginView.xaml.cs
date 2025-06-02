using FinanzApp.Data;
using System.Collections.Generic;

namespace FinanzApp;

public partial class LoginView : ContentPage
{
    readonly FinanceService _service = new();

    public LoginView()
    {
        InitializeComponent();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        // Set the width of the input fields to half of the previous value
        double fieldWidth = width / 6;
        usernameEntry.WidthRequest = fieldWidth;
        passwordEntry.WidthRequest = fieldWidth;
    }

    private async void OnLoginClicked(object? sender, EventArgs e)
    {
        var username = usernameEntry.Text?.Trim();
        var password = passwordEntry.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Fehler", "Ung\u00fcltige Anmeldedaten", "OK");
            return;
        }

        var user = await _service.GetUserAsync(username);
        if (user is null)
        {
            await DisplayAlert("Fehler", "Ung\u00fcltige Anmeldedaten", "OK");
            return;
        }

        var hash = FinanceService.HashPassword(password);
        if (hash != user?.PasswordHash)
        {
            await DisplayAlert("Fehler", "Ung\u00fcltige Anmeldedaten", "OK");
            return;
        }

        App.LoggedInUser = username;
        var entries = await _service.GetEntriesAsync(username);
        var balances = _service.CalculateMonthlyBalances(entries);
        App.MonthlyBalances.Clear();
        foreach (var kv in balances)
            App.MonthlyBalances[kv.Key] = kv.Value;
        await Shell.Current.GoToAsync(nameof(HomeView));
    }

    private async void OnRegisterClicked(object? sender, EventArgs e)
    {
        var username = usernameEntry.Text?.Trim();
        var password = passwordEntry.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Fehler", "Benutzername und Passwort erforderlich", "OK");
            return;
        }

        var success = await _service.RegisterUserAsync(username, password);
        if (!success)
        {
            await DisplayAlert("Fehler", "Benutzername existiert bereits", "OK");
            return;
        }

        await DisplayAlert("Info", "Benutzer registriert", "OK");
    }
}
