using API.Hooks;
using Network;
using Newtonsoft.Json;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public partial class RustServerMetricsModule
{
	[HookAttribute.Patch("IPostNetWriteSend", "IPostNetWriteSend", typeof(NetWrite), "Send", new System.Type[] { typeof(SendInfo) })]
	[HookAttribute.Identifier("5b33b80c737148448152d5686baa7a4d")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class NetWrite_PacketID_5b33b80c737148448152d5686baa7a4d : API.Hooks.Patch
	{
		public static void Postfix(SendInfo info, NetWrite __instance)
		{
			MetricsLogger.Instance?.OnNetWriteSend(__instance, info);
		}
	}
}
