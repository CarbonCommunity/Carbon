using System.Collections.Generic;
using Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands;
namespace Oxide.Ext.Discord.Builders.ApplicationCommands
{
    /// <summary>
    /// Builder for Sub Command Groups
    /// </summary>
    public class SubCommandGroupBuilder : IApplicationCommandBuilder
    {
        private readonly CommandOption _option;

        internal SubCommandGroupBuilder(string name, string description, ApplicationCommandBuilder builder)
        {
            _option = new CommandOption
            {
                Name = name,
                Description = description,
                Type = CommandOptionType.SubCommandGroup,
                Options = new List<CommandOption>()
            };
            builder.Command.Options.Add(_option);
        }

        /// <summary>
        /// Adds a sub command to this sub command group
        /// </summary>
        /// <param name="name">Name of the command</param>
        /// <param name="description">Description of the command</param>
        /// <returns><see cref="SubCommandBuilder"/></returns>
        public SubCommandBuilder AddSubCommand(string name, string description)
        {
            return new SubCommandBuilder(_option.Options, name, description, this);
        }
    }
}