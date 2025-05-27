namespace FinanzApp;

public partial class LoginView : ContentPage
{
    const string ValidUser = "Stefan";
    const string ValidPass = "1234";

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
        if (usernameEntry.Text == ValidUser && passwordEntry.Text == ValidPass)
        {
            await Shell.Current.GoToAsync(nameof(HomeView));
        }
        else
        {
            await DisplayAlert("Fehler", "Ung\u00fcltige Anmeldedaten", "OK");
        }
    }
}
