namespace Janitra.Data.Models
{
    public enum TestResultType
    {
		/// <summary>
		/// Citra Crashed
		/// </summary>
		Crash,
		
		/// <summary>
		/// The test ran until completion
		/// </summary>
		Completed,

		/// <summary>
		/// The test did not complete before the timeout was reached. Usually indicates a deadlock
		/// </summary>
		Timeout
    }
}
