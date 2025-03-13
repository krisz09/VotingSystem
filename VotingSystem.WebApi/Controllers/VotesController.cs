using Microsoft.AspNetCore.Mvc;

namespace VotingSystem.WebApi.Controllers
{
    public class VotesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
