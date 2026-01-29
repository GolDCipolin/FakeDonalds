using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment_2425.Models
{
    //table name
    [Table("BasketItems")]
    public class BasketItem
    {
        //creates primary key and automatically increases
        [PrimaryKey]
        [AutoIncrement]
        [Column("id")]
        public int Id { get; set; }
        //store user id of whos added the item
        [Indexed]
        [Column("userid")]
        public int UserId { get; set; }
        //gets id of food or drink item that gets added to the basket
        [Indexed]
        [Column("itemid")]
        public int ItemId { get; set; }
        //name of the item
        [Column("item")]
        public string? Item { get; set; }
        //price of the item
        [Column("price")]
        public float Price { get; set; }
        //image of the item
        [Column("image")]
        public string? Image { get; set; }
        //when it got added
        public DateTime OrderDate { get; set; }
        //representing the whole order this item belongs to
        [Column("ordernumber")]
        public int OrderNumber { get; set; }
    }
}