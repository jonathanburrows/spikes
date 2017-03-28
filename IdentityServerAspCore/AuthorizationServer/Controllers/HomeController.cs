using AuthorizationServer.Attributes;
using AuthorizationServer.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AuthorizationServer.Controllers
{
    [SecurityHeader]
    public class HomeController : Controller
    {
        private IIdentityServerInteractionService InteractionService { get; }

        public HomeController(IIdentityServerInteractionService interactionService)
        {
            if (interactionService == null)
            {
                throw new ArgumentNullException(nameof(interactionService));
            }
            InteractionService = interactionService;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> Error(string errorId)
        {
            var model = new ErrorModel();

            var message = await InteractionService.GetErrorContextAsync(errorId);
            if (message != null)
            {
                model.Error = message;
            }

            return View("Error", model);
        }
    }
}
