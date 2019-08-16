﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoGameCompendium.Data;
using VideoGameCompendium.Models;

namespace VideoGameCompendium.Controllers
{
    public class HomeController : Controller
    {
        static public DBConnector db = new DBConnector();

        public IActionResult Index()
        {
            //var g = db.BrowseGames();
            //db.AddToCollection("UserIDTest1", "GameIDTest1");
            //db.AddToCollection("Bill", "Uncharted");
            //db.AddToCollection("Bill", "Battlefield");
            //db.AddToCollection("Bill", "Witcher 3");

            //db.AddToCollection("Stan", "Titan Quest");
            //db.AddToCollection("Stan", "Brawl stars");
            //db.AddToCollection("Stan", "Bloodborn");

            //List<Game> games = db.GetCollection("Bill");

            //db.AddToFavorites("UserIDTest1", "GameIDTest1");
            //db.RemoveFromFavorites("UserIDTest1", "GameIDTest1");
            List<Game> games = new List<Game>();

            Game g = db.GetGameByID(4423);
            Game g2 = db.GetGameByID(12862);
            Game g3 = db.GetGameByID(6033);

            games.Add(g);
            //games.Add(g2);
            games.Add(g3);

            return View(games);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Collection()
        {
            //find user based on login
            User user = db.GetUserByID(HttpContext.Request.Cookies["userID"]);

            //query user collection
            List<Game> collection = db.GetCollection(user.ID);

            //return user collection as list
            //return View(collection);
            return View();
        }

        [HttpPost]
        public IActionResult Collection(int userID)
        {
            return View();
        }
    }
}