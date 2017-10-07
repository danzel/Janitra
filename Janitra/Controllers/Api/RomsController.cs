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
	[Route("api/roms")]
    public class RomsController : Controller
    {
	    private readonly JanitraContext _context;
	    private readonly IMapper _mapper;

	    /// <summary>
	    /// Constructor
	    /// </summary>
	    public RomsController(JanitraContext context)
	    {
		    _context = context;

		    _mapper = CreateMapper();
	    }

	    private IMapper CreateMapper()
	    {
		    var config = new MapperConfiguration(cfg =>
		    {
			    cfg.CreateMap<Rom, JsonRom>(MemberList.Destination);
		    });

		    config.AssertConfigurationIsValid();

		    return config.CreateMapper();
	    }

		/// <summary>
		/// Get a list of all of the commercial roms that tests are performed against
		/// </summary>
		/// <returns></returns>
		[HttpGet("list")]
	    public async Task<JsonRom[]> List()
		{
			return await _context.Roms.Select(r => _mapper.Map<JsonRom>(r)).ToArrayAsync();
		}

		public class JsonRom
		{
			[Required]
			public int RomId { get; set; }

			/// <summary>
			/// In the format: Game Name (Region)
			/// </summary>
			[Required]
			public string Name { get; set; }

			[Required]
			public RomType RomType { get; set; }

			[Required]
			public int AddedByUserId { get; set; }

			[Required]
			public string RomFileName { get; set; }

			/// <summary>
			/// Hex encoded sha256 hash value of the ROM (Lowercase)
			/// </summary>
			[Required, MinLength(64), MaxLength(64)]
			public string RomSha256 { get; set; }
		}
	}
}
