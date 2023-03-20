using System;
using System.IO;
using System.Linq;
using API.Events;
using Carbon.Oxide.Metadata;
using Newtonsoft.Json;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Core;

public class HookValidator
{
	public static HookPackage OxideHooks { get; private set; }

	public static void Initialize()
	{
		Community.Runtime.Events.Subscribe(
			CarbonEvent.HooksInstalled, x => HookValidator.Refresh());
	}

	public static async void Refresh()
	{
		string url = "https://raw.githubusercontent.com/OxideMod/Oxide.Rust/develop/resources/Rust.opj";
		byte[] buffer = await Community.Runtime.Downloader.Download(url);

		if (buffer is { Length: > 0 })
		{
			Logger.Warn($"Downloaded [{Path.GetFileName(url)}], processing {buffer.Length} bytes from memory");
			string json = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);

			OxideHooks = JsonConvert.DeserializeObject<HookPackage>(json);
			Logger.Debug($"Refreshed {OxideHooksCount} oxide hooks.");

			Community.Runtime.Events.Trigger(
				CarbonEvent.HookValidatorRefreshed, EventArgs.Empty);
		}
	}

	static readonly string[] IgnoredInternalHooks = new string[]
	{
		"OnPlayerDisconnected",
		"OnPlayerSleepEnded",
		"OnServerMessage",
		"OnPlayerRespawned",
		"OnPlayerDeath",
		"CanCombineDroppedItem",
		"OnEntityTakeDamage",
		"OnPluginLoaded",
		"OnPluginUnloaded",
		"OnEntityKill",
		"OnEntityDeath",
		"OnEntitySpawned",
		"CanClientLogin",
		"CanUserLogin",
		"OnUserApprove",
		"OnUserApproved",
		"OnPlayerChat",
		"OnUserChat",
		"OnPlayerOfflineChat"
	};

	public static bool IsIncompatibleOxideHook(string hook)
	{
		if (IgnoredInternalHooks.Contains(hook))
		{
			return false;
		}

		if (Community.Runtime.HookManager.LoadedStaticHooks.Any(x => x.HookName == hook) ||
			Community.Runtime.HookManager.LoadedDynamicHooks.Any(x => x.HookName == hook)) return false;

		if (OxideHooks != null)
		{
			foreach (var manifest in OxideHooks.Manifests)
			{
				foreach (var entry in manifest.Hooks)
				{
					var hookName = entry.Hook.HookName.Split(' ')[0];
					if (hookName.Contains("/") || hookName != hook) continue;
					return true;
				}
			}
		}

		return false;
	}

	private static int OxideHooksCount
	{
		get
		{
			if (OxideHooks == null) return 0;

			int count = 0;
			foreach (var manifest in OxideHooks.Manifests)
				foreach (var entry in manifest.Hooks)
					count++;
			return count;
		}
	}
}
