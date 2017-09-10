using System.Linq;
using System.Threading.Tasks;
using Janitra.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Janitra.Api.Controllers
{
	public class BuildsController : Controller
	{
		private readonly JanitraContext _context;

		public BuildsController(JanitraContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.CitraBuilds.Where(b => b.ActivelyTesting).OrderByDescending(b => b.CitraBuildId).ToArrayAsync());
		}
	}
}