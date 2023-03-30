using System;

namespace Oxide.Ext.Discord.Entities.Users
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/user#user-object-user-flags">User Flags</a>
    /// </summary>
    [Flags]
    public enum UserFlags
    {
        /// <summary>
        /// Default value for flags, when none are given to an account.
        /// </summary>
        None = 0,

        /// <summary>
        /// Flag given to users who are a Discord employee
        /// </summary>
        Staff = 1 << 0,
        
        /// <summary>
        /// Flag given to users who are owners of a partnered Discord server
        /// </summary>
        Partner = 1 << 1,
        
        /// <summary>
        /// Flag given to users who are HypeSquad Events Coordinator
        /// </summary>
        HypeSquad = 1 << 2,
        
        /// <summary>
        /// Flag given to users who have participated in the 𝐁ug report program and are level 1.
        /// </summary>
        BugHunterLevel1 = 1 << 3,
        
        /// <summary>
        /// Flag given to users who are in the HypeSquad House of Bravery.
        /// </summary>
        HypeSquadOnlineHouse1 = 1 << 6,
        
        /// <summary>
        /// Flag given to users who are in the HypeSquad House of Brilliance.
        /// </summary>
        HypeSquadOnlineHouse2 = 1 << 7,
        
        /// <summary>
        /// Flag given to users who are in the HypeSquad House of Balance.
        /// </summary>
        HypeSquadOnlineHouse3 = 1 << 8,
        
        /// <summary>
        /// Flag given to users who subscribed to Nitro before games were added.
        /// </summary>
        PremiumEarlySupporter = 1 << 9,
        
        /// <summary>
        /// Flag given to users who are part of a team.
        /// </summary>
        TeamPseudoUser = 1 << 10,

        /// <summary>
        /// Flag given to users who have participated in the 𝐁ug report program and are level 2.
        /// </summary>
        BugHunterLevel2 = 1 << 14,
        
        /// <summary>
        /// Flag given to users who are verified bots.
        /// </summary>
        VerifiedBot = 1 << 16,
        
        /// <summary>
        /// Flag given to users that developed bots and early verified their accounts.
        /// </summary>
        VerifiedDeveloper = 1 << 17,
        
        /// <summary>
        /// Flag given to users that are discord certified moderators
        /// </summary>
        CertifiedModerator = 1 << 18,
        
        /// <summary>
        /// User is a Bot uses only HTTP interactions and is shown in the online member list
        /// </summary>
        BotHttpInteractions = 1 << 19,
        
        #region Obsolete
        /// <summary>
        /// Flag given to users who are a Discord employee.
        /// </summary>
        [Obsolete("Replaced with Staff. Will be removed April 2022")]
        DiscordEmployee = 1 << 0,
        
        /// <summary>
        /// Flag given to users who are owners of a partnered Discord server.
        /// </summary>
        [Obsolete("Replaced with Partner. Will be removed April 2022")]
        PartneredServerOwner = 1 << 1,
        
        /// <summary>
        /// Flag given to users in HypeSquad events.
        /// </summary>
        [Obsolete("Replaced with HypeSquad. Will be removed April 2022")]
        HyperSquadEvents = 1 << 2,

        /// <summary>
        /// Flag given to users who are in the HypeSquad House of Bravery.
        /// </summary>
        [Obsolete("Replaced with HypeSquadOnlineHouse1. Will be removed April 2022")]
        HouseBravery = 1 << 6,
        
        /// <summary>
        /// Flag given to users who are in the HypeSquad House of Brilliance.
        /// </summary>
        [Obsolete("Replaced with HypeSquadOnlineHouse2. Will be removed April 2022")]
        HouseBrilliance = 1 << 7,
        
        /// <summary>
        /// Flag given to users who are in the HypeSquad House of Balance.
        /// </summary>
        [Obsolete("Replaced with HypeSquadOnlineHouse3. Will be removed April 2022")]
        HouseBalance = 1 << 8,
        
        /// <summary>
        /// Flag given to users who subscribed to Nitro before games were added.
        /// </summary>
        [Obsolete("Replaced with PremiumEarlySupporter. Will be removed April 2022")]
        EarlySupporter = 1 << 9,
        
        /// <summary>
        /// Flag given to users who are part of a team.
        /// </summary>
        [Obsolete("Replaced with TeamPseudoUser. Will be removed April 2022")]
        TeamUser = 1 << 10,

        /// <summary>
        ///  Flag given to users that developed bots and early verified their accounts.
        /// </summary>
        [Obsolete("Replaced with VerifiedDeveloper. Will be removed April 2022")]
        EarlyVerifiedBotDeveloper = 1 << 17,
        
        /// <summary>
        /// Flag given to users that are discord certified moderators
        /// </summary>
        [Obsolete("Replaced with CertifiedModerator. Will be removed April 2022")]
        DiscordCertifiedModerator = 1 << 18,
        #endregion
    }
}