using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using VideoGameCompendium.Data;
using VideoGameCompendium.Models;

namespace VideoGameCompendium.Controllers
{
    public class AccountController : Controller
    {
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
            string id = Request.Cookies["userID"];

            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            Response.Cookies.Delete(id);

            return RedirectToAction("Index","Home");
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(string username, string password, string confirmPassword, string image)
        {
            //Validate User
            if(!ValidateInput(username, password, confirmPassword))
            {
                ViewBag.Username = username;
                return View();
            }

            //Create User
            User user = new User(username, password, "", image, false);

            HomeController.db.InsertUser(ref user);
            
            //Create the identity for the user  
            var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, username)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

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