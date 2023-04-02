using System;
using System.Collections.Generic;
using System.Globalization;
using Oxide.Ext.Discord.Entities.Messages.Embeds;
using Oxide.Ext.Discord.Entities.Permissions;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Builders
{
    /// <summary>
    /// Builds a new DiscordEmbed
    /// </summary>
    public class DiscordEmbedBuilder
    {
        private readonly DiscordEmbed _embed = new DiscordEmbed();
        
        /// <summary>
        /// Adds a title to the embed message
        /// </summary>
        /// <param name="title">Title to add</param>
        /// <returns>This</returns>
        public DiscordEmbedBuilder AddTitle(string title)
        {
            if (title != null && title.Length > 256)
            {
                throw new InvalidEmbedException("Title cannot be more than 256 characters");
            }
            
            _embed.Title = title;
            return this;
        }
        
        /// <summary>
        /// Adds a description to the embed message
        /// </summary>
        /// <param name="description">description to add</param>
        /// <returns>This</returns>
        public DiscordEmbedBuilder AddDescription(string description)
        {
            if (description == null) throw new ArgumentNullException(nameof(description));
            if (description.Length > 4096)
            {
                throw new InvalidEmbedException("Description cannot be more than 4096 characters");
            }
            
            _embed.Description = description;
            return this;
        }

        /// <summary>
        /// Adds a url to the embed message
        /// </summary>
        /// <param name="url"></param>
        /// <returns>This</returns>
        public DiscordEmbedBuilder AddUrl(string url)
        {
            _embed.Url = url;
            return this;
        }

        /// <summary>
        /// Adds an author to the embed message. The author will appear above the title
        /// </summary>
        /// <param name="name">Name of the author</param>
        /// <param name="iconUrl">Icon Url to use for the author</param>
        /// <param name="url">Url to go to when the authors name is clicked on</param>
        /// <param name="proxyIconUrl">Backup icon url. Can be left null if you only have one icon url</param>
        /// <returns>This</returns>
        public DiscordEmbedBuilder AddAuthor(string name, string iconUrl = null, string url = null, string proxyIconUrl = null)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (name.Length > 256)
            {
                throw new InvalidEmbedException("Author name cannot be more than 256 characters");
            }
            
            _embed.Author = new EmbedAuthor(name, iconUrl, url, proxyIconUrl);
            return this;
        }

        /// <summary>
        /// Adds a footer to the embed message
        /// </summary>
        /// <param name="text">Text to be added to the footer</param>
        /// <param name="iconUrl">Icon url to add in the footer. Appears to the left of the text</param>
        /// <param name="proxyIconUrl">Backup icon url. Can be left null if you only have one icon url</param>
        /// <returns>This</returns>
        public DiscordEmbedBuilder AddFooter(string text, string iconUrl = null, string proxyIconUrl = null)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));
            if (text.Length > 2048)
            {
                throw new InvalidEmbedException("Footer text cannot be more than 2048 characters");
            }
            
            _embed.Footer = new EmbedFooter(text, iconUrl, proxyIconUrl);
            return this;
        }

        /// <summary>
        /// Adds a Discord Color to the embed
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public DiscordEmbedBuilder AddColor(DiscordColor color)
        {
            _embed.Color = color;
            return this;
        }
        
        /// <summary>
        /// Adds an int based color to the embed. Color appears as a bar on the left side of the message
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public DiscordEmbedBuilder AddColor(uint color)
        {
            _embed.Color = new DiscordColor(color);
            return this;
        }

        /// <summary>
        /// Adds a hex based color. Color appears as a bar on the left side of the message
        /// </summary>
        /// <param name="color">Color in string hex format</param>
        /// <returns>This</returns>
        /// <exception cref="Exception">Exception thrown if color is outside of range</exception>
        public DiscordEmbedBuilder AddColor(string color)
        {
            _embed.Color = new DiscordColor(uint.Parse(color.TrimStart('#'), NumberStyles.AllowHexSpecifier));
            return this;
        }

        /// <summary>
        /// Adds a RGB based color. Color appears as a bar on the left side of the message
        /// </summary>
        /// <param name="red">Red value</param>
        /// <param name="green">Green value</param>
        /// <param name="blue">Blue value</param>
        /// <returns>This</returns>
        public DiscordEmbedBuilder AddColor(byte red, byte green, byte blue)
        {
            _embed.Color = new DiscordColor(red, green, blue);
            return this;
        }
        
        /// <summary>
        /// Adds a RGB based color. Color appears as a bar on the left side of the message
        /// </summary>
        /// <param name="red">Red value between 0 - 255</param>
        /// <param name="green">Green value between 0 - 255</param>
        /// <param name="blue">Blue value between 0 - 255</param>
        /// <returns>This</returns>
        /// <exception cref="Exception">Thrown if red, green, or blue is outside of range</exception>
        public DiscordEmbedBuilder AddColor(int red, int green, int blue)
        {
            _embed.Color = new DiscordColor(red, green, blue);
            return this;
        }
        
        /// <summary>
        /// Adds a RGB based color. Color appears as a bar on the left side of the message
        /// </summary>
        /// <param name="red">Red value between 0 - 1</param>
        /// <param name="green">Green value between 0 - 1</param>
        /// <param name="blue">Blue value between 0 - 1</param>
        /// <returns>This</returns>
        /// <exception cref="Exception">Thrown if red, green, or blue is outside of range</exception>
        public DiscordEmbedBuilder AddColor(float red, float green, float blue)
        {
            _embed.Color = new DiscordColor(red, green, blue);
            return this;
        }
        
        /// <summary>
        /// Adds a RGB based color. Color appears as a bar on the left side of the message
        /// </summary>
        /// <param name="red">Red value between 0 - 1</param>
        /// <param name="green">Green value between 0 - 1</param>
        /// <param name="blue">Blue value between 0 - 1</param>
        /// <returns>This</returns>
        /// <exception cref="Exception">Thrown if red, green, or blue is outside of range</exception>
        public DiscordEmbedBuilder AddColor(double red, double green, double blue)
        {
            _embed.Color = new DiscordColor(red, green, blue);
            return this;
        }

        /// <summary>
        /// Adds a timestamp to an embed with the current time
        /// </summary>
        /// <returns>This</returns>
        public DiscordEmbedBuilder AddNowTimestamp()
        {
            _embed.Timestamp = DateTime.UtcNow;
            return this;
        }
        
        /// <summary>
        /// Adds a timestamp to an embed with the given time
        /// </summary>
        /// <param name="timestamp">Timestamp to set for the embed</param>
        /// <returns>This</returns>
        public DiscordEmbedBuilder AddTimestamp(DateTime timestamp)
        {
            _embed.Timestamp = timestamp;
            return this;
        }

        /// <summary>
        /// Adds a blank field.
        /// If inline it will add a blank column.
        /// If not inline will add a blank row
        /// </summary>
        /// <param name="inline">If the field is inline</param>
        /// <returns>This</returns>
        public DiscordEmbedBuilder AddBlankField(bool inline)
        {
            if ( _embed.Fields == null)
            {
                _embed.Fields = new List<EmbedField>();
            }

            if ( _embed.Fields.Count >= 25)
            {
                throw new InvalidEmbedException("Embeds cannot have more than 25 fields");
            }
            
            _embed.Fields.Add(new EmbedField("\u200b", "\u200b", inline));
            return this;
        }

        /// <summary>
        /// Adds a new field with the name as the title and value as the value.
        /// If inline will add a new column. If row will add in a new row.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="inline"></param>
        /// <returns></returns>
        public DiscordEmbedBuilder AddField(string name, string value, bool inline)
        {
            if ( _embed.Fields == null)
            {
                _embed.Fields = new List<EmbedField>();
            }

            if (_embed.Fields.Count >= 25)
            {
                throw new InvalidEmbedException("Embeds cannot have more than 25 fields");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new InvalidEmbedException("Embed Fields cannot have a null or empty name");
            }
            
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidEmbedException("Embed Fields cannot have a null or empty value");
            }

            if (name.Length > 256)
            {
                throw new InvalidEmbedException("Field name cannot be more than 256 characters");
            }
            
            if (value.Length > 1024)
            {
                throw new InvalidEmbedException("Field value cannot be more than 1024 characters");
            }
            
            _embed.Fields.Add(new EmbedField(name, value, inline));
            return this;
        }

        /// <summary>
        /// Adds an image to the embed. The url should point to the url of the image.
        /// If using attachment image you can make the url: "attachment://{image name}.{image extension}
        /// </summary>
        /// <param name="url">Url for the image</param>
        /// <param name="width">width of the image</param>
        /// <param name="height">height of the image</param>
        /// <param name="proxyUrl">Backup url for the image</param>
        /// <returns></returns>
        public DiscordEmbedBuilder AddImage(string url, int? width = null, int? height = null, string proxyUrl = null)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            _embed.Image = new EmbedImage(url, width, height, proxyUrl);
            return this;
        }

        /// <summary>
        /// Adds a thumbnail in the top right corner of the embed
        /// If using attachment image you can make the url: "attachment://{image name}.{image extension}
        /// </summary>
        /// <param name="url">Url for the image</param>
        /// <param name="width">width of the image</param>
        /// <param name="height">height of the image</param>
        /// <param name="proxyUrl">Backup url for the image</param>
        /// <returns></returns>
        public DiscordEmbedBuilder AddThumbnail(string url, int? width = null, int? height = null, string proxyUrl = null)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            _embed.Thumbnail = new EmbedThumbnail(url, width, height, proxyUrl);
            return this;
        }

        /// <summary>
        /// Adds a video to the embed
        /// </summary>
        /// <param name="url">Url for the video</param>
        /// <param name="width">Width of the video</param>
        /// <param name="height">Height of the video</param>
        /// <param name="proxyUrl">Proxy Url for the video</param>
        /// <returns></returns>
        public DiscordEmbedBuilder AddVideo(string url, int? width = null, int? height = null, string proxyUrl = null)
        {
            if (url == null) throw new ArgumentNullException(nameof(url));
            _embed.Video = new EmbedVideo(url, width, height, proxyUrl);
            return this;
        }

        /// <summary>
        /// Adds a provider to the embed
        /// </summary>
        /// <param name="name">Name for the provider</param>
        /// <param name="url">Url for the provider</param>
        /// <returns></returns>
        public DiscordEmbedBuilder AddProvider(string name, string url)
        {
            _embed.Provider = new EmbedProvider(name, url);
            return this;
        }

        /// <summary>
        /// Returns the built embed
        /// </summary>
        /// <returns><see cref="DiscordEmbed"/></returns>
        public DiscordEmbed Build()
        {
            return _embed;
        }
        
        /// <summary>
        /// Returns the built embed in a list
        /// </summary>
        /// <returns>List of <see cref="DiscordEmbed"/></returns>
        public List<DiscordEmbed> BuildList()
        {
            return new List<DiscordEmbed> {_embed};
        }
    }
}