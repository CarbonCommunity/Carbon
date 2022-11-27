using Oxide.Core;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Core
{
	public partial class Core_ServerMgr
	{
		/*
		[Hook.AlwaysPatched]
		[Hook("OnServerShutdown"), Hook.Category(Hook.Category.Enum.Server)]
		[Hook.Info("Useful for saving something / etc on server shutdown.")]
		[Hook.Patch(typeof(ServerMgr), "Shutdown")]
		*/

		public class Core_ServerMgr_Shutdown_8a0574c7d2d9420580a5ee90a37de357
		{
			public static Metadata metadata = new Metadata("OnServerShutdown",
				typeof(ServerMgr), "Shutdown", new System.Type[] { });

			static Core_ServerMgr_Shutdown_8a0574c7d2d9420580a5ee90a37de357()
			{
				metadata.SetAlwaysPatch(true);
			}

			public static void Prefix()
			{
				Carbon.Logger.Log($"Saving Carbon plugins & shutting down");

				Interface.Oxide.OnShutdown();

				HookCaller.CallStaticHook("OnServerSave");
				HookCaller.CallStaticHook("OnServerShutdown");

				Community.Runtime.HarmonyProcessor.Clear();
				Community.Runtime.ScriptProcessor.Clear();
			}
		}
	}
}