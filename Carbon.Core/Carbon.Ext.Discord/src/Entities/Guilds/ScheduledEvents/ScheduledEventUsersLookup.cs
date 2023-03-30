using Oxide.Ext.Discord.Builders;
namespace Oxide.Ext.Discord.Entities.Guilds.ScheduledEvents
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/guild-scheduled-event#list-scheduled-events-for-guild-query-string-params">Scheduled Event Lookup Structure</a> within Discord.
    ///
    /// Provide a user id to before and after for pagination.
    /// Users will always be returned in ascending order by user_id.
    /// If both before and after are provided, only before is respected.
    /// Fetching users in-between before and after is not supported.
    /// </summary>
    public class ScheduledEventUsersLookup
    {
        /// <summary>
        /// Number of users to return (up to maximum 100)
        /// </summary>
        public int? Limit { get; set; }
        
        /// <summary>
        /// Include guild member data if it exists
        /// Default false
        /// </summary>
        public bool? WithMember { get; set; }
        
        /// <summary>
        /// Consider only users before given user id
        /// Default null
        /// </summary>
        public Snowflake? Before { get; set; }
        
        /// <summary>
        /// Consider only users after given user id
        /// Default null
        /// </summary>
        public Snowflake? After { get; set; }
        
        internal string ToQueryString()
        {
            QueryStringBuilder builder = new QueryStringBuilder();
            if (Limit.HasValue)
            {
                builder.Add("limit", Limit.Value.ToString());
            }
            
            if (WithMember.HasValue)
            {
                builder.Add("with_member", WithMember.Value.ToString());
            }
            
            if (Before.HasValue)
            {
                builder.Add("before", Before.Value.ToString());
            }
            
            if (After.HasValue)
            {
                builder.Add("after", After.Value.ToString());
            }

            return builder.ToString();
        }
    }
}