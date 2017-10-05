using System.Collections.Generic;
using System.Threading.Tasks;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace Janitra.Controllers
{
	public class RomsController : Controller
	{
		public async Task<IActionResult> Index()
		{
			return View(new[]
				{
					new Rom { RomId = 1, Name = "Pokemon Moon (Aus/Eur?)", RomType = RomType.Rom3DS, Movies = new List<RomMovie> { new RomMovie { Name = "Intro" } } }
				}
			);
		}

		public async Task<IActionResult> View(int id)
		{
			return View(new Rom
			{
				RomType = RomType.Rom3DS,
				Name = "Pokemon Moon (Aus/Eur?)",
				Movies = new[]
				{
					new RomMovie { Name = "Intro", Results = new RomMovieResult[0] }
				}
			});
		}
	}
}