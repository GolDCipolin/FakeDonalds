using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using assignment_2425.Models;

namespace assignment_2425.Database
{
    public class LocalDbService
    {
        //name of the database
        private const string DB_NAME = "fakedonalds_db.db";
        //connection to the database
        private readonly SQLiteAsyncConnection _connection;

        public LocalDbService()
        {
            //path to the database
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "fakedonalds_db.db");
            _connection = new SQLiteAsyncConnection(dbPath);
            //enables foreign keys
            _connection.ExecuteAsync("PRAGMA foreign_keys = ON;").Wait();

            Console.WriteLine($"Database Path: {dbPath}");

            try
            {
                //creates the tables if they don't exist
                _connection.CreateTableAsync<Items>().Wait();
                _connection.CreateTableAsync<User>().Wait();
                _connection.CreateTableAsync<BasketItem>().Wait();
                _connection.CreateTableAsync<Orders>().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database failed: {ex.Message}");
            }
            //gives the database with default burgers and drinks
            SeedData();
        }

        //adds items if the table is empty
        private async void SeedData()
        {
            var ExistingItems = await _connection.Table<Items>().ToListAsync();
            if (ExistingItems.Count > 0)
            {
                Console.WriteLine("Items table already has data. Skipping seeding.");
                return;
            }

            Console.WriteLine("Seeding Items table with sample data.");

            await _connection.DeleteAllAsync<Items>();

            var SampleItems = new List<Items>
            {
                new Items { Item = "Fish Burger", Price = 5.99f, Drink = false, Image = "burger1.svg" },
                new Items { Item = "Beef Burger", Price = 3.99f, Drink = false, Image = "burger2.svg" },
                new Items { Item = "Double Beef Burger", Price = 2.99f, Drink = false, Image = "burger3.svg" },
                new Items { Item = "Chicken Burger", Price = 2.99f, Drink = false, Image = "burger4.svg" },
                new Items { Item = "Coke", Price = 1.99f, Drink = true, Image = "drink1.svg" },
                new Items { Item = "Sprite", Price = 1.99f, Drink = true, Image = "drink2.svg" },
                new Items { Item = "Oasis", Price = 1.99f, Drink = true, Image = "drink3.svg" },
                new Items { Item = "Water", Price = 3.99f, Drink = true, Image = "drink4.svg" }
            };

            await _connection.InsertAllAsync(SampleItems);
        }

        //registers the user if the username is not taken
        public async Task<bool> RegisterUser(User user)
        {
            var existingUser = await _connection.Table<User>().Where(x => x.Username == user.Username).FirstOrDefaultAsync();
            if (existingUser != null)
            {
                return false;
            }
            await _connection.InsertAsync(user);
            return true;
        }

        //logs the user in if the username and password match
        public async Task<User?> LoginUser(string username, string password)
        {
            var user = await _connection.Table<User>()
                        .Where(x => x.Username == username && x.Password == password)
                        .FirstOrDefaultAsync();

            if (user != null)
            {
                Preferences.Set("CurrentUserId", user.Id);
                Console.WriteLine($"Logged in as User ID: {user.Id}");
                return user;
            }
            else
            {
                Console.WriteLine("Login failed. User not found.");
                return null;
            }
        }

        //item queries for the database
        public async Task<List<Items>> GetBurgers()
        {
            return await _connection.Table<Items>().Where(x => x.Drink == false).ToListAsync();
        }

        public async Task<List<Items>> GetDrinks()
        {
            return await _connection.Table<Items>().Where(x => x.Drink == true).ToListAsync();
        }

        public async Task<List<Items>> GetItems()
        {
            return await _connection.Table<Items>().ToListAsync();
        }

        public async Task<Items> GetById(int id)
        {
            return await _connection.Table<Items>().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task Create(Items items)
        {
            await _connection.InsertAsync(items);
        }

        public async Task Update(Items items)
        {
            await _connection.UpdateAsync(items);
        }

        public async Task Delete(Items items)
        {
            await _connection.DeleteAsync(items);
        }


        // BASKET STUFF

        //adds an item to the basket
        public async Task AddToBasket(int userId, Items item)
        {
            var basketItem = new BasketItem
            {
                UserId = userId,
                ItemId = item.Id,
                Item = item.Item,
                Image = item.Image,
                Price = item.Price
            };
            await _connection.InsertAsync(basketItem);
            Console.WriteLine($"Item '{item.Item}' added to basket for User ID: {userId}");
        }

        //gets the basket items with the item names
        public async Task<List<BasketItem>> GetBasketWithItemNames(int userId)
        {
            var query = @"SELECT b.id, b.userid, b.itemid, i.item AS Item, b.price, b.image 
                  FROM BasketItems b 
                  JOIN Items i ON b.itemid = i.id 
                  WHERE b.userid = ?";

            var result = await _connection.QueryAsync<BasketItem>(query, userId);
            return result.ToList();
        }
        public async Task<List<User>> GetUsers()
        {
            return await _connection.Table<User>().ToListAsync();
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _connection.Table<User>().Where(x => x.Username == username).FirstOrDefaultAsync();
        }

        public async Task<List<BasketItem>> GetBasketByUserId(int userId)
        {
            var items = await _connection.Table<BasketItem>().Where(x => x.UserId == userId).ToListAsync();
            Console.WriteLine($"Found {items.Count} basket items for UserId {userId}");
            return items;
        }

        public async Task<List<BasketItem>> GetBasketItems(int userId)
        {
            if (_connection == null)
            {
                Console.WriteLine("SQLite connection is null.");
                return new List<BasketItem>();
            }

            try
            {
                return await _connection.Table<BasketItem>().Where(x => x.UserId == userId).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting items: {ex.Message}");
                return new List<BasketItem>();
            }
        }

        public async Task DeleteBasketItem(BasketItem item)
        {
            await _connection.DeleteAsync(item);
        }

        public async Task RecreateBasketTable()
        {
            await _connection.DropTableAsync<BasketItem>();
            await _connection.CreateTableAsync<BasketItem>();
            Console.WriteLine("Recreated BasketItem table with ItemName.");
        }

        public async Task DeleteBasketByUserId(int userId)
        {
            var query = "DELETE FROM BasketItems WHERE userid = ?";
            await _connection.ExecuteAsync(query, userId);
            Console.WriteLine($"Basket cleared for User ID: {userId}");
        }

        //order queries for the database
        public async Task<int> GetMaxOrderNumber()
        {
            var maxOrder = await _connection.Table<Orders>().OrderByDescending(o => o.OrderNumber).FirstOrDefaultAsync();
            return maxOrder?.OrderNumber ?? 0;
        }

        public async Task InsertOrder(Orders order)
        {
            await _connection.InsertAsync(order);
        }

        public async Task<List<Orders>> GetOrdersByUserId(int userId)
        {
            var orders = await _connection.Table<Orders>()
                                          .Where(o => o.UserId == userId)
                                          .OrderByDescending(o => o.OrderDate)
                                          .ToListAsync();
            Console.WriteLine($"Total Orders Found for UserId {userId}: {orders.Count}");
            return orders;
        }

        public async Task<List<Orders>> GetAllOrders()
        {
            var orders = await _connection.Table<Orders>().ToListAsync();
            foreach (var order in orders)
            {
                Console.WriteLine($"OrderId: {order.OrderId}, OrderNumber: {order.OrderNumber}, UserId: {order.UserId}, Price: {order.Price}, Date: {order.OrderDate}");
            }
            return orders;
        }
    }
}
