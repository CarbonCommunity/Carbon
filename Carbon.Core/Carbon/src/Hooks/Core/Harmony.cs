///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Carbon.Core;
using UnityEngine;

namespace Carbon.Hooks
{
	[CarbonHook.AlwaysPatched, CarbonHook.Hidden]
	[CarbonHook("IOnHarmonyLoad"), CarbonHook.Category(Hook.Category.Enum.Core)]
	[CarbonHook.Patch(typeof(ConVar.Harmony), "Load")]
	public class Harmony_Load
	{
		public const string CARBON_LOADED = nameof(CARBON_LOADED);

		public static bool Prefix(ConsoleSystem.Arg args)
		{
			var mod = args.Args != null && args.Args.Length > 0 ? args.Args[0] : null;

			if (!mod.Equals("carbon", System.StringComparison.OrdinalIgnoreCase) &&
				 !mod.Equals("carbon-unix", System.StringComparison.OrdinalIgnoreCase)) return true;

			if (string.IsNullOrEmpty(mod) ||
				!mod.StartsWith("carbon", System.StringComparison.OrdinalIgnoreCase)) return true;

			var oldMod = PlayerPrefs.GetString(CARBON_LOADED);

			if (oldMod == mod)
			{
				Carbon.Logger.Warn($"An instance of Carbon v{Community.Version} is already loaded.");
				return false;
			}
			else
			{
				Community.Runtime?.Uninitalize();
				HarmonyLoader.TryUnloadMod(oldMod);
				Carbon.Logger.Warn($"Unloaded previous: {oldMod}");
				Community.Runtime = null;
			}

			PlayerPrefs.SetString(CARBON_LOADED, mod);

			return true;
		}
	}

	[CarbonHook.AlwaysPatched, CarbonHook.Hidden]
	[CarbonHook("IOnHarmonyUnload"), CarbonHook.Category(Hook.Category.Enum.Core)]
	[CarbonHook.Patch(typeof(ConVar.Harmony), "Unload")]
	public class Harmony_Unload
	{
		public static bool Prefix(ConsoleSystem.Arg args)
		{
			var mod = args.Args != null && args.Args.Length > 0 ? args.Args[0] : null;

			if (string.IsNullOrEmpty(mod)) return true;

			if (mod.Equals("carbon", System.StringComparison.OrdinalIgnoreCase) ||
				 mod.Equals("carbon-unix", System.StringComparison.OrdinalIgnoreCase))
				mod = CarbonDefines.Name;

			if (!mod.StartsWith("carbon", System.StringComparison.OrdinalIgnoreCase)) return true;

			PlayerPrefs.SetString(Harmony_Load.CARBON_LOADED, string.Empty);
			Community.Runtime?.Uninitalize();
			Community.Runtime = null;

			HarmonyLoader.TryUnloadMod(mod);
			return false;
		}
	}
}
