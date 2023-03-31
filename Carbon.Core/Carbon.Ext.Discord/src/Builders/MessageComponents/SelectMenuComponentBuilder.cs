using System;
using Oxide.Ext.Discord.Entities.Emojis;
using Oxide.Ext.Discord.Entities.Interactions.MessageComponents;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Builders.MessageComponents
{
    /// <summary>
    /// Builder for Select Menus
    /// </summary>
    public class SelectMenuComponentBuilder
    {
        private readonly SelectMenuComponent _menu;
        private readonly MessageComponentBuilder _builder;
        
        internal SelectMenuComponentBuilder(SelectMenuComponent menu, MessageComponentBuilder builder)
        {
            _menu = menu;
            _builder = builder;
        }

        /// <summary>
        /// Adds an option to a select menu;
        /// </summary>
        /// <param name="label">Display text for the select option</param>
        /// <param name="value">Selected value for the select option</param>
        /// <param name="description">Description of the select option</param>
        /// <param name="default">Default selected option</param>
        /// <param name="emoji">Emoji to display with the option</param>
        /// <exception cref="Exception">Throw is more than 25 options are added</exception>
        public SelectMenuComponentBuilder AddOption(string label, string value, string description, bool @default = false, DiscordEmoji emoji = null)
        {
            if (string.IsNullOrEmpty(label))
                throw new ArgumentException("Value cannot be null or empty.", nameof(label));
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));

            if (_menu.Options.Count >= 25)
            {
                throw new InvalidMessageComponentException("Select Menu Options cannot have more than 25 options");
            }
            
            _menu.Options.Add(new SelectMenuOption
            {
                Label = label,
                Value = value,
                Description = description,
                Default = @default,
                Emoji = emoji
            });
            return this;
        }

        /// <summary>
        /// Returns the root builder
        /// </summary>
        /// <returns><see cref="MessageComponentBuilder"/></returns>
        public MessageComponentBuilder Build()
        {
            return _builder;
        }
    }
}