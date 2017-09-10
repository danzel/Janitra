using Microsoft.AspNetCore.Mvc;

namespace Janitra.Api.Controllers
{
    public class HomeController : Controller
    {
		public IActionResult Index()
		{
			return View();
		}
    }
}
