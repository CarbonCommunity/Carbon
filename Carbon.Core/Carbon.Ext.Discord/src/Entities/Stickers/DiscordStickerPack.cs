using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Api;

namespace Oxide.Ext.Discord.Entities.Stickers
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/sticker#sticker-pack-object">Sticker Pack Object</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DiscordStickerPack
    {
        /// <summary>
        /// ID of the sticker pack
        /// </summary>
        [JsonProperty("id")]
        public Snowflake Id { get; set; }
        
        /// <summary>
        /// The stickers in the pack
        /// </summary>
        [JsonProperty("stickers")]
        public List<DiscordSticker> Stickers { get; set; }
        
        /// <summary>
        /// Name of the sticker pack
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
        
        /// <summary>
        /// ID of the pack's SKU
        /// </summary>
        [JsonProperty("sku_id")]
        public Snowflake SkuId { get; set; }
        
        /// <summary>
        /// ID of a sticker in the pack which is shown as the pack's icon
        /// </summary>
        [JsonProperty("cover_sticker_id")]
        public Snowflake? CoverStickerId { get; set; }
        
        /// <summary>
        /// Description of the sticker pack
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }
        
        /// <summary>
        /// ID of the sticker pack's banner image
        /// </summary>
        [JsonProperty("banner_asset_id")]
        public Snowflake? BannerAssetId { get; set; }
        
        /// <summary>
        /// Returns the list of sticker packs available to Nitro subscribers.
        /// See <a href="https://discord.com/developers/docs/resources/sticker#list-nitro-sticker-packs">List Nitro Sticker Packs</a>
        /// </summary>
        /// <param name="client">Client to use</param>
        /// <param name="callback">Callback with a list of Nitro sticker packs</param>
        /// <param name="error">Callback when an error occurs with error information</param>
        public static void GetNitroStickerPacks(DiscordClient client, Action<List<DiscordStickerPack>> callback, Action<RestError> error = null)
        {
            client.Bot.Rest.DoRequest($"/sticker-packs", RequestMethod.GET, null, callback, error);
        }
    }
}