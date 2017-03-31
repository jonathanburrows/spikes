using Microsoft.AspNetCore.Mvc;

namespace Consumer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Login() => View();

        public IActionResult Logout() => View();
    }
}
