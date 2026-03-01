using System.Reflection;
using API.Events;
using API.Hooks;
using Carbon.Compat.Patches.Harmony;

namespace Carbon.Compat;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

internal static class HookProcessor
{
	public static void HookClear()
	{
		foreach (IHook hook in Community.Runtime.HookManager.LoadedDynamicHooks)
		{
			if (hook.TargetMethods.Count == 0)
			{
				return;
			}

			MethodBase cache = hook.TargetMethods[0];
			string asmName = cache.DeclaringType.Assembly.GetName().Name;
			string typeName = cache.DeclaringType.FullName;
			string methodName = cache.Name;

			Components.Harmony.PatchInfoEntry patchInfo = Components.Harmony.CurrentPatches.FirstOrDefault(x => x.AssemblyName == asmName && x.TypeName == typeName && x.MethodName == methodName);

			if (patchInfo.Harmony == null)
			{
				continue;
			}

			if ((hook.Options & HookFlags.Patch) != HookFlags.Patch)
			{
				Community.Runtime.HookManager.Unsubscribe(hook.Identifier, "CCL.Static");
			}
		}
	}

    public static void HookReload()
    {
        foreach (IHook hook in Community.Runtime.HookManager.LoadedDynamicHooks)
        {
	        if (hook == null || hook.TargetMethods == null || hook.TargetMethods.Count == 0)
	        {
		        return;
	        }

            MethodBase cache = hook.TargetMethods[0];
            string asmName = cache.DeclaringType.Assembly.GetName().Name;
            string typeName = cache.DeclaringType.FullName;
            string methodName = cache.Name;

            Components.Harmony.PatchInfoEntry patchInfo = Components.Harmony.CurrentPatches.FirstOrDefault(x => x.AssemblyName == asmName && x.TypeName == typeName && x.MethodName == methodName);

            if (patchInfo.Harmony == null)
            {
	            continue;
            }

            if ((hook.Options & HookFlags.Patch) != HookFlags.Patch)
            {
	            Community.Runtime.HookManager.Subscribe(hook.Identifier, "CCL.Static");
            }
        }

        Community.Runtime.HookManager.ForceUpdateHooks();
    }
}
