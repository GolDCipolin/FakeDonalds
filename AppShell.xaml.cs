using Microsoft.Maui.Storage;
using System.Diagnostics;
using Microsoft.Maui.Controls;

namespace assignment_2425
{
    public partial class AppShell : Shell
    {
        private int currentUserId;

        public AppShell()
        {
            InitializeComponent();

            //gets the user id sets 0 if it cannot find the user id
            currentUserId = Preferences.Get("CurrentUserId", 0);
            //registers all pages allows to navigate to them
            Routing.RegisterRoute(nameof(BasketPage), typeof(BasketPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(RegisterPage), typeof(RegisterPage));
            Routing.RegisterRoute(nameof(FoodPage), typeof(FoodPage));
            Routing.RegisterRoute(nameof(BurgerPage), typeof(BurgerPage));
            Routing.RegisterRoute(nameof(DrinkPage), typeof(DrinkPage));
            Routing.RegisterRoute(nameof(OrderPage), typeof(OrderPage));
            Routing.RegisterRoute(nameof(CheckoutPage), typeof(CheckoutPage));
            //event handlers for navigation events
            Navigating += appShellNavigating;
            Navigated += onNavigated;
        }

        //runs before navigation happens
        private async void appShellNavigating(object? sender, ShellNavigatingEventArgs e)
        {
            //refreshes the user id
            currentUserId = Preferences.Get("CurrentUserId", 0);

            //if (_currentUserId == 0)
            //{
            //    await DisplayAlert("Warning", "You must be logged in.", "OK");
            //    //await Shell.Current.GoToAsync("//LoginPage");
            //    return;
            //}
            //debug message
            if (e.Target.Location.OriginalString.Contains("BasketPage"))
            {
                Console.WriteLine($"Navigating to the BasketPage UserId: {currentUserId}");
            }
        }

        //runs after navigation happens
        private async void onNavigated(object sender, ShellNavigatedEventArgs e)
        {
            //check if TTS is enabled
            bool isTtsEnabled = Preferences.Get("TTS_Enabled", false);
            //if TTS is not enabled return
            if (!isTtsEnabled)
            {
                return;
            }
            //gets current page name from navigation
            string tabTitle = e.Current.Location.OriginalString ?? "unknown";
            //if page valid then speak it
            if (!string.IsNullOrEmpty(tabTitle))
            {
                await TextToSpeech.Default.SpeakAsync($"Switching to {tabTitle} tab.");
                Console.WriteLine($"TTS: Switching to {tabTitle}");
            }
        }




    }
}
