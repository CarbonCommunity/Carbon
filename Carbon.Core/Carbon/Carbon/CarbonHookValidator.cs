///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///

using System.Collections.Generic;
using System.Reflection;
using Carbon.Oxide.Metadata;
using Newtonsoft.Json;

namespace Carbon.Core
{
	public class CarbonHookValidator
	{
		public static List<string> CarbonHooks { get; private set; } = new List<string>(500);
		public static HookPackage OxideHooks { get; private set; }

		public static void Refresh()
		{
			CarbonHooks.Clear();

			foreach (var entry in typeof(CarbonHookValidator).Assembly.GetTypes())
			{
				var hook = entry.GetCustomAttribute<Hook>();
				if (hook == null) continue;
				CarbonHooks.Add(hook.Name);
			}

			CarbonCore.Instance.CorePlugin.webrequest.Enqueue("https://raw.githubusercontent.com/OxideMod/Oxide.Rust/develop/resources/Rust.opj", null, (error, data) =>
			{
				OxideHooks = JsonConvert.DeserializeObject<HookPackage>(data);
			}, null);
		}

		public static bool IsIncompatibleOxideHook(string hook)
		{
			if (CarbonHooks.Contains(hook)) return false;

			if (OxideHooks != null)
			{
				foreach (var manifest in OxideHooks.Manifests)
				{
					foreach (var entry in manifest.Hooks)
					{
						var hookName = (string.IsNullOrEmpty(entry.Hook.BaseHookName) ? entry.Hook.HookName : entry.Hook.BaseHookName).Split(' ')[0];
						if (hookName.Contains("/")) continue;

						if (hookName == hook) return true;
					}
				}
			}

			return false;
		}
	}
}
