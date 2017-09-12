namespace Janitra.Data.Models
{
	public enum ExecutionResult
	{
		/// <summary>
		/// Citra Crashed
		/// </summary>
		Crash = 0,

		/// <summary>
		/// The test ran until completion
		/// </summary>
		Completed = 1,

		/// <summary>
		/// The test did not complete before the timeout was reached. Usually indicates a deadlock
		/// </summary>
		Timeout = 2
	}
}