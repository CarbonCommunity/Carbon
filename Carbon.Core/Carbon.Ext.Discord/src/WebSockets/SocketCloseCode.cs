namespace Oxide.Ext.Discord.WebSockets
{
    //https://discord.com/developers/docs/topics/opcodes-and-status-codes#gateway-gateway-close-event-codes
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/topics/opcodes-and-status-codes#gateway-gateway-close-event-codes">Socket Close Event Codes</a>
    /// </summary>
    public enum SocketCloseCode
    {
        /// <summary>
        /// We're not sure what went wrong. Try reconnecting?
        /// </summary>
        UnknownError = 4000,
        /// <summary>
        /// You sent an invalid Gateway opcode or an invalid payload for an opcode. Don't do that!
        /// </summary>
        UnknownOpcode = 4001,
        
        /// <summary>
        /// You sent an invalid payload to us. Don't do that!
        /// </summary>
        DecodeError = 4002,
        
        /// <summary>
        /// You sent us a payload prior to identifying.
        /// </summary>
        NotAuthenticated = 4003,
        
        /// <summary>
        /// The account token sent with your identify payload is incorrect.
        /// </summary>
        AuthenticationFailed = 4004,
        
        /// <summary>
        /// You sent more than one identify payload. Don't do that!
        /// </summary>
        AlreadyAuthenticated = 4005,
        
        /// <summary>
        /// The sequence sent when resuming the session was invalid. Reconnect and start a new session.
        /// </summary>
        InvalidSequence = 4007,
        
        /// <summary>
        /// Woah nelly! You're sending payloads to us too quickly. Slow it down! You will be disconnected on receiving this.
        /// </summary>
        RateLimited = 4008,
        
        /// <summary>
        /// Your session timed out. Reconnect and start a new one.
        /// </summary>
        SessionTimedOut = 4009,
        
        /// <summary>
        /// You sent us an invalid shard when identifying.
        /// </summary>
        InvalidShard = 4010,
        
        /// <summary>
        /// The session would have handled too many guilds - you are required to shard your connection in order to connect.
        /// </summary>
        ShardingRequired = 4011,
        
        /// <summary>
        /// You sent an invalid version for the gateway.
        /// </summary>
        InvalidApiVersion = 4012,
        
        /// <summary>
        /// You sent an invalid intent for a Gateway Intent. You may have incorrectly calculated the bitwise value.
        /// </summary>
        InvalidIntents = 4013,
        
        /// <summary>
        /// You sent a disallowed intent for a Gateway Intent. You may have tried to specify an intent that you have not enabled or are not whitelisted for.
        /// </summary>
        DisallowedIntent = 4014,
        
        /// <summary>
        /// Used when a code is sent that we don't have yet.
        /// </summary>
        UnknownCloseCode = 4999
    }
}