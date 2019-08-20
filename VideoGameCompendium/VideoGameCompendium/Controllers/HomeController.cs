using System;
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
            //User iaro = new User("Iaro", "pass", "cool", "hui", true);
            //User garrett = new User("Garrett", "pass", "meh", "hui", false);
            //User gage = new User("Gage", "pass", "meh", "hui", false);
            //db.InsertUser(ref iaro);
            //db.InsertUser(ref garrett);
            //db.InsertUser(ref gage);
            //
            //db.AddFollower(garrett.ID, iaro.ID);
            //db.AddFollower(gage.ID, iaro.ID);
            //int num = db.GetFollowersNum(iaro.ID);
            //int num2 = db.GetFollowersNum(garrett.ID);
            //var v = db.GetFollowers(iaro.ID);
            //var v3 = db.GetFollowers(garrett.ID);
            //var v1 = db.IsFollowing(garrett.ID, iaro.ID);
            //var v2 = db.IsFollowing(iaro.ID, garrett.ID);
            //db.Unfollow(garrett.ID, iaro.ID);

            return View();
        }
        
        public IActionResult Browse()
        {
            return View(db.BrowseGames());
        }

        [Authorize]
        [HttpGet]
        public IActionResult Collection()
        {
            //find user based on login
            //User user = db.GetUserByID(HttpContext.Request.Cookies["userID"]);

            //query user collection
            //List<Game> collection = db.GetCollection(user.ID);

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