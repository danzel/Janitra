using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Janitra.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Janitra.Api.Controllers
{
	public class TestDefinitionsController : Controller
	{
		private readonly JanitraContext _context;

		public TestDefinitionsController(JanitraContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.TestDefinitions.Include(td => td.TestRom).Where(b => b.ActivelyTesting).OrderBy(b => b.TestDefinitionId).ToArrayAsync());
		}

		[HttpGet]
		[Authorize(Roles = "Developer")]
		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		[Authorize(Roles = "Developer")]
		//[ValidateAntiForgeryToken]
		public IActionResult Add(AddTestModel test)
		{
			if (ModelState.IsValid)
			{
				//TODO
				Console.WriteLine();
			}
			return View();
		}
	}

	public class AddTestModel
	{
		[Required]
		public string Name { get; set; }

		[Required]
		public string Notes { get; set; }

		[Required, Url]
		public string CodeUrl { get; set; }

		[Required]
		public IFormFile RomFile { get; set; }

		[Required]
		public IFormFile MovieFile { get; set; }
	}
}