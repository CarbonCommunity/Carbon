using System;

namespace Oxide.Ext.Discord.Attributes
{
    /// <summary>
    /// Used to identify direct message bot commands
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class DirectMessageCommandAttribute : BaseCommandAttribute
    {
        /// <summary>
        /// Creates a discord command to be used in direct messages to the bot
        /// </summary>
        /// <param name="name">Name of the command</param>
        /// <param name="isLocalized">If the command name is the localization key for the command</param>
        public DirectMessageCommandAttribute(string name, bool isLocalized = false) : base(name, isLocalized)
        {

        }
    }
}
