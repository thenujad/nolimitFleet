using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;

using nolimit.Data;

namespace YourNamespace.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _db; // Replace YourDbContext with your actual DbContext class

        public AccountController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if the user's credentials are valid
                var user = _db.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

                if (user != null)
                {
                    // Store the user's information in the session
                    HttpContext.Session.SetString("UserName", user.Username);
                    HttpContext.Session.SetInt32("UserId", user.Id); // Store the user ID

                    // You can store additional user information if needed

                    // Redirect the user to the desired page after successful login
                    return RedirectToAction("Index", "Home");
                }

                // If the credentials are invalid, display an error message
                ModelState.AddModelError(string.Empty, "Invalid username or password");
                ViewData["ErrorMessage"] = "Invalid username or password";

            }

            // If there are any validation errors, return to the login page with the model
            return View(model);
        }



        public async Task<IActionResult> Logout()
        {
            // Sign out the user
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Redirect the user to the login page
            return RedirectToAction("Login");
        }
    }
}
