using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Janitra.Data.Models
{
	/// <summary>
	/// Defines a test. Has a dependency on a ROM (either homebrew or commercial)
	/// </summary>
	public class TestDefinition
	{
		[Key]
		public int TestDefinitionId { get; set; }

		/// <summary>
		/// What this test is testing. For HW Test, name it what it tests.
		/// </summary>
		[Required]
		public string TestName { get; set; }

		public int TestRomId { get; set; }

		/// <summary>
		/// Url for the location of the movie file for this test
		/// </summary>
		[Required]
		public string MovieUrl { get; set; }

		/// <summary>
		/// Hex encoded sha256 hash value of the ROM (Lowercase)
		/// </summary>
		[Required]
		[MinLength(64)]
		[MaxLength(64)]
		public string MovieSha256 { get; set; }

		/// <summary>
		/// Whether a JanitraBot should download and run this test
		/// </summary>
		public bool ActivelyTesting { get; set; }

		public DateTimeOffset AddedAt { get; set; }

		public int AddedByUserId { get; set; }

		[Required(AllowEmptyStrings = true)]
		public string Notes { get; set; }

		//Navigation Fields

		/// <summary>
		/// The ROM used for this test
		/// </summary>
		public TestRom TestRom { get; set; }

		/// <summary>
		/// The User that added this test
		/// </summary>
		public User AddedByUser { get; set; }

		public ICollection<TestResult> TestResults { get; set; }
	}
}