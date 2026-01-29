using assignment_2425.Database;
using assignment_2425.Models;
using System.Collections.ObjectModel;

namespace assignment_2425;

[QueryProperty(nameof(UserId), "UserId")]
public partial class BasketPage : ContentPage
{
    private int _userId;
    //loads user basket items
    public int UserId
    {
        get => _userId;
        set
        {
            _userId = value;
            loadBasketItems();
        }
    }
    //keeps the ui in sync when items gets added or removed
    public ObservableCollection<BasketItem> BasketItems { get; set; } = new ObservableCollection<BasketItem>();
    private readonly LocalDbService _localDbService;
    public static bool OnCheckoutPage { get; set; } = false;
    private bool _isBasketEmpty = false;
    private bool _isRefreshing = false;

    public BasketPage(LocalDbService dbService)
    {
        InitializeComponent();
        _localDbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
        BindingContext = this;
        //gets the current logged in user id
        _userId = Preferences.Get("CurrentUserId", 0);
        Console.WriteLine($"My User ID is: {_userId}");
        //checks if the user is logged in, if they are it will proceed with loading the basket
        //if not it will redirect to the login page
        if (_userId > 0)
        {
            loadBasketItems();
        }
        else
        {
            DisplayAlert("Error", "You must be logged in.", "OK");
            Shell.Current.GoToAsync("//LoginPage");
        }
        SaveTheme.ApplyBackground(BackgroundImage); //for background image
    }

    //private async void LoadUsers()
    //{
    //    var users = await _localDbService.GetUsers();
    //    foreach (var user in users)
    //    {
    //        UserPicker.Items.Add(user.Username);
    //    }
    //}

    //private async void OnUserSelected(object sender, EventArgs e)
    //{
    //    if (UserPicker.SelectedIndex == -1) return;

    //    var selectedUser = UserPicker.Items[UserPicker.SelectedIndex];
    //    var user = await _localDbService.GetUserByUsername(selectedUser);
    //    if (user != null)
    //    {
    //        var basketItems = await _localDbService.GetBasketItems(user.Id);
    //        BasketItems.Clear();
    //        foreach (var item in basketItems)
    //        {
    //            BasketItems.Add(item);
    //        }
    //    }
    //}

    //UNCOMMENT TO BE ABLE TO PICK USER FROM DROPDOWN

    //loads user basket items
    private async Task loadBasketItems()
    {
        if (_userId <= 0) return;

        BasketItems.Clear();
        var basketItems = await _localDbService.GetBasketItems(_userId);

        if (basketItems.Count == 0)
        {
            Console.WriteLine($"No items found for User ID: {_userId}");
            await Shell.Current.GoToAsync("//FoodPage");
        }
        else
        {
            foreach (var item in basketItems)
            {
                Console.WriteLine($"Item: {item.Item}, Price: {item.Price}");
                BasketItems.Add(item); //updates the ui automatically
            }
        }
    }

    //used when clear basket is pressed
    private async void onClearBasketClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Confirm", "Are you sure you want to clear the basket?", "Yes", "No");
        if (!confirm)
        {
            return;
        }

        await _localDbService.DeleteBasketByUserId(_userId);

        BasketItems.Clear();
        await DisplayAlert("Success", "Your basket has been cleared.", "OK");
        OnAppearing(); //refreshes the page
    }

    //gets used whenever the page is shown
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!_isRefreshing)
        {
            _isRefreshing = true;
            await loadBasketItems();
            _isRefreshing = false;
        }

        //checks if the basket is empty and redirects to the food page if it is
        _isBasketEmpty = await checkIfBasketIsEmpty();
        if (_isBasketEmpty)
        {
            await DisplayAlert("Basket Empty", "Your basket is empty. Redirecting to the FoodPage.", "OK");
            await Shell.Current.GoToAsync("//FoodPage");
        }
        else
        {
            await refreshBasket();
        }

        _isRefreshing = false;

        SaveTheme.ApplyBackground(BackgroundImage); //for background image
    }

    //checks if there are no items in the basket if true then returns true
    private async Task<bool> checkIfBasketIsEmpty()
    {
        if (_userId <= 0) return true;

        var basketItems = await _localDbService.GetBasketItems(_userId);
        return basketItems.Count == 0;
    }
    
    //updates the basket ui
    private async Task refreshBasket()
    {
        if (_userId <= 0 || _isRefreshing) return;

        BasketItems.Clear();
        var basketItems = await _localDbService.GetBasketItems(_userId);

        foreach (var item in basketItems)
        {
            Console.WriteLine($"Item: {item.Item}, Price: {item.Price}");
            BasketItems.Add(item);
        }
    }

    //gets to the checkout page
    private async void goToCheckout(object sender, EventArgs e)
    {
        bool isTtsEnabled = Preferences.Get("TTS_Enabled", false);
        if (isTtsEnabled)
        {
            string textToRead = "Proceeding to the checkout page";
            var settings = new SpeechOptions()
            {
                Volume = 1.0f,
                Pitch = 1.1f
            };
            await TextToSpeech.Default.SpeakAsync(textToRead, settings);
            OnCheckoutPage = true;
            await Shell.Current.GoToAsync($"CheckoutPage?UserId={_userId}");
        }
        OnCheckoutPage = true;
        await Shell.Current.GoToAsync($"CheckoutPage?UserId={_userId}");
    }

    //when when the delete all button (bin) button is pressed it will clear the basket
    private async void onDeleteButtonClicked(object sender, EventArgs e)
    {
        var button = sender as ImageButton;
        var item = button?.BindingContext as BasketItem;
        if (item == null) return;
        bool confirm = await DisplayAlert("Confirm", $"Are you sure you want to remove {item.Item} from the basket?", "Yes", "No");
        if (!confirm) return;
        await _localDbService.DeleteBasketItem(item);
        BasketItems.Remove(item);
        OnAppearing(); //22491886
    }
}
