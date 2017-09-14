using Microsoft.AspNetCore.Mvc;

namespace Janitra.Controllers
{
    public class HomeController : Controller
    {
		public IActionResult Index()
		{
			return View();
		}
    }
}
