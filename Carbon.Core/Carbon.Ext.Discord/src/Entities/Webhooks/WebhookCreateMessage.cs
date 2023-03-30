using Newtonsoft.Json;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Entities.Webhooks
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/webhook#execute-webhook-jsonform-params">Webhook Create Message</a>
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class WebhookCreateMessage : MessageCreate
    {
        /// <summary>
        /// Override the default username of the webhook
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }

        /// <summary>
        /// Override the default avatar of the webhook
        /// </summary>
        [JsonProperty("avatar_url")]
        public string AvatarUrl { get; set; } 
        
        internal void ValidateWebhookMessage()
        {
            if (!Flags.HasValue)
            {
                return;
            }

            if ((Flags.Value & ~MessageFlags.SuppressEmbeds) != 0)
            {
                throw new InvalidMessageException("Invalid Message Flags Used for Webhook Message. Only supported flags are MessageFlags.SuppressEmbeds");
            }
        }
    }
}
