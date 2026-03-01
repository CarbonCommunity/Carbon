using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Carbon.Compat.Patches.Harmony;
using HarmonyLib;
using JetBrains.Annotations;

namespace Carbon.Compat.Lib;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class HarmonyCompat
{
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static DynamicMethod InstancePatchCompat(Harmony instance, MethodBase original, HarmonyMethod prefix = null, HarmonyMethod postfix = null, HarmonyMethod transpiler = null)
	{
		MethodBase calling = new StackTrace().GetFrame(1).GetMethod();
		HarmonyPatchProcessor.RegisterPatch(original, $"{calling.DeclaringType.Assembly.GetName().Name} - {calling}", instance);
		HookProcessor.HookReload();
		instance.Patch(original, prefix, postfix, transpiler);
		return null;
	}

	internal static HashSet<Type> typeCache = new();

	public static void PatchProcessorCompat(Harmony instance, Type type, HarmonyMethod attributes)
	{
		if (typeCache.Contains(type))
		{
			return;
		}

		typeCache.Add(type);

		MethodInfo[] methods = type.GetMethods();
		MethodInfo postfix = methods.FirstOrDefault(x => x.GetCustomAttributes(typeof(HarmonyPostfix), false).Length > 0);
		MethodInfo prefix = methods.FirstOrDefault(x => x.GetCustomAttributes(typeof(HarmonyPrefix), false).Length > 0);
		MethodInfo transpiler = methods.FirstOrDefault(x => x.GetCustomAttributes(typeof(HarmonyTranspiler), false).Length > 0);
		MethodInfo patchTargetMethod = methods.FirstOrDefault(x =>
			x.GetCustomAttributes(typeof(HarmonyTargetMethods), false).Length > 0 ||
			x.GetCustomAttributes(typeof(HarmonyTargetMethod), false).Length > 0) ?? throw new NullReferenceException($"Failed to find target method in {type.FullName}");
		IEnumerable<MethodBase> methodsToPatch = null;
		MethodBase single = null;

		if (patchTargetMethod.ReturnType == typeof(IEnumerable<MethodBase>))
		{
			methodsToPatch = ((IEnumerable<MethodBase>)patchTargetMethod.Invoke(null,
				patchTargetMethod.GetParameters().Length > 0 ? [ null ] : []));
		}
		else if (patchTargetMethod.ReturnType == typeof(MethodBase))
		{
			single = (MethodBase)patchTargetMethod.Invoke(null,
				patchTargetMethod.GetParameters().Length > 0 ? [ null ] : []);
		}
		else
		{
			return;
		}

		void ProcessType(MethodBase original, bool pregen)
		{
			if (pregen)
			{
				var assembly = type.Assembly.GetName().Name;
				HarmonyPatchProcessor.RegisterPatch(assembly + ".dll", original.DeclaringType.Assembly.GetName().Name, original.Name,
					original.DeclaringType.FullName, $"{assembly} - {type.FullName}", instance);

				return;
			}

			try
			{
				if (!original.IsDeclaredMember())
				{
					original = original.GetDeclaredMember();
				}

				PatchProcessor patcher = new PatchProcessor(instance, original);

				if (postfix != null)
				{
					patcher.AddPostfix(postfix);
				}

				if (prefix != null)
				{
					patcher.AddPrefix(prefix);
				}

				if (transpiler != null)
				{
					patcher.AddTranspiler(transpiler);
				}

				patcher.Patch();
			}
			catch (Exception ex)
			{
				Logger.Error($"[HarmonyCompat] Failed to patch '{original.Name}'", ex);
			}
		}

		bool pregen = true;

		loop:

		if (methodsToPatch != null)
		{
			foreach (MethodBase original in methodsToPatch)
			{
				ProcessType(original, pregen);
			}
		}
		else if (single != null)
		{
			ProcessType(single, pregen);
		}

		if (pregen)
		{
			pregen = false;
			HookProcessor.HookReload();
			goto loop;
		}
	}
}
