using BestStoreMVC.Domain.IdentityEntities;
using BestStoreMVC.Enums;
using BestStoreMVC.Models.LoginDtos;
using BestStoreMVC.Models.RegisterDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BestStoreMVC.Controllers
{
    [Route("[Controller]/[action]")]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly RoleManager<ApplicationRole> _roleManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }


        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Register register)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return View(register);
            }

            // Check duplicates before attempting to create the user
            if (!string.IsNullOrWhiteSpace(register.Email))
            {
                var existingByEmail = await _userManager.FindByEmailAsync(register.Email);
                if (existingByEmail != null)
                {
                    ModelState.AddModelError(nameof(register.Email), "Email is already in use.");
                }
            }

            if (!string.IsNullOrWhiteSpace(register.Number))
            {
                var phoneExists = await _userManager.Users.AnyAsync(u => u.PhoneNumber == register.Number);
                if (phoneExists)
                {
                    ModelState.AddModelError(nameof(register.Number), "Phone number is already in use.");
                }
            }

            // Confirm password match
            if (register.Password != register.ConfirmPassword)
            {
                ModelState.AddModelError(nameof(register.ConfirmPassword), "Passwords do not match.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return View(register);
            }

            var user = new ApplicationUser
            {
                Email = register.Email,
                PhoneNumber = register.Number,
                UserName = register.Email,
                PersonName = register.Name
            };

            // Use CreateAsync(user, password) so Identity validates the password and stores a hashed password.
            var result = await _userManager.CreateAsync(user, register.Password);

            if (result.Succeeded)
            {

                // Ensure both roles exist before assigning
                var adminRoleName = UserTypeOptions.Admin.ToString();
                var userRoleName = UserTypeOptions.User.ToString();

                if (!await _roleManager.RoleExistsAsync(adminRoleName))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = adminRoleName });
                }

                if (!await _roleManager.RoleExistsAsync(userRoleName))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = userRoleName });
                }

                // Assign the chosen role
                var roleToAssign = register.UserType == UserTypeOptions.Admin ? adminRoleName : userRoleName;
                await _userManager.AddToRoleAsync(user, roleToAssign);

                // add a claim for the display name (use ClaimTypes.GivenName or a custom type)
                if (!string.IsNullOrWhiteSpace(user.PersonName))
                {
                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.GivenName, user.PersonName));
                }

                // Provide a friendly notice on the Login page
                TempData["SuccessMessage"] = "Account created. Please log in.";

                // Redirect to the Login page instead of signing in
                return RedirectToAction("Login", "Account");
            }

            // Add Identity errors to ModelState so they show in validation summary
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return View(register);

        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task< IActionResult> Login(Login login, string? ReturnUrl)
        {
            if (ModelState.IsValid == false)
            {
                ViewBag.Errors = ModelState.Values.SelectMany(temp => temp.Errors).Select(e => e.ErrorMessage);
                return View (login);
            }

          
            var result = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, isPersistent : login.IsPersistent, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if(!string.IsNullOrEmpty(ReturnUrl)&& Url.IsLocalUrl(ReturnUrl))
                {
                    return LocalRedirect(ReturnUrl);
                }

                //Find user 

                var user = await _userManager.FindByNameAsync(login.UserName);
                if(user != null)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    if (roles.Contains(UserTypeOptions.User.ToString()))
                        {
                            // admin -> Admin area Project controller
                            return RedirectToAction("Index", "Home", new { area = "Users" });
                        }

                }
                return RedirectToAction("Index", "Home");
            }

            else
            {
                ModelState.AddModelError("Login", "Invalid Email or Password");
            }
                return View(login);
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
           
        }

    }
}
