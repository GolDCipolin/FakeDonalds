using assignment_2425.Database;
using assignment_2425.Models;
using System.Collections.ObjectModel;

namespace assignment_2425;

public partial class BurgerPage : ContentPage
{
    private readonly LocalDbService _dbService;
    private int _userId;
    //holds all the burger items to be displayed
    public ObservableCollection<Items> BurgersCollection { get; set; }
    //loads the burger items from the database
    public BurgerPage(LocalDbService dbService)
    {
        InitializeComponent();
        _dbService = dbService;
        _userId = Preferences.Get("CurrentUserId", 0);  
        BurgersCollection = new ObservableCollection<Items>();
        loadData();
        BindingContext = this;
        SaveTheme.ApplyBackground(BackgroundImage); //for background image
    }
    //just in case the constructor gets called without a parameter
    public BurgerPage() : this(new LocalDbService())
    {
    }
    //loads the burger items from the database
    private async void loadData()
    {
        BurgersCollection.Clear(); //clears other existing items
        var burgers = await _dbService.GetBurgers();
        foreach (var burger in burgers)
        {
            BurgersCollection.Add(burger); //adds the burger items to the collection
        }
    }
    //this gets used when the add basket button gets pressed
    private async void onAddToBasketClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button == null) return;

        button.IsEnabled = false;
        //gets the burger item that the button is binded to
        //confirms that the user is logged in
        var item = button.BindingContext as Items;
        int currentUserId = Preferences.Get("CurrentUserId", 0);
        //saves the item to the basket
        if (item != null && currentUserId > 0)
        {
            await _dbService.AddToBasket(currentUserId, item);

            //vibrates the device for 300 milliseconds (also to make sure it actually does it)
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(300));
            Console.WriteLine($"Vibrating for 300 milliseconds");

            await DisplayAlert("Success", $"{item.Item} added to basket!", "OK");

            //if the TTS is enabled it will read out
            bool isTtsEnabled = Preferences.Get("TTS_Enabled", false);
            if (isTtsEnabled)
            {
                string textToRead = $"{item.Item}, priced at {item.Price:F2} pounds, has been added to the basket.";
                var settings = new SpeechOptions()
                {
                    Volume = 1.0f,
                    Pitch = 1.1f 
                };
                await TextToSpeech.Default.SpeakAsync(textToRead, settings);
            }

            Console.WriteLine($"Added {item.Item} for User ID: {currentUserId}");
        }
        else
        {
            //if the user is not logged in it will display an error message
            await DisplayAlert("Error", "You must be logged in.", "OK");
            await Shell.Current.GoToAsync("//LoginPage");
        }
        button.IsEnabled = true;
    }

    //for background image
    protected override void OnAppearing()
    {
        base.OnAppearing();
        SaveTheme.ApplyBackground(BackgroundImage);
    }
}
