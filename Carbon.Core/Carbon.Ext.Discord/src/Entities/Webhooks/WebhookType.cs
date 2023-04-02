namespace Oxide.Ext.Discord.Entities.Webhooks
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/webhook#webhook-object-webhook-types">Webhook Types</a>
    /// </summary>
    public enum WebhookType
    {
        /// <summary>
        /// Incoming Webhooks can post messages to channels with a generated token
        /// </summary>
        Incoming = 1,
        
        /// <summary>
        /// Channel Follower Webhooks are internal webhooks used with Channel Following to post new messages into channels
        /// </summary>
        ChannelFollower = 2
    }
}