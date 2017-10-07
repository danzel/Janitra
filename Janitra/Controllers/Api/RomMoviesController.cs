using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Janitra.Controllers.Api
{
	[Route("api/rom-movies")]
	public class RomMoviesController : Controller
	{
		private readonly JanitraContext _context;
		private readonly IMapper _mapper;

		/// <summary>
		/// Constructor
		/// </summary>
		public RomMoviesController(JanitraContext context)
		{
			_context = context;

			_mapper = CreateMapper();
		}

		private IMapper CreateMapper()
		{
			var config = new MapperConfiguration(cfg => { cfg.CreateMap<RomMovie, JsonRomMovie>(MemberList.Destination); });

			config.AssertConfigurationIsValid();

			return config.CreateMapper();
		}

		/// <summary>
		/// Gets a list of all movies for the given rom that are actively testing
		/// </summary>
		[HttpGet("list/{romId}")]
		public async Task<JsonRomMovie[]> List([FromRoute] int romId)
		{
			return await _context.RomMovies.Where(rm => rm.ActivelyTesting && rm.RomId == romId).Select(r => _mapper.Map<JsonRomMovie>(r)).ToArrayAsync();
		}

		public class JsonRomMovie
		{
			[Required]
			public int RomMovieId { get; set; }
			[Required]
			public string Name { get; set; }
			[Required]
			public int RomId { get; set; }
			[Required]
			public string MovieUrl { get; set; }
			[Required]
			public string MovieSha256 { get; set; }

			#region Citra settings

			/// <summary>
			/// The system region that Citra will use during emulation
			/// </summary>
			[Required]
			public int CitraRegionValue { get; set; }

			#endregion
		}
	}
}