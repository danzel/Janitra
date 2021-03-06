﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Janitra.Data.Models
{
	public class RomMovieResult
	{
		public int RomMovieResultId { get; set; }

		public int CitraBuildId { get; set; }
		public int JanitraBotId { get; set; }
		public int RomMovieId { get; set; }

		public DateTimeOffset ReportedAt { get; set; }

		[Required, Url]
		public string LogUrl { get; set; }

		public TimeSpan TimeTaken { get; set; }

		public ExecutionResult ExecutionResult { get; set; }

		//Navigation Fields
		public CitraBuild CitraBuild { get; set; }
		public JanitraBot JanitraBot { get; set; }
		public RomMovie RomMovie { get; set; }
		public ICollection<RomMovieResultScreenshot> Screenshots { get; set; }
	}
}