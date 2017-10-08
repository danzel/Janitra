using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Janitra.Services;
using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Janitra.Controllers
{
	/// <summary>
	/// Responsible for managing the list of Janitra Bots
	/// </summary>
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
		public async Task<IActionResult> Index()
		{
			return View(await _context.JanitraBots.Include(j => j.AddedByUser).OrderBy(j => j.JanitraBotId).ToArrayAsync());
		}

		public async Task<IActionResult> View(int id)
		{
			var bot = await _context.JanitraBots.Include(j => j.AddedByUser).SingleAsync(j => j.JanitraBotId == id);
			return View(new ViewJanitraBotViewModel
			{
				CurrentUser = _currentUser.User,
				JanitraBot = bot
			});
		}

		[Authorize(Roles = "Developer")]
		[HttpGet]
		public async Task<IActionResult> Add()
		{
			return View();
		}

		[Authorize(Roles = "Developer")]
		[HttpPost]
		public async Task<IActionResult> Add(AddJanitraBotViewModel bot)
		{
			if (ModelState.IsValid)
			{
				string accessKey = SecureRandomStringGenerator.Generate();

				var janitraBot = new JanitraBot
				{
					AccessKey = CryptoHelper.Crypto.HashPassword(accessKey),
					AddedByUser = _currentUser.User,
					HardwareDetails = bot.HardwareDetails,
					Name = bot.Name,
					Os = bot.Os,
					RunsHwTests = User.IsInRole("Developer") ? bot.RunsHwTests : false
				};
				await _context.JanitraBots.AddAsync(janitraBot);
				await _context.SaveChangesAsync();

				return View("AccessKey", new JanitraBotAccessKeyViewModel
				{
					JanitraBotId = janitraBot.JanitraBotId,
					AccessKey = accessKey
				});
			}
			return View(bot);
		}

		[Authorize(Roles = "Developer")]
		[HttpPost]
		public async Task<IActionResult> ResetAccessKey(ResetAccessKeyViewModel reset)
		{
			var bot = await _context.JanitraBots.SingleOrDefaultAsync(b => b.JanitraBotId == reset.JanitraBotId && b.AddedByUserId == _currentUser.User.UserId);

			//User doesn't own this bot, or bot doesn't exist
			if (bot == null)
				return Forbid();

			string accessKey = SecureRandomStringGenerator.Generate();

			bot.AccessKey = CryptoHelper.Crypto.HashPassword(accessKey);
			await _context.SaveChangesAsync();

			return View("AccessKey", new JanitraBotAccessKeyViewModel
			{
				JanitraBotId = reset.JanitraBotId,
				AccessKey = accessKey
			});
		}
	}

	public class AddJanitraBotViewModel
	{
		[Required]
		public string Name { get; set; }

		[Required]
		public OsType Os { get; set; }

		[Required]
		public string HardwareDetails { get; set; }

		[Required]
		public bool RunsHwTests { get; set; }
	}

	public class JanitraBotAccessKeyViewModel
	{
		[Required]
		public int JanitraBotId { get; set; }

		[Required]
		public string AccessKey { get; set; }
	}

	public class ViewJanitraBotViewModel
	{
		public JanitraBot JanitraBot { get; set; }
		public User CurrentUser { get; set; }
	}

	public class ResetAccessKeyViewModel
	{
		[Required]
		public int JanitraBotId { get; set; }
	}
}