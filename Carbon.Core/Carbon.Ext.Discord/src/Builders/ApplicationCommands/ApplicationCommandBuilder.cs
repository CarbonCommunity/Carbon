using System;
using System.Collections.Generic;
using Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands;
using Oxide.Ext.Discord.Exceptions;

namespace Oxide.Ext.Discord.Builders.ApplicationCommands
{
    /// <summary>
    /// Builder to use when building application commands
    /// </summary>
    public class ApplicationCommandBuilder : IApplicationCommandBuilder
    {
        internal readonly CommandCreate Command;
        private readonly List<CommandOption> _options;
        private CommandOptionType? _chosenType;

        /// <summary>
        /// Creates a new Application Command Builder
        /// </summary>
        /// <param name="name">Name of the command</param>
        /// <param name="description">Description of the command</param>
        /// <param name="type">Command type</param>
        public ApplicationCommandBuilder(string name, string description, ApplicationCommandType type)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Value cannot be null or empty.", nameof(description));

            _options = new List<CommandOption>();
            Command = new CommandCreate
            {
                Name = name,
                Description = description,
                Type = type,
                Options = _options
            };
        }

        /// <summary>
        /// Set whether the command is enabled by default when the app is added to a guild
        /// </summary>
        /// <param name="enabled">If the command is enabled</param>
        /// <returns>This</returns>
        public ApplicationCommandBuilder SetEnabled(bool enabled)
        {
            Command.DefaultPermissions = enabled;
            return this;
        }

        /// <summary>
        /// Creates a new SubCommandGroup
        /// SubCommandGroups contain subcommands
        /// Your root command can only contain 
        /// </summary>
        /// <param name="name">Name of the command</param>
        /// <param name="description">Description of the command</param>
        /// <returns><see cref="SubCommandGroupBuilder"/></returns>
        /// <exception cref="Exception">Thrown if trying to add a subcommand group to</exception>
        public SubCommandGroupBuilder AddSubCommandGroup(string name, string description)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Value cannot be null or empty.", nameof(description));

            if (_chosenType.HasValue && _chosenType.Value != CommandOptionType.SubCommandGroup && _chosenType.Value != CommandOptionType.SubCommand)
            {
                throw new InvalidApplicationCommandException("Cannot mix sub command / sub command groups with command options");
            }

            if (Command.Type == ApplicationCommandType.Message || Command.Type == ApplicationCommandType.User)
            {
                throw new InvalidApplicationCommandException("Message and User commands cannot have sub command groups");
            }

            _chosenType = CommandOptionType.SubCommandGroup;

            return new SubCommandGroupBuilder(name, description, this);
        }

        /// <summary>
        /// Adds a sub command to the root command
        /// </summary>
        /// <param name="name">Name of the sub command</param>
        /// <param name="description">Description for the sub command</param>
        /// <returns><see cref="SubCommandBuilder"/></returns>
        /// <exception cref="Exception">Thrown if previous type was not SubCommand or Creation type is not ChatInput</exception>
        public SubCommandBuilder AddSubCommand(string name, string description)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Value cannot be null or empty.", nameof(name));
            if (string.IsNullOrEmpty(description))
                throw new ArgumentException("Value cannot be null or empty.", nameof(description));

            if (_chosenType.HasValue && _chosenType.Value != CommandOptionType.SubCommandGroup && _chosenType.Value != CommandOptionType.SubCommand)
            {
                throw new InvalidApplicationCommandException("Cannot mix sub command / sub command groups with command options");
            }

            if (Command.Type == ApplicationCommandType.Message || Command.Type == ApplicationCommandType.User)
            {
                throw new InvalidApplicationCommandException("Message and User commands cannot have sub commands");
            }

            _chosenType = CommandOptionType.SubCommand;

            return new SubCommandBuilder(_options, name, description, this);
        }

        /// <summary>
        /// Adds a command option.
        /// </summary>
        /// <param name="type">The type of option. Cannot be SubCommand or SubCommandGroup</param>
        /// <param name="name">Name of the option</param>
        /// <param name="description">Description for the option</param>
        /// <returns><see cref="CommandOptionBuilder"/></returns>
        public CommandOptionBuilder AddOption(CommandOptionType type, string name, string description)
        {
            if (_chosenType.HasValue && (_chosenType.Value == CommandOptionType.SubCommandGroup || _chosenType.Value == CommandOptionType.SubCommand))
            {
                throw new InvalidApplicationCommandException("Cannot mix sub command / sub command groups with command options");
            }

            return new CommandOptionBuilder(_options, type, name, description, this);
        }

        /// <summary>
        /// Returns the created command
        /// </summary>
        /// <returns></returns>
        public CommandCreate Build()
        {
            return Command;
        }
    }
}