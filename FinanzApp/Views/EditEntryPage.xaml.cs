using FinanzApp.Data;
using System;

namespace FinanzApp;

public partial class EditEntryPage : ContentPage
{
    readonly FinanceService _service = new();
    readonly FinanceEntry _original;

    public EditEntryPage(FinanceEntry entry)
    {
        InitializeComponent();
        _original = entry;
        DateEntry.Text = entry.Datum.ToString("dd.MM.yyyy");
        AmountEntry.Text = entry.Betrag.ToString();
        NameEntry.Text = entry.Name;
        InfoLabel.Text = $"Aktueller Eintrag: {entry.Datum:dd.MM.yyyy} {entry.Betrag} {entry.Name}";
    }

    void OnCancelClicked(object? sender, EventArgs e)
    {
        if (this.Window is not null)
            Application.Current?.CloseWindow(this.Window);
        this.Window?.Close();
    }

    async void OnSaveClicked(object? sender, EventArgs e)
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

        bool success = await _service.UpdateEntryAsync(App.LoggedInUser,
                                                       _original.Datum, _original.Betrag, _original.Name,
                                                       datum, betrag, name);
        if (!success)
        {
            await DisplayAlert("Fehler", "Update fehlgeschlagen oder Eintrag existiert bereits", "OK");
            return;
        }

        if (this.Window is not null)
            Application.Current?.CloseWindow(this.Window);
        this.Window?.Close();
    }
}
