using assignment_2425.Database;
using assignment_2425.Models;

namespace assignment_2425;

public partial class RegisterPage : ContentPage
{
    //handles the database service
    private readonly LocalDbService _dbService;

    //constructor that gets the database service
    public RegisterPage(LocalDbService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
        SaveTheme.ApplyBackground(BackgroundImage); //for background image
    }

    //runs when register button is pressed
    private async void onRegisterClicked(object sender, EventArgs e)
    {
        //creates a new user object
        var user = new User
        {
            Username = UsernameEntry.Text,
            Email = EmailEntry.Text,
            Password = PasswordEntry.Text
        };

        //registers the user in the database
        bool success = await _dbService.RegisterUser(user);
        //if the registration is successful it will display a success message
        if (success)
        {
            await DisplayAlert("Success", "Registration successful!", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            //if the username is already taken it will display an error message
            await DisplayAlert("Error", "Username already exists!", "OK");
        }
    }

    //refreshes password field to hide the password
    private void passwordTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.NewTextValue))
        {
            PasswordEntry.IsPassword = false;
            PasswordEntry.IsPassword = true;
        }
    }

    //show password checkbox changed event handler
    private void OnShowPasswordChanged(object sender, CheckedChangedEventArgs e)
    {
        PasswordEntry.IsPassword = !e.Value;
    }

    //for background image
    protected override void OnAppearing()
    {
        base.OnAppearing();
        SaveTheme.ApplyBackground(BackgroundImage);
    }
}