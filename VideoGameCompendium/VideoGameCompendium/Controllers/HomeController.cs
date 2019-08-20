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