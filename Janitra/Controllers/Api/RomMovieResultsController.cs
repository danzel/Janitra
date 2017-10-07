using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Janitra.Data;
using Janitra.Data.Models;
using Janitra.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Janitra.Controllers.Api
{
	[Route("api/rom-movie-results")]
	public class RomMovieResultsController : Controller
	{
		private readonly JanitraContext _context;
		private readonly ILogger<RomMovieResultsController> _logger;
		private readonly IFileStorageService _fileStorage;
		private readonly IMapper _mapper;

		/// <summary>
		/// Constructor
		/// </summary>
		public RomMovieResultsController(JanitraContext context, ILogger<RomMovieResultsController> logger, IFileStorageService fileStorage)
		{
			_context = context;
			_logger = logger;
			_fileStorage = fileStorage;

			_mapper = CreateMapper();
		}

		private IMapper CreateMapper()
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<RomMovieResult, JsonRomMovieResult>(MemberList.Destination);
				cfg.CreateMap<NewRomMovieResult, RomMovieResult>(MemberList.Source)
					.ForSourceMember(nrmr => nrmr.AccessKey, o => o.Ignore())
					.ForSourceMember(nrmr => nrmr.Log, o => o.Ignore())
					.ForSourceMember(nrmr => nrmr.Screenshots, o => o.Ignore())
					.ForMember(rmr => rmr.Screenshots, o => o.Ignore());
			});

			config.AssertConfigurationIsValid();

			return config.CreateMapper();
		}

		/// <summary>
		/// List all of the RomMovieResults for the given janitraBot
		/// </summary>
		[HttpGet("list")]
		public async Task<JsonRomMovieResult[]> List([FromQuery] int janitraBotId)
		{
			return await _context.RomMovieResults.Where(r => r.JanitraBotId == janitraBotId).Select(r => _mapper.Map<JsonRomMovieResult>(r)).ToArrayAsync();
		}

		[HttpPost("add")]
		public async Task<IActionResult> Add([FromBody] NewRomMovieResult movieResult)
		{
			var bot = await _context.JanitraBots.SingleOrDefaultAsync(b => b.JanitraBotId == movieResult.JanitraBotId);
			if (bot == null)
				return Forbid("No bot matches that JanitraBotId");
			if (!CryptoHelper.Crypto.VerifyHashedPassword(bot.AccessKey, movieResult.AccessKey))
				return Forbid("The given AccessKey does not match");
			if (bot.RunsHwTests)
				return Forbid("This bot is not allowed to run commercial rom tests");

			_logger.LogInformation("Received new result from {botid} for movie {movieid}", bot.JanitraBotId, movieResult.RomMovieId);

			var result = _mapper.Map<RomMovieResult>(movieResult);

			result.LogUrl = await _fileStorage.StoreLog(movieResult.Log);
			result.Screenshots = new List<RomMovieResultScreenshot>();
			foreach (var screenshot in movieResult.Screenshots)
			{
				var topUrl = await _fileStorage.StoreScreenshot(screenshot.TopImage);
				var bottomUrl = await _fileStorage.StoreScreenshot(screenshot.BottomImage);

				result.Screenshots.Add(new RomMovieResultScreenshot
				{
					FrameNumber = screenshot.FrameNumber,
					TopImageUrl = topUrl,
					BottomImageUrl = bottomUrl
				});
			}

			result.JanitraBot = bot;
			result.ReportedAt = DateTimeOffset.UtcNow;

			await _context.RomMovieResults.AddAsync(result);
			await _context.SaveChangesAsync();
			return Ok();
		}

		public class JsonRomMovieResult
		{
			[Required]
			public int RomMovieResultId { get; set; }

			[Required]
			public int CitraBuildId { get; set; }

			[Required]
			public int RomMovieId { get; set; }

			[Required]
			public DateTimeOffset ReportedAt { get; set; }
		}

		public class NewRomMovieResult
		{
			/// <summary>
			/// JanitraBot Token
			/// </summary>
			[Required]
			public string AccessKey { get; set; }

			[Required]
			public int CitraBuildId { get; set; }

			[Required]
			public int RomMovieId { get; set; }

			[Required]
			public int JanitraBotId { get; set; }

			[Required]
			[JsonConverter(typeof(StringEnumConverter))]
			public ExecutionResult ExecutionResult { get; set; }

			[Required]
			public byte[] Log { get; set; }

			[Required]
			public NewScreenshot[] Screenshots { get; set; }
		}

		public class NewScreenshot
		{
			[Required]
			public int FrameNumber { get; set; }

			[Required]
			public byte[] TopImage { get; set; }

			[Required]
			public byte[] BottomImage { get; set; }
		}
	}
}