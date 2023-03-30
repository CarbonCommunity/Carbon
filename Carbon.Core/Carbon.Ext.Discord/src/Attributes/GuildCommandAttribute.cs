using System;

namespace Oxide.Ext.Discord.Attributes
{
    /// <summary>
    /// Used to identify guild bot commands
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class GuildCommandAttribute : BaseCommandAttribute
    {
        /// <summary>
        /// Creates a discord command to be used in guild
        /// </summary>
        /// <param name="name">Name of the command</param>
        /// <param name="isLocalized">If the command name is the localization key for the command</param>
        public GuildCommandAttribute(string name, bool isLocalized = false) : base(name, isLocalized)
        {

        }
    }
}
