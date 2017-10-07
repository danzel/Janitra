using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Janitra.Data;
using Janitra.Data.Models;
using Janitra.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Janitra.Controllers
{
	public class MoviesController : Controller
	{
		private readonly JanitraContext _context;
		private readonly ILogger<MoviesController> _logger;
		private readonly IFileStorageService _fileStorageService;
		private readonly CurrentUser _currentUser;

		public MoviesController(JanitraContext context, ILogger<MoviesController> logger, IFileStorageService fileStorageService, CurrentUser currentUser)
		{
			_context = context;
			_logger = logger;
			_fileStorageService = fileStorageService;
			_currentUser = currentUser;
		}

		public async Task<IActionResult> View(int id)
		{
			return View(await _context.RomMovies.Include(r => r.Rom).Include(r => r.Results).SingleAsync(r => r.RomMovieId == id));
		}

		[HttpGet]
		public async Task<IActionResult> Add(int id)
		{
			return View(new AddMovieViewModel { RomId = id, CitraRegionValue = -1 });
		}

		[HttpPost]
		public async Task<IActionResult> Add(AddMovieViewModel addMovie)
		{
			if (ModelState.IsValid)
			{
				//TODO: Verify movie file
				var movieBytes = new byte[addMovie.MovieFile.Length];
				var movieStream = new MemoryStream(movieBytes);
				await addMovie.MovieFile.CopyToAsync(movieStream);

				var movie = new RomMovie
				{
					ActivelyTesting = true,
					AddedAt = DateTimeOffset.UtcNow,
					AddedByUser = _currentUser.User,
					CitraRegionValue = addMovie.CitraRegionValue,
					Description = addMovie.Description,
					//TODO: Length = TODO: Calculate from movie file
					MovieUrl = await _fileStorageService.StoreMovie(movieBytes),
					MovieSha256 = SHA256Hash.HashBytes(movieBytes),
					Name = addMovie.Name,
					RomId = addMovie.RomId
				};

				await _context.RomMovies.AddAsync(movie);
				await _context.SaveChangesAsync();

				return RedirectToAction("View", new { id = movie.RomMovieId });
			}
			return View(addMovie);
		}
	}

	public class AddMovieViewModel
	{
		[Required]
		public int RomId { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Description { get; set; }

		[Required]
		public IFormFile MovieFile { get; set; }

		[Required]
		public int CitraRegionValue { get; set; }
	}
}