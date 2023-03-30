using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Permissions;

namespace Oxide.Ext.Discord.Entities.Messages.Embeds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/channel#embed-object">Embed Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DiscordEmbed
    {
        /// <summary>
        /// Title of embed
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Type of embed (always "rich" for webhook embeds)
        /// </summary>
        [Obsolete("Embed types should be considered deprecated and might be removed in a future API version")]
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Description of embed
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Url of embed
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Timestamp of embed content
        /// </summary>
        [JsonProperty("timestamp")]
        public DateTime? Timestamp { get; set; }

        /// <summary>
        /// Color code of the embed
        /// </summary>
        [JsonProperty("color")]
        public DiscordColor Color { get; set; }

        /// <summary>
        /// Footer information
        /// <see cref="EmbedFooter"/>
        /// </summary>
        [JsonProperty("footer")]
        public EmbedFooter Footer { get; set; }

        /// <summary>
        /// Image information
        /// <see cref="EmbedImage"/>
        /// </summary>
        [JsonProperty("image")]
        public EmbedImage Image { get; set; }

        /// <summary>
        /// Thumbnail information
        /// <see cref="EmbedThumbnail"/>
        /// </summary>
        [JsonProperty("thumbnail")]
        public EmbedThumbnail Thumbnail { get; set; }

        /// <summary>
        /// Video information
        /// <see cref="EmbedVideo"/>
        /// </summary>
        [JsonProperty("video")]
        public EmbedVideo Video { get; set; }

        /// <summary>
        /// Provider information
        /// <see cref="EmbedProvider"/>
        /// </summary>
        [JsonProperty("provider")]
        public EmbedProvider Provider { get; set; }

        /// <summary>
        /// Author information
        /// <see cref="EmbedAuthor"/>
        /// </summary>
        [JsonProperty("author")]
        public EmbedAuthor Author { get; set; }

        /// <summary>
        /// Fields information
        /// <see cref="EmbedField"/>
        /// </summary>
        [JsonProperty("fields")]
        public List<EmbedField> Fields { get; set; }
    }
}