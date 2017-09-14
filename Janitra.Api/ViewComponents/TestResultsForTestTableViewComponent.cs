using System.Linq;
using System.Threading.Tasks;
using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Janitra.Api.ViewComponents
{
    public class TestResultsForTestTableViewComponent : ViewComponent
    {
	    private readonly JanitraContext _context;
	    public TestResultsForTestTableViewComponent(JanitraContext context)
	    {
		    _context = context;
	    }

		public async Task<IViewComponentResult> InvokeAsync(int testDefinitionId)
		{
			//TODO: Limit this to the last X builds.

			var results = await _context.TestResults
				.Include(tr => tr.CitraBuild)
				.Include(tr => tr.JanitraBot)
				.Where(tr => tr.TestDefinitionId == testDefinitionId && tr.CitraBuild.BuildType == BuildType.CitraMaster)
				.ToArrayAsync();


			var bots = results.Select(tr => tr.JanitraBot).Distinct().OrderBy(b => b.Name).ToArray();
			var builds = results.Select(tr => tr.CitraBuild).Distinct().OrderByDescending(b => b.CitraBuildId).ToArray();

			return View(new TestResultsForTestTableViewModel(bots, builds, results));
		}
	}

	public class TestResultsForTestTableViewModel
	{
		public JanitraBot[] Bots { get; set; }
		public CitraBuild[] Builds { get; set; }

		public TestResult[] Results { get; set; } //TODO: This should have an efficient lookup on [bot/build]

		public TestResultsForTestTableViewModel(JanitraBot[] bots, CitraBuild[] builds, TestResult[] results)
		{
			Bots = bots;
			Builds = builds;
			Results = results;
		}
	}
}
