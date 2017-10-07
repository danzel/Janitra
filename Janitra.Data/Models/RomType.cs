namespace Janitra.Data.Models
{
	public enum RomType
	{
		/// <summary>
		/// A 3DS Rom. A game available on cartridge (possibly also downloadable from the eshop)
		/// </summary>
		Cartridge,
		/// <summary>
		/// A System Application, installed as part of the firmware
		/// </summary>
		SystemApplication,
		/// <summary>
		/// An application only available on the E Shop
		/// </summary>
		EShop,
		/// <summary>
		/// A Virtual Console title from the E Shop
		/// </summary>
		VirtualConsole
	}
}