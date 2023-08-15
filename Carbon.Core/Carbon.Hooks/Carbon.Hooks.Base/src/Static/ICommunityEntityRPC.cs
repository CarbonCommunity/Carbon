using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using API.Hooks;
using Carbon.Client;
using Carbon.Components;
using HarmonyLib;
using Network;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * Copyright (c) 2023 Patrette
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Static
{
	public partial class Static_CommunityEntity
	{
		[HookAttribute.Patch("ICommunityEntityRPC", "ICommunityEntityRPC", typeof(CommunityEntity), "OnRpcMessage", new System.Type[] { typeof(BasePlayer), typeof(uint), typeof(Message) })]
		[HookAttribute.Identifier("fd7e5a1e22824e8ea23a758316ba83d6")]
		[HookAttribute.Options(HookFlags.Hidden | HookFlags.Static | HookFlags.IgnoreChecksum)]

		public class Static_CommunityEntity_OnRPCMessage_fd7e5a1e22824e8ea23a758316ba83d6 : API.Hooks.Patch
		{
			private static bool Prefix(BasePlayer player, uint rpc, Message msg, CommunityEntity __instance, ref bool __result)
			{
				if (RPC.HandleRPCMessage(player, rpc, msg) is bool value)
				{
					__result = value;
					return false;
				}

				return true;
			}
		}
	}
}
