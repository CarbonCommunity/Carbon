using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Guilds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild#guild-widget-object-guild-widget-structure">Guild Widget Settings Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildWidgetSettings
    {
        /// <summary>
        /// Whether the widget is enabled
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// The widget channel id
        /// </summary>
        [JsonProperty("channel_id")]
        public Snowflake ChannelId { get; set; }
    }
}
