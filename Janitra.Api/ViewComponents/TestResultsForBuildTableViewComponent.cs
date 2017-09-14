using System.Linq;
using System.Threading.Tasks;
using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Janitra.Api.ViewComponents
{
    public class TestResultsForBuildTableViewComponent : ViewComponent
    {
		private readonly JanitraContext _context;
		public TestResultsForBuildTableViewComponent(JanitraContext context)
		{
			_context = context;
		}

		public async Task<IViewComponentResult> InvokeAsync(int citraBuildId)
		{
			//TODO: Limit this to the last X builds.

			var results = await _context.TestResults
				.Include(tr => tr.TestDefinition)
				.Include(tr => tr.JanitraBot)
				.Where(tr => tr.CitraBuildId == citraBuildId && tr.CitraBuild.BuildType == BuildType.CitraMaster && tr.TestDefinition.ActivelyTesting)
				.ToArrayAsync();


			var bots = results.Select(tr => tr.JanitraBot).Distinct().OrderBy(b => b.Name).ToArray();
			var tests = results.Select(tr => tr.TestDefinition).Distinct().OrderBy(b => b.TestName).ToArray();

			return View(new TestResultsForBuildTableViewModel(bots, tests, results));
		}
	}

	public class TestResultsForBuildTableViewModel
	{
		public JanitraBot[] Bots { get; set; }
		public TestDefinition[] Tests { get; set; }

		public TestResult[] Results { get; set; } //TODO: This should have an efficient lookup on [bot/test]

		public TestResultsForBuildTableViewModel(JanitraBot[] bots, TestDefinition[] tests, TestResult[] results)
		{
			Bots = bots;
			Tests = tests;
			Results = results;
		}
	}
}
