using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Janitra.Api.Services;
using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Janitra.Api.Controllers
{
	public class TestDefinitionsController : Controller
	{
		private readonly JanitraContext _context;
		private readonly CurrentUser _currentUser;
		private readonly IFileStorageService _fileStorageService;
		private readonly IMapper _mapper;

		public TestDefinitionsController(JanitraContext context, IFileStorageService fileStorageService, CurrentUser currentUser)
		{
			_context = context;
			_fileStorageService = fileStorageService;
			_currentUser = currentUser;

			_mapper = CreateMapper();
		}

		private IMapper CreateMapper()
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<AddTestModel, TestDefinition>(MemberList.Source)
					.ForSourceMember(atm => atm.MovieFile, o => o.Ignore())
					.ForSourceMember(atm => atm.RomFile, o => o.Ignore())
					.ForSourceMember(atm => atm.CodeUrl, o => o.Ignore());
			});

			config.AssertConfigurationIsValid();

			return config.CreateMapper();
		}

		public async Task<IActionResult> Index()
		{
			return View(await _context.TestDefinitions.Include(td => td.TestRom).Where(b => b.ActivelyTesting).OrderBy(b => b.TestDefinitionId).ToArrayAsync());
		}

		public async Task<IActionResult> ViewTest(int id)
		{
			return View(await _context.TestDefinitions.SingleAsync(td => td.TestDefinitionId == id));
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
		public async Task<IActionResult> Add(AddTestModel test)
		{
			if (ModelState.IsValid)
			{
				var movieBytes = new byte[test.MovieFile.Length];
				var movie = new MemoryStream(movieBytes);
				await test.MovieFile.CopyToAsync(movie);

				var romBytes = new byte[test.RomFile.Length];
				var rom = new MemoryStream(romBytes);
				await test.RomFile.CopyToAsync(rom);

				var now = DateTimeOffset.UtcNow;

				var def = new TestDefinition
				{
					ActivelyTesting = true,
					AddedAt = now,
					AddedByUser = _currentUser.User,
					MovieUrl = await _fileStorageService.StoreTestRom(movieBytes),
					MovieSha256 = SHA256Hash.HashBytes(movieBytes),
					Notes = test.Notes,
					TestName = test.TestName,
					TestRom = new TestRom
					{
						AddedAt = now,
						AddedByUser = _currentUser.User,
						CodeUrl = test.CodeUrl,
						RomSha256 = SHA256Hash.HashBytes(romBytes),
						RomUrl = await _fileStorageService.StoreTestRom(romBytes)
					}
				};

				await _context.AddAsync(def);
				await _context.SaveChangesAsync();

				return RedirectToAction("ViewTest", new { id = def.TestDefinitionId });
			}
			return View(test);
		}
	}

	public class AddTestModel
	{
		[Required]
		public string TestName { get; set; }

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