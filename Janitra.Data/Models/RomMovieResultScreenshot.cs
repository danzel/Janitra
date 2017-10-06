using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Janitra.Data.Models
{
	public class RomMovieResultScreenshot
	{
		public int RomMovieResultScreenshotId { get; set; }

		public int RomMovieResultId { get; set; }

		public int FrameNumber { get; set; }

		[Required, Url]
		public string TopImageUrl { get; set; }
		[Required, Url]
		public string BottomImageUrl { get; set; }
	}
}