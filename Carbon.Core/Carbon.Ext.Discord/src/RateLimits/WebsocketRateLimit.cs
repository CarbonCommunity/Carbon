namespace Oxide.Ext.Discord.RateLimits
{
    /// <summary>
    /// Represents a WebSocket Rate Limit
    /// </summary>
    public class WebsocketRateLimit : BaseRateLimit
    {
        /// <summary>
        /// Constructor for WebsocketRateLimit
        /// </summary>
        public WebsocketRateLimit() : base(110, 60)
        {
            
        }
    }
}