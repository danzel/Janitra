using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Janitra.Data.Models
{
	public class RomMovie
	{
		public int RomMovieId { get; set; }

		[Required]
		public string Name { get; set; }

		[Required]
		public string Description { get; set; }

		public int RomId { get; set; }

		public int AddedByUserId { get; set; }
		public DateTimeOffset AddedAt { get; set; }

		public bool ActivelyTesting { get; set; }

		public TimeSpan Length { get; set; }

		/// <summary>
		/// Url for the location of the movie file for this test
		/// </summary>
		[Required, Url]
		public string MovieUrl { get; set; }

		/// <summary>
		/// Hex encoded sha256 hash value of the ROM (Lowercase)
		/// </summary>
		[Required, MinLength(64), MaxLength(64)]
		public string MovieSha256 { get; set; }

		#region Citra settings

		/// <summary>
		/// The system region that Citra will use during emulation
		/// </summary>
		public int CitraRegionValue { get; set; }

		#endregion

		//TODO: Additional files that should be placed in places (DLC, Updates, Save Games)


		//Navigation Fields

		public User AddedByUser { get; set; }
		public Rom Rom { get; set; }
		public ICollection<RomMovieResult> Results { get; set; }
	}
}