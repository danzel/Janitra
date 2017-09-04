using System;
using System.Collections.Generic;
using System.Text;

namespace Janitra.Data.Models
{
	/// <summary>
	/// What type of build this is. The reason 
	/// </summary>
	public enum BuildType
	{
		/// <summary>
		/// Any other type of build
		/// </summary>
		Custom = 0,

		//TODO: Other build types as below

		/// <summary>
		/// Built off the citra-emu/citra@master branch
		/// </summary>
		CitraMaster = 1,

		/// <summary>
		/// A pull request against the citra-emu/citra repository
		/// </summary>
		CitraPullRequest = 2
	}
}