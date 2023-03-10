/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2022 Oxide, uMod
 * All rights reserved.
 *
 */

using System;
using Oxide.Core.Libraries.Covalence;
using Oxide.Ext.Discord.Entities.Users;

namespace Oxide.Ext.Discord.Extensions
{
	// Token: 0x02000031 RID: 49
	public static class DiscordUserExt
	{
		// Token: 0x060001A0 RID: 416 RVA: 0x0000D628 File Offset: 0x0000B828
		public static void SendChatMessage(this DiscordUser user, string message)
		{
			IPlayer player = user.Player;
			bool flag = player != null && player.IsConnected;
			if (flag)
			{
				player.Message(message);
			}
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x0000D658 File Offset: 0x0000B858
		public static void SendChatMessage(this DiscordUser user, string message, string prefix, params object[] args)
		{
			IPlayer player = user.Player;
			bool flag = player != null && player.IsConnected;
			if (flag)
			{
				player.Message(message, prefix, args);
			}
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000D68C File Offset: 0x0000B88C
		public static bool HasPermission(this DiscordUser user, string permission)
		{
			IPlayer player = user.Player;
			return player != null && player.HasPermission(permission);
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000D6B4 File Offset: 0x0000B8B4
		public static bool IsLinked(this DiscordUser user)
		{
			return DiscordExtension.DiscordLink.IsLinked(user.Id);
		}
	}
}
