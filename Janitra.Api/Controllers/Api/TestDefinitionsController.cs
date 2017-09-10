using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Janitra.Api.Services;
using Janitra.Data;
using Janitra.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Janitra.Api.Controllers.Api
{
	/// <summary>
	/// Responsible for managing the Test Definitions
	/// </summary>
	[Route("api/test-definitions")]
	public class TestDefinitionsController : Controller
	{
		private readonly JanitraContext _context;
		private readonly CurrentUser _currentUser;
		private readonly IFileStorageService _fileStorage;
		private readonly IMapper _mapper;

		/// <summary>
		/// Constructor
		/// </summary>
		public TestDefinitionsController(JanitraContext context, CurrentUser currentUser, IFileStorageService fileStorage)
		{
			_context = context;
			_currentUser = currentUser;
			_fileStorage = fileStorage;

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
		/*
		[Authorize(Roles = "Developer")]
		[HttpPost("add")]
		[ProducesResponseType(typeof(AddTestResult), StatusCodes.Status200OK)]
		public async Task<IActionResult> Add([FromBody] NewTestDefinition newTest)
		{
			//newTestRom xor TestRomId
			if (newTest.TestRomId.HasValue == (newTest.NewTestRom != null))
				return BadRequest("Exactly one of TestRomId and NewTestRom must be set");

			var test = _mapper.Map<TestDefinition>(newTest);
			
			//Add the test rom (or set based on the selected one)
			if (newTest.NewTestRom != null)
			{
				var testRomUrl = await _fileStorage.StoreTestRom(newTest.NewTestRom.RomBytes);

				var testRom = _mapper.Map<TestRom>(newTest.NewTestRom);
				testRom.RomUrl = testRomUrl;
				testRom.AddedByUser = _currentUser.User;
				testRom.AddedAt = DateTimeOffset.UtcNow;
				testRom.RomSha256 = SHA256Hash.HashBytes(newTest.NewTestRom.RomBytes);
				await _context.AddAsync(testRom);
				test.TestRom = testRom;
			}
			else
			{
				test.TestRomId = newTest.TestRomId.Value;
			}

			//Add the movie
			test.MovieUrl = await _fileStorage.StoreMovie(newTest.MovieBytes);
			test.MovieSha256 = SHA256Hash.HashBytes(newTest.MovieBytes);

			//Add unmapped fields
			test.ActivelyTesting = true;
			test.AddedByUser = _currentUser.User;
			test.AddedAt = DateTimeOffset.UtcNow;

			await _context.AddAsync(test);
			await _context.SaveChangesAsync();

			return Ok(new AddTestResult
			{
				TestDefinitionId = test.TestDefinitionId
			});
		}
		*/
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