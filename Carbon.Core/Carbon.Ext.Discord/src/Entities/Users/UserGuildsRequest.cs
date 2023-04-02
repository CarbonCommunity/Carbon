using System.Text;
using Oxide.Ext.Discord.Builders;

namespace Oxide.Ext.Discord.Entities.Users
{
    /// <summary>
    /// Represents a <a href="https://discord.com/developers/docs/resources/user#get-current-user-guilds-query-string-params">Users Guild Request</a>
    /// </summary>
    public class UserGuildsRequest
    {
        /// <summary>
        /// Get guilds before this guild ID
        /// </summary>
        public Snowflake? Before { get; set; }
        
        /// <summary>
        /// Get guilds after this guild ID
        /// </summary>
        public Snowflake? After { get; set; }

        /// <summary>
        /// Max number of guilds to return (1-200)
        /// </summary>
        public int Limit { get; set; } = 200;

        /// <summary>
        /// Returns the query string for the request
        /// </summary>
        /// <returns></returns>
        public virtual string ToQueryString()
        {
            QueryStringBuilder builder = new QueryStringBuilder();
            builder.Add("limit", Limit.ToString());

            if (Before.HasValue)
            {
                builder.Add("before", Before.ToString());
            }
            
            if (After.HasValue)
            {
                builder.Add("after", After.ToString());
            }

            return builder.ToString();
        }
    }
}