using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Carbon.Core;
using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using Oxide.Ext.Discord.Attributes;
using Oxide.Ext.Discord.Configuration;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Libraries.Command
{
    /// <summary>
    /// Represents a library for discord commands
    /// </summary>
    public class DiscordCommand : Library
    {
        /// <summary>
        /// Available command prefixes used by the extension
        /// </summary>
        public readonly char[] CommandPrefixes;

        private readonly Hash<string, DirectMessageCommand> _directMessageCommands = new Hash<string, DirectMessageCommand>();
        private readonly Hash<string, GuildCommand> _guildCommands = new Hash<string, GuildCommand>();

        private Lang _lang;

        private Lang Lang
        {
            get
            {
                if (_lang != null)
                {
                    return _lang;
                }

                return _lang = Interface.Oxide.GetLibrary<Lang>();
            }
        }

		public DiscordCommand()
		{
			var config = ConfigFile.Load<DiscordConfig>(Path.Combine(Defines.GetConfigsFolder(), "discord.config.json"));
			CommandPrefixes = config.Commands.CommandPrefixes;
		}

        /// <summary>
        /// Discord Commands Constructor
        /// </summary>
        /// <param name="prefixes">Command prefixes used by the extension</param>
        public DiscordCommand(char[] prefixes)
        {
            CommandPrefixes = prefixes;
        }
        
        /// <summary>
        /// Returns if there are any guild discord commands are registered
        /// </summary>
        /// <returns></returns>
        public bool HasCommands()
        {
            return HasDirectMessageCommands() || HasGuildCommands();
        }
        
        /// <summary>
        /// Returns if there are any guild discord commands are registered
        /// </summary>
        /// <returns></returns>
        public bool HasDirectMessageCommands()
        {
            return _directMessageCommands.Count != 0;
        }
        
        /// <summary>
        /// Returns if there are any guild discord commands are registered
        /// </summary>
        /// <returns></returns>
        public bool HasGuildCommands()
        {
            return _guildCommands.Count != 0;
        }

        /// <summary>
        /// Adds a discord direct message command
        /// Sourced from Command.cs of OxideMod (https://github.com/OxideMod/Oxide.Rust/blob/develop/src/Libraries/Command.cs#L123)
        /// </summary>
        /// <param name="name">Name of the command</param>
        /// <param name="plugin">Plugin to add the command for</param>
        /// <param name="callback">Method name of the callback</param>
        public void AddDirectMessageCommand(string name, Plugin plugin, string callback)
        {
            AddDirectMessageCommand(name, plugin, (message, command, args) => plugin.CallHook(callback, message, command, args));
        }
        
        /// <summary>
        /// Adds a localized discord direct message command
        /// Sourced from Command.cs of OxideMod (https://github.com/OxideMod/Oxide.Rust/blob/develop/src/Libraries/Command.cs#L123)
        /// </summary>
        /// <param name="langKey">Lang Key on the plugin that contains the command</param>
        /// <param name="plugin">Plugin to add the localized command for</param>
        /// <param name="callback">Method name of the callback</param>
        public void AddDirectMessageLocalizedCommand(string langKey, Plugin plugin, string callback)
        {
            foreach (string langType in Lang.GetLanguages(plugin))
            {
                Dictionary<string, string> langKeys = Lang.GetMessages(langType, plugin as RustPlugin);
                if (langKeys.TryGetValue(langKey, out string command) && !string.IsNullOrEmpty(command))
                {
                    AddDirectMessageCommand(command, plugin, callback);
                }
            }
        }

        /// <summary>
        /// Adds a discord direct message command
        /// Sourced From Command.cs of OxideMod (https://github.com/OxideMod/Oxide.Rust/blob/develop/src/Libraries/Command.cs#L134)
        /// </summary>
        /// <param name="command">Command to add</param>
        /// <param name="plugin">Plugin to add the command for</param>
        /// <param name="callback">Method name of the callback</param>
        public void AddDirectMessageCommand(string command, Plugin plugin, Action<DiscordMessage, string, string[]> callback)
        {
            string commandName = command.ToLowerInvariant();

            if (_directMessageCommands.TryGetValue(commandName, out DirectMessageCommand cmd))
            {
                string previousPluginName = cmd.Plugin?.Name ?? "an unknown plugin";
                string newPluginName = plugin?.Name ?? "An unknown plugin";
                DiscordExtension.GlobalLogger.Warning($"{newPluginName} has replaced the '{commandName}' discord direct message command previously registered by {previousPluginName}");
            }

            cmd = new DirectMessageCommand(commandName, plugin, callback);

            // Add the new command to collections
            _directMessageCommands[commandName] = cmd;
        }

        /// <summary>
        /// Adds a discord guild command
        /// Sourced from Command.cs of OxideMod (https://github.com/OxideMod/Oxide.Rust/blob/develop/src/Libraries/Command.cs#L123)
        /// </summary>
        /// <param name="name">The name of the command</param>
        /// <param name="plugin">Plugin to add the command for</param>
        /// <param name="allowedChannels">Channel or Category Id's this command is allowed in</param>
        /// <param name="callback">Method name of the callback</param>
        public void AddGuildCommand(string name, Plugin plugin, List<Snowflake> allowedChannels, string callback)
        {
            AddGuildCommand(name, plugin, allowedChannels, (message, command, args) => plugin.CallHook(callback, message, command, args));
        }

        /// <summary>
        /// Adds a localized discord guild command
        /// Sourced from Command.cs of OxideMod (https://github.com/OxideMod/Oxide.Rust/blob/develop/src/Libraries/Command.cs#L123)
        /// </summary>
        /// <param name="langKey">Lang Key on the plugin that contains the command</param>
        /// <param name="plugin">Plugin to add the localized command for</param>
        /// <param name="allowedChannels">Channel or Category Id's this command is allowed in</param>
        /// <param name="callback">Method name of the callback</param>
        public void AddGuildLocalizedCommand(string langKey, Plugin plugin, List<Snowflake> allowedChannels, string callback)
        {
            foreach (string langType in Lang.GetLanguages(plugin))
            {
                Dictionary<string, string> langKeys = Lang.GetMessages(langType, plugin as RustPlugin );
                if (langKeys.TryGetValue(langKey, out string command) && !string.IsNullOrEmpty(command))
                {
                    AddGuildCommand(command, plugin, allowedChannels, callback);
                }
            }
        }

        /// <summary>
        /// Adds a discord guild command
        /// Sourced From Command.cs of OxideMod (https://github.com/OxideMod/Oxide.Rust/blob/develop/src/Libraries/Command.cs#L134)
        /// </summary>
        /// <param name="command">Name of the command</param>
        /// <param name="plugin">Plugin to add the localized command for</param>
        /// <param name="allowedChannels">Channel or Category Id's this command is allowed in</param>
        /// <param name="callback">Method name of the callback</param>
        public void AddGuildCommand(string command, Plugin plugin, List<Snowflake> allowedChannels, Action<DiscordMessage, string, string[]> callback)
        {
            string commandName = command.ToLowerInvariant();

            if (_guildCommands.TryGetValue(commandName, out GuildCommand cmd))
            {
                string previousPluginName = cmd.Plugin?.Name ?? "an unknown plugin";
                string newPluginName = plugin?.Name ?? "An unknown plugin";
                DiscordExtension.GlobalLogger.Warning($"{newPluginName} has replaced the '{commandName}' discord guild command previously registered by {previousPluginName}");
            }

            cmd = new GuildCommand(commandName, plugin, allowedChannels, callback);

            // Add the new command to collections
            _guildCommands[commandName] = cmd;
        }

        /// <summary>
        /// Removes a previously registered discord command
        /// Sourced From Command.cs of OxideMod (https://github.com/OxideMod/Oxide.Rust/blob/develop/src/Libraries/Command.cs#L286)
        /// </summary>
        /// <param name="command">Command to remove</param>
        /// <param name="plugin">Plugin the command is for</param>
        public void RemoveDiscordCommand(string command, Plugin plugin)
        {
            DirectMessageCommand dmCommand = _directMessageCommands[command];
            if (dmCommand != null && dmCommand.Plugin == plugin)
            {
                RemoveDmCommand(dmCommand);
            }
            
            GuildCommand guildCommand = _guildCommands[command];
            if (guildCommand != null && guildCommand.Plugin == plugin)
            {
                RemoveGuildCommand(guildCommand);
            }
        }

        /// <summary>
        /// Removes a discord command
        /// Sourced from Command.cs of OxideMod (https://github.com/OxideMod/Oxide.Rust/blob/develop/src/Libraries/Command.cs#L314)
        /// </summary>
        /// <param name="command"></param>
        private void RemoveDmCommand(DirectMessageCommand command)
        {
            DirectMessageCommand dmCommand = _directMessageCommands[command.Name];
            dmCommand.OnRemoved();
            _directMessageCommands.Remove(command.Name);
        }

        private void RemoveGuildCommand(GuildCommand command)
        {
            GuildCommand guildCommand = _guildCommands[command.Name];
            guildCommand.OnRemoved();
            _guildCommands.Remove(command.Name);
        }

        /// <summary>
        /// Called when a plugin has been unloaded
        /// </summary>
        /// <param name="sender"></param>
        internal void OnPluginUnloaded(Plugin sender)
        {
            List<DirectMessageCommand> dmCommands = new List<DirectMessageCommand>();
            List<GuildCommand> guildCommands = new List<GuildCommand>();
            // Remove all discord commands which were registered by the plugin
            foreach (DirectMessageCommand cmd in _directMessageCommands.Values)
            {
                if (cmd.Plugin.Name == sender.Name)
                {
                    dmCommands.Add(cmd);
                }
            }
            
            foreach (GuildCommand cmd in _guildCommands.Values)
            {
                if (cmd.Plugin.Name == sender.Name)
                {
                    guildCommands.Add(cmd);
                }
            }

            for (int index = 0; index < dmCommands.Count; index++)
            {
                DirectMessageCommand cmd = dmCommands[index];
                RemoveDmCommand(cmd);
            }
            
            for (int index = 0; index < guildCommands.Count; index++)
            {
                GuildCommand cmd = guildCommands[index];
                RemoveGuildCommand(cmd);
            }
        }

        /// <summary>
        /// Handles the specified direct message command
        /// Sourced from Command.cs of OxideMod (https://github.com/OxideMod/Oxide.Rust/blob/develop/src/Libraries/Command.cs#L361)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="channel"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        internal bool HandleDirectMessageCommand(BotClient client, DiscordMessage message, DiscordChannel channel, string name, string[] args)
        {
            DirectMessageCommand command = _directMessageCommands[name];
            if (command == null || !command.CanRun(client) || !command.CanHandle(message, channel))
            {
                return false;
            }
            
            if (!command.Plugin.IsLoaded)
            {
                _directMessageCommands.Remove(name);
                return false;
            }

            if (!client.IsPluginRegistered(command.Plugin))
            {
                return false;
            }

            command.HandleCommand(message, name, args);
            return true;
        }

        /// <summary>
        /// Handles the specified direct message command
        /// Sourced from Command.cs of OxideMod (https://github.com/OxideMod/Oxide.Rust/blob/develop/src/Libraries/Command.cs#L361)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="channel"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <param name="message"></param>
        internal bool HandleGuildCommand(BotClient client, DiscordMessage message, DiscordChannel channel, string name, string[] args)
        {
            GuildCommand command = _guildCommands[name];
            if (command == null || !command.CanRun(client) || !command.CanHandle(message, channel))
            {
                return false;
            }
            
            if (!command.Plugin.IsLoaded)
            {
                _guildCommands.Remove(name);
                return false;
            }

            if (!client.IsPluginRegistered(command.Plugin))
            {
                return false;
            }

            command.HandleCommand(message, name, args);
            return true;
        }
        
        internal void ProcessPluginCommands(Plugin plugin)
        {
            foreach (MethodInfo method in plugin.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object[] customAttributes = method.GetCustomAttributes(typeof(DirectMessageCommandAttribute), true);
                if (customAttributes.Length != 0)
                {
                    DirectMessageCommandAttribute command = (DirectMessageCommandAttribute)customAttributes[0];
                    if (command.IsLocalized)
                    {
                        DiscordExtension.DiscordCommand.AddDirectMessageLocalizedCommand(command.Name, plugin, method.Name);
                        DiscordExtension.GlobalLogger.Debug($"Adding Localized Direct Message Command {command.Name} Method: {method.Name}");
                    }
                    else
                    {
                        DiscordExtension.DiscordCommand.AddDirectMessageCommand(command.Name, plugin, method.Name);
                        DiscordExtension.GlobalLogger.Debug($"Adding Direct Message Command {command.Name} Method: {method.Name}");
                    }
                }
                
                customAttributes = method.GetCustomAttributes(typeof(GuildCommandAttribute), true);
                if (customAttributes.Length != 0)
                {
                    GuildCommandAttribute command = (GuildCommandAttribute)customAttributes[0];
                    if (command.IsLocalized)
                    {
                        DiscordExtension.DiscordCommand.AddGuildLocalizedCommand(command.Name, plugin, null, method.Name);
                        DiscordExtension.GlobalLogger.Debug($"Adding Localized Guild Command {command.Name} Method: {method.Name}");
                    }
                    else
                    {
                        DiscordExtension.DiscordCommand.AddGuildCommand(command.Name, plugin, null, method.Name);
                        DiscordExtension.GlobalLogger.Debug($"Adding Guild Command {command.Name} Method: {method.Name}");
                    }
                }
            }
        }
    }
}
