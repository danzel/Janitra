using System;
using System.ComponentModel.DataAnnotations;

namespace Janitra.Data.Models
{
	/// <summary>
	/// Represents a built version of citra, usually 
	/// </summary>
	public class CitraBuild
	{
		[Key]
		public int CitraBuildId { get; set; }

		/// <summary>
		/// The git hash of this build (if known)
		/// </summary>
		public string GitHash { get; set; }

		public DateTimeOffset DateAdded { get; set; }

		/// <summary>
		/// Whether a JanitraBot should download and run tests against this build
		/// </summary>
		public bool ActivelyTesting { get; set; }

		/// <summary>
		/// Zip file location of the build for windows
		/// </summary>
		public string WindowsUri { get; set; }
		/// <summary>
		/// TarXz file location of the build
		/// </summary>
		public string LinuxUri { get; set; }
		/// <summary>
		/// TarGz file location of the build
		/// </summary>
		public string OsxUri { get; set; }

		/// <summary>
		/// UserId of the user that added this build to the system
		/// </summary>
		public int AddedByUserId { get; set; }

		//Navigation Fields
		public User AddedByUser { get; set; }
	}
}