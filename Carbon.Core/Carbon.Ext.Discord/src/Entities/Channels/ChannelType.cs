namespace Oxide.Ext.Discord.Entities.Channels
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/channel#channel-object-channel-types">Types of Channels</a>
    /// </summary>
    public enum ChannelType
    {
        /// <summary>
        /// A text channel within a server
        /// </summary>
        GuildText = 0,
        
        /// <summary>
        /// A direct message between users
        /// </summary>
        Dm = 1,
        
        /// <summary>
        /// A voice channel within a server
        /// </summary>
        GuildVoice = 2,
        
        /// <summary>
        /// A direct message between multiple users
        /// </summary>
        GroupDm = 3,
        
        /// <summary>
        /// An organizational category that contains up to 50 channels
        /// </summary>
        GuildCategory = 4,
        
        /// <summary>
        /// A channel that users can follow and crosspost into their own server
        /// </summary>
        GuildNews = 5,
        
        /// <summary>
        /// A channel in which game developers can sell their game on Discord
        /// </summary>
        GuildStore = 6,
        
        /// <summary>
        /// A temporary sub-channel within a GUILD_NEWS channel
        /// </summary>
        GuildNewsThread = 10,
        
        /// <summary>
        /// A temporary sub-channel within a GUILD_TEXT channel
        /// </summary>
        GuildPublicThread = 11,
        
        /// <summary>
        /// A temporary sub-channel within a GUILD_TEXT channel that is only viewable by those invited and those with the MANAGE_THREADS permission
        /// </summary>
        GuildPrivateThread = 12,
        
        /// <summary>
        /// A voice channel for <a href="https://support.discord.com/hc/en-us/articles/1500005513722">hosting events with an audience</a>
        /// </summary>
        GuildStageVoice = 13,
    }
}
