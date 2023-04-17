using System.ComponentModel;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Helpers.Converters;

namespace Oxide.Ext.Discord.Entities.Users.Connections
{
	/// <summary>
	/// Represents a <a href="https://discord.com/developers/docs/resources/user#connection-object-connection-structure">Connection Type</a> for a connection
	/// </summary>
	[JsonConverter(typeof(DiscordEnumConverter))]
	public enum ConnectionType
	{
		/// <summary>
		/// Discord Extension doesn't currently support this connection type
		/// </summary>
		Unknown,

		/// <summary>
		/// Connection type is Twitch
		/// </summary>
		[System.ComponentModel.Description("twitch")] Twitch,

		/// <summary>
		/// Connection type is Youtube
		/// </summary>
		[System.ComponentModel.Description("youtube")] Youtube,
	}
}
