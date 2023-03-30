namespace Oxide.Ext.Discord.Entities.Stickers
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/sticker#sticker-types">Sticker Types</a>
    /// </summary>
    public enum StickerType
    {
        /// <summary>
        /// An official sticker in a pack, part of Nitro or in a removed purchasable pack
        /// </summary>
        Standard = 1,
        
        /// <summary>
        /// A sticker uploaded to a Boosted guild for the guild's members
        /// </summary>
        Guild = 2
    }
}