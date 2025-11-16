using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BestStoreMVC.Areas.Users.Controllers
{
    [Area("Users")]
    [Authorize(Roles = "User")]

    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}
