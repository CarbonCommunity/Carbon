using Oxide.Ext.Discord.Builders;
namespace Oxide.Ext.Discord.Entities.Invites
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/guild-scheduled-event#list-scheduled-events-for-guild-query-string-params">Scheduled Event Lookup Structure</a> within Discord.
    /// </summary>
    public class InviteLookup
    {
        /// <summary>
        /// Whether the invite should contain approximate member counts
        /// </summary>
        public bool? WithCounts { get; set; }
        
        /// <summary>
        /// Whether the invite should contain the expiration date
        /// </summary>
        public bool? WithExpiration { get; set; }
        
        /// <summary>
        /// The guild scheduled event to include with the invite
        /// </summary>
        public bool? GuildScheduledEventId { get; set; }

        internal string ToQueryString()
        {
            QueryStringBuilder builder = new QueryStringBuilder();
            if (WithCounts.HasValue)
            {
                builder.Add("with_counts", WithCounts.Value.ToString());
            }
            
            if (WithExpiration.HasValue)
            {
                builder.Add("with_expiration", WithExpiration.Value.ToString());
            }
            
            if (GuildScheduledEventId.HasValue)
            {
                builder.Add("guild_scheduled_event_id", GuildScheduledEventId.Value.ToString());
            }

            return builder.ToString();
        }
    }
}