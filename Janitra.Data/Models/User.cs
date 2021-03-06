﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Janitra.Data.Models
{
	public class User
	{
		[Key]
		public int UserId { get; set; }

		/// <summary>
		/// The name of the OAuth Provider the user authenticated with
		/// </summary>
		[Required]
		public string OAuthProvider { get; set; }

		/// <summary>
		/// The Id the OAuth provider gave for the user
		/// </summary>
		[Required]
		public string OAuthId { get; set; }

		/// <summary>
		/// The (login) name of the user from the oauth provider, how they will be identified in the app
		/// </summary>
		[Required]
		public string OAuthName { get; set; }

		public UserLevel UserLevel { get; set; }

		//Navigation Fields

		public ICollection<CitraBuild> AddedCitraBuilds { get; set; }
		public ICollection<JanitraBot> AddedJanitraBots { get; set; }
		public ICollection<Rom> AddedRoms { get; set; }
		public ICollection<RomMovie> AddedRomMovies { get; set; }
		public ICollection<TestDefinition> AddedTestDefinitions { get; set; }
		public ICollection<TestRom> AddedTestRoms { get; set; }
	}
}