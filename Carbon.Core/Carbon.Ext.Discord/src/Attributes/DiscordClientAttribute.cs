using System;

namespace Oxide.Ext.Discord.Attributes
{
    /// <summary>
    /// Used to identify which field in a plugin to create a discord client for and set the client to.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DiscordClientAttribute : Attribute
    {
    }
}
