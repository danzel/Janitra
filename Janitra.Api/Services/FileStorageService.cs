using System.Threading.Tasks;

namespace Janitra.Api.Services
{
	public interface IFileStorageService
	{
		/// <summary>
		/// Save the given test rom to online storage and return the URL
		/// </summary>
		Task<string> StoreTestRom(byte[] romBytes);

		/// <summary>
		/// Save the given movie to online storage and return the URL
		/// </summary>
		Task<string> StoreMovie(byte[] movieBytes);

		Task<string> StoreLog(byte[] logBytes);

		Task<string> StoreScreenshot(byte[] screenshotBytes);
	}

	/// <summary>
	/// Doesn't save files, returns fake URLs
	/// </summary>
	internal class NullFileStorageService : IFileStorageService
	{
		public Task<string> StoreTestRom(byte[] romBytes)
		{
			return Task.FromResult("http://example.org");
		}

		public Task<string> StoreMovie(byte[] movieBytes)
		{
			return Task.FromResult("http://example.org");
		}

		public Task<string> StoreLog(byte[] logBytes)
		{
			return Task.FromResult("http://example.org");
		}

		public Task<string> StoreScreenshot(byte[] screenshotBytes)
		{
			return Task.FromResult("http://example.org");
		}
	}
}