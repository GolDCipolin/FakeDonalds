using assignment_2425.Database;
using assignment_2425.Models;
using System.Collections.ObjectModel;

namespace assignment_2425
{
    public partial class MainPage : ContentPage
    {
        //handles the database service
        private readonly LocalDbService _dbService;

        //holds the burger items to be displayed
        public ObservableCollection<Items> BurgersCollection { get; set; }
        public ObservableCollection<Items> DrinksCollection { get; set; }

        //loads the burger items from the database
        public MainPage()
        {
            InitializeComponent();
            _dbService = new LocalDbService(); //initializes the database service

            //initializes the burger and drink collections
            BurgersCollection = new ObservableCollection<Items>();
            DrinksCollection = new ObservableCollection<Items>();
            loadData();
            //lets xaml to access the collections
            BindingContext = this;

            SaveTheme.ApplyBackground(BackgroundImage);
        }

        //loads the burger and drink items from the database
        private async void loadData()
        {
            await Task.Delay(1000); //delays the loading for 1 second
            //gets the burger and drink items from the database
            var burgers = await _dbService.GetBurgers();
            var drinks = await _dbService.GetDrinks();

            //clears the existing items
            BurgersCollection.Clear();
            DrinksCollection.Clear();

            //adds the burger and drink items to the collections
            foreach (var burger in burgers)
                BurgersCollection.Add(burger);

            foreach (var drink in drinks)
                DrinksCollection.Add(drink);

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            SaveTheme.ApplyBackground(BackgroundImage);
        }

    }
}
