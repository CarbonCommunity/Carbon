///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;

[Hook.AlwaysPatched]
[Hook.Parameter("booted", typeof(bool), true)]
[Hook.Info("Called after the server startup has been completed and is awaiting connections.")]
[Hook.Info("Also called for plugins that are hotloaded while the server is already started running.")]
[Hook.Info("Boolean parameter, false if called on plugin hotload and true if called on server initialization.")]
[Hook("OnServerInitialized"), Hook.Category(Hook.Category.Enum.Server)]
[Hook.Patch(typeof(ServerMgr), "OpenConnection")]
public class OnServerInitialized
{
	public static void Postfix()
	{
		CarbonLoader.OnPluginProcessFinished();
	}
}
