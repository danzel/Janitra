namespace Janitra.Data.Models
{
	/// <summary>
	/// Whether the result of a test is correct or not
	/// </summary>
	public enum AccuracyStatus
	{
		/// <summary>
		/// Not sure if this is an accurate result or not
		/// </summary>
		Unset = 0,

		/// <summary>
		/// The result is incorrect
		/// </summary>
		Incorrect = 1,

		/// <summary>
		/// The result is mostly correct
		/// </summary>
		Good = 2,

		/// <summary>
		/// The result is pixel perfect with on device
		/// </summary>
		Perfect = 3
	}
}