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
        public IActionResult Login(string userName, string password)
        {
            if (!string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password))
            {
                return RedirectToAction("Login");
            }

            //Check the user name and password  
            //Here can be implemented checking logic from the database  

            if (userName == "Admin" && password == "password")
            {

                //Create the identity for the user  
                var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, userName)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                var principal = new ClaimsPrincipal(identity);

                var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        
        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
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
            ValidateInput(username, password, confirmPassword);

            //Create User
            User user = new User(username, password, null, image, false);

            HomeController.db.InsertUser(ref user);
            return RedirectToAction("Index", "Home");
        }


        private bool ValidateInput(string username, string password, string confirmPassword)
        {
            //Check null or empty
            if (string.IsNullOrWhiteSpace(username))
            {
                ViewBag.UsernameError = "Username cannot be empty";
                ViewBag.ErrorCount++;
            }
            else
            {

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

            //Check if username exists


            return false;
        }
    }
}