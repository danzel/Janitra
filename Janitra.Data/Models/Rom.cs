using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Janitra.Data.Models
{
	/// <summary>
	/// Represents a commercial rom used for compatibility/performance testing
	/// </summary>
	public class Rom
	{
		public int RomId { get; set; }

		[Required]
		public string Name { get; set; }

		public RomType RomType { get; set; }

		public int AddedByUserId { get; set; }

		[Required]
		public string RomFileName { get; set; }

		//Maybe add: Product Code (CTR-?-????), Programid (16 chars hex string)

		/// <summary>
		/// Hex encoded sha256 hash value of the ROM (Lowercase)
		/// </summary>
		[Required, MinLength(64), MaxLength(64)]
		public string RomSha256 { get; set; }

		//Navigation Fields
		public User AddedByUser { get; set; }

		public ICollection<RomMovie> Movies { get; set; }
	}
}