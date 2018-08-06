using System.Diagnostics;
using System.Threading.Tasks;
using FabricDemo.IdentityServer.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Mvc;

namespace FabricDemo.IdentityServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;

        public HomeController(IIdentityServerInteractionService interaction)
        {
            _interaction = interaction;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Error(string errorId = null)
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            if (!string.IsNullOrEmpty(errorId))
            {
                // retrieve error details from identityserver
                var message = await _interaction.GetErrorContextAsync(errorId);
                if (message != null)
                {
                    errorViewModel.Error = message;
                }
            }
            return View(errorViewModel);
        }
    }
}
