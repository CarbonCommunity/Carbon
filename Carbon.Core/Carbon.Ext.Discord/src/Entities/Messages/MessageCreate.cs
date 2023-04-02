using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Interactions.MessageComponents;
using Oxide.Ext.Discord.Entities.Messages.AllowedMentions;
using Oxide.Ext.Discord.Entities.Messages.Embeds;
using Oxide.Ext.Discord.Exceptions;
using Oxide.Ext.Discord.Helpers;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Messages
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/channel#create-message-parameters-for-contenttype-applicationjson">Message Create Structure</a> to be created in discord
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class MessageCreate : IFileAttachments
    {
        /// <summary>
        /// Contents of the message
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }
        
        /// <summary>
        /// Used for validating a message was sent (Up to 25 characters)
        /// </summary>
        [JsonProperty("nonce")]
        public string Nonce { get; set; }
        
        /// <summary>
        /// Whether this was a TTS message
        /// </summary>
        [JsonProperty("tts")]
        public bool? Tts { get; set; }
        
        /// <summary>
        /// Embeds for the message
        /// </summary>
        [JsonProperty("embeds")]
        public List<DiscordEmbed> Embeds { get; set; }
        
        /// <summary>
        /// Allowed mentions for a message
        /// Allows for more granular control over mentions without various hacks to the message content. 
        /// </summary>
        [JsonProperty("allowed_mentions")]
        public AllowedMention AllowedMention { get; set; }

        /// <summary>
        /// Include to make your message a reply
        /// </summary>
        [JsonProperty("message_reference")]
        public MessageReference MessageReference { get; set; }
        
        /// <summary>
        /// Used to create message components on a message
        /// </summary>
        [JsonProperty("components")]
        public List<ActionRowComponent> Components { get; set; }
        
        /// <summary>
        /// IDs of up to 3 stickers in the server to send in the message
        /// </summary>
        [JsonProperty("sticker_ids")]
        public List<Snowflake> StickerIds { get; set; }
        
        /// <summary>
        /// Attachments for the message
        /// </summary>
        [JsonProperty("attachments")]
        public List<MessageAttachment> Attachments { get; set; }
        
        /// <summary>
        /// Attachments for the message
        /// </summary>
        [JsonProperty("flags ")]
        public MessageFlags? Flags { get; set; }
        
        /// <summary>
        /// Attachments for a discord message
        /// </summary>
        public List<MessageFileAttachment> FileAttachments { get; set; }

        /// <summary>
        /// Adds an attachment to the message
        /// </summary>
        /// <param name="filename">Name of the file</param>
        /// <param name="data">byte[] of the attachment</param>
        /// <param name="contentType">Attachment content type</param>
        /// <param name="description">Description for the attachment</param>
        public void AddAttachment(string filename, byte[] data, string contentType, string description = null)
        {
            Validation.ValidateFilename(filename);
            
            if (FileAttachments == null)
            {
                FileAttachments = new List<MessageFileAttachment>();
            }

            if (Attachments == null)
            {
                Attachments = new List<MessageAttachment>();
            }

            FileAttachments.Add(new MessageFileAttachment(filename, data, contentType));
            Attachments.Add(new MessageAttachment {Id = new Snowflake((ulong)FileAttachments.Count), Filename = filename, Description = description});
        }

        internal void Validate()
        {
            if (string.IsNullOrEmpty(Content) && (Embeds == null || Embeds.Count == 0) && (FileAttachments == null || FileAttachments.Count == 0))
            {
                throw new InvalidMessageException("Discord Messages require Either Content, An Embed, Or a File");
            }

            if (!string.IsNullOrEmpty(Content) && Content.Length > 2000)
            {
                throw new InvalidMessageException("Content cannot be more than 2000 characters");
            }
        }

        internal void ValidateChannelMessage()
        {
            if (!Flags.HasValue)
            {
                return;
            }

            if ((Flags.Value & ~MessageFlags.SuppressEmbeds) != 0)
            {
                throw new InvalidMessageException("Invalid Message Flags Used for Channel Message. Only supported flags are MessageFlags.SuppressEmbeds");
            }
        }

        internal void ValidateInteractionMessage()
        {
            if (!Flags.HasValue)
            {
                return;
            }

            if ((Flags.Value & ~(MessageFlags.SuppressEmbeds | MessageFlags.Ephemeral)) != 0)
            {
                throw new InvalidMessageException("Invalid Message Flags Used for Interaction Message. Only supported flags are MessageFlags.SuppressEmbeds, and MessageFlags.Ephemeral");
            }
        }
    }
}