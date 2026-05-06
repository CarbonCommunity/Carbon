using Carbon.Compat.Converters;
using Carbon.Compat.Lib;

namespace Carbon.Compat.Patches.Harmony;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

public class HarmonyTypeRef : BaseHarmonyPatch
{
    public override void Apply(ModuleDefinition assembly, ReferenceImporter importer, ref BaseConverter.Context context)
    {
        foreach (TypeReference type in assembly.GetImportedTypeReferences())
        {
            AssemblyReference reference = type.Scope as AssemblyReference;

            if (reference != null && reference.Name == HarmonyASM)
            {
	            if (type.Namespace.StartsWith(Harmony1NS))
	            {
		            type.Namespace = Harmony2NS;
	            }

                if (type.Name == "HarmonyInstance")
                {
                    type.Name = "Harmony";
                }
            }
        }
    }
}
