namespace Oxide.Ext.Discord.Entities.Guilds.ScheduledEvents
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild-scheduled-event#guild-scheduled-event-object-guild-scheduled-event-entity-types">Scheduled Entity Type</a>
    /// </summary>
    public enum ScheduledEventEntityType
    {
        /// <summary>
        /// Event will be held in a stage instance
        /// </summary>
        StageInstance =	1,
        
        /// <summary>
        /// Event will be held in a voice channel
        /// </summary>
        Voice =	2,
        
        /// <summary>
        /// Event will be held externally outside of discord.
        /// </summary>
        External = 3,
    }
}