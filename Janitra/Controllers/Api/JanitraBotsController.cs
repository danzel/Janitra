using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Janitra.Services;
using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Janitra.Controllers.Api
{
	/// <summary>
	/// Responsible for managing the list of Janitra Bots
	/// </summary>
	[Route("api/janitra-bots")]
	public class JanitraBotsController : Controller
	{
		private readonly JanitraContext _context;
		private readonly CurrentUser _currentUser;
		private readonly IMapper _mapper;

		/// <summary>
		/// Constructor
		/// </summary>
		public JanitraBotsController(JanitraContext context, CurrentUser currentUser)
		{
			_context = context;
			_currentUser = currentUser;

			_mapper = CreateMapper();
		}

		private IMapper CreateMapper()
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<JanitraBot, JsonJanitraBot>(MemberList.Destination)
					.ForMember(jjb => jjb.AddedByUserName, o => o.MapFrom(jb => jb.AddedByUser.OAuthName));

				cfg.CreateMap<NewJanitraBot, JanitraBot>(MemberList.Source);
			});

			config.AssertConfigurationIsValid();

			return config.CreateMapper();
		}

		/// <summary>
		/// Get the list of all Janitra Bots in the system
		/// </summary>
		[HttpGet("list")]
		public async Task<JsonJanitraBot[]> List()
		{
			return await _context.JanitraBots
				.Include(j => j.AddedByUser)
				.OrderBy(j => j.JanitraBotId)
				.Select(j => _mapper.Map<JsonJanitraBot>(j))
				.ToArrayAsync();
		}

		/// <summary>
		/// Add a new Janitra Bot
		/// </summary>
		/// <remarks>
		/// Only accessible by Developer level users.
		/// </remarks>
		[Authorize(Roles = "Developer")]
		[HttpPost("add")]
		public async Task<AddBotResult> Add([FromBody] NewJanitraBot botDetails)
		{
			string accessKey = SecureRandomStringGenerator.Generate();

			var bot = _mapper.Map<JanitraBot>(botDetails);
			bot.AccessKey = CryptoHelper.Crypto.HashPassword(accessKey);
			bot.AddedByUser = _currentUser.User;

			await _context.JanitraBots.AddAsync(bot);
			await _context.SaveChangesAsync();

			return new AddBotResult
			{
				JanitraBotId = bot.JanitraBotId,
				AccessKey = accessKey
			};
		}

		//TODO? Regenerate access key (if user loses it)

		public class JsonJanitraBot
		{
			[Required]
			public int JanitraBotId { get; set; }
			[Required]
			public string Name { get; set; }
			[Required]
			public string HardwareDetails { get; set; }
			[Required]
			[JsonConverter(typeof(StringEnumConverter))]
			public OsType Os { get; set; }
			[Required]
			public int AddedByUserId { get; set; }
			[Required]
			public string AddedByUserName { get; set; }
		}

		public class NewJanitraBot
		{
			[Required]
			public string Name { get; set; }
			[Required]
			public string HardwareDetails { get; set; }
			[Required]
			[JsonConverter(typeof(StringEnumConverter))]
			public OsType Os { get; set; }
		}


		public class AddBotResult
		{
			[Required]
			public int JanitraBotId { get; set; }
			[Required]
			public string AccessKey { get; set; }
		}
	}
}