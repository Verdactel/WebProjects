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

        [Authorize]
        [HttpGet]
        public IActionResult Collection()
        {
            //find user based on login
                //db.GetUserByID

            //query user collection
                //List<Game> collection = db.GetCollection(userId);

            //return user collection as list
                //return View(collection);
            return View();
        }
    }
}