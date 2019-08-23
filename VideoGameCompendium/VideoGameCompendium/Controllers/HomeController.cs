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
			
            return View();
        }

        [HttpGet]
        public IActionResult Browse()
        {
            return View(db.BrowseGames());
        }

        [HttpPost]
        public IActionResult Browse(string search)
        {
            if (search != null)
            {
                return View(db.BrowseGames(search));
            }

            return View(db.BrowseGames());
        }

        [Authorize]
        [HttpGet]
        public IActionResult Collection()
        {
            //find user based on login
            User user = db.GetUserByID(Request.Cookies["userID"]);

            //query user collection
            List<Game> collection = db.GetCollection(user.ID);

            //return user collection as list
            if(collection != null)
            {
                return View(collection);
            }
            return View();
        }

        [HttpPost]
        public IActionResult Collection(string userID)
        {
            List<Game> collection = db.GetCollection(userID);
            if(collection != null)
            {
                return View(collection);
            }
            return View();
        }

        [HttpGet]
        public IActionResult UserProfile()
        {
            User user = db.GetUserByID(Request.Cookies["userID"]);
            return View(user);
        }

        [HttpGet]
        public IActionResult UserAccount()
        {
            User user = db.GetUserByID(Request.Cookies["userID"]);
            return View(user);
        }

        [HttpPost]
        public IActionResult UserAccount(string id, string bio, string prevBio)
        {
            User user = db.GetUserByID(id);

            if (bio == null) bio = prevBio;

            user.Bio = bio;
            db.EditUser(id, user);

            Response.Cookies.Delete("userID");

            return RedirectToAction("UserAccount", "Home");
        }

        [HttpGet]
        public IActionResult Game(int id)
        {
            Game game = db.GetGameByID(id);

            User user = db.GetUserByID(Request.Cookies["userID"]);
            ViewBag.User = user;

            //if (game != null)
            return View(game);
            //else
            //    return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult Rate(int stars, int gameId, string userId)
        {
            bool result = db.RateGame(gameId, userId, stars);
            if (!result)
                result = db.EditRating(gameId, userId, stars);

            if (!result)
                return new JsonResult(result);
            else
                return new JsonResult(db.GetAverageRating(gameId));
        }

        [HttpGet]
        public JsonResult DoCollection(int gameId, string userId)
        {
            bool result = false;

            if (db.GetCollection(userId).Select(x => x.Id).ToList().Contains(gameId))
                result = db.RemoveFromCollection(userId, gameId.ToString());
            else
                result = db.AddToCollection(userId, gameId.ToString());

            if (!result)
                return new JsonResult("Error");
            else
                return new JsonResult(db.GetCollection(userId).Select(x=>x.Id).ToList().Contains(gameId) ? 1 : 2);
        }
    }
}