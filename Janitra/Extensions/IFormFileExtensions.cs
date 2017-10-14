using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace Janitra.Extensions
{
	internal static class IFormFileExtensions
	{
		public static async Task<byte[]> GetAsBytes(this IFormFile file)
		{
			var bytes = new byte[file.Length];

			var rom = new MemoryStream(bytes);
			await file.CopyToAsync(rom);

			return bytes;
		}

		/// <summary>
		/// Load the image in the given file and convert it to PNG, returning the bytes of the PNG image
		/// </summary>
		public static async Task<byte[]> GetAsPngBytes(this IFormFile file)
		{
			using (var image = Image.Load(await file.GetAsBytes()))
			{
				using (var output = new MemoryStream())
				{
					image.Save(output, new PngEncoder { PngColorType = PngColorType.Rgb, CompressionLevel = 9 });
					return output.ToArray();
				}
			}
		}
	}
}