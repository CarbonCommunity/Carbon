using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Interactions.ApplicationCommands;
using Oxide.Ext.Discord.Entities.Interactions.MessageComponents;
using Oxide.Ext.Discord.Entities.Permissions;
using Oxide.Ext.Discord.Entities.Users;
using Oxide.Ext.Discord.Helpers;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Entities.Interactions
{
    /// <summary>
    /// Parses Interaction Data to make it easier to process for application commands
    /// </summary>
    public class InteractionDataParsed
    {
        /// <summary>
        /// Interaction this data is for
        /// </summary>
        public readonly DiscordInteraction Interaction;
        
        /// <summary>
        /// <see cref="InteractionData"/> for the Interaction
        /// </summary>
        public readonly InteractionData Data;
        
        /// <summary>
        /// <see cref="ApplicationCommandType"/> The type of interaction that was triggered
        /// </summary>
        public readonly ApplicationCommandType? Type;
        
        /// <summary>
        /// Parsed command for the interaction if an application command
        /// </summary>
        public readonly string Command;

        /// <summary>
        /// Command group for the interaction if <see cref="ApplicationCommandType.ChatInput"/> Command Type if an application command
        /// Null if command group is not used for the command.
        /// Defaults to empty string if command does not have a command group
        /// </summary>
        public string CommandGroup { get; private set; } = string.Empty;
        
        /// <summary>
        /// Sub Command for the interaction if <see cref="ApplicationCommandType.ChatInput"/> Command Typ  if an application command
        /// Null if sub command group is not used for the command.
        /// Defaults to empty string if command does not have sub command
        /// </summary>
        public string SubCommand { get; private set; } = string.Empty;
        
        /// <summary>
        /// Interaction Data Supplied Args if <see cref="ApplicationCommandType.ChatInput"/> Command Type if an application command
        /// </summary>
        public InteractionDataArgs Args { get; private set; }
        
        /// <summary>
        /// Returns true if this command was used in a guild; false otherwise.
        /// </summary>
        public bool InGuild => Interaction.GuildId.HasValue;

        /// <summary>
        /// The CustomId of the <see cref="BaseInteractableComponent"/> that triggered the interaction if a component triggered this interaction
        /// </summary>
        public readonly string TriggeredComponentId;
        
        /// <summary>
        /// If a <see cref="SelectMenuComponent"/> triggered this interaction. The values selected from the select menu.
        /// </summary>
        public readonly List<string> SelectMenuValues;

        /// <summary>
        /// Discord User's locale converted to oxide lang locale
        /// </summary>
        public readonly string UserOxideLocale;

        /// <summary>
        /// Discord Guild's locale converted to oxide lang locale
        /// </summary>
        public readonly string GuildOxideLocale;

        /// <summary>
        /// Constructor for the data parser.
        /// </summary>
        /// <param name="interaction">Interaction to be parsed</param>
        public InteractionDataParsed(DiscordInteraction interaction)
        {
            Interaction = interaction;
            Data = interaction.Data;
            //Check if MessageComponent and parse data accordingly
            if (Data.ComponentType.HasValue)
            {
                TriggeredComponentId = Data.CustomId;
                if (Data.ComponentType == MessageComponentType.SelectMenu)
                {
                    SelectMenuValues = Data.Values;
                }
                return;
            }
            Type = Data.Type;
            Command = Data.Name;
            //If ApplicationCommand is Message or User it can't have arguments
            if (Type == ApplicationCommandType.Message || Type == ApplicationCommandType.User)
            {
                return;
            }

            UserOxideLocale = LocaleConverter.GetOxideLocale(interaction.Locale);
            GuildOxideLocale = LocaleConverter.GetOxideLocale(interaction.GuildLocale);
            
            //Parse the arguments for the application command
            ParseCommand(Data.Options);
        }

        private void ParseCommand(List<InteractionDataOption> options)
        {
            if (options == null || options.Count == 0)
            {
                return;
            }
            
            InteractionDataOption option = options[0];
            switch (option.Type)
            {
                case CommandOptionType.SubCommandGroup:
                    CommandGroup = option.Name;
                    ParseCommand(option.Options);
                    break;
                    
                case CommandOptionType.SubCommand:
                    SubCommand = option.Name;
                    ParseCommand(option.Options);
                    break;
                    
                default:
                    Args = new InteractionDataArgs(Interaction, options);
                    break;
            }
        }
    }

    /// <summary>
    /// Args supplied for the interaction
    /// </summary>
    public class InteractionDataArgs
    {
        private readonly Hash<string, InteractionDataOption> _args = new Hash<string, InteractionDataOption>();
        private readonly DiscordInteraction _interaction;

        internal InteractionDataArgs(DiscordInteraction interaction, List<InteractionDataOption> options)
        {
            _interaction = interaction;
            for (int index = 0; index < options.Count; index++)
            {
                InteractionDataOption option = options[index];
                _args[option.Name] = option;
            }
        }

        /// <summary>
        /// Returns if a given arg exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasArg(string name)
        {
            return _args.ContainsKey(name);
        }

        /// <summary>
        /// Returns the string value supplied to command option matching the name.
        /// If the arg was optional and wasn't supplied default supplied value will be used.
        /// </summary>
        /// <param name="name">Name of the command option</param>
        /// <param name="default">Default value to return if not supplied</param>
        /// <returns>String for the matching command option name</returns>
        /// <exception cref="Exception">Thrown if the option type is not a string</exception>
        public string GetString(string name, string @default = default(string))
        {
            return (string)GetArg(name, CommandOptionType.String)?.Value ?? @default;
        }

        /// <summary>
        /// Returns the int value supplied to command option matching the name.
        /// If the arg was optional and wasn't supplied default supplied value will be used.
        /// </summary>
        /// <param name="name">Name of the command option</param>
        /// <param name="default">Default value to return if not supplied</param>
        /// <returns>Int for the matching command option name</returns>
        /// <exception cref="Exception">Thrown if the option type is not an int</exception>
        public int GetInt(string name, int @default = default(int))
        {
            return (int?)GetArg(name, CommandOptionType.Integer)?.Value ?? @default;
        }
        
        /// <summary>
        /// Returns the bool value supplied to command option matching the name.
        /// If the arg was optional and wasn't supplied default supplied value will be used.
        /// </summary>
        /// <param name="name">Name of the command option</param>
        /// <param name="default">Default value to return if not supplied</param>
        /// <returns>Bool for the matching command option name</returns>
        /// <exception cref="Exception">Thrown if the option type is not a bool</exception>
        public bool GetBool(string name, bool @default = default(bool))
        {
            return (bool?)GetArg(name, CommandOptionType.Boolean)?.Value ?? @default;
        }
        
        /// <summary>
        /// Returns the <see cref="DiscordUser"/> that was resolved from the command.
        /// </summary>
        /// <param name="name">Name of the command option</param>
        /// <returns><see cref="DiscordUser"/> resolved for the matching command option name</returns>
        /// <exception cref="Exception">Thrown if the option type is not a <see cref="DiscordUser"/> or mentionable</exception>
        public DiscordUser GetUser(string name)
        {
            return _interaction.Data.Resolved.Users[GetEntityId(GetArg(name, CommandOptionType.User)?.Value)];
        }

        /// <summary>
        /// Returns the <see cref="DiscordChannel"/> that was resolved from the command.
        /// </summary>
        /// <param name="name">Name of the command option</param>
        /// <returns><see cref="DiscordChannel"/> resolved for the matching command option name</returns>
        /// <exception cref="Exception">Thrown if the option type is not a <see cref="DiscordChannel"/></exception>
        public DiscordChannel GetChannel(string name)
        {
            return _interaction.Data.Resolved.Channels[GetEntityId(GetArg(name, CommandOptionType.Channel)?.Value)];
        }
        
        /// <summary>
        /// Returns the <see cref="DiscordRole"/> that was resolved from the command.
        /// </summary>
        /// <param name="name">Name of the command option</param>
        /// <returns><see cref="DiscordRole"/> resolved for the matching command option name</returns>
        /// <exception cref="Exception">Thrown if the option type is not a <see cref="DiscordRole"/> or mentionable</exception>
        public DiscordRole GetRole(string name)
        {
            return _interaction.Data.Resolved.Roles[GetEntityId(GetArg(name, CommandOptionType.Role)?.Value)];
        }

        /// <summary>
        /// Returns the double value supplied to command option matching the name.
        /// If the arg was optional and wasn't supplied default supplied value will be used.
        /// </summary>
        /// <param name="name">Name of the command option</param>
        /// <param name="default">Default value to return if not supplied</param>
        /// <returns>double for the matching command option name</returns>
        /// <exception cref="Exception">Thrown if the option type is not a double</exception>
        public double GetNumber(string name, double @default)
        {
            return (double?)GetArg(name, CommandOptionType.Number)?.Value ?? @default;
        }

        private InteractionDataOption GetArg(string name, CommandOptionType requested)
        {
            InteractionDataOption arg = _args[name];
            if (arg == null)
            {
                return null;
            }

            if (arg.Type != CommandOptionType.Mentionable)
            {
                ValidateArgs(name, arg.Type, requested);
            }
            else if (requested != CommandOptionType.Role && requested != CommandOptionType.User)
            {
                throw new Exception($"Attempted to parse {name} role/user type to: {requested} which is not valid.");
            }
            
            return arg;
        }

        private void ValidateArgs(string name, CommandOptionType arg, CommandOptionType requested)
        {
            if (arg != requested)
            {
                throw new Exception($"Attempted to parse {name} {arg} type to: {requested} which is not valid.");
            }
        }

        private Snowflake GetEntityId(JToken value)
        {
            if (value == null)
            {
                return default(Snowflake);
            }

            return value.ToObject<Snowflake>();
        }
    }
}