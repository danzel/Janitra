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
		/// The result is correct
		/// </summary>
		Correct = 1,

		/// <summary>
		/// The result is incorrect
		/// </summary>
		Incorrect = 2
	}
}