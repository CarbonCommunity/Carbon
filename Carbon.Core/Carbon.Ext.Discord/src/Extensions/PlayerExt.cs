/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using System.Collections.Generic;
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Entities;
using Oxide.Ext.Discord.Entities.Channels;
using Oxide.Ext.Discord.Entities.Guilds;
using Oxide.Ext.Discord.Entities.Messages;
using Oxide.Ext.Discord.Entities.Messages.Embeds;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Extensions
{
	// Token: 0x02000034 RID: 52
	public static class PlayerExt
	{
		// Token: 0x060001A6 RID: 422 RVA: 0x0000D804 File Offset: 0x0000BA04
		public static void SendDiscordMessage(this IPlayer player, DiscordClient client, string message)
		{
			MessageCreate message2 = new MessageCreate
			{
				Content = message
			};
			player.SendDiscordMessage(client, message2);
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000D82C File Offset: 0x0000BA2C
		public static void SendDiscordMessage(this IPlayer player, DiscordClient client, DiscordEmbed embed)
		{
			MessageCreate message = new MessageCreate
			{
				Embeds = new List<DiscordEmbed>
				{
					embed
				}
			};
			player.SendDiscordMessage(client, message);
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000D860 File Offset: 0x0000BA60
		public static void SendDiscordMessage(this IPlayer player, DiscordClient client, List<DiscordEmbed> embeds)
		{
			MessageCreate message = new MessageCreate
			{
				Embeds = embeds
			};
			player.SendDiscordMessage(client, message);
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000D885 File Offset: 0x0000BA85
		public static void SendDiscordMessage(this IPlayer player, DiscordClient client, MessageCreate message)
		{
			PlayerExt.SendMessage(client, player.GetDiscordUserId(), message);
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000D898 File Offset: 0x0000BA98
		private static void SendMessage(DiscordClient client, Snowflake? id, MessageCreate message)
		{
			bool flag = id == null || !id.Value.IsValid();
			if (!flag)
			{
				DiscordChannel discordChannel = client.Bot.DirectMessagesByUserId[id.Value];
				bool flag2 = discordChannel != null;
				if (flag2)
				{
					discordChannel.CreateMessage(client, message, null, null);
				}
				else
				{
					DiscordUser.CreateDirectMessageChannel(client, id.Value, delegate(DiscordChannel newChannel)
					{
						newChannel.CreateMessage(client, message, null, null);
					}, null);
				}
			}
		}

		// Token: 0x060001AB RID: 427 RVA: 0x0000D940 File Offset: 0x0000BB40
		public static bool IsLinked(this IPlayer player)
		{
			return DiscordExtension.DiscordLink.IsLinked(player.Id);
		}

		// Token: 0x060001AC RID: 428 RVA: 0x0000D964 File Offset: 0x0000BB64
		public static Snowflake? GetDiscordUserId(this IPlayer player)
		{
			return DiscordExtension.DiscordLink.GetDiscordId(player);
		}

		// Token: 0x060001AD RID: 429 RVA: 0x0000D984 File Offset: 0x0000BB84
		public static DiscordUser GetDiscordUser(this IPlayer player)
		{
			return DiscordExtension.DiscordLink.GetDiscordUser(player);
		}

		// Token: 0x060001AE RID: 430 RVA: 0x0000D9A4 File Offset: 0x0000BBA4
		public static GuildMember GetGuildMember(this IPlayer player, DiscordGuild guild)
		{
			return DiscordExtension.DiscordLink.GetLinkedMember(player, guild);
		}
	}
}
