using API.Events;
using Facepunch;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	internal static bool _isPlayerTakingDamage = false;
	internal static readonly string[] _emptyStringArray = new string[0];

	internal object IOnServerInitialized(bool inited)
	{
		if (!Community.IsServerInitialized)
		{
			Community.IsServerInitialized = true;

			Analytics.on_server_initialized();
		}

		if (!ConVar.Server.autoUploadMap)
		{
			Community.Runtime.MarkServerInitialized(true);
		}

		Community.Runtime.Events.Trigger(CarbonEvent.OnServerInitialized, EventArgs.Empty);
		return null;
	}
	internal static object IOnServerInitialized()
	{
		return Community.Runtime.Core.IOnServerInitialized(true);
	}
	internal static object IOnServerShutdown()
	{
		Logger.Log($"Saving plugin configuration and data..");

		var temp = Pool.Get<List<BaseHookable>>();
		temp.AddRange(Community.Runtime.ModuleProcessor.Modules);

		foreach (var module in temp)
		{
			if (module is BaseModule m)
			{
				try
				{
					m.Shutdown();
				}
				catch (Exception ex)
				{
					Logger.Error($"Failed shutting down module '{m.Name} v{m.Version}'", ex);
				}
			}
		}

		Pool.FreeUnmanaged(ref temp);

		// OnServerShutdown
		HookCaller.CallStaticHook(2414711472);

		// OnServerSave
		HookCaller.CallStaticHook(2396958305);

		Logger.Log($"Shutting down Carbon..");
		Interface.Oxide.OnShutdown();

		var plugins = Pool.Get<List<RustPlugin>>();
		ModLoader.Packages.GetAllHookables(plugins);
		foreach (var plugin in plugins)
		{
			ModLoader.UninitializePlugin(plugin, unloadDependantPlugins: false);
		}
		Pool.FreeUnmanaged(ref plugins);

		return null;
	}
}
