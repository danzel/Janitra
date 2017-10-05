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

		public string Description { get; set; }

		public int RomId { get; set; }

		public int AddedByUserId { get; set; }
		public DateTimeOffset AddedAt { get; set; }

		public TimeSpan Length { get; set; }

		//TODO: Game/Emulator set up details - region_value  (hw renderer?, jits?)
		//TODO: Additional files that should be places in places


		//Navigation Fields
		public Rom Rom { get; set; }

		public ICollection<RomMovieResult> Results { get; set; }
	}
}