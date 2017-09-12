namespace Janitra.Data.Models
{
	public enum UserLevel
	{
		/// <summary>
		/// Can't do anything a not logged in user can do
		/// </summary>
		Default = 0,

		/// <summary>
		/// Can add CitraBuilds, TestDefinitions, set accuracy of results and generate keys for JanitraBots
		/// </summary>
		Developer = 1
	}
}