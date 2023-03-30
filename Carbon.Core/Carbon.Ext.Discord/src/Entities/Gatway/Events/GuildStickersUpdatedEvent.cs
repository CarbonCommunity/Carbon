using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Stickers;
using Oxide.Ext.Discord.Helpers.Converters;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#guild-stickers-update">Guild Stickers Update</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildStickersUpdatedEvent
    {
        /// <summary>
        /// ID of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake GuildId { get; set; }

        /// <summary>
        /// List of emojis
        /// </summary>
        [JsonConverter(typeof(HashListConverter<DiscordSticker>))]
        [JsonProperty("stickers")]
        public Hash<Snowflake, DiscordSticker> Stickers { get; set; }
    }
}