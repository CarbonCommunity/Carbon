using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Builders;

namespace Oxide.Ext.Discord.Entities.Guilds
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/guild#get-guild-prune-count">Guild Prune Get</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class GuildPruneGet
    {
        /// <summary>
        /// Number of days to count prune for (1 - 30)
        /// </summary>
        [JsonProperty("days")]
        public int Days { get; set; }
        
        /// <summary>
        /// List of roles to include
        /// </summary>
        [JsonProperty("include_roles")]
        public List<Snowflake> IncludeRoles { get; set; }
        
        /// <summary>
        /// Returns the query string for the Guild Prune Get endpoint
        /// </summary>
        /// <returns>Guild Prune Get Query String</returns>
        public virtual string ToQueryString()
        {
            QueryStringBuilder builder = new QueryStringBuilder();
            builder.Add("days", Days.ToString());
            if (IncludeRoles != null)
            {
                builder.AddList("include_roles", IncludeRoles, ",");
            }

            return builder.ToString();
        }
    }
}