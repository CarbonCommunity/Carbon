///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using Oxide.Core;

[Hook.AlwaysPatched]
[Hook("OnServerSave"), Hook.Category(Hook.Category.Enum.Server)]
[Hook.Info("Called before the server saves.")]
[Hook.Patch(typeof(SaveRestore), "DoAutomatedSave")]
public class OnServerSave
{
	public static void Prefix(bool AndWait = false)
	{
		Carbon.Logger.Log($"Saving Carbon plugins");
		HookExecutor.CallStaticHook("OnServerSave");
		Interface.Oxide.Permission.SaveData();
	}
}
