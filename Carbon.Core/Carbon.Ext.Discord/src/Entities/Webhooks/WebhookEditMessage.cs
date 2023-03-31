using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Messages.AllowedMentions;
using Oxide.Ext.Discord.Entities.Messages.Embeds;

namespace Oxide.Ext.Discord.Entities.Webhooks
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/webhook#edit-webhook-message-jsonform-params">Webhook Edit Message Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class WebhookEditMessage
    {
        /// <summary>
        /// The message contents (up to 2000 characters)
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }
        
        /// <summary>
        /// Embedded rich content (Up to 10 embeds)
        /// </summary>
        [JsonProperty("embeds")]
        public List<DiscordEmbed> Embeds { get; set; }
        
        /// <summary>
        /// Allowed mentions for the message
        /// </summary>
        [JsonProperty("allowed_mentions")]
        public AllowedMention AllowedMentions { get; set; }
        
        /// <summary>
        /// Adds a new embed to the list of embed to send
        /// </summary>
        /// <param name="embed">Embed to add</param>
        /// <returns>This</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if more than 10 embeds are added in a send as that is the discord limit</exception>
        public WebhookEditMessage AddEmbed(DiscordEmbed embed)
        {
            if (Embeds.Count >= 10)
            {
                throw new IndexOutOfRangeException("Only 10 embed are allowed per message");
            }

            Embeds.Add(embed);
            return this;
        }
    }
}