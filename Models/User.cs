using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace assignment_2425.Models
{
    [Table("Users")]
    public class User
    {
        //creates primary key and automatically increases
        //creates unique username
        [PrimaryKey]
        [AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        //username of the user
        [Unique]
        [Column("username")]
        public string? Username { get; set; }

        //password of the user
        [Column("password")]
        public string? Password { get; set; }

        //email of the user
        [Column("email")]
        public string? Email { get; set; }

    }
}
