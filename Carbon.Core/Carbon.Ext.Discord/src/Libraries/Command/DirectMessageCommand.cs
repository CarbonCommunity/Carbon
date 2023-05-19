using System;
using Oxide.Core.Plugins;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Messages;

namespace Oxide.Ext.Discord.Libraries.Command
{
	internal class DirectMessageCommand : BaseCommand
	{
		internal DirectMessageCommand(string name, Plugin plugin, Action<DiscordMessage, string, string[]> callback) : base(name, plugin, callback)
		{

		}

		public override bool CanHandle(DiscordMessage message, DiscordChannel channel)
		{
			return !message.GuildId.HasValue;
		}
	}
}
