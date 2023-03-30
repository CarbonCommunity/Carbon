using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Emojis;
using Oxide.Ext.Discord.Helpers;
using Oxide.Ext.Discord.Helpers.Cdn;

namespace Oxide.Ext.Discord.Entities.Activities
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/gateway#activity-object">Activity Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DiscordActivity
    {
        /// <summary>
        /// The activity's name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Activity type
        /// See <see cref="ActivityType"/>
        /// </summary>
        [JsonProperty("type")]
        public ActivityType Type { get; set; }
        
        /// <summary>
        /// Stream url, is validated when type is 1
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Unix timestamp of when the activity was added to the user's session
        /// </summary>
        [JsonProperty("created_at")]
        public long CreatedAt { get; set; }
        
        /// <summary>
        /// Timestamp of when the activity was added to the user's session
        /// </summary>
        public DateTime CreatedAtDateTime => CreatedAt.ToDateTimeOffsetFromMilliseconds();
        
        /// <summary>
        /// Unix timestamps for start and/or end of the game
        /// See <see cref="ActivityTimestamps"/>
        /// </summary>
        [JsonProperty("timestamps")]
        public List<ActivityTimestamps> Timestamps { get; set; }
        
        /// <summary>
        /// Application id for the game
        /// </summary>
        [JsonProperty("application_id")]
        public Snowflake? ApplicationId { get; set; }
        
        /// <summary>
        /// What the player is currently doing
        /// </summary>
        [JsonProperty("details")]
        public string Details { get; set; }
        
        /// <summary>
        /// The user's current party status
        /// </summary>
        [JsonProperty("state")]
        public string State { get; set; }
        
        /// <summary>
        /// tTe emoji used for a custom status
        /// See <see cref="Emoji"/>
        /// </summary>
        [JsonProperty("emoji")]
        public DiscordEmoji Emoji { get; set; }
        
        /// <summary>
        /// Information for the current party of the player
        /// See <see cref="ActivityParty"/>
        /// </summary>
        [JsonProperty("party")]
        public ActivityParty Party { get; set; }
        
        /// <summary>
        /// Images for the presence and their hover texts
        /// See <see cref="ActivityAssets"/>
        /// </summary>
        [JsonProperty("assets")]
        public ActivityAssets Assets { get; set; }
        
        /// <summary>
        /// Secrets for Rich Presence joining and spectating
        /// See <see cref="ActivitySecrets"/>
        /// </summary>
        [JsonProperty("secrets")]
        public ActivitySecrets Secrets { get; set; }
        
        /// <summary>
        /// Whether or not the activity is an instanced game session
        /// </summary>
        [JsonProperty("instance")]
        public bool? Instance { get; set; }
        
        /// <summary>
        /// Describes what the payload includes
        /// See <see cref="ActivityFlags"/>
        /// </summary>
        [JsonProperty("flags")]
        public ActivityFlags? Flags { get; set; }
        
        /// <summary>
        /// The custom buttons shown in the Rich Presence (max 2)
        /// See <see cref="ActivityButton"/>
        /// </summary>
        [JsonProperty("buttons")]
        public List<ActivityButton> Buttons { get; set; }

        /// <summary>
        /// Returns the large image url for the presence asset
        /// </summary>
        public string GetLargeImageUrl => ApplicationId.HasValue ? DiscordCdn.GetApplicationAssetUrl(ApplicationId.Value, Assets.LargeImage) : null;
        
        /// <summary>
        /// Returns the small image url for the presence asset
        /// </summary>
        public string GetSmallImageUrl => ApplicationId.HasValue ? DiscordCdn.GetApplicationAssetUrl(ApplicationId.Value, Assets.SmallImage) : null;
    }
}