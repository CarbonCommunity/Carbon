using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Hooks;
using API.Logger;
using Carbon.Core;
using Carbon.Extensions;
using Carbon.Test;
using ConVar;
using Facepunch;
using JetBrains.Annotations;
using UnityEngine;
using Application = Rust.Application;
using ILogger = API.Logger.ILogger;

namespace Carbon.Plugins;

[Info(nameof(TestRunner), "Carbon Community LTD.", "1.0.0")]
public class TestRunner : CarbonPlugin
{
	private const string StName = nameof(TestRunner);
	private static TestRunner _instance = null!;
	private static bool _developing = false;
	private static string _logIdentifier = "";

	[UsedImplicitly]
	private async Task OnServerInitialized()
	{
		try
		{
			_instance = this;
			_logIdentifier = CommandLine.GetSwitch("-testrunner-identifier", "");

			if (!_developing)
			{
				ServerMgr.Instance.StartCoroutine(ForceShutdownCoroutine());
			}

			ToggleAllHookDebugging(false);

			Integrations.Logger = new CustomLogger();

			Log("STARTED");

			RunTests();

			while (Integrations.IsRunning())
			{
				await Task.Delay(200);
			}
		}
		catch (Exception e)
		{
			Log($"FAILED {e}");
		}
		finally
		{
			Log("ENDED");
			if (!_developing)
			{
				ForceShutdown();
			}
		}
	}

	private IEnumerator ForceShutdownCoroutine()
	{
		const int timeout = 2 * 60;
		Log($"Will forcefully shutdown in: {timeout}");
		yield return new WaitForSeconds(timeout);
		ForceShutdown();
	}

	private void ForceShutdown()
	{
		try
		{
			Global.quit(null);
		}
		finally
		{
			Application.isQuitting = true;
			Process.GetCurrentProcess().Kill();
			Application.Quit();
		}
	}

	private void RunTests()
	{
		var type = typeof(TestRunner);
		Integrations.EnqueueBed(Integrations.Get(type.Name, type, this));

		Integrations.Run(0.1f, -1);
	}

	[UsedImplicitly]
	[Integrations.Test.Assert(Timeout = 6_000)]
	public async Task RunTest_AllHooksPatch(Integrations.Test.Assert test)
	{
		var hookManager = Community.Runtime.HookManager;

		PrintOutHooksInfo();

		foreach (var hook in hookManager.LoadedDynamicHooks)
		{
			hookManager.Subscribe(hook.HookName, StName);
		}

		foreach (var hook in hookManager.LoadedPatches)
		{
			hookManager.Subscribe(hook.HookName, StName);
		}

		foreach (var hook in hookManager.LoadedStaticHooks)
		{
			hookManager.Subscribe(hook.HookName, StName);
		}

		hookManager.ForceUpdateHooks();

		test.Log("");

		await AsyncEx.WaitForSeconds(1);

		PrintOutHooksInfo();

		test.IsTrue(hookManager.LoadedPatches.Count() == hookManager.InstalledPatches.Count(),
			"loaded patches == all patches");
		test.IsTrue(hookManager.LoadedDynamicHooks.Count() == hookManager.InstalledDynamicHooks.Count(),
			"loaded dynamic hooks == all dynamic hooks");
		test.IsTrue(hookManager.LoadedStaticHooks.Count() == hookManager.InstalledStaticHooks.Count(),
			"loaded static hooks == all static hooks");

		test.Complete();

		return;

		void PrintOutHooksInfo()
		{
			test.Log($"{nameof(IPatchManager.LoadedPatches)}: {hookManager.LoadedPatches.Count()}");
			test.Log($"{nameof(IPatchManager.InstalledPatches)}: {hookManager.InstalledPatches.Count()}");
			test.Log($"{nameof(IPatchManager.LoadedDynamicHooks)}: {hookManager.LoadedDynamicHooks.Count()}");
			test.Log($"{nameof(IPatchManager.InstalledDynamicHooks)}: {hookManager.InstalledDynamicHooks.Count()}");
			test.Log($"{nameof(IPatchManager.LoadedStaticHooks)}: {hookManager.LoadedStaticHooks.Count()}");
			test.Log($"{nameof(IPatchManager.InstalledStaticHooks)}: {hookManager.InstalledStaticHooks.Count()}");
		}
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

	private void Log(string msg)
	{
		PrintWarning($"{_logIdentifier}{(_logIdentifier != "" ? " " : "")}{msg}");
	}

	private class CustomLogger : ILogger
	{
		public void Console(string message, Severity severity = Severity.Notice, Exception? exception = null)
		{
			_instance.Log(message);
		}
	}
}
