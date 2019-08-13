using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoGameCompendium.Models
{
    public class User
    {
        public string ID;
        public string Username;
        public string Password;
        public string Bio;
        public string Image;
        public bool IsAdmin;
        List<Game> Collection = null;

        public User() { }

        public User(string username, string password, string bio, string image, bool isAdmin)
        {
            Username = username;
            Password = password;
            Bio = bio;
            Image = image;
            IsAdmin = isAdmin;

            Collection = new List<Game>();
        }
    }
}
