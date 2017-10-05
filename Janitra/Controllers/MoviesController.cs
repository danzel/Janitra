using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace Janitra.Controllers
{
	public class MoviesController : Controller
	{
		public async Task<IActionResult> View(int id)
		{
			return View(new RomMovie
			{
				Name = "Intro",
				Rom = new Rom { Name = "Pokemon Moon (Eur)" },
				Description = "Tap through the intro of the game up until choosing our first pokemon",
				Results = new List<RomMovieResult>
				{
					new RomMovieResult { CitraBuild = new CitraBuild { GitHash = "todohashhere" }, JanitraBot = new JanitraBot { Name = "Dave Test Bot", Os = OsType.Windows }, ExecutionResult = ExecutionResult.Completed, TimeTaken = new TimeSpan(0, 5, 2) }
				}
			});
		}
	}
}