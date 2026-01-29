using assignment_2425.Database;
using assignment_2425.Models;

namespace assignment_2425;

[QueryProperty(nameof(UserId), "UserId")]
public partial class LoginPage : ContentPage
{
    private readonly LocalDbService _dbService;
    private bool _isRefreshing;

    //tracks the user id that is logged in
    private int _userId;
    public int UserId
    {
        get => _userId;
        set => _userId = value;
    }

    //tracks if the page is refreshing
    public bool IsRefreshing
    {
        get => _isRefreshing;
        set
        {
            _isRefreshing = value;
            OnPropertyChanged(nameof(IsRefreshing));
        }
    }

    //initializes the page ui
    public LoginPage()
    {
        InitializeComponent();
        _dbService = new LocalDbService();
        BindingContext = this; //binds the page to the view model
        updateView(); //updates the view
    }

    //used to update the view
    private void updateView()
    {
        bool isLoggedIn = Preferences.Get("IsLoggedIn", false);
        LoggedIn.IsVisible = isLoggedIn;
        LogIn.IsVisible = !isLoggedIn;
        //sets the text to speech toggle to the value stored in the preferences
        bool ttsEnabled = Preferences.Get("TTS_Enabled", false);
        ttsswitch.IsToggled = ttsEnabled;
    }

    //login button click event
    private async void onLoginClicked(object sender, EventArgs e)
    {
        var user = await _dbService.LoginUser(UsernameEntry.Text, PasswordEntry.Text);
        if (user != null)
        {
            Preferences.Set("IsLoggedIn", true); //sets the user as logged in
            Console.WriteLine($"UserId passed to FoodPage: {user.Id}");
            await DisplayAlert("Success", $"Welcome, {user.Username}!", "Ok");
            updateView(); //updates the view
        }
        else
        {
            await DisplayAlert("Error", "Invalid username or password", "Ok");
        }
    }

    //register button click event
    private async void onRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage(_dbService));
    }

    //logoff button click event
    private async void onLogoffClicked(object sender, EventArgs e)
    {
        Preferences.Set("IsLoggedIn", false);
        Preferences.Remove("CurrentUserId");
        await DisplayAlert("Logged Out", "You have logged out.", "Ok");
        updateView();
    }

    //go to the order page passing the user id
    private async void goOrderPage(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"OrderPage?UserId={_userId}");
    }

    //refreshes the page when the user pulls down
    private async void OnRefresh(object sender, EventArgs e)
    {
        await Task.Delay(2000);

        updateView();

        IsRefreshing = false;
    }

    //tts switch toggled event handler
    private async void onToggled(object sender, ToggledEventArgs e)
    {
        bool ttsEnabled = e.Value;
        Preferences.Set("TTS_Enabled", ttsEnabled);
        if (ttsEnabled == true)
        {
            await TextToSpeech.Default.SpeakAsync("Text to Speech is now enabled.");
            Console.WriteLine("Toggled On");
        }
        else
        {
            await TextToSpeech.Default.SpeakAsync("Text to Speech is now disabled.");
            Console.WriteLine("Toggled Off");
        }
    }

    //password text changed event handler to show the password when text is entered
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

    //for toggling dark mode
    private void onToggledDarkMode(object sender, ToggledEventArgs e)
    {
        SaveTheme.SetDarkMode(e.Value);
        SaveTheme.ApplyBackground(BackgroundImage);
    }

}
