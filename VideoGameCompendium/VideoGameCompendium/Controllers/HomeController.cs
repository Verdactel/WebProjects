using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoGameCompendium.Data;
using VideoGameCompendium.Models;
using System.IO;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;

namespace VideoGameCompendium.Controllers
{
    public class HomeController : Controller
    {
        static public DBConnector db = new DBConnector();

        private readonly IFileProvider _fileProvider;
        private readonly IHostingEnvironment _hostingEnvironment;
        public HomeController(IFileProvider fileprovider, IHostingEnvironment env)
        {
            _fileProvider = fileprovider;
            _hostingEnvironment = env;
        }

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
        public IActionResult CreateComment(string text, string userId, string recieverId)
        {
            Comment comment = new Comment(text, userId, recieverId);



            db.AddComment(ref comment);
            return RedirectToAction("Game", "Home");
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
            if (collection != null)
            {
                return View(collection);
            }
            return View();
        }

        [HttpGet("{userID}")]
        public IActionResult Collection(string userID)
        {
            List<Game> collection = db.GetCollection(userID);
            if (collection != null)
            {
                return View(collection);
            }
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult UserProfile()
        {
            User user = db.GetUserByID(Request.Cookies["userID"]);
            ViewBag.Comments = db.GetComments(user.ID);
            ViewBag.Followers = db.GetFollowers(user.ID);
            return View(user);
        }

        [HttpGet("{id}")]
        public IActionResult UserProfile(string id)
        {
            User user = db.GetUserByID(id);
            ViewBag.Comments = db.GetComments(id);
            ViewBag.Followers = db.GetFollowers(id);
            return View(user);
        }

        [Authorize]
        [HttpGet]
        public IActionResult UserAccount()
        {
            User user = db.GetUserByID(Request.Cookies["userID"]);
            return View(user);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UserAccount(string id, string bio, string prevBio, IFormFile file)
        {
            User user = db.GetUserByID(id);

            if (bio == null)
            {
                if(prevBio == null)
                {
                    bio = "";
                }
                bio = prevBio;
            }

            user.Bio = bio;
            db.EditUser(id, user);

            // Code to upload image if not null
            if (file != null || file.Length != 0)
            {
                #region Image
                // Create a File Info 
                FileInfo fi = new FileInfo(file.FileName);

                // This code creates a unique file name to prevent duplications 
                // stored at the file location
                var newFilename = file.FileName + "_" + string.Format("{0:d}",
                                  (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                var webPath = _hostingEnvironment.WebRootPath;
                var path = Path.Combine("", webPath + @"\Images\" + newFilename);

                // This stream the physical file to the allocate wwwroot/ImageFiles folder
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                #endregion
                // Save the path to the record
                System.IO.File.Delete(Path.Combine("", webPath + @"\Images\" + user.Image));
                user.Image = newFilename;
                db.EditUser(Request.Cookies["userID"], user);
            }
            //Response.Cookies.Delete("userID");

            return RedirectToAction("UserAccount", "Home");
        }

        [HttpGet]
        public IActionResult Game(int id)
        {
            Game game = db.GetGameByID(id);

            User user = db.GetUserByID(Request.Cookies["userID"]);
            ViewBag.User = user;

            List<Comment> comments = db.GetComments(id.ToString());
            ViewBag.Comments = comments;
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

        [HttpPost]
        public IActionResult PostCommentGame([Bind("text, SenderId, RecieverId, PostTime")] Comment comment, int gameID)
        {
            Game game = db.GetGameByID(gameID);
            db.AddComment(ref comment);
            return RedirectToAction("Game", "Home", game);
        }

        [HttpPost]
        public IActionResult PostCommentUser([Bind("text, SenderId, RecieverId, PostTime")] Comment comment, string userID)
        {
            User user = db.GetUserByID(userID);
            db.AddComment(ref comment);
            return RedirectToAction("UserProfile", "Home", user);
        }
    }
}