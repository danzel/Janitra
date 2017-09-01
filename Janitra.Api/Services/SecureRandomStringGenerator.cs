using System;
using System.Security.Cryptography;

namespace Janitra.Api.Services
{
    internal static class SecureRandomStringGenerator
    {
	    static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

		public static string Generate()
		{

			var bytes = new byte[16];
			lock (_rng)
				_rng.GetBytes(bytes);
			var hashedBytes = SHA256.Create().ComputeHash(bytes);
			var result = BitConverter.ToString(hashedBytes);

			return result.Replace("-", "").ToLowerInvariant();
		}
    }
}
