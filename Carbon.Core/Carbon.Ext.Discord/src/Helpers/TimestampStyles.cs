namespace Oxide.Ext.Discord.Helpers
{
    /// <summary>
    /// Available flags for timestamp formatting
    /// </summary>
    public enum TimestampStyles
    {
        /// <summary>
        /// Displays the short time for the timestamp
        /// Ex: 16:20
        /// </summary>
        ShortTime,
        
        /// <summary>
        /// Displays the long time for the timestamp
        /// Ex: 16:20:30
        /// </summary>
        LongTime,
        
        /// <summary>
        /// Displays the short date for the timestamp
        /// Ex: 20/04/2021
        /// </summary>
        ShortDate,
        
        /// <summary>
        /// Displays the long date for the timestamp
        /// Ex: 20 April 2021
        /// </summary>
        LongDate,
        
        /// <summary>
        /// Displays the short date/time for the timestamp
        /// Ex: 20 April 2021 16:20
        /// </summary>
        ShortDateTime,
        
        /// <summary>
        /// Displays the long date/time for the timestamp
        /// Ex: Tuesday, 20 April 2021 16:20
        /// </summary>
        LongDateTime,
        
        /// <summary>
        /// Displays the relative time since the timestamp
        /// Ex: 2 months ago
        /// </summary>
        RelativeTime
    }
}