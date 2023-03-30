using System;
using System.Collections.Generic;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Libraries.Command
{
	internal class GuildCommand : BaseCommand
    {
        private readonly List<Snowflake> _allowedChannels;

        public GuildCommand(string name, Plugin plugin, List<Snowflake> allowedChannels, Action<DiscordMessage, string, string[]> callback) : base(name, plugin, callback)
        {
            _allowedChannels = allowedChannels ?? new List<Snowflake>();
        }

        public override bool CanHandle(DiscordMessage message, DiscordChannel channel)
        {
            if (!message.GuildId.HasValue)
            {
                return false;
            }

            if (_allowedChannels.Count != 0 && !_allowedChannels.Contains(channel.Id) && (!channel.ParentId.HasValue || !_allowedChannels.Contains(channel.ParentId.Value)))
            {
                return false;
            }

            return true;
        }
    }
}
