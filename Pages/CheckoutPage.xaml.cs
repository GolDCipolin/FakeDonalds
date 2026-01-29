using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using System.Collections.ObjectModel;
using System.Net;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using System.IO;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Media;
using CommunityToolkit.Maui.Core;
using System.Globalization;
using System.Threading;
using ZXing;
using assignment_2425.Models;
using assignment_2425.Database;

namespace assignment_2425
{
    //userid is passed from the basket page
    [QueryProperty(nameof(UserId), "UserId")]
    public partial class CheckoutPage : ContentPage
    {

        private int _userId;
        public int UserId
        {
            get => _userId;
            set
            {
                _userId = value;
                Console.WriteLine($"UserId set to {_userId}");
            }
        }
        public ObservableCollection<BasketItem> BasketItems { get; set; } = new ObservableCollection<BasketItem>();
        private readonly LocalDbService _localDbService;

        public CheckoutPage(LocalDbService dbService)
        {
            InitializeComponent();
            _localDbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
            BindingContext = this;
            Console.WriteLine($"CheckoutPage Constructor: UserId = {_userId}");
            SaveTheme.ApplyBackground(BackgroundImage); //for background image
        }

        //used when page appears
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //checks if the user is on the checkout page
            if (!BasketPage.OnCheckoutPage)
            {
                Console.WriteLine("Redirecting back to BasketPage.");
                await Shell.Current.GoToAsync("//BasketPage");
                return;
            }
            //enables the camera for QR scanning and loading the basket items
            checkAndRequestCameraPermission();
            BarcodeReader.IsDetecting = true;

            if (_userId > 0)
            {
                await loadBasketItems();
            }
            else
            {
                Console.WriteLine("UserId is not valid yet.");
            }

            BasketPage.OnCheckoutPage = false;

            SaveTheme.ApplyBackground(BackgroundImage); //for background image
        }
        //loads the basket items from the database
        private async Task loadBasketItems()
        {
            Console.WriteLine($"Loading basket items for UserId = {_userId}");
            var items = await _localDbService.GetBasketByUserId(_userId);
            foreach (var item in items)
            {
                BasketItems.Add(item);
            }
            Console.WriteLine($"BasketItems loaded with count: {BasketItems.Count}");
        }

        //used when the user picks a delivery option
        private void onDeliveryOptionChanged(object sender, EventArgs e)
        {
            //hides all the options
            TableDeliveryLayout.IsVisible = false;
            HomeDeliveryLayout.IsVisible = false;
            StoreCollectionLayout.IsVisible = false;

            //shows whichevers selected
            switch (DeliveryOptionPicker.SelectedIndex)
            {
                case 0: TableDeliveryLayout.IsVisible = true;
                    BarcodeReader.IsDetecting = true;
                    break;
                case 1: HomeDeliveryLayout.IsVisible = true; break;
                case 2: StoreCollectionLayout.IsVisible = true; break;
            }
        }

        //used when the user confirms the home delivery
        private async void onConfirmHomeDeliveryClicked(object sender, EventArgs e)
        {
            var address = HomeAddressEntry.Text;
            if (!string.IsNullOrWhiteSpace(address))
            {
                await DisplayAlert("Home Delivery", $"Your order will be delivered to: {address}", "OK");
                //checks if the TTS is enabled if it is then reads out the address
                bool isTtsEnabled = Preferences.Get("TTS_Enabled", false);
                if (isTtsEnabled)
                {
                    string textToRead = $"Order complete! It will be delivered to the address {address}";
                    var settings = new SpeechOptions()
                    {
                        Volume = 1.0f,
                        Pitch = 1.1f
                    };
                    await Shell.Current.GoToAsync("//MainPage");
                    await TextToSpeech.Default.SpeakAsync(textToRead, settings);
                    await createOrder(_userId, BasketItems.ToList());
                }
                else
                {
                    await createOrder(_userId, BasketItems.ToList());
                    await Shell.Current.GoToAsync("//MainPage");
                }
            }
            else
            {
                await DisplayAlert("Error", "Please enter a valid address.", "OK");
            }
        }

        //used when the user uses speech-to-text to enter the address
        private async void onSpeakAddressClicked(object sender, EventArgs e)
        {
            var isPermissionGranted = await SpeechToText.Default.RequestPermissions(CancellationToken.None);
            if (!isPermissionGranted)
            {
                await DisplayAlert("Permission Denied", "Microphone access is required for speech recognition.", "OK");
                return;
            }

            try
            {
                Console.WriteLine("Starting speech recognition...");

                var speechOptions = new SpeechToTextOptions
                {
                    Culture = new CultureInfo("en-US")
                };

                await SpeechToText.Default.StartListenAsync(speechOptions);
                //used when the speech is recognized
                SpeechToText.Default.RecognitionResultUpdated += (s, resultArgs) =>
                {
                    var interimResult = resultArgs.RecognitionResult;
                    if (!string.IsNullOrWhiteSpace(interimResult))
                    {
                        Console.WriteLine($"Interim result: {interimResult}");
                    }
                };

                //used when the speech is finished
                SpeechToText.Default.RecognitionResultCompleted += async (s, resultArgs) =>
                {
                    var finalResult = resultArgs.RecognitionResult?.Text;
                    if (!string.IsNullOrWhiteSpace(finalResult))
                    {
                        HomeAddressEntry.Text = finalResult;
                        Console.WriteLine($"Final result: {finalResult}");
                    }
                    else
                    {
                        await DisplayAlert("No Input", "No speech was recognized. Please try again.", "OK");
                    }
                };
                //waits for 10 seconds before stopping the speech recognition
                await Task.Delay(10000);
                await SpeechToText.Default.StopListenAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Speech recognition failed: {ex.Message}", "OK");
            }
        }

        //used when the user confirms the store collection
        private async void onFindNearestStoreClicked(object sender, EventArgs e)
        {
            try
            {
                var location = await Geolocation.Default.GetLocationAsync();
                if (location != null)
                {
                    Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}");
                    await DisplayAlert("Location Found", $"Your location: {location.Latitude}, {location.Longitude}", "OK");
                    await DisplayAlert("Order Confirmed", "Your order has been placed!", "OK");
                    //checks if the TTS is enabled if it is then reads out the order confirmation
                    bool isTtsEnabled = Preferences.Get("TTS_Enabled", false);
                    if (isTtsEnabled)
                    {
                        string textToRead = $"Order complete! It will be delivered to the nearest store to you.";
                        var settings = new SpeechOptions()
                        {
                            Volume = 1.0f,
                            Pitch = 1.1f
                        };
                        await Shell.Current.GoToAsync("//MainPage");
                        await TextToSpeech.Default.SpeakAsync(textToRead, settings);
                        await createOrder(_userId, BasketItems.ToList());
                    }
                    await Shell.Current.GoToAsync("//MainPage");
                    await createOrder(_userId, BasketItems.ToList());
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to get location: {ex.Message}", "OK");
            }
        }

        //used when the user confirms the table delivery
        private void onBarcodeDetected(object sender, BarcodeDetectionEventArgs e)
        {
            var result = e.Results.FirstOrDefault();
            if (result != null)
            {
                Console.WriteLine($"QR Code Detected: {result.Value}");

                BarcodeReader.IsDetecting = false;

                Dispatcher.Dispatch(async () =>
                {
                    if (result.Value.StartsWith("Table", StringComparison.OrdinalIgnoreCase))
                    {
                        await DisplayAlert("QR Code Scanned", $"Table Code: {result.Value}", "OK");
                        await createOrder(_userId, BasketItems.ToList());
                        await Shell.Current.GoToAsync("//MainPage");
                    }
                    else
                    {
                        await DisplayAlert("Invalid QR Code", "The scanned QR code is not a valid table code. Please try again.", "OK");
                        BarcodeReader.IsDetecting = true;
                    }
                });
            }
            else
            {
                Console.WriteLine("No QR code detected.");
            }
        }

        //checks and requests the camera permission
        private async Task checkAndRequestCameraPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Camera>();
            }

            if (status == PermissionStatus.Granted)
            {
                Console.WriteLine("Camera permission granted.");
                BarcodeReader.IsDetecting = true;
            }
            else
            {
                await DisplayAlert("Permission Denied", "Camera access is required in order to scan QR codes.", "OK");
            }
        }

        //creates an order for the user
        private async Task createOrder(int userId, List<BasketItem>? basketItems)
        {
            if (basketItems == null || basketItems.Count == 0)
            {
                Console.WriteLine("BasketItems is either empty or null");
                return;
            }

            Console.WriteLine($"Making an order for UserId: {userId}. Basket item count: {basketItems.Count}");

            var maxOrderNumber = await _localDbService.GetMaxOrderNumber();
            int newOrderNumber = maxOrderNumber + 1;

            foreach (var item in basketItems)
            {
                var order = new Orders
                {
                    OrderDate = DateTime.Now,
                    OrderNumber = newOrderNumber,
                    UserId = userId,
                    ItemId = item.ItemId,
                    ItemName = item.Item,
                    Price = item.Price
                };

                await _localDbService.InsertOrder(order);
                Console.WriteLine($"Order: OrderId={order.OrderId}, Order number={order.OrderNumber}, User id={order.UserId}");
            }
            //clears the basket after the order is made
            await _localDbService.DeleteBasketByUserId(userId);
            BasketItems.Clear();
        }

    }
}
