namespace Oxide.Ext.Discord.Entities.Guilds.ScheduledEvents
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild-scheduled-event#guild-scheduled-event-object-guild-scheduled-event-status">Guild Scheduled Event Status</a>
    /// </summary>
    public enum ScheduledEventStatus
    {
        /// <summary>
        /// Scheduled Event is scheduled and has not happened yet.
        /// </summary>
        Scheduled = 1,
        
        /// <summary>
        /// Scheduled event is currently occuring
        /// </summary>
        Active = 2,
        
        /// <summary>
        /// Scheduled event has completed
        /// </summary>
        Completed = 3,
        
        /// <summary>
        /// Scheduled event was canceled.
        /// </summary>
        Canceled = 4,
    }
}