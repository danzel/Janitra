using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace Janitra.Api.Controllers
{
	/// <summary>
	/// Responsible for loading full data sets for comparing the test results of two builds
	/// </summary>
	[Route("compare")]
    public class CompareController : Controller
    {
	    private readonly JanitraContext _context;

	    /// <summary>
	    /// Constructor
	    /// </summary>
	    public CompareController(JanitraContext context)
	    {
		    _context = context;
	    }

		/// <summary>
		/// Loads all details for a comparison between two builds
		/// </summary>
		/// <param name="from">The build to compare from</param>
		/// <param name="to">The build to compare to</param>
		/// <remarks>
		/// from/to can be any of: CitraBuildId, "master", "previous-master"
		/// </remarks>
		[HttpGet("{from}/{to}")]
		public async Task<CompareResult> Get([FromRoute] string from, [FromRoute] string to)
		{
			//TODO: Probably want to cache these, we also hit a lot of APIs twice, when we could just use one single better query
			// Would be nice if we could share mappers between controllers, maybe just do one mapper somewhere

			var result = new CompareResult();

			using (var controller = new CitraBuildsController(_context, null))
			{
				if (from == "master")
					result.FromBuild = (await controller.List()).First(b => b.BuildType == BuildType.CitraMaster);
				else if (from == "previous-master")
					result.FromBuild = (await controller.List(true)).Where(b => b.BuildType == BuildType.CitraMaster).Skip(1).First();
				else
					result.FromBuild = await controller.Get(int.Parse(from));

				if (to == "master")
					result.ToBuild = (await controller.List()).First(b => b.BuildType == BuildType.CitraMaster);
				else if (from == "previous-master")
					result.ToBuild = (await controller.List(true)).Where(b => b.BuildType == BuildType.CitraMaster).Skip(1).First();
				else
					result.ToBuild = await controller.Get(int.Parse(to));
			}

			using (var controller = new TestResultsController(_context, null, null))
			{
				result.FromResults = await controller.List(result.FromBuild.CitraBuildId);
				result.ToResults = await controller.List(result.ToBuild.CitraBuildId);
			}

			using (var controller = new TestDefinitionsController(_context, null, null))
			{
				var requiredTestDefinitions = new HashSet<int>(result.FromResults.Concat(result.ToResults).Select(r => r.TestDefinitionId));

				result.TestDefinitions = (await controller.List(true)).Where(t => requiredTestDefinitions.Contains(t.TestDefinitionId)).ToArray();
			}

			using (var controller = new JanitraBotsController(_context, null))
			{
				var requiredBots = new HashSet<int>(result.FromResults.Concat(result.ToResults).Select(r => r.JanitraBotId));

				result.JanitraBots = (await controller.List()).Where(t => requiredBots.Contains(t.JanitraBotId)).ToArray();
			}

			return result;
		}

		public class CompareResult
		{
			public CitraBuildsController.JsonCitraBuild FromBuild { get; set; }
			public CitraBuildsController.JsonCitraBuild ToBuild { get; set; }

			public TestResultsController.JsonTestResult[] FromResults { get; set; }
			public TestResultsController.JsonTestResult[] ToResults { get; set; }

			public TestDefinitionsController.JsonTestDefinition[] TestDefinitions { get; set; }

			public JanitraBotsController.JsonJanitraBot[] JanitraBots { get; set; }
		}
	}
}
