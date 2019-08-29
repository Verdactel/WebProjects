using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using VideoGameCompendium.Data;
using VideoGameCompendium.Models;

namespace VideoGameCompendium.Controllers
{
    public class AccountController : Controller
    {
        private readonly IFileProvider _fileProvider;
        private readonly IHostingEnvironment _hostingEnvironment;
        public AccountController(IFileProvider fileprovider, IHostingEnvironment env)
        {
            _fileProvider = fileprovider;
            _hostingEnvironment = env;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            //Check the user name and password  
            //Here can be implemented checking logic from the database  
            User user = HomeController.db.CheckLogin(username, password);
            if (user != null)
            {
                //Create the identity for the user  
                var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, username)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Response.Cookies.Append("userID", user.ID, new Microsoft.AspNetCore.Http.CookieOptions()
                {
                    Path = "/",
                    HttpOnly = false,
                    IsEssential = true //<- there
                });
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login", "Account");
        }
        
        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            Response.Cookies.Delete("userID");

            return RedirectToAction("Index","Home");
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(string username, string password, string confirmPassword, string image, IFormFile file)
        {
            //Validate User
            if(!ValidateInput(username, password, confirmPassword))
            {
                ViewBag.Username = username;
                return View();
            }

            #region Image

            User user = null;
            var filetest = file;
            // Code to upload image if not null
            if (filetest != null)
            {
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

                //Create user
                user = new User(username, password, "", newFilename, false);
            }
            else
            {
                user = new User(username, password, "", "DefaultUser.png", false);
            }
            #endregion

            HomeController.db.InsertUser(ref user);
            
            //Create the identity for the user  
            var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, username)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            HttpContext.Response.Cookies.Append("userID", user.ID, new Microsoft.AspNetCore.Http.CookieOptions()
            {
                Path = "/",
                HttpOnly = false,
                IsEssential = true
            });

            return RedirectToAction("Index", "Home");
        }


        private bool ValidateInput(string username, string password, string confirmPassword)
        {
            //Set Viewbag
            ViewBag.ErrorCount = 0;

            //Check null or empty
            if (string.IsNullOrWhiteSpace(username))
            {
                ViewBag.UsernameError = "Username cannot be empty";
                ViewBag.ErrorCount++;
            }
            else
            {
                //Check if username exists
                if (HomeController.db.CheckForUsername(username))
                {
                    ViewBag.UsernameError = "Username already exists";
                    return false;
                }
            }

            if(string.IsNullOrWhiteSpace(password))
            {
                ViewBag.PasswordError = "Password cannot be empty";
                ViewBag.ErrorCount++;
            }
            else
            {
                if (!password.Equals(confirmPassword))
                {
                    ViewBag.PasswordError = "Passwords do not match";
                    ViewBag.ErrorCount++;
                }
            }

            //Check for character minimum/maximum


            return ViewBag.ErrorCount == 0;
        }
    }
}