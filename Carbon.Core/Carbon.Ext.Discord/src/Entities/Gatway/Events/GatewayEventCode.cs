namespace Oxide.Ext.Discord.Entities.Gatway.Events
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/opcodes-and-status-codes#gateway-gateway-opcodes">Gateway Opcodes</a>
    /// </summary>
    public enum GatewayEventCode
    {
        /// <summary>
        /// An event was dispatched.
        /// </summary>
        Dispatch = 0,
        
        /// <summary>
        /// Fired periodically by the client to keep the connection alive.
        /// </summary>
        Heartbeat = 1,
        
        /// <summary>
        /// You should attempt to reconnect and resume immediately.
        /// </summary>
        Reconnect = 7,
        
        /// <summary>
        /// The session has been invalidated. You should reconnect and identify/resume accordingly.
        /// </summary>
        InvalidSession = 9,
        
        /// <summary>
        /// Sent immediately after connecting, contains the heartbeat_interval to use.
        /// </summary>
        Hello = 10,
        
        /// <summary>
        /// Sent in response to receiving a heartbeat to acknowledge that it has been received.
        /// </summary>
        HeartbeatAcknowledge = 11
    }
}