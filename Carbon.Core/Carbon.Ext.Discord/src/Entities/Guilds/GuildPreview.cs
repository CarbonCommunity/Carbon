using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Emojis;
using Oxide.Ext.Discord.Entities.Stickers;

namespace Oxide.Ext.Discord.Entities.Guilds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild#guild-preview-object">Guild Preview Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildPreview
    {
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
        /// Custom guild emojis
        /// </summary>
        [JsonProperty("emojis")]
        public List<DiscordEmoji> Emojis { get; set; }
        
        /// <summary>
        /// Enabled guild features
        /// See <see cref="GuildFeatures"/>
        /// </summary>
        [JsonProperty("features")]
        public List<GuildFeatures> Features { get; set; }
        
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
        /// The description of a Community guild
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// Custom guild stickers
        /// </summary>
        [JsonProperty("stickers")]
        public List<DiscordSticker> Stickers { get; set; }
    }
}