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
            var v = db.BrowseGames("Desert");

            // -- Garrett Test Section -- //
            //bool success = db.RemoveComment("5d5b545607e4373938d0ee63");
            //bool success = db.AddComment("Nice", "123", "Stan");
            Comment comment = new Comment("Niiiiiiice", "Stan", "123");
            comment.Id = "5d5b54ae9268f82ff822c961";
            bool success = db.EditComment(comment);

            return View(v);
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