using System.Collections.Generic;
using Newtonsoft.Json;

namespace Oxide.Ext.Discord.Entities.Channels
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/guild#create-guild-channel-json-params">Guild Channel Create Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChannelCreate
    {
        /// <summary>
        /// The name of the channel (1-100 characters)
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// the type of channel <see cref="ChannelType"/>
        /// </summary>
        [JsonProperty("type")]
        public ChannelType Type { get; set; }
        
        /// <summary>
        /// The channel topic (0-1024 characters)
        /// </summary>
        [JsonProperty("topic")]        
        public string Topic { get; set; }
                
        /// <summary>
        /// The bitrate (in bits) of the voice channel
        /// 8000 to 96000 (128000 for VIP servers)
        /// </summary>
        [JsonProperty("bitrate")]
        public int? Bitrate { get; set; }
                
        /// <summary>
        /// The user limit of the voice channel
        /// 0 refers to no limit, 1 to 99 refers to a user limit
        /// </summary>
        [JsonProperty("user_limit")]
        public int? UserLimit { get; set; }
                
        /// <summary>
        /// Amount of seconds a user has to wait before sending another message (0-21600);
        /// bots, as well as users with the permission manage_messages or manage_channel, are unaffected
        /// </summary>
        [JsonProperty("rate_limit_per_user")]
        public int? RateLimitPerUser { get; set; }
                
        /// <summary>
        /// Sorting position of the channel
        /// </summary>
        [JsonProperty("position")]
        public int? Position { get; set; }
                
        /// <summary>
        /// Explicit permission overwrites for members and roles <see cref="Overwrite"/>
        /// </summary>
        [JsonProperty("permission_overwrites")]
        public List<Overwrite> PermissionOverwrites { get; set; }
                
        /// <summary>
        /// ID of the parent category for a channel (each parent category can contain up to 50 channels)
        /// </summary>
        [JsonProperty("parent_id")]
        public Snowflake? ParentId { get; set; }
        
        /// <summary>
        /// Whether the channel is nsfw
        /// </summary>
        [JsonProperty("nsfw")]
        public bool? Nsfw { get; set; }
    }
}