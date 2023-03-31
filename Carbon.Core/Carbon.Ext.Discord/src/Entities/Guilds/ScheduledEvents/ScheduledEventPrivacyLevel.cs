namespace Oxide.Ext.Discord.Entities.Guilds.ScheduledEvents
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild-scheduled-event#guild-scheduled-event-object-guild-scheduled-event-privacy-level">Guild Scheduled Event Privacy Level</a>
    /// </summary>
    public enum ScheduledEventPrivacyLevel
    {
        /// <summary>
        /// No Privacy Level
        /// </summary>
        None = 0,
        
        /// <summary>
        /// The scheduled event is only accessible to guild members
        /// </summary>
        GuildOnly = 2
    }
}