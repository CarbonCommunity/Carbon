using System.ComponentModel;

namespace Oxide.Ext.Discord.Entities.Stickers
{
	/// <summary>
	/// Represents <a href="https://discord.com/developers/docs/resources/sticker#sticker-format-types">Sticker Format Types</a>
	/// </summary>
	public enum StickerFormatType
	{
		/// <summary>
		/// Sticker format type PNG
		/// </summary>
		[System.ComponentModel.Description("PNG")]
		Png = 1,

		/// <summary>
		/// Sticker format type APNG
		/// </summary>
		[System.ComponentModel.Description("APNG")]
		Apng = 2,

		/// <summary>
		/// Sticker format type LOTTIE
		/// </summary>
		[System.ComponentModel.Description("LOTTIE")]
		Lottie = 3
	}
}
