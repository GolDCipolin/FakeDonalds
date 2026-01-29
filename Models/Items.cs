using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment_2425.Models
{
    //makes the table name "Items"
    [SQLite.Table("Items")]
    public class Items
    {
        //creates primary key and automatically increases
        [PrimaryKey]
        [AutoIncrement]
        [SQLite.Column("id")]
        public int Id { get; set; }
        //name of the item
        [SQLite.Column("item")]
        public string? Item { get; set; }
        //price of the item
        [SQLite.Column("price")]
        public float Price { get; set; }
        //if the item is a food item
        [SQLite.Column("drink")]
        public bool Drink { get; set; }
        //image of the item
        [SQLite.Column("image")]
        public string? Image { get; set; }

    }
}
