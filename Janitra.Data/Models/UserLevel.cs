namespace Janitra.Data.Models
{
	public enum UserLevel
	{
		Default = 0,

		/// <summary>
		/// Can add CitraBuilds and generate keys for JanitraBots
		/// </summary>
		Developer = 1,

		/// <summary>
		/// Can generate keys for JanitraBots
		/// </summary>
		Tester = 2
	}
}