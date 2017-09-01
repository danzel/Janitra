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
	/// Responsible for managing the list of Janitra Bots
	/// </summary>
	[Route("janitra-bots")]
	public class JanitraBotsController : Controller
	{
		private readonly JanitraContext _context;
		private readonly CurrentUser _currentUser;

		/// <summary>
		/// Constructor
		/// </summary>
		public JanitraBotsController(JanitraContext context, CurrentUser currentUser)
		{
			_context = context;
			_currentUser = currentUser;
		}

		/// <summary>
		/// Get the list of all Janitra Bots in the system
		/// </summary>
		[HttpGet("list")]
		public async Task<JsonJanitraBot[]> List()
		{
			return await _context.JanitraBots.Include(j => j.AddedByUser).OrderBy(j => j.JanitraBotId).Select(j => new JsonJanitraBot
			{
				JanitraBotId = j.JanitraBotId,
				Name = j.Name,
				HardwareDetails = j.HardwareDetails,
				Os = j.Os,
				AddedByUserId = j.AddedByUserId,
				AddedByUserName = j.AddedByUser.OAuthName
			}).ToArrayAsync();
		}

		/// <summary>
		/// Add a new Janitra Bot
		/// </summary>
		/// <remarks>
		/// Only accessible by Developer level users.
		/// </remarks>
		[Authorize(Roles = "Developer")]
		[HttpPost("add")]
		public async Task<AddResult> Add([FromBody] NewJanitraBot botDetails)
		{
			string accessKey = SecureRandomStringGenerator.Generate();

			var bot = new JanitraBot
			{
				AccessKey = CryptoHelper.Crypto.HashPassword(accessKey),
				AddedByUserId = _currentUser.User.UserId,
				HardwareDetails = botDetails.HardwareDetails,
				Name = botDetails.Name,
				Os = botDetails.Os
			};

			await _context.JanitraBots.AddAsync(bot);

			return new AddResult
			{
				JanitraBotId = bot.JanitraBotId,
				AccessKey = accessKey
			};
		}

		public class JsonJanitraBot
		{
			public int JanitraBotId { get; set; }
			public string Name { get; set; }
			public string HardwareDetails { get; set; }
			public OsType Os { get; set; }
			public int AddedByUserId { get; set; }
			public string AddedByUserName { get; set; }
		}

		public class NewJanitraBot
		{
			public string Name { get; set; }
			public string HardwareDetails { get; set; }
			public OsType Os { get; set; }
		}


		public class AddResult
		{
			public int JanitraBotId { get; set; }
			public string AccessKey { get; set; }
		}
	}
}