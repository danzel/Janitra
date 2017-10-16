using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace Janitra.Services
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
	
	internal class AzureBlobStorageService : IFileStorageService
	{
		private readonly ILogger<AzureBlobStorageService> _logger;
		private readonly CloudBlobClient _client;
		private readonly string _baseUrl;

		public AzureBlobStorageService(string connectionString, ILogger<AzureBlobStorageService> logger)
		{
			_logger = logger;

			var account = CloudStorageAccount.Parse(connectionString);
			_client = account.CreateCloudBlobClient();

			_baseUrl = "https://" + account.Credentials.AccountName + ".blob.core.windows.net/";
		}

		private async Task<string> Store(string containerName, byte[] bytes, string extension, string contentType)
		{
			var container = _client.GetContainerReference(containerName);
			await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);

			var fileName = SHA256Hash.HashBytes(bytes);
			if (extension != null)
				fileName += extension;
			var blob = container.GetBlockBlobReference(fileName);

			if (await blob.ExistsAsync())
			{
				_logger.LogInformation("Skipping Storing file {fileName} in container {container}, already exists", fileName, containerName);
			}
			else
			{
				_logger.LogInformation("Storing file {fileName} in container {container}", fileName, containerName);
				blob.Properties.ContentType = contentType;
				await blob.UploadFromByteArrayAsync(bytes, 0, bytes.Length);
			}

			return _baseUrl + containerName + "/" + fileName;
		}

		public async Task<string> StoreTestRom(byte[] romBytes)
		{
			return await Store("testroms", romBytes, ".3dsx", "application/octet-stream");
		}

		public async Task<string> StoreMovie(byte[] movieBytes)
		{
			return await Store("movies", movieBytes, ".cts", "application/octet-stream");
		}

		public async Task<string> StoreLog(byte[] logBytes)
		{
			return await Store("logs", logBytes, ".txt", "text/plain");
		}

		public async Task<string> StoreScreenshot(byte[] screenshotBytes)
		{
			return await Store("screenshots", screenshotBytes, ".png", "image/png");
		}
	}
}