using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Channels.Stages;
using Oxide.Ext.Discord.Entities.Channels.Threads;
using Oxide.Ext.Discord.Entities.Emojis;
using Oxide.Ext.Discord.Entities.Gatway.Events;
using Oxide.Ext.Discord.Entities.Guilds.ScheduledEvents;
using Oxide.Ext.Discord.Entities.Integrations;
using Oxide.Ext.Discord.Entities.Invites;
using Oxide.Ext.Discord.Entities.Permissions;
using Oxide.Ext.Discord.Entities.Stickers;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Entities.Voice;
using Oxide.Ext.Discord.Exceptions;
using Oxide.Ext.Discord.Helpers.Cdn;
using Oxide.Ext.Discord.Helpers.Converters;
using Oxide.Ext.Discord.Interfaces;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Guilds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild#guild-object">Guild Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DiscordGuild : ISnowflakeEntity
    {
        #region Discord Fields
        /// <summary>
        /// Guild id
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }
        
        /// <summary>
        /// Name of the guild (2-100 characters)
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Base64 128x128 image for the guild icon
        /// </summary>
        [JsonProperty("icon")]        
        public string Icon { get; set; }
        
        /// <summary>
        /// Icon hash
        /// </summary>
        [JsonProperty("icon_Hash")]
        public string IconHash { get; set; }
        
        /// <summary>
        /// Splash hash
        /// </summary>
        [JsonProperty("splash")]
        public string Splash { get; set; }

        /// <summary>
        /// Discovery splash hash
        /// Only present for guilds with the "DISCOVERABLE" feature
        /// </summary>
        [JsonProperty("discovery_splash")]
        public string DiscoverySplash { get; set; }
  
        /// <summary>
        /// True if the user is the owner of the guild
        /// </summary>
        [JsonProperty("owner")]
        public bool? Owner { get; set; }
        
        /// <summary>
        /// ID of owner
        /// </summary>
        [JsonProperty("owner_id")]
        public Snowflake OwnerId { get; set; }

        /// <summary>
        /// Total permissions for the user in the guild (excludes overrides)
        /// </summary>
        [JsonProperty("permissions")]
        public string Permissions { get; set; }

        /// <summary>
        /// ID of afk channel
        /// </summary>
        [JsonProperty("afk_channel_id")]
        public Snowflake? AfkChannelId { get; set; }
        
        /// <summary>
        /// Afk timeout in seconds
        /// </summary>
        [JsonProperty("afk_timeout")]
        public int? AfkTimeout { get; set; }
  
        /// <summary>
        /// True if the server widget is enabled
        /// </summary>
        [JsonProperty("widget_enabled")]
        public bool? WidgetEnabled { get; set; }
  
        /// <summary>
        /// The channel id that the widget will generate an invite to, or null if set to no invite
        /// </summary>
        [JsonProperty("widget_channel_id")]
        public Snowflake? WidgetChannelId { get; set; }
        
        /// <summary>
        /// Verification level
        /// </summary>
        [JsonProperty("verification_level")]
        public GuildVerificationLevel? VerificationLevel { get; set; }
        
        /// <summary>
        /// Default message notification level
        /// </summary>
        [JsonProperty("default_message_notifications")]
        public DefaultNotificationLevel? DefaultMessageNotifications { get; set; }
        
        /// <summary>
        /// Explicit content filter level
        /// </summary>
        [JsonProperty("explicit_content_filter")]
        public ExplicitContentFilterLevel? ExplicitContentFilter { get; set; }

        /// <summary>
        /// Roles in the guild
        /// </summary>
        [JsonConverter(typeof(HashListConverter<DiscordRole>))]
        [JsonProperty("roles")]
        public Hash<Snowflake, DiscordRole> Roles { get; set; }
  
        /// <summary>
        /// Custom guild emojis
        /// </summary>
        [JsonConverter(typeof(HashListConverter<DiscordEmoji>))]
        [JsonProperty("emojis")]

        public Hash<Snowflake, DiscordEmoji> Emojis { get; set; }

        /// <summary>
        /// Enabled guild features
        /// See <see cref="GuildFeatures"/>
        /// </summary>
        [JsonProperty("features")]
        public List<GuildFeatures> Features { get; set; }
  
        /// <summary>
        /// Required MFA level for the guild
        /// See <see cref="GuildMFALevel"/>
        /// </summary>
        [JsonProperty("mfa_level")]
        public GuildMFALevel? MfaLevel { get; set; }
  
        /// <summary>
        /// Application id of the guild creator if it is bot-created
        /// </summary>
        [JsonProperty("application_id")]
        public Snowflake? ApplicationId { get; set; }
        
        /// <summary>
        /// The id of the channel where guild notices such as welcome messages and boost events are posted
        /// </summary>
        [JsonProperty("system_channel_id")]
        public Snowflake? SystemChannelId { get; set; }
        
        /// <summary>
        /// System channel flags
        /// See <see cref="SystemChannelFlags"/>
        /// </summary>
        [JsonProperty("system_channel_flags")]
        public SystemChannelFlags SystemChannelFlags { get; set; }

        /// <summary>
        /// The id of the channel where Community guilds can display rules and/or guidelines
        /// </summary>
        [JsonProperty("rules_channel_id")]
        public Snowflake? RulesChannelId { get; set; }
        
        /// <summary>
        /// When this guild was joined at
        /// </summary>
        [JsonProperty("joined_at")]
        public DateTime? JoinedAt { get; set; }
  
        /// <summary>
        /// True if this is considered a large guild
        /// </summary>
        [JsonProperty("large")]
        public bool? Large { get; set; }
  
        /// <summary>
        /// True if this guild is unavailable due to an outage
        /// </summary>
        [JsonProperty("unavailable")]
        public bool? Unavailable { get; set; }
  
        /// <summary>
        /// Total number of members in this guild
        /// </summary>
        [JsonProperty("member_count")]
        public int? MemberCount { get; set; }

        /// <summary>
        /// States of members currently in voice channels; lacks the guild_id key
        /// </summary>
        [JsonConverter(typeof(HashListConverter<VoiceState>))]
        [JsonProperty("voice_states")]
        public Hash<Snowflake, VoiceState> VoiceStates { get; set; }
  
        /// <summary>
        /// Users in the guild
        /// </summary>
        [JsonConverter(typeof(HashListConverter<GuildMember>))]
        [JsonProperty("members")]
        public Hash<Snowflake, GuildMember> Members { get; set; }

        /// <summary>
        /// Channels in the guild
        /// </summary>
        [JsonConverter(typeof(HashListConverter<DiscordChannel>))]
        [JsonProperty("channels")]
        public Hash<Snowflake, DiscordChannel> Channels { get; set; }
        
        /// <summary>
        /// All active threads in the guild that current user has permission to view
        /// </summary>
        [JsonConverter(typeof(HashListConverter<DiscordChannel>))]
        [JsonProperty("threads")]
        public Hash<Snowflake, DiscordChannel> Threads { get; set; }

        /// <summary>
        /// Presences of the members in the guild
        /// will only include non-offline members if the size is greater than large threshold
        /// </summary>
        [JsonProperty("presences")]
        public List<PresenceUpdatedEvent> Presences { get; set; }
  
        /// <summary>
        /// The maximum number of presences for the guild (the default value, currently 25000, is in effect when null is returned)
        /// </summary>
        [JsonProperty("max_presences")]
        public int? MaxPresences { get; set; }
  
        /// <summary>
        /// The maximum number of members for the guild
        /// </summary>
        [JsonProperty("max_members")]
        public int? MaxMembers { get; set; }
  
        /// <summary>
        /// The vanity url code for the guild
        /// </summary>
        [JsonProperty("vanity_url_code")]
        public string VanityUrlCode { get; set; }
  
        /// <summary>
        /// The description of a Community guild
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// Banner hash
        /// </summary>
        [JsonProperty("banner")]
        public string Banner { get; set; }

        /// <summary>
        /// Premium tier (Server Boost level)
        /// </summary>
        [JsonProperty("premium_tier")]
        public GuildPremiumTier? PremiumTier { get; set; }
  
        /// <summary>
        /// The number of boosts this guild currently has
        /// </summary>
        [JsonProperty("premium_subscription_count")]
        public int? PremiumSubscriptionCount { get; set; }

        /// <summary>
        /// The preferred locale of a Community guild
        /// Used in server discovery and notices from Discord
        /// Defaults to "en-US"
        /// </summary>
        [JsonProperty("preferred_locale")]
        public string PreferredLocale { get; set; }
        
        /// <summary>
        /// The maximum amount of users in a video channel
        /// </summary>
        [JsonProperty("public_updates_channel_id")]
        public Snowflake? PublicUpdatesChannelId { get; set; }
        
        /// <summary>
        /// The maximum amount of users in a video channel
        /// </summary>
        [JsonProperty("max_video_channel_users")]
        public int? MaxVideoChannelUsers { get; set; }
        
        /// <summary>
        /// Approximate number of members in this guild
        /// </summary>
        [JsonProperty("approximate_member_count")]
        public int? ApproximateMemberCount { get; set; }
        
        /// <summary>
        /// Approximate number of non-offline members in this guild
        /// </summary>
        [JsonProperty("approximate_presence_count")]
        public int? ApproximatePresenceCount { get; set; }
        
        /// <summary>
        /// The welcome screen of a Community guild
        /// Shown to new members, returned in an Invite's guild object
        /// </summary>
        [JsonProperty("welcome_screen")]
        public GuildWelcomeScreen WelcomeScreen { get; set; }
        
        /// <summary>
        /// Guild NSFW level
        /// <a href="https://support.discord.com/hc/en-us/articles/1500005389362-NSFW-Server-Designation">NSFW Information</a>
        /// </summary>
        [JsonProperty("nsfw_level")]
        public GuildNsfwLevel NsfwLevel { get; set; }
        
        /// <summary>
        /// Stage instances in the guild
        /// <see cref="StageInstance"/>
        /// </summary>
        [JsonConverter(typeof(HashListConverter<StageInstance>))]
        [JsonProperty("stage_instances")]
        public Hash<Snowflake, StageInstance> StageInstances { get; set; }
        
        /// <summary>
        /// Custom guild stickers
        /// <see cref="DiscordSticker"/>
        /// </summary>
        [JsonConverter(typeof(HashListConverter<DiscordSticker>))]
        [JsonProperty("stickers")]
        public Hash<Snowflake, DiscordSticker> Stickers { get; set; }
        
        /// <summary>
        /// The scheduled events in the guild
        /// <see cref="DiscordSticker"/>
        /// </summary>
        [JsonConverter(typeof(HashListConverter<GuildScheduledEvent>))]
        [JsonProperty("guild_scheduled_events")]
        public Hash<Snowflake, GuildScheduledEvent> ScheduledEvents { get; set; }        
        
        /// <summary>
        /// The scheduled events in the guild
        /// </summary>
        [JsonProperty("premium_progress_bar_enabled")]
        public bool PremiumProgressBarEnabled { get; set; }
        #endregion

        #region Extension Fields
        /// <summary>
        /// Returns true if all guild members have been loaded
        /// </summary>
        public bool HasLoadedAllMembers { get; internal set; }
        #endregion

        #region Helper Properties
        /// <summary>
        /// Returns if the guild is available to use
        /// </summary>
        public bool IsAvailable => Unavailable.HasValue && !Unavailable.Value;

        /// <summary>
        /// Returns the guild Icon Url
        /// </summary>
        public string IconUrl => DiscordCdn.GetGuildIconUrl(Id, Icon);
        
        /// <summary>
        /// Returns the Guilds Splash Url
        /// </summary>
        public string SplashUrl => DiscordCdn.GetGuildSplashUrl(Id, Splash);
        
        /// <summary>
        /// Returns the guilds Discovery Splash
        /// </summary>
        public string DiscoverySplashUrl => DiscordCdn.GetGuildDiscoverySplashUrl(Id, DiscoverySplash);
        
        /// <summary>
        /// Return the guilds Banner Url
        /// </summary>
        public string BannerUrl => DiscordCdn.GetGuildBannerUrl(Id, Banner);

        /// <summary>
        /// Returns the everyone role for the guild.
        /// </summary>
        public DiscordRole EveryoneRole => Roles[Id];
        #endregion

        #region Helper Methods
        /// <summary>
        /// Returns a channel with the given name (Case Insensitive)
        /// </summary>
        /// <param name="name">Name of the channel</param>
        /// <returns>Channel with the given name; Null otherwise</returns>
        public DiscordChannel GetChannel(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            foreach (DiscordChannel channel in Channels.Values)
            {
                if (channel.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return channel;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the parent channel for a channel if it exists
        /// </summary>
        /// <param name="channel">Channel to find the parent for</param>
        /// <returns>Parent channel for the given channel; null otherwise</returns>
        public DiscordChannel GetParentChannel(DiscordChannel channel)
        {
            if (!channel.ParentId.HasValue || !channel.ParentId.Value.IsValid())
            {
                return null;
            }

            return Channels[channel.ParentId.Value];
        }

        /// <summary>
        /// Returns a Role with the given name (Case Insensitive)
        /// </summary>
        /// <param name="name">Name of the role</param>
        /// <returns>Role with the given name; Null otherwise</returns>
        public DiscordRole GetRole(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            
            foreach (DiscordRole role in Roles.Values)
            {
                if (role.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return role;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the booster role for the guild if it exists
        /// </summary>
        /// <returns>Booster role; null otherwise</returns>
        public DiscordRole GetBoosterRole()
        {
            foreach (DiscordRole role in Roles.Values)
            {
                if (role.IsBoosterRole())
                {
                    return role;
                }
            }

            return null;
        }
        
        /// <summary>
        /// Returns a GuildMember with the given username (Case Insensitive)
        /// </summary>
        /// <param name="userName">Username of the GuildMember</param>
        /// <returns>GuildMember with the given username; Null otherwise</returns>
        public GuildMember GetMember(string userName)
        {
            if (userName == null)
            {
                throw new ArgumentNullException(nameof(userName));
            }
            
            if (userName.Contains("#"))
            {
                string[] splitName = userName.Split('#');
                userName = splitName[0];
                string discriminator = splitName[1];

                foreach (GuildMember member in Members.Values)
                {
                    if (member.User.Username.Equals(userName, StringComparison.OrdinalIgnoreCase) && member.User.Discriminator == discriminator)
                    {
                        return member;
                    }
                }
                
                return null;
            }

            foreach (GuildMember member in Members.Values)
            {
                if (member.User.Username.Equals(userName, StringComparison.OrdinalIgnoreCase))
                {
                    return member;
                }
            }
                
            return null;
        }

        /// <summary>
        /// Finds guild emoji by name
        /// </summary>
        /// <param name="name">Name of the emoji</param>
        /// <returns>Emoji if found; null otherwise</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public DiscordEmoji GetEmoji(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            foreach (DiscordEmoji emoji in Emojis.Values)
            {
                if (emoji.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return emoji;
                }
            }

            return null;
        }
        #endregion

        #region API Methods
        /// <summary>
        /// Create a new guild.
        /// See <a href="https://discord.com/developers/docs/resources/guild#create-guild">Create Guild</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="create">Guild Create Object</param>
        /// <param name="callback">Callback with the created guild</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public static void CreateGuild(DiscordClient client, GuildCreate create, Action<DiscordGuild> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds", RequestMethod.POST, create, callback, error);
        }

        /// <summary>
        /// Returns the guild object for the given id
        /// See <a href="https://discord.com/developers/docs/resources/guild#get-guild">Get Guild</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="guildId">Guild ID to lookup</param>
        /// <param name="callback">callback with the guild for the given ID</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public static void GetGuild(DiscordClient client, Snowflake guildId, Action<DiscordGuild> callback = null, Action<RestError> error = null)
        {
            if (!guildId.IsValid()) throw new InvalidSnowflakeException(nameof(guildId));
            client.Bot.Rest.DoRequest($"/guilds/{guildId}", RequestMethod.GET, null, callback, error);
        }
        
        /// <summary>
        /// Returns the guild preview object for the given id.
        /// If the user is not in the guild, then the guild must be Discoverable.
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="guildId">Guild ID to get preview for</param>
        /// <param name="callback">Callback with the guild preview for the ID</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public static void GetGuildPreview(DiscordClient client, Snowflake guildId, Action<GuildPreview> callback = null, Action<RestError> error = null)
        {
            if (!guildId.IsValid()) throw new InvalidSnowflakeException(nameof(guildId));
            client.Bot.Rest.DoRequest($"/guilds/{guildId}/preview", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Modify a guild's settings.
        /// Requires the MANAGE_GUILD permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#modify-guild">Modify Guild</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with the updated guild</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ModifyGuild(DiscordClient client, Action<DiscordGuild> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}", RequestMethod.PATCH, this, callback, error);
        }

        /// <summary>
        /// Delete a guild permanently.
        /// User must be owner
        /// See <a href="https://discord.com/developers/docs/resources/guild#delete-guild">Delete Guild</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void DeleteGuild(DiscordClient client, Action callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}", RequestMethod.DELETE, null, callback, error);
        }

        /// <summary>
        /// Returns a list of guild channel objects.
        /// Does not include threads
        /// See <a href="https://discord.com/developers/docs/resources/guild#get-guild-channels">Get Guild Channels</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with the list of channels</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildChannels(DiscordClient client, Action<List<DiscordChannel>> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/channels", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Create a new channel object for the guild.
        /// Requires the MANAGE_CHANNELS permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#create-guild-channel">Create Guild Channel</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="channel">Channel to create</param>
        /// <param name="callback">Callback with created channel</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void CreateGuildChannel(DiscordClient client, ChannelCreate channel, Action<DiscordChannel> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/channels", RequestMethod.POST, channel, callback, error);
        }

        /// <summary>
        /// Modify the positions of a set of channel objects for the guild.
        /// Requires MANAGE_CHANNELS permission.
        /// Only channels to be modified are required, with the minimum being a swap between at least two channels.
        /// See <a href="https://discord.com/developers/docs/resources/guild#modify-guild-channel-positions">Modify Guild Channel Positions</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="positions">List new channel positions for each channel</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ModifyGuildChannelPositions(DiscordClient client, List<GuildChannelPosition> positions, Action callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/channels", RequestMethod.PATCH, positions, callback, error);
        }

        /// <summary>
        /// Returns all active threads in the guild, including public and private threads. Threads are ordered by their id, in descending order.
        /// See <a href="https://discord.com/developers/docs/resources/guild#list-active-threads">List Active Threads</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with a list of threads and thread members for a guild</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ListActiveThreads(DiscordClient client, Action<ThreadList> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/threads/active", RequestMethod.GET, null, callback, error);
        }
        
        /// <summary>
        /// Returns a guild member object for the specified user.
        /// See <a href="https://discord.com/developers/docs/resources/guild#get-guild-member">Get Guild Member</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="userId">UserID to get guild member for</param>
        /// <param name="callback">Callback with guild member matching user Id</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildMember(DiscordClient client, Snowflake userId, Action<GuildMember> callback = null, Action<RestError> error = null)
        {
            if (!userId.IsValid()) throw new InvalidSnowflakeException(nameof(userId));
            client.Bot.Rest.DoRequest($"/guilds/{Id}/members/{userId}", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Returns a list of guild member objects that are members of the guild.
        /// In the future, this endpoint will be restricted in line with our Privileged Intents
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="limit">Max number of members to return (1-1000)</param>
        /// <param name="afterSnowflake">The highest user id in the previous page</param>
        /// <param name="callback">Callback with list of guild members</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ListGuildMembers(DiscordClient client, int limit = 1000, string afterSnowflake = "0", Action<List<GuildMember>> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/members?limit={limit}&after={afterSnowflake}", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Searches for guild members by username or nickname
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="search">Username or nickname to match</param>
        /// <param name="limit">Max number of members to return (1-1000)</param>
        /// <param name="callback">Callback with matching guild members</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void SearchGuildMembers(DiscordClient client, string search, int limit = 1, Action<List<GuildMember>> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/members/search?query={search}&limit={limit}", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Adds a user to the guild, provided you have a valid oauth2 access token for the user with the guilds.join scope. 
        /// See <a href="https://discord.com/developers/docs/resources/guild#add-guild-member">Add Guild Member</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="member">Member to copy from</param>
        /// <param name="accessToken">Member access token</param>
        /// <param name="roles">List of roles to grant</param>
        /// <param name="callback">Callback with the added guild member</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void AddGuildMember(DiscordClient client, GuildMember member, string accessToken, List<DiscordRole> roles, Action<GuildMember> callback = null, Action<RestError> error = null)
        {
            GuildMemberAdd add = new GuildMemberAdd
            {
                Deaf = member.Deaf,
                Mute = member.Mute,
                Nick = member.Nickname,
                AccessToken = accessToken,
                Roles = new List<Snowflake>()
            };

            foreach (DiscordRole role in roles)
            {
                add.Roles.Add(role.Id);
            }
            
            AddGuildMember(client, member.User.Id, add, callback, error);
        }

        /// <summary>
        /// Adds a user to the guild, provided you have a valid oauth2 access token for the user with the guilds.join scope. 
        /// See <a href="https://discord.com/developers/docs/resources/guild#add-guild-member">Add Guild Member</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="userId">User ID of the user to add</param>
        /// <param name="member">Member to copy from</param>
        /// <param name="callback">Callback with the added guild member</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void AddGuildMember(DiscordClient client, Snowflake userId, GuildMemberAdd member, Action<GuildMember> callback = null, Action<RestError> error = null)
        {
            if (!userId.IsValid()) throw new InvalidSnowflakeException(nameof(userId));
            client.Bot.Rest.DoRequest($"/guilds/{Id}/members/{userId}", RequestMethod.PUT, member, callback, error);
        }

        /// <summary>
        /// Modify attributes of a guild member
        /// See <a href="https://discord.com/developers/docs/resources/guild#modify-guild-member">Modify Guild Member</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="userId">User ID of the user to update</param>
        /// <param name="update">Changes to make to the user</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ModifyGuildMember(DiscordClient client, Snowflake userId, GuildMemberUpdate update, Action<GuildMember> callback = null, Action<RestError> error = null)
        {
            if (!userId.IsValid()) throw new InvalidSnowflakeException(nameof(userId));
            client.Bot.Rest.DoRequest($"/guilds/{Id}/members/{userId}", RequestMethod.PATCH, update, callback, error);
        }
        
        /// <summary>
        /// Modify attributes of a guild member
        /// See <a href="https://discord.com/developers/docs/resources/guild#modify-guild-member">Modify Guild Member</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="userId">User ID of the user to update</param>
        /// <param name="nick">Nickname for the user</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ModifyUsersNick(DiscordClient client, Snowflake userId, string nick, Action<GuildMember> callback = null, Action<RestError> error = null)
        {
            if (!userId.IsValid()) throw new InvalidSnowflakeException(nameof(userId));
            GuildMemberUpdate update = new GuildMemberUpdate
            {
                Nick = nick
            };
            
            ModifyGuildMember(client, userId, update, callback, error);
        }
        
        /// <summary>
        /// Modifies the current members nickname in the guild
        /// See <a href="https://discord.com/developers/docs/resources/guild#modify-current-member">Modify Current Member</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="nick">New members nickname</param>
        /// <param name="callback">Callback with the updated guild member</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ModifyCurrentMember(DiscordClient client, string nick, Action<GuildMember> callback, Action<RestError> error)
        {
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                ["nick"] = nick
            };
            
            client.Bot.Rest.DoRequest($"/guilds/{Id}/members/@me", RequestMethod.PATCH, data, callback, error);
        }

        /// <summary>
        /// Modifies the nickname of the current user in a guild
        /// See <a href="https://discord.com/developers/docs/resources/guild#modify-current-user-nick">Modify Current User Nick</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="nick">New user nickname</param>
        /// <param name="callback">Callback with updated nickname</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        [Obsolete("Please use ModifyCurrentMember Instead. This will be removed in April 2022 Update")]
        public void ModifyCurrentUsersNick(DiscordClient client, string nick, Action<string> callback = null, Action<RestError> error = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                ["nick"] = nick
            };

            client.Bot.Rest.DoRequest($"/guilds/{Id}/members/@me/nick", RequestMethod.PATCH, data, callback, error);
        }

        /// <summary>
        /// Adds a role to a guild member.
        /// Requires the MANAGE_ROLES permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#add-guild-member-role">Add Guild Member Role</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="user">User to add role to</param>
        /// <param name="role">Role to add</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void AddGuildMemberRole(DiscordClient client, DiscordUser user, DiscordRole role, Action callback = null, Action<RestError> error = null) => AddGuildMemberRole(client, user.Id, role.Id, callback, error);

        /// <summary>
        /// Adds a role to a guild member.
        /// Requires the MANAGE_ROLES permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#add-guild-member-role">Add Guild Member Role</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="userId">User ID to add role to</param>
        /// <param name="roleId">Role ID to add</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void AddGuildMemberRole(DiscordClient client, Snowflake userId, Snowflake roleId, Action callback = null, Action<RestError> error = null)
        {
            if (!userId.IsValid()) throw new InvalidSnowflakeException(nameof(userId));
            if (!roleId.IsValid()) throw new InvalidSnowflakeException(nameof(roleId));
            client.Bot.Rest.DoRequest($"/guilds/{Id}/members/{userId}/roles/{roleId}", RequestMethod.PUT, null, callback, error);
        }

        /// <summary>
        /// Removes a role from a guild member.
        /// Requires the MANAGE_ROLES permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#remove-guild-member-role">Remove Guild Member Role</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="user">User to remove role form</param>
        /// <param name="role">Role to remove</param>
        /// <param name="callback">callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void RemoveGuildMemberRole(DiscordClient client, DiscordUser user, DiscordRole role, Action callback = null, Action<RestError> error = null) => RemoveGuildMemberRole(client, user.Id, role.Id, callback, error);

        /// <summary>
        /// Removes a role from a guild member.
        /// Requires the MANAGE_ROLES permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#remove-guild-member-role">Remove Guild Member Role</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="userId">User ID to remove role form</param>
        /// <param name="roleId">Role ID to remove</param>
        /// <param name="callback">callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void RemoveGuildMemberRole(DiscordClient client, Snowflake userId, Snowflake roleId, Action callback = null, Action<RestError> error = null)
        {
            if (!userId.IsValid()) throw new InvalidSnowflakeException(nameof(userId));
            if (!roleId.IsValid()) throw new InvalidSnowflakeException(nameof(roleId));
            client.Bot.Rest.DoRequest($"/guilds/{Id}/members/{userId}/roles/{roleId}", RequestMethod.DELETE, null, callback, error);
        }

        /// <summary>
        /// Remove a member from a guild.
        /// Requires KICK_MEMBERS permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#remove-guild-member">Remove Guild Member</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="member">Guild Member to remove</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void RemoveGuildMember(DiscordClient client, GuildMember member, Action callback = null, Action<RestError> error = null) => RemoveGuildMember(client, member.User.Id, callback, error);

        /// <summary>
        /// Remove a member from a guild.
        /// Requires KICK_MEMBERS permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#remove-guild-member">Remove Guild Member</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="userId">User ID of the user to remove</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void RemoveGuildMember(DiscordClient client, Snowflake userId, Action callback = null, Action<RestError> error = null)
        {
            if (!userId.IsValid()) throw new InvalidSnowflakeException(nameof(userId));
            client.Bot.Rest.DoRequest($"/guilds/{Id}/members/{userId}", RequestMethod.DELETE, null, callback, error);
        }

        /// <summary>
        /// Returns a list of ban objects for the users banned from this guild.
        /// See <a href="https://discord.com/developers/docs/resources/guild#get-guild-bans">Get Guild Bans</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with the list of guild bans</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildBans(DiscordClient client, Action<List<GuildBan>> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/bans", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Returns a guild ban for a specific user
        /// See <a href="https://discord.com/developers/docs/resources/guild#get-guild-ban">Get Guild Ban</a>
        /// Returns 404 if not found
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="userId">User ID to get guild ban for</param>
        /// <param name="callback">Callback with the guild ban for the user</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildBan(DiscordClient client, Snowflake userId, Action<GuildBan> callback = null, Action<RestError> error = null)
        {
            if (!userId.IsValid()) throw new InvalidSnowflakeException(nameof(userId));
            client.Bot.Rest.DoRequest($"/guilds/{Id}/bans/{userId}", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Create a guild ban, and optionally delete previous messages sent by the banned user.
        /// Requires the BAN_MEMBERS permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#create-guild-ban">Create Guild Ban</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="member">Guild Member to ban</param>
        /// <param name="ban">User ban information</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void CreateGuildBan(DiscordClient client, GuildMember member, GuildBanCreate ban, Action callback = null, Action<RestError> error = null) => CreateGuildBan(client, member.User.Id, ban, callback, error);

        /// <summary>
        /// Create a guild ban, and optionally delete previous messages sent by the banned user.
        /// Requires the BAN_MEMBERS permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#create-guild-ban">Create Guild Ban</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="userId">User ID to ban</param>
        /// <param name="ban">User ban information</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void CreateGuildBan(DiscordClient client, Snowflake userId, GuildBanCreate ban, Action callback = null, Action<RestError> error = null)
        {
            if (!userId.IsValid()) throw new InvalidSnowflakeException(nameof(userId));
            client.Bot.Rest.DoRequest($"/guilds/{Id}/bans/{userId}", RequestMethod.PUT, ban, callback, error);
        }

        /// <summary>
        /// Remove the ban for a user.
        /// Requires the BAN_MEMBERS permissions.
        /// See <a href="https://discord.com/developers/docs/resources/guild#remove-guild-ban">Remove Guild Ban</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="userId">User ID of the user to unban</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void RemoveGuildBan(DiscordClient client, Snowflake userId, Action callback = null, Action<RestError> error = null)
        {
            if (!userId.IsValid()) throw new InvalidSnowflakeException(nameof(userId));
            client.Bot.Rest.DoRequest($"/guilds/{Id}/bans/{userId}", RequestMethod.DELETE, null, callback, error);
        }

        /// <summary>
        /// Returns a list of role objects for the guild.
        /// See <a href="https://discord.com/developers/docs/resources/guild#get-guild-roles">Get Guild Roles</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with a list of role objects</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildRoles(DiscordClient client, Action<List<DiscordRole>> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/roles", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Create a new role for the guild.
        /// Requires the MANAGE_ROLES permission.
        /// Returns the new role object on success.
        /// See <a href="https://discord.com/developers/docs/resources/guild#create-guild-role">Create Guild Role</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="role">New role to create</param>
        /// <param name="callback">Callback with the created role</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void CreateGuildRole(DiscordClient client, DiscordRole role, Action<DiscordRole> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/roles", RequestMethod.POST, role, callback, error);
        }

        /// <summary>
        /// Modify the positions of a set of role objects for the guild.
        /// Requires the MANAGE_ROLES permission.
        /// Returns a list of all of the guild's role objects on success.
        /// See <a href="https://discord.com/developers/docs/resources/guild#modify-guild-role-positions">Modify Guild Role Positions</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="positions">List of role with updated positions</param>
        /// <param name="callback">Callback with a list of all guild role objects</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ModifyGuildRolePositions(DiscordClient client, List<GuildRolePosition> positions, Action<List<DiscordRole>> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/roles", RequestMethod.PATCH, positions, callback, error);
        }

        /// <summary>
        /// Modify a guild role.
        /// Requires the MANAGE_ROLES permission.
        /// Returns the updated role on success.
        /// See <a href="https://discord.com/developers/docs/resources/guild#modify-guild-role">Modify Guild Role</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="role">Role to update</param>
        /// <param name="callback">Callback with the updated role</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ModifyGuildRole(DiscordClient client, DiscordRole role, Action<DiscordRole> callback = null, Action<RestError> error = null) => ModifyGuildRole(client, role.Id, role, callback, error);

        /// <summary>
        /// Modify a guild role.
        /// Requires the MANAGE_ROLES permission.
        /// Returns the updated role on success.
        /// See <a href="https://discord.com/developers/docs/resources/guild#modify-guild-role">Modify Guild Role</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="roleId">Role ID to update</param>
        /// <param name="role">Role to update</param>
        /// <param name="callback">Callback with the updated role</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ModifyGuildRole(DiscordClient client, Snowflake roleId, DiscordRole role, Action<DiscordRole> callback = null, Action<RestError> error = null)
        {
            if (!roleId.IsValid()) throw new InvalidSnowflakeException(nameof(roleId));
            client.Bot.Rest.DoRequest($"/guilds/{Id}/roles/{roleId}", RequestMethod.PATCH, role, callback, error);
        }

        /// <summary>
        /// Delete a guild role.
        /// Requires the MANAGE_ROLES permission
        /// See <a href="https://discord.com/developers/docs/resources/guild#delete-guild-role">Delete Guild Role</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="role">Role to Delete</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void DeleteGuildRole(DiscordClient client, DiscordRole role, Action callback = null, Action<RestError> error = null) => DeleteGuildRole(client, role.Id, callback, error);

        /// <summary>
        /// Delete a guild role.
        /// Requires the MANAGE_ROLES permission
        /// See <a href="https://discord.com/developers/docs/resources/guild#delete-guild-role">Delete Guild Role</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="roleId">Role ID to Delete</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void DeleteGuildRole(DiscordClient client, Snowflake roleId, Action callback = null, Action<RestError> error = null)
        {
            if (!roleId.IsValid()) throw new InvalidSnowflakeException(nameof(roleId));
            client.Bot.Rest.DoRequest($"/guilds/{Id}/roles/{roleId}", RequestMethod.DELETE, null, callback, error);
        }

        /// <summary>
        /// Returns an object with one 'pruned' key indicating the number of members that would be removed in a prune operation.
        /// Requires the KICK_MEMBERS permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#get-guild-prune-count">Get Guild Prune Count</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="prune">Prune get request</param>
        /// <param name="callback">Callback with the number of members that would be pruned</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildPruneCount(DiscordClient client, GuildPruneGet prune, Action<int?> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest<JObject>($"/guilds/{Id}/prune?{prune.ToQueryString()}", RequestMethod.GET, null, returnValue =>
            {
                int? pruned = returnValue.GetValue("pruned").ToObject<int?>();
                callback?.Invoke(pruned);
            }, error);
        }

        /// <summary>
        /// Begin a prune operation.
        /// Requires the KICK_MEMBERS permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#begin-guild-prune">Begin Guild Prune</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="prune">Prune begin request</param>
        /// <param name="callback">Callback with number of pruned members</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void BeginGuildPrune(DiscordClient client, GuildPruneBegin prune, Action<int?> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest<JObject>($"/guilds/{Id}/prune?{prune.ToQueryString()}", RequestMethod.POST, null, returnValue =>
            {
                int? pruned = returnValue.GetValue("pruned").ToObject<int?>();
                callback?.Invoke(pruned);
            }, error);
        }

        /// <summary>
        /// Returns a list of voice region objects for the guild.
        /// Unlike the similar /voice route, this returns VIP servers when the guild is VIP-enabled.
        /// See <a href="https://discord.com/developers/docs/resources/guild#get-guild-voice-regions">Get Guild Voice Regions</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with list of guild voice regions</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildVoiceRegions(DiscordClient client, Action<List<VoiceRegion>> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/regions", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Returns a list of invite objects (with invite metadata) for the guild.
        /// Requires the MANAGE_GUILD permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#get-guild-invites">Get Guild Invites</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with a list of guild invites</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildInvites(DiscordClient client, Action<List<InviteMetadata>> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/invites", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Returns a list of integration objects for the guild.
        /// Requires the MANAGE_GUILD permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#get-guild-integrations">Get Guild Integrations</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with a list of guild integrations</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildIntegrations(DiscordClient client, Action<List<Integration>> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/integrations", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Delete the attached integration object for the guild.
        /// Deletes any associated webhooks and kicks the associated bot if there is one.
        /// Requires the MANAGE_GUILD permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#delete-guild-integration">Delete Guild Integration</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="integration">Integration to delete</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void DeleteGuildIntegration(DiscordClient client, Integration integration, Action callback = null, Action<RestError> error = null) => DeleteGuildIntegration(client, integration.Id, callback, error);

        /// <summary>
        /// Delete the attached integration object for the guild.
        /// Deletes any associated webhooks and kicks the associated bot if there is one.
        /// Requires the MANAGE_GUILD permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#delete-guild-integration">Delete Guild Integration</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="integrationId">Integration ID to delete</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void DeleteGuildIntegration(DiscordClient client, Snowflake integrationId, Action callback = null, Action<RestError> error = null)
        {
            if (!integrationId.IsValid()) throw new InvalidSnowflakeException(nameof(integrationId));
            client.Bot.Rest.DoRequest($"/guilds/{Id}/integrations/{integrationId}", RequestMethod.DELETE, null, callback, error);
        }

        /// <summary>
        /// Returns a guild widget object.
        /// Requires the MANAGE_GUILD permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#get-guild-widget-settings">Get Guild Widget Settings</a>
        /// </summary>
        /// <param name="client">client to use</param>
        /// <param name="callback">Callback with guild widget settings</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildWidgetSettings(DiscordClient client, Action<GuildWidgetSettings> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/widget", RequestMethod.GET, null, callback, error);
        }
        
        /// <summary>
        /// Modify a guild widget object for the guild.
        /// Requires the MANAGE_GUILD permission.
        /// See <a href="https://discord.com/developers/docs/resources/guild#modify-guild-widget">Modify Guild Widget</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="widget">Updated widget</param>
        /// <param name="callback">Callback with update guild widget</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ModifyGuildWidget(DiscordClient client, GuildWidget widget, Action<GuildWidget> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/widget", RequestMethod.PATCH, widget, callback, error);
        }

        /// <summary>
        /// Returns the widget for the guild.
        /// See <a href="https://discord.com/developers/docs/resources/guild#get-guild-widget">Get Guild Widget</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with guild widget</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildWidget(DiscordClient client, Action<GuildWidget> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/widget.json", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Returns the Welcome Screen object for the guild.
        /// Requires the `MANAGE_GUILD` permission.
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with welcome screen for the guild</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildWelcomeScreen(DiscordClient client, Action<GuildWelcomeScreen> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/welcome-screen", RequestMethod.GET, null, callback, error);
        }

        /// <summary>
        /// Modify the guild's Welcome Screen.
        /// Requires the MANAGE_GUILD permission.
        /// Returns the updated Welcome Screen object.
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="update">Update to be made to the welcome screen</param>
        /// <param name="callback">Callback with updated welcome screen for the guild</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ModifyWelcomeScreen(DiscordClient client, WelcomeScreenUpdate update, Action<GuildWelcomeScreen> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/welcome-screen", RequestMethod.PATCH, update, callback, error);
        }

        /// <summary>
        /// Returns a partial invite object for guilds with that feature enabled.
        /// Requires the MANAGE_GUILD permission.
        /// Code will be null if a vanity url for the guild is not set.
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with invite </param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildVanityUrl(DiscordClient client, Action<InviteMetadata> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/vanity-url", RequestMethod.GET, null, callback, error);
        }
        
        /// <summary>
        /// Returns a list of emoji objects for the given guild.
        /// See <a href="https://discord.com/developers/docs/resources/emoji#list-guild-emojis">List Guild Emojis</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with list of guild emojis</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ListGuildEmojis(DiscordClient client, Action<List<DiscordEmoji>> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/emojis", RequestMethod.GET, null, callback, error);
        }
        
        /// <summary>
        /// Returns an emoji object for the given guild and emoji IDs.
        /// See <a href="https://discord.com/developers/docs/resources/emoji#get-guild-emoji">Get Guild Emoji</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="emjoiId">Emoji to lookup</param>
        /// <param name="callback">Callback with the guild emoji</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildEmoji(DiscordClient client, string emjoiId, Action<DiscordEmoji> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/emojis/{emjoiId}", RequestMethod.GET, null, callback, error);
        }
        
        /// <summary>
        /// Create a new emoji for the guild.
        /// Requires the MANAGE_EMOJIS permission.
        /// Returns the new emoji object on success.
        /// See <a href="https://discord.com/developers/docs/resources/emoji#create-guild-emoji">Create Guild Emoji</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="emoji">Emoji to create</param>
        /// <param name="callback">Callback with the created emoji</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void CreateGuildEmoji(DiscordClient client, EmojiCreate emoji, Action<DiscordEmoji> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/emojis", RequestMethod.POST, emoji, callback, error);
        }
        
        /// <summary>
        /// Modify the given emoji.
        /// Requires the MANAGE_EMOJIS permission.
        /// Returns the updated emoji object on success.
        /// See <a href="https://discord.com/developers/docs/resources/emoji#modify-guild-emoji">Modify Guild Emoji</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="emojiId">Emoji ID to update</param>
        /// <param name="emoji">Emoji update</param>
        /// <param name="callback">Callback with the updated emoji</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void UpdateGuildEmoji(DiscordClient client, string emojiId, EmojiUpdate emoji, Action<DiscordEmoji> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/emojis/{emojiId}", RequestMethod.PATCH, emoji, callback, error);
        }
        
        /// <summary>
        /// Delete the given emoji.
        /// Requires the MANAGE_EMOJIS permission.
        /// See <a href="https://discord.com/developers/docs/resources/emoji#delete-guild-emoji">Delete Guild Emoji</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="emojiId">Emoji ID to delete</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void DeleteGuildEmoji(DiscordClient client, string emojiId, Action callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/emojis/{emojiId}", RequestMethod.DELETE, null, callback, error);
        }

        /// <summary>
        /// Modifies the current user's voice state.
        /// See <a href="https://discord.com/developers/docs/resources/guild#update-current-user-voice-state">Update Current User Voice State</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="channelId">Channel ID of the stage channel</param>
        /// <param name="suppress">Changes the users suppressed state</param>
        /// <param name="requestToSpeak">Sets the user's request to speak</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ModifyCurrentUserVoiceState(DiscordClient client, Snowflake channelId, bool? suppress = null, DateTime? requestToSpeak = null, Action callback = null, Action<RestError> error = null)
        {
            if (!channelId.IsValid()) throw new InvalidSnowflakeException(nameof(channelId));
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                ["channel_id"] = channelId.ToString()
            };

            if (suppress.HasValue)
            {
                data["suppress"] = suppress.Value;
            }

            if (requestToSpeak.HasValue)
            {
                data["request_to_speak_timestamp"] = requestToSpeak;
            }
            
            client.Bot.Rest.DoRequest($"/guilds/{Id}/voice-states/@me", RequestMethod.PATCH, data, callback, error);
        }
        
        /// <summary>
        /// Modifies another user's voice state.
        /// See <a href="https://discord.com/developers/docs/resources/guild#modify-user-voice-state">Update Users Voice State</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="userId">User ID of the users state to update</param>
        /// <param name="channelId">Channel ID of the stage channel</param>
        /// <param name="suppress">Changes the users suppressed state</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ModifyUserVoiceState(DiscordClient client, Snowflake userId, Snowflake channelId, bool? suppress = null, Action callback = null, Action<RestError> error = null)
        {
            if (!userId.IsValid()) throw new InvalidSnowflakeException(nameof(userId));
            if (!channelId.IsValid()) throw new InvalidSnowflakeException(nameof(channelId));
            Dictionary<string, object> data = new Dictionary<string, object>
            {
                ["channel_id"] = channelId.ToString()
            };

            if (suppress.HasValue)
            {
                data["suppress"] = suppress.Value;
            }
            
            client.Bot.Rest.DoRequest($"/guilds/{Id}/voice-states/{userId}", RequestMethod.PATCH, data, callback, error);
        }
        
        /// <summary>
        /// Returns an array of sticker objects for the given guild.
        /// Includes user fields if the bot has the MANAGE_EMOJIS_AND_STICKERS permission.
        /// See <a href="https://discord.com/developers/docs/resources/sticker#list-guild-stickers">List Guild Stickers</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with the list of stickers</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ListGuildStickers(DiscordClient client, Action<List<DiscordSticker>> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/stickers", RequestMethod.GET, null, callback, error);
        }
        
        /// <summary>
        /// Returns a sticker object for the given guild and sticker IDs.
        /// Includes the user field if the bot has the MANAGE_EMOJIS_AND_STICKERS permission.
        /// See <a href="https://discord.com/developers/docs/resources/sticker#get-guild-sticker">Get Guild Sticker</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="stickerId">ID of the sticker to get</param>
        /// <param name="callback">Callback with the discord sticker</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void GetGuildSticker(DiscordClient client, Snowflake stickerId, Action<DiscordSticker> callback = null, Action<RestError> error = null)
        {
            if (!stickerId.IsValid()) throw new InvalidSnowflakeException(nameof(stickerId));
            client.Bot.Rest.DoRequest($"/guilds/{Id}/stickers/{stickerId}", RequestMethod.GET, null, callback, error);
        }
        
        /// <summary>
        /// Create a new sticker for the guild.
        /// Requires the MANAGE_EMOJIS_AND_STICKERS permission.
        /// Returns the new sticker object on success.
        /// See <a href="https://discord.com/developers/docs/resources/sticker#create-guild-sticker">Create Guild Sticker</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="sticker">Sticker to create</param>
        /// <param name="callback">Callback with the discord sticker</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void CreateGuildSticker(DiscordClient client, GuildStickerCreate sticker, Action<DiscordSticker> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/stickers", RequestMethod.POST, sticker, callback, error);
        }
        
        /// <summary>
        /// Modify the given sticker.
        /// Requires the MANAGE_EMOJIS_AND_STICKERS permission.
        /// Returns the updated sticker object on success.
        /// See <a href="https://discord.com/developers/docs/resources/sticker#modify-guild-sticker">Modify Guild Sticker</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="sticker">Sticker to modify</param>
        /// <param name="callback">Callback with the updated discord sticker</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ModifyGuildSticker(DiscordClient client, DiscordSticker sticker, Action<DiscordSticker> callback = null, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/guilds/{Id}/stickers/{sticker.Id}", RequestMethod.PATCH, sticker, callback, error);
        }
        
        /// <summary>
        /// Delete the given sticker.
        /// Requires the MANAGE_EMOJIS_AND_STICKERS permission.
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="stickerId">ID of the sticker to delete</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        /// See <a href="https://discord.com/developers/docs/resources/sticker#delete-guild-sticker">Delete Guild Sticker</a>
        public void DeleteGuildSticker(DiscordClient client, Snowflake stickerId, Action callback = null, Action<RestError> error = null)
        {
            if (!stickerId.IsValid()) throw new InvalidSnowflakeException(nameof(stickerId));
            client.Bot.Rest.DoRequest($"/guilds/{Id}/stickers/{stickerId}", RequestMethod.DELETE, null, callback, error);
        }
        #endregion

        #region Entity Update Methods
        internal DiscordGuild Update(DiscordGuild updatedGuild)
        {
            DiscordGuild previous = (DiscordGuild)MemberwiseClone();
            if (updatedGuild.Name != null)
                Name = updatedGuild.Name;
            if (updatedGuild.Icon != null)
                Icon = updatedGuild.Icon;
            if (updatedGuild.IconHash != null)
                IconHash = updatedGuild.IconHash;
            if (updatedGuild.Splash != null)
                Splash = updatedGuild.Splash;
            if (updatedGuild.DiscoverySplash != null)
                DiscoverySplash = updatedGuild.DiscoverySplash;
            if(updatedGuild.OwnerId.IsValid()) 
                OwnerId = updatedGuild.OwnerId;
            if (updatedGuild.AfkChannelId != null)
                AfkChannelId = updatedGuild.AfkChannelId;
            if (updatedGuild.AfkTimeout != null)
                AfkTimeout = updatedGuild.AfkTimeout;
            if (updatedGuild.WidgetEnabled != null)
                WidgetEnabled = updatedGuild.WidgetEnabled;
            if (updatedGuild.WidgetChannelId != null)
                WidgetChannelId = updatedGuild.WidgetChannelId;
            VerificationLevel = updatedGuild.VerificationLevel;
            DefaultMessageNotifications = updatedGuild.DefaultMessageNotifications;
            ExplicitContentFilter = updatedGuild.ExplicitContentFilter;
            if (updatedGuild.Roles != null)
                Roles = updatedGuild.Roles;
            if (updatedGuild.Emojis != null)
                Emojis = updatedGuild.Emojis;
            if (updatedGuild.Features != null)
                Features = updatedGuild.Features;
            if (updatedGuild.MfaLevel != null)
                MfaLevel = updatedGuild.MfaLevel;
            if (updatedGuild.ApplicationId != null)
                ApplicationId = updatedGuild.ApplicationId;
            if (updatedGuild.SystemChannelId != null)
                SystemChannelId = updatedGuild.SystemChannelId;
            SystemChannelFlags = updatedGuild.SystemChannelFlags;
            if (RulesChannelId != null)
                RulesChannelId = updatedGuild.RulesChannelId;
            if (updatedGuild.JoinedAt != null)
                JoinedAt = updatedGuild.JoinedAt;
            if (updatedGuild.Large != null)
                Large = updatedGuild.Large;
            if (updatedGuild.Unavailable != null && (!Unavailable.HasValue || Unavailable.Value))
                Unavailable = updatedGuild.Unavailable;
            if (updatedGuild.MemberCount != null)
                MemberCount = updatedGuild.MemberCount;
            if (updatedGuild.VoiceStates != null)
                VoiceStates = updatedGuild.VoiceStates;
            if (updatedGuild.Members != null)
                Members = updatedGuild.Members;
            if (updatedGuild.Channels != null)
                Channels = updatedGuild.Channels;
            if (updatedGuild.Threads != null)
                Threads = updatedGuild.Threads;
            if (updatedGuild.Presences != null)
                Presences = updatedGuild.Presences;
            if (updatedGuild.MaxPresences != null)
                MaxPresences = updatedGuild.MaxPresences;
            if (updatedGuild.MaxMembers != null)
                MaxMembers = updatedGuild.MaxMembers;
            if (updatedGuild.VanityUrlCode != null)
                VanityUrlCode = updatedGuild.VanityUrlCode;
            if (updatedGuild.Description != null)
                Description = updatedGuild.Description;
            if (updatedGuild.Banner != null)
                Banner = updatedGuild.Banner;
            if (updatedGuild.PremiumTier != null)
                PremiumTier = updatedGuild.PremiumTier;
            if (updatedGuild.PremiumSubscriptionCount != null)
                PremiumSubscriptionCount = updatedGuild.PremiumSubscriptionCount;
            if (updatedGuild.PreferredLocale != null)
                PreferredLocale = updatedGuild.PreferredLocale;
            if (updatedGuild.PublicUpdatesChannelId != null)
                PublicUpdatesChannelId = updatedGuild.PublicUpdatesChannelId;
            if (updatedGuild.MaxVideoChannelUsers != null)
                MaxVideoChannelUsers = updatedGuild.MaxVideoChannelUsers;
            if (updatedGuild.ApproximateMemberCount != null)
                ApproximateMemberCount = updatedGuild.ApproximateMemberCount;
            if (updatedGuild.ApproximatePresenceCount != null)
                ApproximatePresenceCount = updatedGuild.ApproximatePresenceCount;
            if (updatedGuild.WelcomeScreen != null)
                WelcomeScreen = updatedGuild.WelcomeScreen;
            NsfwLevel = updatedGuild.NsfwLevel;
            if (updatedGuild.StageInstances != null)
                StageInstances = updatedGuild.StageInstances;
            if (updatedGuild.Stickers != null)
                Stickers = updatedGuild.Stickers;
            if (updatedGuild.ScheduledEvents != null)
                ScheduledEvents = updatedGuild.ScheduledEvents;
            PremiumProgressBarEnabled = updatedGuild.PremiumProgressBarEnabled;
            return previous;
        }
        #endregion
    }
}
