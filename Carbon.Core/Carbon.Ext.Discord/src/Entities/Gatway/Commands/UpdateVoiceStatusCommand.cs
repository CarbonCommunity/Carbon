using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Gatway.Commands
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#update-voice-state">Update Voice State</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class UpdateVoiceStatusCommand
    {
        /// <summary>
        /// ID of the guild
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake GuildId { get; set; }

        /// <summary>
        /// ID of the voice channel client wants to join (null if disconnecting)
        /// </summary>
        [JsonProperty("channel_id")]
        public Snowflake? ChannelId { get; set; }

        /// <summary>
        /// Is the client muted
        /// </summary>
        [JsonProperty("self_mute")]
        public bool SelfMute { get; set; }

        /// <summary>
        /// Is the client deafened
        /// </summary>
        [JsonProperty("self_deaf")]
        public bool SelfDeaf { get; set; }
    }
}