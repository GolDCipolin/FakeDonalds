using assignment_2425.Database;
using assignment_2425.Models;

namespace assignment_2425;

//lets the parameter be passed from the previous page
[QueryProperty(nameof(DbService), "DbService")]
[QueryProperty(nameof(UserId), "UserId")]
public partial class FoodPage : ContentPage
{
    private LocalDbService? _dbService;
    private int _userId;

    //the properties allows the navigation to pass the parameters

    public LocalDbService? DbService
    {
        get => _dbService;
        set => _dbService = value;
    }

    public int UserId
    {
        get => _userId;
        set => _userId = value;
    }

    //initializes the page ui
    public FoodPage()
    {
        InitializeComponent();
        SaveTheme.ApplyBackground(BackgroundImage); //for background image
    }

    //used when the category button is pressed
    private async void categoryClick(object sender, EventArgs e)
    {
        //if (_dbService == null || _userId <= 0)
        //{
        //    await DisplayAlert("Error", "User information is missing.", "OK");
        //    return;
        //}

        var button = sender as Button;
        if (button != null)
        {
            //navigates to the selected category page
            if (button.Text == "Burgers")
            {
                await Shell.Current.GoToAsync(nameof(BurgerPage), new Dictionary<string, object>
            {
                { "DbService", _dbService },
                { "UserId", _userId }
            });
            }
            //navigates to the selected category page
            else if (button.Text == "Drinks")
            {
                await Shell.Current.GoToAsync(nameof(DrinkPage), new Dictionary<string, object>
            {
                { "DbService", _dbService },
                { "UserId", _userId }
            });
            }
        }
    }

    //for background image
    protected override void OnAppearing()
    {
        base.OnAppearing();
        SaveTheme.ApplyBackground(BackgroundImage);
    }

}
