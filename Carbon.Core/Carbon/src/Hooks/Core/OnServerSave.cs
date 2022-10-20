///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon;
using Carbon.Core;
using Oxide.Core;

namespace Carbon.Hooks
{
	[Hook.AlwaysPatched]
	[Hook("OnServerSave"), Hook.Category(Hook.Category.Enum.Server)]
	[Hook.Info("Called before the server saves.")]
	[Hook.Patch(typeof(SaveRestore), "DoAutomatedSave")]
	public class OnServerSave
	{
		public static void Prefix(bool AndWait = false)
		{
			Carbon.Logger.Log($"Saving Carbon plugins");
			HookCaller.CallStaticHook("OnServerSave");
			Interface.Oxide.Permission.SaveData();
		}
	}
}
