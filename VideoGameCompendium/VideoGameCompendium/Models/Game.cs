using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoGameCompendium.Models
{
    public class Game
    {
        readonly string Image;
        readonly string Title;
        readonly string[] Platforms;
        readonly string[] Genres;
        readonly string[] ESRB;
        readonly string[] ReleaseDate;
        readonly string Description;
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
