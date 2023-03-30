using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Invites;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#invite-create">Invite Create</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class InviteCreatedEvent
    {
        /// <summary>
        /// The channel the invite is for
        /// </summary>
        [JsonProperty("channel_id")]
        public Snowflake ChannelId { get; set; }

        /// <summary>
        /// The unique invite code
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// The time at which the invite was created
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// The guild of the invite
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake? GuildId { get; set; }

        /// <summary>
        /// The user that created the invite
        /// </summary>
        [JsonProperty("inviter")]
        public DiscordUser Inviter { get; set; }

        /// <summary>
        /// How long the invite is valid for (in seconds)
        /// </summary>
        [JsonProperty("max_age")]
        public int MaxAge { get; set; }

        /// <summary>
        /// The maximum number of times the invite can be use
        /// </summary>
        [JsonProperty("max_uses")]
        public int MaxUses { get; set; }

        /// <summary>
        /// The target user for this invite
        /// </summary>
        [JsonProperty("target_user")]
        public DiscordUser TargetUser { get; set; }

        /// <summary>
        /// The type of user target for this invite
        /// </summary>
        [JsonProperty("target_user")]
        public TargetUserType TargetUserType { get; set; }

        /// <summary>
        /// Whether or not the invite is temporary (invited users will be kicked on disconnect unless they're assigned a role)
        /// </summary>
        [JsonProperty("temporary")]
        public bool? Temporary { get; set; }

        /// <summary>
        /// How many times the invite has been used (always will be 0)
        /// </summary>
        [JsonProperty("uses")]
        public int? Uses { get; set; }
    }
}
