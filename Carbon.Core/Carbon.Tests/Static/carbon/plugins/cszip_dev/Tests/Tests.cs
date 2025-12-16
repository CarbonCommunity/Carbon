using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Carbon.Core;
using Carbon.Test;

namespace Carbon.Plugins;

[Info("Tests", "Carbon Community LTD", "1.0.0")]
public partial class Tests : CarbonPlugin
{
	public static Tests singleton;

	private void Init()
	{
		singleton = this;

		ToggleAllHookDebugging(false);
	}
	private void OnServerInitialized()
	{
		foreach(var type in HookableType.GetNestedTypes(BindingFlags.Public))
		{
			Integrations.EnqueueBed(Integrations.Get(type.Name, type, Activator.CreateInstance(type)));
		}

		Integrations.EnqueueBed(Integrations.Get(nameof(Cleanup), typeof(Cleanup), new Cleanup()));

		Logger.Log(string.Empty);

		Integrations.Run(delay: 0.1f, -1);
	}
	private static void ToggleAllHookDebugging(bool wants)
	{
		foreach (var plugin in ModLoader.Packages.SelectMany(package => package.Plugins))
		{
			plugin.HookPool.EnableDebugging(wants);
		}
		foreach (var module in Community.Runtime.ModuleProcessor.Modules)
		{
			module.HookPool.EnableDebugging(wants);
		}
	}

	private class Cleanup
	{
		[Integrations.Test]
		public void quit(Integrations.Test test)
		{
			test.Log("Quitting");
			Logger.Log(string.Empty);
			ToggleAllHookDebugging(false);

			SingletonComponent<ServerMgr>.Instance?.Shutdown();
			Rust.Application.isQuitting = true;
			Network.Net.sv?.Stop(nameof (quit));
			Process.GetCurrentProcess().Kill();
			Rust.Application.Quit();
		}
	}
}
