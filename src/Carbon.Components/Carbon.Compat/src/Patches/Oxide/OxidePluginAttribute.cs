using Carbon.Compat.Converters;
using Facepunch.Crypt;

namespace Carbon.Compat.Patches.Oxide;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

public class OxidePluginAttribute : BaseOxidePatch
{
    public override void Apply(ModuleDefinition assembly, ReferenceImporter importer, ref BaseConverter.Context context)
    {
        string author = context.Author ?? "CCL";

        foreach (TypeDefinition type in assembly.GetAllTypes())
        {
            if (!type.IsBaseType(x => x.Name == "RustPlugin" && x.DefinitionAssembly().Name == "Carbon.Common")) continue;
            {
                if (type.Name.ToString().IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                {
                    string newName = "plugin_" + Md5.Calculate(type.Name);
                    Logger.Warn($"Plugin \"{type.Name}\" has an invalid name, renaming to {newName}");
                    type.Name = newName;
                }

                CustomAttribute infoAttr = type.CustomAttributes.FirstOrDefault(x => x.Constructor.DeclaringType.FullName == "InfoAttribute" && x.Constructor.DeclaringType.DefinitionAssembly().Name == "Carbon.Common");

                if (infoAttr != null)
                {
	                continue;
                }

                type.CustomAttributes.Add(new CustomAttribute(importer.ImportType(typeof(InfoAttribute)).CreateMemberReference(".ctor", MethodSignature.CreateInstance(assembly.CorLibTypeFactory.Void, assembly.CorLibTypeFactory.String, assembly.CorLibTypeFactory.String, assembly.CorLibTypeFactory.Double)).ImportWith(importer))
                {
                    Signature = new CustomAttributeSignature(
                        new CustomAttributeArgument(assembly.CorLibTypeFactory.String, $"{assembly.Assembly.Name}-{type.Name}"),
                        new CustomAttributeArgument(assembly.CorLibTypeFactory.String, author),
                        new CustomAttributeArgument(assembly.CorLibTypeFactory.Double, 0d))
                });
            }
        }
    }
}
