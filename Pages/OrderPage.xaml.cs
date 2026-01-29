using assignment_2425.Database;
using assignment_2425.Models;
using System.Collections.ObjectModel;

namespace assignment_2425
{
    public partial class OrderPage : ContentPage
    {
        //handles the database service
        private readonly LocalDbService _localDbService;

        //holds the user orders to be displayed
        public ObservableCollection<Orders> UserOrders { get; set; } = new ObservableCollection<Orders>();

        //tracks if the page is refreshing
        private bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set
            {
                _isRefreshing = value;
                //updates the ui
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        //constructor that takes the database services as a parameter
        public OrderPage(LocalDbService dbService)
        {
            InitializeComponent();
            _localDbService = dbService ?? throw new ArgumentNullException(nameof(dbService));
            BindingContext = this;
            loadOrders(); //loads order when the page loads first
            SaveTheme.ApplyBackground(BackgroundImage);
        }

        //loads the past orders of the user
        private async void loadOrders()
        {
            //gets the current user id
            int currentUserId = Preferences.Get("CurrentUserId", 0);
            //gets the orders of the user
            var orders = await _localDbService.GetOrdersByUserId(currentUserId);
            //clears the existing orders
            UserOrders.Clear();
            foreach (var order in orders)
            {
                //adds the orders to the collection
                UserOrders.Add(order);
            }
        }

        //refreshes the page
        private async void onRefresh(object sender, EventArgs e)
        {
            await loadOrdersAsync(); //loads the orders

            IsRefreshing = false; //stops the refreshing
        }

        //same as loadOrders but as a Task for refresh support
        private async Task loadOrdersAsync()
        {
            int currentUserId = Preferences.Get("CurrentUserId", 0);
            var orders = await _localDbService.GetOrdersByUserId(currentUserId);

            UserOrders.Clear();
            foreach (var order in orders)
            {
                UserOrders.Add(order);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SaveTheme.ApplyBackground(BackgroundImage);
        }
    }
}
