using System;

namespace Oxide.Ext.Discord.Entities.Applications
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/application#application-object-application-flags">Application Flags</a>
    /// </summary>
    [Flags]
    public enum ApplicationFlags
    {
        /// <summary>
        /// This application has no flags
        /// </summary>
        None = 0,
        
        /// <summary>
        /// The application is verified and can use the GUILD_PRESENCES intent
        /// </summary>
        GatewayPresence = 1 << 12,
        
        /// <summary>
        /// The application has the GUILD_PRESENCES intent enabled on the bot
        /// </summary>
        GatewayPresenceLimited = 1 << 13,
        
        /// <summary>
        /// The application is verified and can use the GUILD_MEMBERS intent
        /// </summary>
        GatewayGuildMembers = 1 << 14,
        
        /// <summary>
        /// The application has the GUILD_MEMBERS intent enabled on the bot
        /// </summary>
        GatewayGuildMembersLimited = 1 << 15,
        
        /// <summary>
        /// The application is currently pending verification
        /// </summary>
        VerificationPendingGuildLimit = 1 << 16,
        
        /// <summary>
        /// The application has functionality that is specific to the discord client app.
        /// </summary>
        Embedded = 1 << 17,
        
        /// <summary>
        /// The application is verified and can use the Gateway Message intent
        /// </summary>
        GatewayMessageContent = 1 << 18,
        
        /// <summary>
        /// The application has the Gateway Message intent enabled on the bot
        /// </summary>
        GatewayMessageContentLimited = 1 << 19,
    }
}