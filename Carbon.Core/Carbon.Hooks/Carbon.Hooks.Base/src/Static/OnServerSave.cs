using System;
using API.Events;
using API.Hooks;
using Oxide.Core;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Static
{
	public partial class Static_SaveRestore
	{
		[HookAttribute.Patch("OnServerSave", typeof(SaveRestore), "DoAutomatedSave", new System.Type[] { typeof(bool) })]
		[HookAttribute.Identifier("eb9f4139698447f594d20fb698c1eb15")]
		[HookAttribute.Options(HookFlags.Static | HookFlags.IgnoreChecksum)]

		// Called before the server saves.

		public class Static_SaveRestore_eb9f4139698447f594d20fb698c1eb15 : API.Hooks.Patch
		{
			public static void Prefix()
			{
				Events.Trigger(CarbonEvent.OnServerSave, EventArgs.Empty);

				Logger.Log($"Saving plugin configuration and data..");
				HookCaller.CallStaticHook("OnServerSave");

				Logger.Log($"Saving Carbon state..");
				Interface.Oxide.Permission.SaveData();
			}
		}
	}
}
