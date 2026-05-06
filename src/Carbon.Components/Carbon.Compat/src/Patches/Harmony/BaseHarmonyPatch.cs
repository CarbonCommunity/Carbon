using Carbon.Compat.Converters;

namespace Carbon.Compat.Patches.Harmony;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

public abstract class BaseHarmonyPatch : IAssemblyPatch
{
    public const string HarmonyASM = "0Harmony";
    public const string Harmony1NS = HarmonyStr;
    public const string Harmony2NS = "HarmonyLib";
    public const string HarmonyStr = "Harmony";
    public abstract void Apply(ModuleDefinition asm, ReferenceImporter importer, ref BaseConverter.Context context);
}
