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
		Unset,

		/// <summary>
		/// The result is correct
		/// </summary>
		Correct,

		/// <summary>
		/// The result is incorrect
		/// </summary>
		Incorrect
	}
}