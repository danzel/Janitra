using System.ComponentModel.DataAnnotations;

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

		//Navigation Fields
		public RomMovieResult RomMovieResult { get; set; }
	}
}