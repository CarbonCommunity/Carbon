using System.Reflection;
using HarmonyLib;
using Carbon.Startup;

namespace Carbon.Startup.Patches;

[HarmonyPatchCategory("location")]
[HarmonyPatch("System.Reflection.RuntimeAssembly", "Location", MethodType.Getter)]
public class AssemblyLocationPatch
{
	public static void Postfix(Assembly __instance, ref string __result)
	{
		if (Carbon.Publicizer.Patch.PatchMapping.TryGetValue(__instance, out var path))
		{
			__result = path;
		}
	}
}
