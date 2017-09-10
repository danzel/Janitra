using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Janitra.Api.Services;
using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Janitra.Api.Controllers.Api
{
	/// <summary>
	/// Responsible for managing the Test Definitions
	/// </summary>
	[Route("api/test-definitions")]
	public class TestDefinitionsController : Controller
	{
		private readonly JanitraContext _context;
		private readonly IMapper _mapper;

		/// <summary>
		/// Constructor
		/// </summary>
		public TestDefinitionsController(JanitraContext context)
		{
			_context = context;

			_mapper = CreateMapper();
		}

		private IMapper CreateMapper()
		{
			var config = new MapperConfiguration(cfg =>
			{
				cfg.CreateMap<TestDefinition, JsonTestDefinition>(MemberList.Destination)
					.ForMember(jjb => jjb.AddedByUserName, o => o.MapFrom(jb => jb.AddedByUser.OAuthName));

				cfg.CreateMap<TestRom, JsonTestRom>(MemberList.Destination)
					.ForMember(jtr => jtr.AddedByUserName, o => o.MapFrom(tr => tr.AddedByUser.OAuthName));
			});

			config.AssertConfigurationIsValid();

			return config.CreateMapper();
		}

		[HttpGet("list")]
		public async Task<JsonTestDefinition[]> List([FromQuery] bool includeInactive = false)
		{
			IQueryable<TestDefinition> query = _context.TestDefinitions
				.Include(td => td.TestRom)
				.Include(td => td.AddedByUser)
				.OrderByDescending(td => td.TestDefinitionId);

			if (!includeInactive)
				query = query.Where(t => t.ActivelyTesting);

			return await query
				.Select(t => _mapper.Map<JsonTestDefinition>(t))
				.ToArrayAsync();
		}

		public class JsonTestDefinition
		{
			[Required]
			public int TestDefinitionId { get; set; }

			[Required]
			public string TestName { get; set; }

			[Required]
			public string Notes { get; set; }

			/// <summary>
			/// Url the movie can be downloaded from
			/// </summary>
			[Required]
			public string MovieUrl { get; set; }

			[Required]
			public string MovieSha256 { get; set; }

			[Required]
			public bool ActivelyTesting { get; set; }

			[Required]
			public DateTimeOffset AddedAt { get; set; }
			[Required]
			public int AddedByUserId { get; set; }

			[Required]
			public string AddedByUserName { get; set; }

			[Required]
			public JsonTestRom TestRom { get; set; }
		}

		public class JsonTestRom
		{
			[Required]
			public int TestRomId { get; set; }

			public string FileName { get; set; }
			public string RomUrl { get; set; }

			[Required]
			public string RomSha256 { get; set; }

			public string CodeUrl { get; set; }

			[Required]
			public int AddedByUserId { get; set; }

			[Required]
			public string AddedByUserName { get; set; }
		}
	}
}