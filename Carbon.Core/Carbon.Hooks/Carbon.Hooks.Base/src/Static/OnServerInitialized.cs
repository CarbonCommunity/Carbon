using System;
using API.Events;
using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Static
{
	public partial class Static_ServerMgr
	{
		[HookAttribute.Patch("OnServerInitialized", "OnServerInitialized", typeof(ServerMgr), "OpenConnection", new System.Type[] { })]
		[HookAttribute.Identifier("b91c13017e4a43fcb2d81244efd8e5b6")]
		[HookAttribute.Options(HookFlags.Static | HookFlags.IgnoreChecksum)]

		// Called after the server startup has been completed and is awaiting connections.
		// Also called for plugins that are hotloaded while the server is already started running.

		public class Static_ServerMgr_OpenConnection_b91c13017e4a43fcb2d81244efd8e5b6 : Patch
		{
			public static void Postfix()
				=> Events.Trigger(CarbonEvent.OnServerInitialized, EventArgs.Empty);
		}
	}
}
