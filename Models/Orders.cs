using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment_2425.Models
{
    [Table("Orders")]
    public class Orders
    {
        //creates primary key and automatically increases
        [PrimaryKey]
        [AutoIncrement]
        [Column("orderid")]
        public int OrderId { get; set; }

        //date of the order
        [Column("orderdate")]
        public DateTime OrderDate { get; set; }

        //order number
        [Column("ordernumber")]
        public int OrderNumber { get; set; }

        //user id of the user who made the order
        [Indexed]
        [Column("userid")]
        public int UserId { get; set; }

        //id of the item
        [Indexed]
        [Column("itemid")]
        public int ItemId { get; set; }

        //name of the item
        [Column("item")]
        public string? ItemName { get; set; }

        //price of the item
        [Column("price")]
        public float Price { get; set; }
    }
}
