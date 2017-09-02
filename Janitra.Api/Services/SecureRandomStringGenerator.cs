using System.Security.Cryptography;

namespace Janitra.Api.Services
{
    internal static class SecureRandomStringGenerator
    {
	    static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

		public static string Generate()
		{

			var bytes = new byte[16];
			lock (Rng)
				Rng.GetBytes(bytes);
			return SHA256Hash.HashBytes(bytes);
		}
    }
}
