using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Janitra.Services;
using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Janitra.Controllers.Api
{
	/// <summary>
	/// Responsible for managing Test Results
	/// </summary>
	[Route("api/test-results")]
	public class TestResultsController : Controller
	{
		private readonly JanitraContext _context;
		private readonly CurrentUser _currentUser;
		private readonly IFileStorageService _fileStorage;
		private readonly IMapper _mapper;

		/// <summary>
		/// Constructor
		/// </summary>
		public TestResultsController(JanitraContext context, CurrentUser currentUser, IFileStorageService fileStorage)
		{
			_context = context;
			_currentUser = currentUser;
			_fileStorage = fileStorage;

			_mapper = CreateMapper();
		}

		private IMapper CreateMapper()
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<TestResult, JsonTestResult>(MemberList.Destination)
					.ForMember(jtr => jtr.TimeTakenSeconds, o => o.ResolveUsing(tr => tr.TimeTaken.TotalSeconds));

				cfg.CreateMap<NewTestResult, TestResult>(MemberList.Source)
					.ForSourceMember(ntr => ntr.Log, o => o.Ignore())
					.ForSourceMember(ntr => ntr.ScreenshotTop, o => o.Ignore())
					.ForSourceMember(ntr => ntr.ScreenshotBottom, o => o.Ignore())
					.ForMember(tr => tr.TimeTaken, o => o.ResolveUsing(ntr => TimeSpan.FromSeconds(ntr.TimeTakenSeconds)))
					.ForSourceMember(ntr => ntr.AccessKey, o => o.Ignore())
					.ForSourceMember(ntr => ntr.TimeTakenSeconds, o => o.Ignore());
			});

			config.AssertConfigurationIsValid();

			return config.CreateMapper();
		}

		/// <summary>
		/// Get all of the test results for the given Build, TestDefinition, JanitraBot. Ordered by TestDefinitionId, JanitraBotId
		/// </summary>
		/// <param name="citraBuildId">What CitraBuild to fetch TestResults for</param>
		/// <param name="testDefinitionId">What TestDefinintion to fetch TestResults for</param>
		/// <param name="janitraBotId">What JanitraBot to fetch TestResults for</param>
		[HttpGet("list")]
		public async Task<JsonTestResult[]> List([FromQuery] int citraBuildId, [FromQuery] int? testDefinitionId = null, [FromQuery] int? janitraBotId = null)
		{
			var query = _context.TestResults
				.Where(tr => tr.CitraBuildId == citraBuildId);

			if (testDefinitionId.HasValue)
				query = query.Where(tr => tr.TestDefinitionId == testDefinitionId.Value);

			if (janitraBotId.HasValue)
				query = query.Where(tr => tr.JanitraBotId == janitraBotId.Value);

			query = query.OrderBy(tr => tr.TestDefinitionId).ThenBy(tr => tr.TestResultId);

			return await query.Select(tr => _mapper.Map<JsonTestResult>(tr)).ToArrayAsync();
		}

		/// <summary>
		/// Submit a test result
		/// </summary>
		[HttpPost("add")]
		public async Task<IActionResult> AddResult([FromBody] NewTestResult testResult)
		{
			var bot = await _context.JanitraBots.SingleOrDefaultAsync(b => b.JanitraBotId == testResult.JanitraBotId);
			if (bot == null)
				return Forbid("No bot matches that JanitraBotId");
			if (!CryptoHelper.Crypto.VerifyHashedPassword(bot.AccessKey, testResult.AccessKey))
				return Forbid("The given AccessKey does not match");

			var result = _mapper.Map<TestResult>(testResult);

			result.LogUrl = await _fileStorage.StoreLog(testResult.Log);
			result.ScreenshotTopUrl = await _fileStorage.StoreScreenshot(testResult.ScreenshotTop);
			result.ScreenshotBottomUrl = await _fileStorage.StoreScreenshot(testResult.ScreenshotBottom);

			result.JanitraBot = bot;
			result.ReportedAt = DateTimeOffset.UtcNow;

			if (testResult.ExecutionResult != ExecutionResult.Completed)
			{
				result.AccuracyStatus = AccuracyStatus.Incorrect;
			}
			else
			{
				//If screenshots match any other result for this test, then we are the same as that test (Correct/Incorrect)
				var matching = await _context.TestResults.FirstOrDefaultAsync(tr =>
					tr.TestDefinitionId == testResult.TestDefinitionId &&
					tr.ScreenshotTopUrl == result.ScreenshotTopUrl &&
					tr.ScreenshotBottomUrl == result.ScreenshotBottomUrl);

				if (matching != null)
				{
					//TODO: Log
					result.AccuracyStatus = matching.AccuracyStatus;
				}
			}
			//TODO: Check for duplicate results (we throw an exception ATM)

			await _context.AddAsync(result);
			await _context.SaveChangesAsync();
			return Ok();
		}

		public class JsonTestResult
		{
			[Required]
			public int TestResultId { get; set; }

			[Required]
			public int CitraBuildId { get; set; }
			[Required]
			public int JanitraBotId { get; set; }
			[Required]
			public int TestDefinitionId { get; set; }

			[Required]
			public DateTimeOffset ReportedAt { get; set; }

			[Required]
			public string LogUrl { get; set; }
			[Required]
			public string ScreenshotTopUrl { get; set; }
			[Required]
			public string ScreenshotBottomUrl { get; set; }

			[Required]
			[JsonConverter(typeof(StringEnumConverter))]
			public ExecutionResult ExecutionResult { get; set; }
			[Required]
			[JsonConverter(typeof(StringEnumConverter))]
			public AccuracyStatus AccuracyStatus { get; set; }

			[Required]
			public double TimeTakenSeconds { get; set; }
		}

		public class NewTestResult
		{
			/// <summary>
			/// JanitraBot Token
			/// </summary>
			[Required]
			public string AccessKey { get; set; }

			[Required]
			public int CitraBuildId { get; set; }
			[Required]
			public int JanitraBotId { get; set; }
			[Required]
			public int TestDefinitionId { get; set; }

			[Required]
			public byte[] Log { get; set; }
			[Required]
			public byte[] ScreenshotTop { get; set; }
			[Required]
			public byte[] ScreenshotBottom { get; set; }

			[Required]
			[JsonConverter(typeof(StringEnumConverter))]
			public ExecutionResult ExecutionResult { get; set; }

			[Required]
			public double TimeTakenSeconds { get; set; }
		}
	}
}