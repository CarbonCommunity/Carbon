using Carbon.Compat.Converters;

namespace Carbon.Compat.Patches;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

public interface IAssemblyPatch
{
    public abstract void Apply(ModuleDefinition assembly, ReferenceImporter importer, ref BaseConverter.Context context);
}
