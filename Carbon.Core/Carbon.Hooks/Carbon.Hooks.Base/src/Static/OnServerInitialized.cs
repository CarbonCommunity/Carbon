﻿using System;
using API.Contracts;
using API.Hooks;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Static
{
	public partial class Static_ServerMgr
	{
		[HookAttribute.Patch("OnServerInitialized", typeof(ServerMgr), "OpenConnection", new System.Type[] { })]
		[HookAttribute.Identifier("b91c13017e4a43fcb2d81244efd8e5b6")]
		[HookAttribute.Options(HookFlags.Static | HookFlags.IgnoreChecksum)]

		// Called after the server startup has been completed and is awaiting connections.
		// Also called for plugins that are hotloaded while the server is already started running.

		public class Static_ServerMgr_OpenConnection_b91c13017e4a43fcb2d81244efd8e5b6
		{
			private static readonly Lazy<IEventManager> _events = new(() =>
			{
				GameObject gameObject = GameObject.Find("Carbon");
				return gameObject?.GetComponent<IEventManager>();
			});

			public static void Postfix()
			{
				_events.Value.Trigger(
					API.Events.CarbonEvent.OnServerInitialized, EventArgs.Empty);
			}
		}
	}
}