using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Authorization;
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

		[Authorize(Roles = "Developer")]
		[HttpGet]
		public async Task<IActionResult> Add()
		{
			return View();
		}

		[Authorize(Roles = "Developer")]
		[HttpPost]
		public async Task<IActionResult> Add(AddRomViewModel rom)
		{
			if (ModelState.IsValid)
			{
				//TODO
			}
			return View(rom);
		}
	}

	public class AddRomViewModel
	{
		[Required]
		public string Name { get; set; }

		[Required]
		public RomType RomType { get; set; }

		[Required]
		public string RomFileName { get; set; }

		[Required, MinLength(64), MaxLength(64)]
		public string RomSha256 { get; set; }
	}
}