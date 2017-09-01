using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Janitra.Api.Services;
using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

		/// <summary>
		/// Constructor
		/// </summary>
		public CitraBuildsController(JanitraContext context, CurrentUser currentUser)
		{
			_context = context;
			_currentUser = currentUser;
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
			return await _context.CitraBuilds.OrderByDescending(c => c.CitraBuildId).Select(c => new JsonCitraBuild
			{
				CitraBuildId = c.CitraBuildId,
				GitHash = c.GitHash,
				DateAdded = c.DateAdded,
				ActivelyTesting = c.ActivelyTesting,
				WindowsUrl = c.WindowsUrl,
				LinuxUrl = c.LinuxUrl,
				OsxUrl = c.OsxUrl
			}).ToArrayAsync();
		}

		/// <summary>
		/// Add a Citra Build
		/// </summary>
		/// <remarks>
		/// Only accessible by Developer level users.
		/// </remarks>
		[Authorize(Roles = "Developer")]
		[HttpPost("add")]
		public async Task Add([FromBody] NewCitraBuild build)
		{
			await _context.CitraBuilds.AddAsync(new CitraBuild
			{
				AddedByUserId = _currentUser.User.UserId,
				ActivelyTesting = true,
				DateAdded = DateTimeOffset.UtcNow,

				GitHash = build.GitHash,
				LinuxUrl = build.LinuxUrl,
				OsxUrl = build.OsxUrl,
				WindowsUrl = build.WindowsUrl
			});
			await _context.SaveChangesAsync();
		}

		public class JsonCitraBuild
		{
			public int CitraBuildId { get; set; }
			public string GitHash { get; set; }
			public DateTimeOffset DateAdded { get; set; }
			public bool ActivelyTesting { get; set; }
			public string WindowsUrl { get; set; }
			public string LinuxUrl { get; set; }
			public string OsxUrl { get; set; }
		}

		public class NewCitraBuild
		{
			public string GitHash { get; set; }

			public string WindowsUrl { get; set; }
			public string LinuxUrl { get; set; }
			public string OsxUrl { get; set; }
		}
	}
}