using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace Janitra.Controllers
{
	public class MovieResultsController : Controller
	{
		public async Task<IActionResult> View(int id)
		{
			return View(new RomMovieResult
			{
				CitraBuild = new CitraBuild { GitHash = "asdasdasdasdasdasdasdasdasdasdasdasdasda"},
				JanitraBot = new JanitraBot {  Name="Dave Test"},
				RomMovie = new RomMovie { Name = "Intro", Rom = new Rom { Name="Pokemon Moon (Eur)"}, Length = TimeSpan.FromMinutes(2) },
				TimeTaken = TimeSpan.FromMinutes(2.5135322),
				Screenshots = new List<RomMovieResultScreenshot>
				{
					new RomMovieResultScreenshot{FrameNumber = 1},
					new RomMovieResultScreenshot{FrameNumber = 2}
				}
			});
		}
	}
}