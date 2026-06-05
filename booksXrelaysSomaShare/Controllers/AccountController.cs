using booksXrelaysSomaShare.Models;
using booksXrelaysSomaShare.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace booksXrelaysSomaShare.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(ApplicationUser user, string password)
        {
            var validator = new UniversityEmailAttribute();

            if (validator.GetValidationResult(user.Email, new ValidationContext(user)) != ValidationResult.Success)
            {
                ModelState.AddModelError("Email", "Only university email addresses are allowed.");
            }

            if (ModelState.IsValid)
            {
                if (user.Email.EndsWith("@stadio.ac.za"))
                    user.CampusName = "STADIO";
                else if (user.Email.EndsWith("@up.ac.za"))
                    user.CampusName = "University of Pretoria";
                else
                    user.CampusName = "Verified University";

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(user);
        }
    }
}