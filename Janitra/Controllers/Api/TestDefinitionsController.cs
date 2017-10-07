using System;
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
				cfg.CreateMap<TestDefinition, JsonTestDefinition>(MemberList.Destination);

				cfg.CreateMap<TestRom, JsonTestRom>(MemberList.Destination);
			});

			config.AssertConfigurationIsValid();

			return config.CreateMapper();
		}

		/// <summary>
		/// Get a list of all TestDefinitions that are being actively tested
		/// </summary>
		/// <returns></returns>
		[HttpGet("list")]
		public async Task<JsonTestDefinition[]> List()
		{
			IQueryable<TestDefinition> query = _context.TestDefinitions
				.Where(t => t.ActivelyTesting)
				.Include(td => td.TestRom)
				.OrderByDescending(td => td.TestDefinitionId);


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

			/// <summary>
			/// Url the movie can be downloaded from
			/// </summary>
			[Required]
			public string MovieUrl { get; set; }

			[Required]
			public string MovieSha256 { get; set; }

			[Required]
			public DateTimeOffset AddedAt { get; set; }

			[Required]
			public JsonTestRom TestRom { get; set; }
		}

		public class JsonTestRom
		{
			[Required]
			public int TestRomId { get; set; }

			[Required]
			public string RomUrl { get; set; }

			[Required]
			public string RomSha256 { get; set; }
		}
	}
}