using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Janitra.Data.Models
{
	public class TestRom
	{
		[Key]
		public int TestRomId { get; set; }

		public RomType RomType { get; set; }

		/// <summary>
		/// Readable name for what to show this rom as.
		/// For HW Test name it something to do with what the test is testing
		/// </summary>
		[Required]
		public string ReadableName { get; set; }

		/// <summary>
		/// File name for the ROM.
		/// For commercial ROMs this should match what it is named when dumped using Godmode9(?)
		/// </summary>
		[Required]
		public string FileName { get; set; }

		/// <summary>
		/// A URL to download the rom from, if it is a homebrew ROM
		/// </summary>
		public string RomUrl { get; set; }

		/// <summary>
		/// Hex encoded sha256 hash value of the ROM (Lowercase)
		/// </summary>
		[Required]
		[MinLength(64)]
		[MaxLength(64)]
		public string RomSha256 { get; set; }

		/// <summary>
		/// For HWTest/Homebrew this is the location where the code for the rom is
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