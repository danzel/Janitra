using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Janitra.Services;
using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Janitra.Controllers
{
	public class BuildsController : Controller
	{
		private readonly JanitraContext _context;
		private readonly CurrentUser _currentUser;
		private readonly IMapper _mapper;

		public BuildsController(JanitraContext context, CurrentUser currentUser)
		{
			_context = context;
			_currentUser = currentUser;

			_mapper = CreateMapper();
		}

		private IMapper CreateMapper()
		{
			var config = new MapperConfiguration(cfg => { cfg.CreateMap<AddBuildViewModel, CitraBuild>(MemberList.Source); });

			config.AssertConfigurationIsValid();

			return config.CreateMapper();
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.CitraBuilds.Where(b => b.ActivelyTesting).OrderByDescending(b => b.CitraBuildId).ToArrayAsync());
		}

		public async Task<IActionResult> View(int id)
		{
			return View(await _context.CitraBuilds.SingleAsync(td => td.CitraBuildId == id));
		}

		[HttpGet]
		[Authorize(Roles = "Developer")]
		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		[Authorize(Roles = "Developer")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Add(AddBuildViewModel build)
		{
			if (ModelState.IsValid)
			{
				var citraBuild = _mapper.Map<CitraBuild>(build);
				citraBuild.ActivelyTesting = true;
				citraBuild.AddedByUser = _currentUser.User;
				citraBuild.DateAdded = DateTimeOffset.UtcNow;

				await _context.CitraBuilds.AddAsync(citraBuild);
				await _context.SaveChangesAsync();

				return RedirectToAction("View", new { id = citraBuild.CitraBuildId });
			}
			return View(build);
		}
	}

	public class AddBuildViewModel
	{
		[Required, MinLength(40), MaxLength(40)]
		public string GitHash { get; set; }

		[Required]
		public DateTimeOffset CommitTime { get; set; }

		[Required]
		public BuildType BuildType { get; set; }

		[Required]
		public string BuildNotes { get; set; }

		[Url]
		public string WindowsUrl { get; set; }

		[Url]
		public string LinuxUrl { get; set; }

		[Url]
		public string OsxUrl { get; set; }
	}
}