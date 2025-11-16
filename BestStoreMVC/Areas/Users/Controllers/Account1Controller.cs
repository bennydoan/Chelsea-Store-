using BestStoreMVC.Domain.IdentityEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Areas.Users.Controllers
{
    [Area("Users")]
    [Authorize]
    public class Account1Controller : Controller

    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public Account1Controller(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Account", new { area = "" });
        }
    }
}
