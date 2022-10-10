///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

[Hook.AlwaysPatched, Hook.Hidden]
[Hook("IOnServerInit"), Hook.Category(Hook.Category.Enum.Core)]
[Hook.Patch(typeof(ServerMgr), "OpenConnection")]
public class OnServerInitialized
{
	public static void Postfix()
	{
		CarbonLoader.OnPluginProcessFinished();
	}
}
