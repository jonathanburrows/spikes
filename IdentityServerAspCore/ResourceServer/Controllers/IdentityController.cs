using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ResourceServer.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class IdentityController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return new JsonResult(User.Claims.Select(u => new { u.Type, u.Value }));
        }
    }
}
