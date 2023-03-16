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
	[HookAttribute.Patch("IPostNetWritePacketID", "IPostNetWritePacketID", typeof(NetWrite), "PacketID", new System.Type[] { typeof(Message.Type) })]
	[HookAttribute.Identifier("4585b3ead7dd41e0ba54198b8efdfb27")]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class NetWrite_PacketID_4585b3ead7dd41e0ba54198b8efdfb27 : API.Hooks.Patch
	{
		public static void Postfix(Message.Type val)
		{
			MetricsLogger.Instance?.OnNetWritePacketID(val);
		}
	}
}
