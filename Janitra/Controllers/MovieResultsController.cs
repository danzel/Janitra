using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Janitra.Controllers
{
	public class MovieResultsController : Controller
	{
		private JanitraContext _context;

		public MovieResultsController(JanitraContext context)
		{
			_context = context;
		}


		public async Task<IActionResult> View(int id)
		{
			return View(await _context.RomMovieResults
				.Where(rmr => rmr.RomMovieResultId == id)
				.Include(rmr => rmr.RomMovie).ThenInclude(rm => rm.Rom)
				.Include(rmr => rmr.CitraBuild)
				.Include(rmr => rmr.JanitraBot)
				.Include(rmr => rmr.Screenshots)
				.SingleAsync()
			);
		}
	}
}