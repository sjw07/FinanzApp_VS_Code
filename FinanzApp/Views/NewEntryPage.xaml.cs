using FinanzApp.Data;
using System;

namespace FinanzApp;

public partial class NewEntryPage : ContentPage
{
    readonly FinanceService _service = new();

    public NewEntryPage()
    {
        InitializeComponent();
    }

    void OnCancelClicked(object? sender, EventArgs e)
    {
        if (this.Window is not null)
            Application.Current?.CloseWindow(this.Window);
        this.Window?.Close();
    }

    async void OnCreateClicked(object? sender, EventArgs e)
    {
        if (!DateTime.TryParse(DateEntry.Text, out var datum))
        {
            await DisplayAlert("Fehler", "Ung\u00fcltiges Datum", "OK");
            return;
        }

        if (!decimal.TryParse(AmountEntry.Text, out var betrag))
        {
            await DisplayAlert("Fehler", "Ung\u00fcltiger Betrag", "OK");
            return;
        }

        var name = NameEntry.Text?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            await DisplayAlert("Fehler", "Name fehlt", "OK");
            return;
        }

        bool success = await _service.AddEntryAsync(App.LoggedInUser, datum, betrag, name);
        if (!success)
        {
            await DisplayAlert("Fehler", "Eintrag existiert bereits", "OK");
            return;
        }

        if (this.Window is not null)
            Application.Current?.CloseWindow(this.Window);
        this.Window?.Close();
    }
}
