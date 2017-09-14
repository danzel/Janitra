using System;
using System.Security.Cryptography;

namespace Janitra.Services
{
    internal static class SHA256Hash
    {
		/// <summary>
		/// Returns the SHA256 hash of the given bytes as a lowercase string
		/// </summary>
		public static string HashBytes(byte[] bytes)
		{
			var hashedBytes = SHA256.Create().ComputeHash(bytes);
			var result = BitConverter.ToString(hashedBytes);

			return result.Replace("-", "").ToLowerInvariant();
		}
	}
}
