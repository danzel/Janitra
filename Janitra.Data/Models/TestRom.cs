using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Janitra.Data.Models
{
	public class TestRom
	{
		[Key]
		public int TestRomId { get; set; }

		/// <summary>
		/// A URL to download the rom from, if it is a homebrew ROM
		/// </summary>
		[Required]
		public string RomUrl { get; set; }

		/// <summary>
		/// Hex encoded sha256 hash value of the ROM (Lowercase)
		/// </summary>
		[Required]
		[MinLength(64)]
		[MaxLength(64)]
		public string RomSha256 { get; set; }

		/// <summary>
		/// This is the location where the code for the rom is
		/// </summary>
		public string CodeUrl { get; set; }

		public int AddedByUserId { get; set; }
		public DateTimeOffset AddedAt { get; set; }

		//Navigation Fields

		/// <summary>
		/// The user that added this ROM
		/// </summary>
		public User AddedByUser { get; set; }

		public ICollection<TestDefinition> UsedByTestDefinitions { get; set; }
	}
}