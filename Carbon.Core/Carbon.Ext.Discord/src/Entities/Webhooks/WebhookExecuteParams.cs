using System;
using System.Text;
using Oxide.Ext.Discord.Builders;

namespace Oxide.Ext.Discord.Entities.Webhooks
{
    /// <summary>
    /// Represents parameters to execute a webhook
    /// </summary>
    public class WebhookExecuteParams
    {
        /// <summary>
        /// Which type of webhook are we trying to send (Discord, Slack, Github)
        /// Defaults to Discord
        /// </summary>
        public WebhookSendType SendType { get; set; } = WebhookSendType.Discord;
        
        /// <summary>
        /// Should we wait for a webhook to return a message or is this a fire and forget.
        /// Not settable by devs as it's controlled by which method is called
        /// </summary>
        public bool Wait { get; internal set; }
        
        /// <summary>
        /// If you're sending a message to a thread instead of a channel put the ID of the thread here.
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
            if (Wait)
            {
                builder.Add("wait", "true");
            }

            if (ThreadId.HasValue)
            {
                builder.Add("thread_id", ThreadId.Value.ToString());
            }
            
            return builder.ToString();
        }
        
        /// <summary>
        /// Returns the URL formatting for the webhook type
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string GetWebhookFormat()
        {
            switch (SendType)
            {
                case WebhookSendType.Discord:
                    return string.Empty;
                case WebhookSendType.Slack:
                    return "/slack";
                case WebhookSendType.Github:
                    return "/github";
                default:
                    throw new ArgumentOutOfRangeException(nameof(SendType), SendType, null);
            }
        }
    }
}