using System;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Api;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Exceptions;
using Oxide.Ext.Discord.Helpers.Cdn;
using Oxide.Ext.Discord.Interfaces;

namespace Oxide.Ext.Discord.Entities.Stickers
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/sticker#sticker-object">Discord Sticker Structure</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DiscordSticker : ISnowflakeEntity
    {
        /// <summary>
        /// ID of the sticker
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }
        
        /// <summary>
        /// ID of the pack the sticker is from
        /// </summary>
        [JsonProperty("pack_id")]
        public Snowflake? PackId { get; set; }
        
        /// <summary>
        /// Name of the sticker
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// Description of the sticker
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// For guild stickers, a unicode emoji representing the sticker's expression.
        /// For nitro stickers, a comma-separated list of related expressions.
        /// autocomplete/suggestion tags for the sticker (max 200 characters)
        /// </summary>
        [JsonProperty("tags")]
        public string Tags { get; set; }

        /// <summary>
        /// Type of sticker.
        /// </summary>
        [JsonProperty("type")]
        public StickerType Type { get; set; }
        
        /// <summary>
        /// Type of sticker format
        /// <see cref="StickerFormatType"/>
        /// </summary>
        [JsonProperty("format_type")]
        public StickerFormatType FormatType { get; set; }
        
        /// <summary>
        /// Whether or not the sticker is available
        /// </summary>
        [JsonProperty("available")]
        public bool? Available { get; set; }
        
        /// <summary>
        /// Id of the guild that owns this sticker
        /// </summary>
        [JsonProperty("guild_id")]
        public Snowflake? GuildId { get; set; }
        
        /// <summary>
        /// The user that uploaded the sticker
        /// </summary>
        [JsonProperty("user")]
        public DiscordUser User { get; set; }
        
        /// <summary>
        /// A sticker's sort order within a pack
        /// </summary>
        [JsonProperty("sort_value")]
        public int? SortValue { get; set; }

        /// <summary>
        /// Returns the Url for the sticker
        /// </summary>
        public string StickerUrl => DiscordCdn.GetSticker(Id);
        
        /// <summary>
        /// Returns a sticker object for the given sticker ID.
        /// See <a href="https://discord.com/developers/docs/resources/sticker#get-sticker">Get Sticker</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="stickerId">ID of the sticker</param>
        /// <param name="callback">Callback with the DiscordSticker</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public static void GetSticker(DiscordClient client, Snowflake stickerId, Action<DiscordSticker> callback, Action<RestError> error = null)
        {
            if (!stickerId.IsValid()) throw new InvalidSnowflakeException(nameof(stickerId));
            client.Bot.Rest.DoRequest($"/stickers/{stickerId}", RequestMethod.GET, null, callback, error);
        }
        
        /// <summary>
        /// Modify the given sticker.
        /// Requires the MANAGE_EMOJIS_AND_STICKERS permission.
        /// Returns the updated sticker object on success.
        /// See <a href="https://discord.com/developers/docs/resources/sticker#modify-guild-sticker">Modify Guild Sticker</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with the updated discord sticker</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void ModifyGuildSticker(DiscordClient client, Action<DiscordSticker> callback = null, Action<RestError> error = null)
        {
            if (Type != StickerType.Guild)
            {
                throw new Exception("This endpoint can only be used for guild stickers");
            }
            
            client.Bot.Rest.DoRequest($"/guilds/{GuildId}/stickers/{Id}", RequestMethod.PATCH, this, callback, error);
        }
        
        /// <summary>
        /// Delete the given sticker.
        /// Requires the MANAGE_EMOJIS_AND_STICKERS permission.
        /// See <a href="https://discord.com/developers/docs/resources/sticker#delete-guild-sticker">Delete Guild Sticker</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback once the action is completed</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public void DeleteGuildSticker(DiscordClient client, Action callback = null, Action<RestError> error = null)
        {
            if (Type != StickerType.Guild)
            {
                throw new Exception("This endpoint can only be used for guild stickers");
            }
            
            client.Bot.Rest.DoRequest($"/guilds/{GuildId}/stickers/{Id}", RequestMethod.DELETE, null, callback, error);
        }
        
        internal void Update(DiscordSticker sticker)
        {
            if (sticker.Name != null)
                Name = sticker.Name;

            if (sticker.Description != null)
                Description = sticker.Description;

            if (sticker.Tags != null)
                Tags = sticker.Tags;
        }
    }
}