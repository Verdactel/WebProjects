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
        public IActionResult CreateComment(string text, string senderId, string recieverId, DateTime postTime)
        {
            Comment comment = new Comment();

            comment.Text = text;
            comment.SenderId = ViewBag.User;
            comment.RecieverId = recieverId;
            comment.PostTime = DateTime.Now;

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

        [HttpPost]
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
            return View(user);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UserProfile(IFormFile file)
        {

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
                // This save the path to the record
                User user = db.GetUserByID(Request.Cookies["userID"]);
                System.IO.File.Delete(Path.Combine("", webPath + @"\Images\" + user.Image));
                user.Image = newFilename;
                db.EditUser(Request.Cookies["userID"], user);
            }
            return RedirectToAction("UserAccount", "Home");
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
    }
}