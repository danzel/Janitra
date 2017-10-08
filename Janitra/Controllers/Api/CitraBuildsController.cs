using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Janitra.Controllers.Api
{
	/// <summary>
	/// Responsible for managing the list of Citra Builds
	/// </summary>
	[Route("api/citra-builds")]
	public class CitraBuildsController : Controller
	{
		private readonly JanitraContext _context;
		private readonly IMapper _mapper;

		/// <summary>
		/// Constructor
		/// </summary>
		public CitraBuildsController(JanitraContext context)
		{
			_context = context;

			_mapper = CreateMapper();
		}

		private IMapper CreateMapper()
		{
			var config = new MapperConfiguration(cfg => { cfg.CreateMap<CitraBuild, JsonCitraBuild>(MemberList.Destination); });

			config.AssertConfigurationIsValid();

			return config.CreateMapper();
		}

		/// <summary>
		/// Get the list of all Citra Builds that are being actively tested
		/// </summary>
		[HttpGet("list")]
		public async Task<JsonCitraBuild[]> List()
		{
			IQueryable<CitraBuild> query = _context.CitraBuilds
				.Where(cb => cb.ActivelyTesting)
				.OrderByDescending(c => c.CitraBuildId);

			return await query
				.Select(c => _mapper.Map<JsonCitraBuild>(c))
				.ToArrayAsync();
		}

		public class JsonCitraBuild
		{
			[Required]
			public int CitraBuildId { get; set; }

			[Required]
			public string GitHash { get; set; }

			[Required]
			public DateTimeOffset CommitTime { get; set; }

			[Required]
			[JsonConverter(typeof(StringEnumConverter))]
			public BuildType BuildType { get; set; }

			[Required]
			public DateTimeOffset DateAdded { get; set; }

			public string WindowsUrl { get; set; }
			public string LinuxUrl { get; set; }
			public string OsxUrl { get; set; }
		}
	}
}