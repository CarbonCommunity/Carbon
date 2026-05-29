using System.IO;
using System.Reflection;
using Carbon.Components;
using Carbon.Profiler;
using HarmonyLib;
using UnityEngine;

namespace Carbon;

public class Patches
{
	[HarmonyPatch(typeof(Bootstrap), nameof(Bootstrap.Init_Tier0))]
	public static class Bootstrap_Init_Tier0
	{
		public static void Postfix()
		{
			if (HarmonyProfiler.IsCarbonInstalled)
			{
				return;
			}
			HarmonyProfiler.InstallCommands();
		}
	}

	[HarmonyPatch("OxideMod", "PluginLoaded")]
	public static class OxideMod_PluginLoaded
	{
		public static bool Prepare()
		{
			return HarmonyProfiler.IsOxideInstalled;
		}

		public static void Prefix(object plugin)
		{
			var type = plugin.GetType();
			var fileName = Path.GetFileNameWithoutExtension((string)type.GetProperty("Filename").GetValue(plugin));
			if (string.IsNullOrEmpty(fileName))
			{
				return;
			}
			MonoProfiler.TryStartProfileFor(MonoProfilerConfig.ProfileTypes.Plugin, type.Assembly, fileName, true);
			Debug.Log($"MonoProfiler.TryStartProfileFor Plugin: {fileName}");
		}
	}

	[HarmonyPatch]
	public static class Extension_Constructor
	{
		public static bool Prepare()
		{
			return HarmonyProfiler.IsOxideInstalled;
		}

		static MethodBase TargetMethod()
		{
			return AccessTools.TypeByName("Oxide.Core.Extensions.Extension").GetConstructors()[0];
		}

		public static void Prefix(object manager, object __instance)
		{
			var type = __instance.GetType();

			HarmonyProfiler.Runner.Invoke(() =>
			{
				var fileName = Path.GetFileNameWithoutExtension((string)type.GetProperty("Filename").GetValue(__instance));
				if (string.IsNullOrEmpty(fileName))
				{
					return;
				}
				MonoProfiler.TryStartProfileFor(MonoProfilerConfig.ProfileTypes.Extension, type.Assembly, fileName);
				Debug.Log($"MonoProfiler.TryStartProfileFor Extension: {fileName}");
			}, 0.1f);
		}
	}
}
