using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Janitra.Api.Services;
using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Janitra.Api.Controllers
{
	/// <summary>
	/// Responsible for managing the list of Citra Builds
	/// </summary>
	[Route("citra-builds")]
	public class CitraBuildsController : Controller
	{
		private readonly JanitraContext _context;
		private readonly CurrentUser _currentUser;
		private readonly IMapper _mapper;

		/// <summary>
		/// Constructor
		/// </summary>
		public CitraBuildsController(JanitraContext context, CurrentUser currentUser)
		{
			_context = context;
			_currentUser = currentUser;

			_mapper = CreateMapper();
		}

		private IMapper CreateMapper()
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<CitraBuild, JsonCitraBuild>(MemberList.Destination);
				cfg.CreateMap<NewCitraBuild, CitraBuild>(MemberList.Source);
			});

			config.AssertConfigurationIsValid();

			return config.CreateMapper();
		}

		/// <summary>
		/// Get the list of all Citra Builds.
		/// </summary>
		/// <remarks>
		/// Defaults to only including active ones.
		/// Returns the latest builds first.
		/// </remarks>
		[HttpGet("list")]
		public async Task<JsonCitraBuild[]> List([FromQuery] bool includeInactive = false)
		{
			IQueryable<CitraBuild> query = _context.CitraBuilds
				.OrderByDescending(c => c.CitraBuildId);

			if (!includeInactive)
				query = query.Where(cb => cb.ActivelyTesting);

			return await query
				.Select(c => _mapper.Map<JsonCitraBuild>(c))
				.ToArrayAsync();
		}

		/// <summary>
		/// Add a Citra Build
		/// </summary>
		/// <remarks>
		/// Only accessible by Developer level users.
		/// </remarks>
		[Authorize(Roles = "Developer")]
		[HttpPost("add")]
		public async Task Add([FromBody] NewCitraBuild newBuild)
		{
			var build = _mapper.Map<CitraBuild>(newBuild);
			build.ActivelyTesting = true;
			build.AddedByUser = _currentUser.User;
			build.DateAdded = DateTimeOffset.UtcNow;

			await _context.CitraBuilds.AddAsync(build);
			await _context.SaveChangesAsync();
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
			public string BuildNotes { get; set; }
			[Required]
			public DateTimeOffset DateAdded { get; set; }
			[Required]
			public bool ActivelyTesting { get; set; }
			public string WindowsUrl { get; set; }
			public string LinuxUrl { get; set; }
			public string OsxUrl { get; set; }
		}

		public class NewCitraBuild
		{
			[Required]
			public string GitHash { get; set; }
			[Required]
			public DateTimeOffset CommitTime { get; set; }
			[Required]
			[JsonConverter(typeof(StringEnumConverter))]
			public BuildType BuildType { get; set; }
			[Required]
			public string BuildNotes { get; set; }

			public string WindowsUrl { get; set; }
			public string LinuxUrl { get; set; }
			public string OsxUrl { get; set; }
		}
	}
}