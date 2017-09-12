using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Janitra.Data.Models
{
	/// <summary>
	/// A bot that runs the tests
	/// </summary>
	public class JanitraBot
	{
		[Key]
		public int JanitraBotId { get; set; }

		/// <summary>
		/// Name of this bot, a way to uniquely identify it if you are running multiple
		/// </summary>
		[Required]
		public string Name { get; set; }

		public OsType Os { get; set; }

		/// <summary>
		/// CPU, Memory, GPU Details of this Bot
		/// </summary>
		[Required(AllowEmptyStrings = true)]
		public string HardwareDetails { get; set; }

		/// <summary>
		/// Calculated by CryptoHelper.Crypto
		/// </summary>
		[Required]
		public string AccessKey { get; set; }

		public int AddedByUserId { get; set; }


		//Navigation Fields
		public User AddedByUser { get; set; }

		public ICollection<TestResult> TestResults { get; set; }
	}
}