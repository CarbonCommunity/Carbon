using System.Collections.Immutable;
using System.Globalization;
using Carbon.Compat.Patches;
using Carbon.Compat.Patches.Harmony;
using JetBrains.Annotations;
using UnityEngine;

namespace Carbon.Compat.Converters;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

[UsedImplicitly]
public class HarmonyConverter : BaseConverter
{
    /// <summary>
    /// Extra reference/IL patches applied right after the Harmony type remap. Packages use it to
    /// support HarmonyMods built against other frameworks.
    /// </summary>
    public static List<IAssemblyPatch> AdditionalPatches { get; } = new();

    public override ImmutableList<IAssemblyPatch> Patches => new List<IAssemblyPatch>()
    {
	    // type ref
	    new HarmonyTypeRef(),
    }
    .Concat(AdditionalPatches)
    .Concat(new IAssemblyPatch[]
    {
	    // harmony
	    new HarmonyPatchProcessor(),

	    //common
	    new ReflectionFlagsPatch(),
	    new AssemblyVersionPatch(),

	    //debug
	    new AssemblyDebugPatch()
    }).ToImmutableList();

    public override string Name => "HarmonyMod";
}
