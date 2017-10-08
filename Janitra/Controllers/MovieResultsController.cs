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
			var romMovieResult = await _context.RomMovieResults
				.Where(rmr => rmr.RomMovieResultId == id)
				.Include(rmr => rmr.RomMovie).ThenInclude(rm => rm.Rom)
				.Include(rmr => rmr.CitraBuild)
				.Include(rmr => rmr.JanitraBot)
				.Include(rmr => rmr.Screenshots)
				.SingleAsync();

			var latestMaster = await _context.CitraBuilds.Where(cb => cb.BuildType == BuildType.CitraMaster).OrderByDescending(cb => cb.CommitTime).FirstAsync();

			var masterMovieFromSameBot = await _context.RomMovieResults.SingleOrDefaultAsync(rmr => rmr.JanitraBotId == romMovieResult.JanitraBotId && rmr.CitraBuildId == latestMaster.CitraBuildId && rmr.RomMovieId == romMovieResult.RomMovieId);

			return View(new MovieResultViewViewModel
				{
					Result = romMovieResult,
					MasterResultId = masterMovieFromSameBot?.RomMovieResultId
				}
			);
		}

		public async Task<IActionResult> Compare(int leftId, int rightId)
		{
			return View(new MovieResultsCompareViewModel
			{
				Left = await _context.RomMovieResults
					.Include(rmr => rmr.CitraBuild)
					.Include(rmr => rmr.RomMovie).ThenInclude(rm => rm.Rom)
					.Include(rmr => rmr.Screenshots)
					.SingleAsync(rmr => rmr.RomMovieResultId == leftId),
				Right = await _context.RomMovieResults
					.Include(rmr => rmr.CitraBuild)
					.Include(rmr => rmr.Screenshots)
					.SingleAsync(rmr => rmr.RomMovieResultId == rightId)
			});
		}
	}

	public class MovieResultViewViewModel
	{
		public RomMovieResult Result { get; set; }

		public int? MasterResultId { get; set; }
	}

	public class MovieResultsCompareViewModel
	{
		public RomMovieResult Left { get; set; }
		public RomMovieResult Right { get; set; }
	}
}