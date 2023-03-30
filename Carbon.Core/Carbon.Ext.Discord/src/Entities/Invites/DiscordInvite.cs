using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Channels.Stages;
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Entities.Invites
{
    /// <summary>
    /// Represents an <a href="https://discord.com/developers/docs/resources/invite#invite-object">Invite Structure</a> that when used, adds a user to a guild or group DM channel.
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DiscordInvite
    {
        /// <summary>
        /// The invite code (unique ID)
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// The guild this invite is for
        /// See <see cref="Guild"/>
        /// </summary>
        [JsonProperty("guild")]
        public DiscordGuild Guild { get; set; }
        
        /// <summary>
        /// The channel this invite is for
        /// See <see cref="Channel"/>
        /// </summary>
        [JsonProperty("channel")]
        public DiscordChannel Channel { get; set; }
        
        /// <summary>
        /// The user who created the invite
        /// See <see cref="DiscordUser"/>
        /// </summary>
        [JsonProperty("inviter")]
        public DiscordUser Inviter { get; set; }
        
        /// <summary>
        /// The target user for this invite
        /// See <see cref="DiscordUser"/>
        /// </summary>
        [JsonProperty("target_user")]
        public DiscordUser TargetUser { get; set; }
        
        /// <summary>
        /// The type of user target for this invite
        /// See <see cref="TargetUserType"/>
        /// </summary>
        [JsonProperty("target_user_type")]
        public TargetUserType? UserTargetType { get; set; }
        
        /// <summary>
        /// Approximate count of online members (only present when target_user is set)
        /// </summary>
        [JsonProperty("approximate_presence_count")]
        public int? ApproximatePresenceCount { get; set; }
        
        /// <summary>
        /// Approximate count of total members
        /// </summary>
        [JsonProperty("approximate_member_count")]
        public int? ApproximateMemberCount { get; set; }
        
        /// <summary>
        /// When the invite code expires
        /// </summary>
        [JsonProperty("expires_at")]
        public DateTime? ExpiresAt { get; set; }
        
        /// <summary>
        /// Stage instance data if there is a public Stage instance in the Stage channel this invite is for
        /// </summary>
        [JsonProperty("stage_instance")]
        public InviteStageInstance StageInstance { get; set; }
        
        /// <summary>
        /// Guild scheduled event data, only included if guild_scheduled_event_id contains a valid guild scheduled event id
        /// </summary>
        [JsonProperty("guild_scheduled_event")]
        public InviteStageInstance GuildScheduledEvent { get; set; }

        /// <summary>
        /// Returns an invite object for the given code.
        /// See <a href="https://discord.com/developers/docs/resources/invite#get-invite">Get Invite</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="inviteCode">Invite code</param>
        /// <param name="lookup">Lookup query string parameters for the request</param>
        /// <param name="callback">Callback with the invite</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public static void GetInvite(DiscordClient client, string inviteCode, InviteLookup lookup = null, Action<DiscordInvite> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/invites/{inviteCode}{lookup?.ToQueryString()}", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Delete an invite.
        /// Requires the MANAGE_CHANNELS permission on the channel this invite belongs to, or MANAGE_GUILD to remove any invite across the guild.
        /// Returns an invite object on success.
        /// See <a href="https://discord.com/developers/docs/resources/invite#delete-invite">Delete Invite</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with the deleted invite</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void DeleteInvite(DiscordClient client, Action<DiscordInvite> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/invites/{Code}", RequestMethod.DELETE, null, callback, error);
        }
    }
}
