using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#invite-delete">Invite Delete</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class InviteDeletedEvent
    {
        /// <summary>
        /// The channel of the invite
        /// </summary>
        [JsonProperty("channel_id")]
        public Snowflake ChannelId { get; set; }

        /// <summary>
        /// The guild of the invite
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake? GuildId { get; set; }

        /// <summary>
        /// The unique invite code
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }
    }
}
