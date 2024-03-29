﻿using System;
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
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public IActionResult Browse(string search = "", string platform = "", string genre = "", int esrb = 12)
        {
            #region Get Genres
            IEnumerable<string> genres = db.GetGenres();
            SelectList genreList = new SelectList(genres);
            ViewBag.Genres = genreList;
            #endregion

            #region Get ESRB
            List<SelectListItem> esrbRatings = new List<SelectListItem>();
            List<string> esrbCode = new List<string>() { "EC", "E", "E 10+", "T", "M", "A" };
            List<int> esrbId = new List<int>() { 7, 8, 9, 10, 11, 12 };

            for (int i = 0; i < esrbCode.Count; i++)
            {
                esrbRatings.Add(new SelectListItem(esrbCode[i], esrbId[i].ToString()));
            }

            ViewBag.ESRB = esrbRatings;
            #endregion
            List<Game> toReturn = db.BrowseGames(search, platform, genre, esrb);
            return View(toReturn);

        }

        [HttpPost]
        public IActionResult CreateComment(string text, string userId, string recieverId)
        {
            Comment comment = new Comment(text, userId, recieverId);

            #region Get ESRB
            List<SelectListItem> esrbRatings = new List<SelectListItem>();
            List<string> esrbCode = new List<string>() { "EC", "E", "E 10+", "T", "M", "A" };
            List<int> esrbId = new List<int>() { 7, 8, 9, 10, 11, 12 };

            for (int i = 0; i < esrbCode.Count; i++)
            {
                esrbRatings.Add(new SelectListItem(esrbCode[i], esrbId[i].ToString()));
            }

            ViewBag.ESRB = esrbRatings;
            #endregion
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

        [HttpGet]
        public IActionResult UserProfile(string id)
        {
            User user = db.GetUserByID(id);
            ViewBag.Comments = db.GetComments(id);
            ViewBag.Followers = db.GetFollowers(id);
            ViewBag.Favorites = db.GetFavorites(id);
            ViewBag.User = db.GetUserByID(Request.Cookies["userID"]);
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
        public async Task<IActionResult> UserAccount(string id, string bio, IFormFile file)
        {
            var filetest = file;
            if (bio == null && filetest == null)
            {
                return RedirectToAction("UserAccount", "Home");
            }
            User user = db.GetUserByID(id);

            if (bio == null)
            {
                if (user.Bio == null)
                {
                    user.Bio = "";
                }
            }
            else
            {
                user.Bio = bio;
            }
            // Code to upload image if not null
            if (filetest != null)
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
            }
            db.EditUser(id, user);

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

            if (game != null)
                return View(game);
            else
                return RedirectToAction("Index");
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
                return new JsonResult(db.GetCollection(userId).Select(x => x.Id).ToList().Contains(gameId) ? 1 : 2);
        }

        [HttpGet]
        public JsonResult DoFavorites(int gameId, string userId)
        {
            bool result = false;
            var val = db.GetFavorites(userId).Select(x => x.Id).ToList().Contains(gameId);
            if (db.GetFavorites(userId).Select(x => x.Id).ToList().Contains(gameId))
                result = db.RemoveFromFavorites(userId, gameId.ToString());
            else
                result = db.AddToFavorites(userId, gameId.ToString());
            if(!result){
                return new JsonResult("Error");
            }
            else
                return new JsonResult(db.GetFavorites(userId).Select(x => x.Id).ToList().Contains(gameId) ? 1 : 2);

        }
        public JsonResult DoFollow(string loggedInId, string leaderId)
        {
            bool result = false;

            if (db.IsFollowing(loggedInId, leaderId))
                result = db.Unfollow(loggedInId, leaderId);
            else
                result = db.AddFollower(loggedInId, leaderId);

            if (!result)
                return new JsonResult("Error");
            else
                return new JsonResult(db.IsFollowing(loggedInId, leaderId) ? 1 : 2);
        }

        [HttpPost]
        public IActionResult PostCommentGame(string text, string userId, int gameId)
        {
            Comment comm = new Comment(text, userId, gameId.ToString());
            db.AddComment(ref comm);
            return RedirectToAction("Game", "Home", new { id = gameId });
        }

        [HttpPost]
        public IActionResult PostCommentUser(string text, string userId, string receiverId)
        {
            Comment comm = new Comment(text, userId, receiverId);
            db.AddComment(ref comm);
            return RedirectToAction("UserProfile", "Home", new { id = receiverId });
        }
    }
}