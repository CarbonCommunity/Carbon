namespace Oxide.Ext.Discord.Entities.Invites
{
    /// <summary>
    /// Represents <a href="https://discord.com/developers/docs/resources/invite#invite-object-target-user-types">Target User Types</a>
    /// </summary>
    public enum TargetUserType
    {
        /// <summary>
        /// Target user type is not defined
        /// </summary>
        Undefined = 0,
        
        /// <summary>
        /// Invite is for a go live stream
        /// </summary>
        Stream = 1
    }
}