namespace Oxide.Ext.Discord.Exceptions
{
    /// <summary>
    /// Exception thrown when an invalid Snowflake ID is used in an API call
    /// </summary>
    public class InvalidSnowflakeException : BaseDiscordException
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="paramName">Name of the parameter that is invalid</param>
        internal InvalidSnowflakeException(string paramName) : base($"Invalid Snowflake ID. Parameter Name: {paramName}")
        {
            
        }
    }
}