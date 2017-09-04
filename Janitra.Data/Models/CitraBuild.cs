using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Janitra.Data.Models
{
	/// <summary>
	/// Represents a built version of citra
	/// </summary>
	public class CitraBuild
	{
		[Key]
		public int CitraBuildId { get; set; }

		/// <summary>
		/// The git hash of this build (if known)
		/// </summary>
		public string GitHash { get; set; }

		public BuildType BuildType { get; set; }

		[Required(AllowEmptyStrings = true)]
		public string BuildNotes { get; set; }

		public DateTimeOffset DateAdded { get; set; }

		/// <summary>
		/// Whether a JanitraBot should download and run tests against this build
		/// </summary>
		public bool ActivelyTesting { get; set; }

		/// <summary>
		/// Zip file location of the build for windows
		/// </summary>
		public string WindowsUrl { get; set; }

		/// <summary>
		/// TarXz file location of the build for linux
		/// </summary>
		public string LinuxUrl { get; set; }

		/// <summary>
		/// TarGz file location of the build for Mac OS X
		/// </summary>
		public string OsxUrl { get; set; }

		/// <summary>
		/// UserId of the user that added this build to the system
		/// </summary>
		public int AddedByUserId { get; set; }

		//Navigation Fields
		public User AddedByUser { get; set; }

		public ICollection<TestResult> TestResults { get; set; }
	}
}