using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NewsPaperWebApp.Models;
using System.Threading.Tasks;

namespace NewsPaperWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Ensure password is not null
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("", "Password cannot be empty.");
                return View(model);
            }

            var user = new IdentityUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["SuccessMessage"] = "Registration successful!";
                return RedirectToAction("Index", "Home"); 
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByNameAsync(model.Username ?? string.Empty);

            if (user == null)
            {
                ModelState.AddModelError("", "User does not exist.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
    model.Username ?? string.Empty, 
    model.Password ?? string.Empty, 
    false, false);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Login successful!";
                return RedirectToAction("Index", "Home");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Account locked due to multiple failed attempts.");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "You have logged out successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}
