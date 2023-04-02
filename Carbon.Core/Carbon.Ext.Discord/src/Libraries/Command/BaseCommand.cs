using System;
using Oxide.Core;
using Oxide.Core.Plugins;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Libraries.Command
{
    /// <summary>
    /// Sourced from Command.cs of OxideMod (https://github.com/OxideMod/Oxide.Rust/blob/develop/src/Libraries/Command.cs#L76)
    /// </summary>
    internal class BaseCommand
    {
        internal readonly string Name;
        internal Plugin Plugin;
        private readonly Action<DiscordMessage, string, string[]> _callback;

        protected BaseCommand(string name, Plugin plugin, Action<DiscordMessage, string, string[]> callback)
        {
            Name = name;
            Plugin = plugin;
            _callback = callback;
        }
        
        public void HandleCommand(DiscordMessage message, string name, string[] args)
        {
            Interface.Oxide.NextTick(() =>
            {
                try
                {
                    Plugin.TrackStart();
                    _callback.Invoke(message, name, args);
                    Plugin.TrackEnd();
                }
                catch(Exception ex)
                {
                    DiscordExtension.GlobalLogger.Exception($"An exception occured in discord command {name} for plugin {Plugin?.Name}", ex);   
                }
            });
        }

        /// <summary>
        /// Returns if a command can run.
        /// They can only run for the client that they were created for.
        /// </summary>
        /// <param name="client">Client to compare against</param>
        /// <returns>True if same bot client; false otherwise</returns>
        public bool CanRun(BotClient client)
        {
            return client != null && DiscordClient.Clients[Plugin.Name]?.Bot == client;
        }

        public virtual bool CanHandle(DiscordMessage message, DiscordChannel channel) => true;

        internal void OnRemoved()
        {
            Plugin = null;
        }
    }
}
