using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoGameCompendium.Models
{
    public class Game
    {
        public readonly string Image;
        public readonly string Title;
        public readonly string[] Platforms;
        public readonly string[] Genres;
        public readonly string[] ESRB;
        public readonly string[] ReleaseDate;
        public readonly string Description;
        //screenshots


        public Game(string image, string title, string[] platforms, string[] genres, string[] esrb, string[] releaseDate, string description)
        {
            Image = image;
            Title = title;
            Platforms = platforms;
            Genres = genres;
            ESRB = esrb;
            ReleaseDate = releaseDate;
            Description = description;
        }

    }
}
