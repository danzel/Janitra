using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Janitra.Data;
using Janitra.Data.Models;
using Janitra.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Janitra.Controllers
{
	public class RomsController : Controller
	{
		private readonly JanitraContext _context;
		private readonly ILogger<RomsController> _logger;
		private readonly CurrentUser _currentUser;

		public RomsController(JanitraContext context, ILogger<RomsController> logger, CurrentUser currentUser)
		{
			_context = context;
			_logger = logger;
			_currentUser = currentUser;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.Roms.Include(r => r.Movies).ToArrayAsync());
		}

		public async Task<IActionResult> View(int id)
		{
			return View(await _context.Roms.Include(r => r.Movies).ThenInclude(m => m.Results).SingleAsync(r => r.RomId == id));
		}

		[Authorize(Roles = "Developer")]
		[HttpGet]
		public async Task<IActionResult> Add()
		{
			return View();
		}

		[Authorize(Roles = "Developer")]
		[HttpPost]
		public async Task<IActionResult> Add(AddRomViewModel addRom)
		{
			if (ModelState.IsValid)
			{
				var rom = new Rom
				{
					Name = addRom.Name,
					RomType = addRom.RomType,
					RomFileName = addRom.RomFileName,
					RomSha256 = addRom.RomSha256.ToLowerInvariant(),
					AddedByUser = _currentUser.User
				};
				await _context.Roms.AddAsync(rom);
				await _context.SaveChangesAsync();

				_logger.LogInformation("Added new Rom {RomId}, by user {UserId}", rom.RomId, _currentUser.User.UserId);

				return RedirectToAction("View", new { id = rom.RomId });
			}
			return View(addRom);
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