using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Stickers
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/sticker#sticker-pack-object">Sticker Pack Object</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildStickerCreate : IFileAttachments
    {
        /// <summary>
        /// Name of the sticker (2-30 characters)
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Description of the sticker (empty or 2-100 characters)
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// Autocomplete/suggestion tags for the sticker (max 200 characters)
        /// </summary>
        [JsonProperty("tags")]
        public string Tags { get; set; }
        
        /// <summary>
        /// Sticker image attachment
        /// </summary>
        public List<MessageFileAttachment> FileAttachments { get; set; }

        /// <summary>
        /// Adds the sticker for guild sticker create
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="contentType">Content type of the file</param>
        /// <param name="data">data for the file</param>
        /// <exception cref="Exception">
        /// Throw if more than 1 sticker is added.
        /// Thrown if the data is more than 500KB
        /// Thrown if the file extension is not .png, .apng, or .json
        /// </exception>
        public void AddSticker(string fileName, string contentType, byte[] data)
        {
            if (FileAttachments.Count != 0)
            {
                throw new Exception("Can only add one sticker at a time");
            }

            if (data.Length > 500 * 1024)
            {
                throw new Exception("Data cannot be larger than 500KB");
            }

            string extension = fileName.Substring(fileName.LastIndexOf('.') + 1);
            switch (extension.ToLower())
            {
                case "png":
                case "apng":
                case "json":
                    break;
                
                default:
                    throw new Exception("Sticker can only be of type png, apng, or lottie json");
            }
            
            FileAttachments.Add(new MessageFileAttachment(fileName, data, contentType));
        }
    }
}