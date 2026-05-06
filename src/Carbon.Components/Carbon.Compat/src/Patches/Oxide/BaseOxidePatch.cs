using Carbon.Compat.Converters;

namespace Carbon.Compat.Patches.Oxide;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

public abstract class BaseOxidePatch : IAssemblyPatch
{
    public const string OxideStr = "Oxide";

    public abstract void Apply(ModuleDefinition assembly, ReferenceImporter importer, ref BaseConverter.Context context);
}
