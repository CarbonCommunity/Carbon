using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Interactions.MessageComponents;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Entities.Messages.AllowedMentions;
using Oxide.Ext.Discord.Entities.Messages.Embeds;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Entities.Interactions
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/interactions/receiving-and-responding#interaction-response-object-interaction-callback-data-structure">Interaction Application Command Callback Data Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class InteractionCallbackData
    {
        /// <summary>
        /// Is the response TTS
        /// </summary>
        [JsonProperty("tts")]
        public bool? Tts { get; set; } 
        
        /// <summary>
        /// Message content
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; } 
        
        /// <summary>
        /// List of embeds
        /// Supports up to 10 embedsF
        /// </summary>
        [JsonProperty("embeds")]
        public List<DiscordEmbed> Embeds { get; set; } 
        
        /// <summary>
        /// Allowed mentions 
        /// </summary>
        [JsonProperty("allowed_mentions")]
        public AllowedMention AllowedMentions { get; set; }
        
        /// <summary>
        /// A developer-defined identifier for the interactable form
        /// Max 100 characters
        /// </summary>
        [JsonProperty("custom_id")]
        public string CustomId { get; set; }
        
        /// <summary>
        /// Title of the modal if Modal Response
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }
        
        /// <summary>
        /// Callback data flags
        /// </summary>
        [JsonProperty("flags")]
        public MessageFlags? Flags { get; set; }
        
        /// <summary>
        /// Message components 
        /// </summary>
        [JsonProperty("components")]
        public List<ActionRowComponent> Components { get; set; }
        
        /// <summary>
        /// Attachment objects with filename and description
        /// </summary>
        [JsonProperty("attachments")]
        public List<MessageAttachment> Attachments { get; set; }

        internal void Validate()
        {
            if (!Flags.HasValue)
            {
                return;
            }

            if ((Flags.Value & ~(MessageFlags.SuppressEmbeds | MessageFlags.Ephemeral)) != 0)
            {
                throw new InvalidInteractionResponseException("Invalid Message Flags Used for Interaction Message. Only supported flags are MessageFlags.SuppressEmbeds or MessageFlags.Ephemeral");
            }
        }
    }
}