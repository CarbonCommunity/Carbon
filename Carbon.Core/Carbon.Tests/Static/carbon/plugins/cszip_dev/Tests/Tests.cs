using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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

		Integrations.EnqueueBed(Integrations.Get(nameof(Cleanup), typeof(Cleanup), new Cleanup(), channel: 5));

		Logger.Log(string.Empty);

		Integrations.Run(delay: 0.1f, -1);
		Integrations.OnFatalTestFailure += OnFatalFailure;
	}

	private void OnFatalFailure()
	{
		Integrations.EnqueueBed(Integrations.Get(nameof(Cleanup), typeof(Cleanup), new Cleanup(), channel: 5));
		Integrations.Run(delay: 0.1f, 5);
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

#if !UNIX
	[DllImport("kernel32.dll")]
	private static extern void ExitProcess(uint uExitCode);
#else
    [DllImport("libc")]
    private static extern void exit(int status);
#endif

	private class Cleanup
	{
		[Integrations.Test(Channel = 5)]
		public void quit(Integrations.Test test)
		{
			test.Log($"Quitting - {Integrations.ExitCode}");
			ToggleAllHookDebugging(false);

			Rust.Application.isQuitting = true;
			Network.Net.sv?.Stop(nameof (quit));
			UnityEngine.Application.Quit((int)Integrations.ExitCode);
#if WIN
			ExitProcess((uint)Integrations.ExitCode);
#else
			exit((int)Integrations.ExitCode);
#endif
		}
	}
}
