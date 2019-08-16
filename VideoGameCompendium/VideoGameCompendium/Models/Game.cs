using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoGameCompendium.Models
{
    public class Game
    {
        public int Id;
        public string Image;
        public string Title;
        public string Description;
        public List<string> Platforms;
        public List<string> Genres;
        public string ESRB;
        public DateTime ReleaseDate;

        public Game(int id, string title, string description, DateTime releaseDate, string image)
        {
            Id = id;
            Title = title;
            Description = description;
            ReleaseDate = releaseDate;
            Platforms = new List<string>();
            Genres = new List<string>();
            Image = image;
        }

    }
}
