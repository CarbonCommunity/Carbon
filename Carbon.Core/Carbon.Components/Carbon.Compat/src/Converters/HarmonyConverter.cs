using System.Collections.Immutable;
using System.Globalization;
using Carbon.Compat.Patches;
using Carbon.Compat.Patches.Harmony;
using Carbon.Compat.Patches.Oxide;
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
    public override ImmutableList<IAssemblyPatch> Patches => _patches;

    private readonly ImmutableList<IAssemblyPatch> _patches = new List<IAssemblyPatch>()
    {
	    // type ref
	    new HarmonyTypeRef(),
	    new OxideTypeRef(),

	    // il switch
	    new OxideILSwitch(),

	    // harmony
	    new HarmonyPatchProcessor(),

	    //common
	    new ReflectionFlagsPatch(),
	    new AssemblyVersionPatch(),

	    //debug
	    new AssemblyDebugPatch()
    }.ToImmutableList();

    public override string Name => "HarmonyMod";
}
