using System.Text;
using Newtonsoft.Json;
using Oxide.Ext.Discord.Builders;

namespace Oxide.Ext.Discord.Entities.Channels
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/channel#get-channel-messages">Get Channel Messages Request</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class ChannelMessagesRequest
    {
        /// <summary>
        /// Get messages around this message ID
        /// Before, after, and around keys are mutually exclusive, only one may be passed at a time
        /// </summary>
        public Snowflake? Around { get; set; }
        
        /// <summary>
        /// Get messages before this message ID
        /// Before, after, and around keys are mutually exclusive, only one may be passed at a time
        /// </summary>
        public Snowflake? Before { get; set; }
        
        /// <summary>
        /// Get messages after this message ID
        /// Before, after, and around keys are mutually exclusive, only one may be passed at a time
        /// </summary>
        public Snowflake? After { get; set; }
        
        /// <summary>
        /// Max number of messages to return (1-100)
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Returns the request as a query string
        /// </summary>
        /// <returns></returns>
        public string ToQueryString()
        {
            QueryStringBuilder builder = new QueryStringBuilder();

            //Per Documentation "The before, after, and around keys are mutually exclusive, only one may be passed at a time."
            if (Around.HasValue)
            {
                builder.Add("around", Around.Value.ToString());
            }
            else if (Before.HasValue)
            {
                builder.Add("before", Before.Value.ToString());
            }
            else if (After.HasValue)
            {
                builder.Add("after", After.Value.ToString());
            }
            
            if (Limit.HasValue)
            {
                builder.Add("limit", Limit.Value.ToString());
            }

            return builder.ToString();
        }
    }
}