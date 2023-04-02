namespace Oxide.Ext.Discord.Entities.Gatway.Commands
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/opcodes-and-status-codes#gateway-gateway-opcodes">Gateway Opcodes</a>
    /// </summary>
    public enum GatewayCommandCode
    {
        /// <summary>
        /// Maintains an active gateway connection
        /// </summary>
        Heartbeat = 1,
        
        /// <summary>
        /// Starts a new session during the initial handshake.
        /// </summary>
        Identify = 2,
        
        /// <summary>
        /// Update the client's status.
        /// </summary>
        PresenceUpdate = 3,
        
        /// <summary>
        /// Used to join/leave or move between voice channels.
        /// </summary>
        VoiceStateUpdate = 4,
        
        /// <summary>
        /// Resume a previous session that was disconnected.
        /// </summary>
        Resume = 6,
        
        /// <summary>
        /// Request information about offline guild members in a large guild.
        /// </summary>
        RequestGuildMembers = 8,
    }
}