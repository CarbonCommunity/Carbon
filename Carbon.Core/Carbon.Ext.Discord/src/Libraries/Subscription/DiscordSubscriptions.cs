using System;
using System.Collections.Generic;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Plugins;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Logging;
using Oxide.Plugins;

namespace Oxide.Ext.Discord.Libraries.Subscription
{
	/// <summary>
	/// Represents Discord Subscriptions Oxide Library
	/// Allows for plugins to subscribe to discord channels
	/// </summary>
	public class DiscordSubscriptions : Library
	{
		private readonly Hash<Snowflake, Hash<string, DiscordSubscription>> _subscriptions = new Hash<Snowflake, Hash<string, DiscordSubscription>>();

		private readonly ILogger _logger;

		public DiscordSubscriptions()
		{
			_logger = DiscordExtension.GlobalLogger;
		}

		/// <summary>
		/// DiscordSubscriptions Constructor
		/// </summary>
		/// <param name="logger">Logger</param>
		public DiscordSubscriptions(ILogger logger)
		{
			_logger = logger;
		}

		/// <summary>
		/// Returns if any subscriptions have been registered
		/// </summary>
		/// <returns>True if there are any subscriptions; False otherwise</returns>
		public bool HasSubscriptions()
		{
			return _subscriptions.Count != 0;
		}

		/// <summary>
		/// Allows a plugin to add a subscription to a discord channel
		/// </summary>
		/// <param name="plugin">Plugin that is subscribing</param>
		/// <param name="channelId">Channel ID of the channel</param>
		/// <param name="message">Callback with the message that was created in the channel</param>
		/// <exception cref="ArgumentNullException">Exception if plugin or message is null</exception>
		/// <exception cref="ArgumentException">Exception if Channel ID is not valid</exception>
		public void AddChannelSubscription(Plugin plugin, Snowflake channelId, Action<DiscordMessage> message)
		{
			if (plugin == null)
			{
				throw new ArgumentNullException(nameof(plugin));
			}

			if (!channelId.IsValid())
			{
				throw new ArgumentException("Value should be valid.", nameof(channelId));
			}

			if (message == null)
			{
				throw new ArgumentNullException(nameof(message));
			}

			_logger.Debug($"{nameof(DiscordSubscriptions)}.{nameof(AddChannelSubscription)} {plugin.Name} added subscription to channel {channelId.ToString()}");

			Hash<string, DiscordSubscription> channelSubs = _subscriptions[channelId];
			if (channelSubs == null)
			{
				channelSubs = new Hash<string, DiscordSubscription>();
				_subscriptions[channelId] = channelSubs;
			}

			channelSubs[plugin.Name] = new DiscordSubscription(channelId, plugin, message);
		}

		/// <summary>
		/// Removes a subscribed channel for a plugin
		/// </summary>
		/// <param name="plugin">Plugin to remove the subscription for</param>
		/// <param name="channelId">Channel ID to remove</param>
		/// <exception cref="ArgumentNullException">Exception if plugin is null</exception>
		/// <exception cref="ArgumentException">Exception if channel ID is not valid</exception>
		public void RemoveChannelSubscription(Plugin plugin, Snowflake channelId)
		{
			if (plugin == null)
			{
				throw new ArgumentNullException(nameof(plugin));
			}

			if (!channelId.IsValid())
			{
				throw new ArgumentException("Value should be valid.", nameof(channelId));
			}

			Hash<string, DiscordSubscription> pluginSubs = _subscriptions[channelId];
			if (pluginSubs == null)
			{
				return;
			}

			pluginSubs.Remove(plugin.Name);

			if (pluginSubs.Count == 0)
			{
				_subscriptions.Remove(channelId);
			}

			_logger.Debug($"{nameof(DiscordSubscriptions)}.{nameof(RemoveChannelSubscription)} {plugin.Name} removed subscription to channel {channelId.ToString()}");
		}

		internal void OnPluginUnloaded(Plugin plugin)
		{
			RemovePluginSubscriptions(plugin);
		}

		/// <summary>
		/// Remove all subscriptions for a plugin
		/// </summary>
		/// <param name="plugin">Plugin to remove subscriptions for</param>
		/// <exception cref="ArgumentNullException">Exception if plugin is null</exception>
		public void RemovePluginSubscriptions(Plugin plugin)
		{
			if (plugin == null)
			{
				throw new ArgumentNullException(nameof(plugin));
			}

			List<Snowflake> emptySubs = new List<Snowflake>();

			int removed = 0;
			foreach (KeyValuePair<Snowflake, Hash<string, DiscordSubscription>> hash in _subscriptions)
			{
				DiscordSubscription sub = hash.Value[plugin.Name];
				if (sub != null)
				{
					hash.Value.Remove(plugin.Name);
					sub.OnRemoved();
					removed++;
					if (hash.Value.Count == 0)
					{
						emptySubs.Add(hash.Key);
					}
				}
			}

			if (emptySubs.Count != 0)
			{
				for (int i = 0; i < emptySubs.Count; i++)
				{
					Snowflake emptySub = emptySubs[i];
					_subscriptions.Remove(emptySub);
				}
			}

			_logger.Debug($"{nameof(DiscordSubscriptions)}.{nameof(RemovePluginSubscriptions)} Removed {removed.ToString()} subscriptions for plugin {plugin.Name}");
		}

		internal void HandleMessage(DiscordMessage message, DiscordChannel channel, BotClient client)
		{
			RunSubs(_subscriptions[message.ChannelId], message, client);

			if (channel.ParentId != null)
			{
				RunSubs(_subscriptions[channel.ParentId.Value], message, client);
			}
		}

		private void RunSubs(Hash<string, DiscordSubscription> subs, DiscordMessage message, BotClient client)
		{
			if (subs == null)
			{
				return;
			}

			foreach (DiscordSubscription sub in subs.Values)
			{
				if (sub.CanRun(client))
				{
					sub.Invoke(message);
				}
			}
		}
	}
}
