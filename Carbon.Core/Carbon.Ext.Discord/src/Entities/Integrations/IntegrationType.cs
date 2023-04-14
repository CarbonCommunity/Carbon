using System.ComponentModel;

namespace Oxide.Ext.Discord.Entities.Integrations
{
	/// <summary>
	/// Represents Integrations types
	/// </summary>
	public enum IntegrationType
	{
		/// <summary>
		/// Integration is for twitch
		/// </summary>
		[System.ComponentModel.Description("twitch")]
		Twitch,

		/// <summary>
		/// Integration is for youtube
		/// </summary>
		[System.ComponentModel.Description("youtube")]
		Youtube,

		/// <summary>
		/// integration is for discord
		/// </summary>
		[System.ComponentModel.Description("discord")]
		Discord
	}
}
