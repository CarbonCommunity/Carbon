using System.Threading.Tasks;
using API.Hooks;
using ConVar;
using Oxide.Core;
using UnityEngine;
using static ConVar.Chat;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class BasePlayer_Player
	{
		[HookAttribute.Patch("IOnPlayerChat", "IOnPlayerChat", typeof(ConVar.Chat), "sayAs", new System.Type[] { typeof(Chat.ChatChannel), typeof(ulong), typeof(string), typeof(string), typeof(BasePlayer) })]
		[HookAttribute.Identifier("8828fd118b234c298a04b4dc9b1a6e11")]
		[HookAttribute.Options(HookFlags.Static | HookFlags.Hidden)]

		public class BasePlayer_Player_8828fd118b234c298a04b4dc9b1a6e11 : Patch
		{
			internal const string Slash = "/";

			public static bool Prefix(ChatChannel targetChannel, ulong userId, string username, string message, BasePlayer player, ref ValueTask<bool> __result)
			{
				if (string.IsNullOrEmpty(message)) return true;

				if (message.StartsWith(Slash) && HookCaller.CallStaticHook(2581265021, player, message) is bool hookValue1)
				{
					__result = new ValueTask<bool>(hookValue1);
					return false;
				}
				else if (HookCaller.CallStaticHook(787516416, userId, username, message, targetChannel, player) is bool hookValue2)
				{
					__result = new ValueTask<bool>(hookValue2);
					return false;
				}

				return true;
			}
		}
	}
}
