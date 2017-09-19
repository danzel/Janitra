using System;
using System.Linq;
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
	public class TestResultsController : Controller
	{
		private readonly JanitraContext _context;
		private readonly ILogger<TestResultsController> _logger;
		private readonly CurrentUser _currentUser;

		public TestResultsController(JanitraContext context, ILogger<TestResultsController> logger, CurrentUser currentUser)
		{
			_context = context;
			_logger = logger;
			_currentUser = currentUser;
		}

		public async Task<IActionResult> View(int id, int changedAccuracy = 0)
		{
			//TODO: Use changedAccuracy
			return View(await _context.TestResults.Include(tr => tr.TestDefinition).Include(tr => tr.JanitraBot).Include(tr => tr.CitraBuild).SingleAsync(tr => tr.TestResultId == id));
		}

		[Authorize(Roles = "Developer")]
		public async Task<IActionResult> UpdateAccuracy(UpdateAccuracyViewModel vm)
		{

			if (ModelState.IsValid)
			{
				var result = await _context.TestResults.SingleAsync(tr => tr.TestResultId == vm.Id);

				//Update all results that match it on (TestDefinitionId, ScreenshotTop, ScreenshotBottom)
				int countChanged = 0;
				foreach (var r in _context.TestResults.Where(tr => tr.TestDefinitionId == result.TestDefinitionId && tr.ScreenshotTopUrl == result.ScreenshotTopUrl && tr.ScreenshotBottomUrl == result.ScreenshotBottomUrl))
				{
					r.AccuracyStatus = vm.Status;
					countChanged++;
				}

				await _context.SaveChangesAsync();

				_logger.LogInformation("Updating accuracy of test result {TestResultId} to {AccuracyStatus} by user {UserId}", result.TestResultId, vm.Status, _currentUser.User.UserId);

				return RedirectToAction("View", new { id = vm.Id, changedAccuracy = countChanged });
			}

			throw new NotImplementedException("TODO");
		}
	}

	public class UpdateAccuracyViewModel
	{
		public int Id { get; set; }
		public AccuracyStatus Status { get; set; }
	}
}