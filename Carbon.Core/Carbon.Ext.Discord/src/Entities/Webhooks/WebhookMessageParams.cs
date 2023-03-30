using Oxide.Ext.Discord.Builders;
namespace Oxide.Ext.Discord.Entities.Webhooks
{
    /// <summary>
    /// Represents webhook message query string parameters 
    /// </summary>
    public class WebhookMessageParams
    {
        /// <summary>
        /// If the message exists in a thread
        /// This field is optional and defaults to null
        /// </summary>
        public Snowflake? ThreadId { get; set; }
        
        /// <summary>
        /// Returns the query string to be used in the webhook URL
        /// </summary>
        /// <returns></returns>
        public string ToQueryString()
        {
            QueryStringBuilder builder = new QueryStringBuilder();

            if (ThreadId.HasValue)
            {
                builder.Add("thread_id", ThreadId.Value.ToString());
            }
            
            return builder.ToString();
        }
    }
}