using System;

namespace Oxide.Ext.Discord.Attributes
{
    /// <summary>
    /// Represents a base attribute for commands 
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class BaseCommandAttribute : Attribute
    {
        /// <summary>
        /// Name of the command
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// If the command name is the localization key
        /// </summary>
        public bool IsLocalized { get; }

        /// <summary>
        /// Constructor for a base command
        /// </summary>
        /// <param name="name">Name of the command</param>
        /// <param name="isLocalized">If the command name is the localization key for the command</param>
        protected BaseCommandAttribute(string name, bool isLocalized = false)
        {
            Name = name;
            IsLocalized = isLocalized;
        }
    }
}