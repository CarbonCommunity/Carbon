using Oxide.Ext.Discord.Builders;
namespace Oxide.Ext.Discord.Entities.Guilds.ScheduledEvents
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/guild-scheduled-event#list-scheduled-events-for-guild-query-string-params">Scheduled Event Lookup Structure</a> within Discord.
    /// </summary>
    public class ScheduledEventLookup
    {
        /// <summary>
        /// Include number of users subscribed to each event
        /// </summary>
        public bool? WithUserCount { get; set; }
        
        internal string ToQueryString()
        {
            QueryStringBuilder builder = new QueryStringBuilder();
            if (WithUserCount.HasValue)
            {
                builder.Add("with_user_count", WithUserCount.Value.ToString());
            }

            return builder.ToString();
        }
    }
}