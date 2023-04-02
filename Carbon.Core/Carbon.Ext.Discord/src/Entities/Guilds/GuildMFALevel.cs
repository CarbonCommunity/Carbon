using System.ComponentModel;

namespace Oxide.Ext.Discord.Entities.Guilds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild#guild-object-mfa-level">MFA Level</a>
    /// </summary>
    public enum GuildMFALevel
    {
        /// <summary>
        /// Guild does not require MFA
        /// </summary>
        [System.ComponentModel.Description ("NONE")]
        None = 0,
        
        /// <summary>
        /// Guild requires elevated MFA
        /// </summary>
        [System.ComponentModel.Description ("ELEVATED")]
        Elevated = 1
    }
}
