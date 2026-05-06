using Carbon.Base;
using Carbon.Compat.Converters;
using Carbon.Compat.Legacy.EventCompat;
using Carbon.Compat.Lib;
using HarmonyLib;

namespace Carbon.Compat.Patches.Oxide;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

public class OxideTypeRef : BaseOxidePatch
{
    public static List<string> PluginToBaseHookable = new()
	{
        "System.Void Oxide.Core.Libraries.Permission::RegisterPermission(System.String, Oxide.Core.Plugins.Plugin)",
        "System.Void Oxide.Core.Libraries.Lang::RegisterMessages(System.Collections.Generic.Dictionary`2<System.String, System.String>, Oxide.Core.Plugins.Plugin, System.String)",
        //"System.String Oxide.Core.Libraries.Lang::GetMessage(System.String, Oxide.Core.Plugins.Plugin, System.String)",
        "System.Void Oxide.Game.Rust.Libraries.Command::RemoveConsoleCommand(System.String, Oxide.Core.Plugins.Plugin)",
        "System.Collections.Generic.Dictionary`2<System.String, System.String> Oxide.Core.Libraries.Lang::GetMessages(System.String, Oxide.Core.Plugins.Plugin)"
    };

    public override void Apply(ModuleDefinition assembly, ReferenceImporter importer, ref BaseConverter.Context context)
    {
        foreach (MemberReference memberReference in assembly.GetImportedMemberReferences())
        {
            AssemblyReference aref = memberReference.DeclaringType.DefinitionAssembly();

            if (memberReference.Signature is MethodSignature methodSignature)
            {
	            if (!Helpers.IsOxideASM(aref))
	            {
		            continue;
	            }

	            string fullName = memberReference.FullName;

	            if (PluginToBaseHookable.Contains(memberReference.FullName))
	            {
		            for (int index = 0; index < methodSignature.ParameterTypes.Count; index++)
		            {
			            TypeSignature typeSig = methodSignature.ParameterTypes[index];

			            if (typeSig.FullName == "Oxide.Core.Plugins.Plugin" && Helpers.IsOxideASM(typeSig.DefinitionAssembly()))
			            {
				            methodSignature.ParameterTypes[index] = importer.ImportTypeSignature(typeof(BaseHookable));
			            }
		            }

		            continue;
	            }

	            if (methodSignature.GenericParameterCount == 1 && fullName == "!!0 Oxide.Core.Interface::Call<?>(System.String, System.Object[])")
	            {
		            memberReference.Parent = importer.ImportType(typeof(OxideCompat));
		            memberReference.Name = nameof(OxideCompat.OxideCallHookGeneric);
		            continue;
	            }

	            if (fullName ==
	                "System.String Oxide.Core.Libraries.Lang::GetMessage(System.String, Oxide.Core.Plugins.Plugin, System.String)")
	            {
		            memberReference.Signature = importer.ImportMethod(AccessTools.Method(typeof(OxideCompat), "GetMessage1")).Signature;
		            memberReference.Parent = importer.ImportType(typeof(OxideCompat));
		            memberReference.Name = "GetMessage1";
		            continue;
	            }
            }
        }

        foreach (TypeReference typeReference in assembly.GetImportedTypeReferences())
        {
            ProcessTypeRef(typeReference, importer);
        }

        ProcessAttrList(assembly.CustomAttributes);

        foreach (TypeDefinition type in assembly.GetAllTypes())
        {
            ProcessAttrList(type.CustomAttributes);

            foreach (FieldDefinition field in type.Fields)
            {
                ProcessAttrList(field.CustomAttributes);
            }

            foreach (MethodDefinition method in type.Methods)
            {
                ProcessAttrList(method.CustomAttributes);
            }

            foreach (PropertyDefinition property in type.Properties)
            {
                ProcessAttrList(property.CustomAttributes);
            }
        }

        void ProcessAttrList(IList<CustomAttribute> list)
        {
            for (int x = 0; x < list.Count; x++)
            {
                CustomAttribute attr = list[x];

				try
				{
					for (int y = 0; y < attr.Signature?.FixedArguments.Count; y++)
					{
						CustomAttributeArgument arg = attr.Signature.FixedArguments[y];
						if (arg.Element is TypeDefOrRefSignature sig)
						{
							ProcessTypeRef(sig.Type as TypeReference, importer);
						}
					}
				}
				catch { } // Ignore
			}
        }
    }

    public static void ProcessTypeRef(TypeReference type, ReferenceImporter importer)
    {
	    if (type == null)
	    {
		    return;
	    }

        if (type.Scope is TypeReference parent)
        {
            if (parent.FullName is "Oxide.Plugins.Timers" or "Oxide.Plugins.Timer" && type.Name == "TimerInstance")
            {
                type.Name = "Timer";
                type.Namespace = "Oxide.Plugins";
                type.Scope = CompatManager.Common.ImportWith(importer);
                return;
            }
        }

        if (type.Scope is not AssemblyReference aref || !Helpers.IsOxideASM(aref))
        {
	        return;
        }

        if (type.FullName == "Oxide.Core.Event" || type.FullName.StartsWith("Oxide.Core.Event`"))
        {
	        type.Scope = (IResolutionScope)importer.ImportType(typeof(OxideEvents));
	        return;
        }

        if (type.FullName == "Oxide.Core.Plugins.PluginEvent")
        {
	        type.Namespace = string.Empty;
        }

        if (type.Namespace.StartsWith("Newtonsoft.Json"))
        {
	        type.Scope = CompatManager.Newtonsoft.ImportWith(importer);
	        return;
        }

        if (type.Namespace.StartsWith("ProtoBuf"))
        {
	        if (type.Namespace == "ProtoBuf" && type.Name == "Serializer")
		        type.Scope = CompatManager.protobuf.ImportWith(importer);
	        else
		        type.Scope = CompatManager.protobufCore.ImportWith(importer);
	        return;
        }

        if (type.Namespace.StartsWith("WebSocketSharp"))
        {
	        if (type.Namespace == "WebSocketSharp.Net" && type.Name == "SslConfiguration")
		        type.Name = "ClientSslConfiguration";

	        type.Scope = CompatManager.wsSharp.ImportWith(importer);
	        return;
        }

        if (type.Name == "VersionNumber")
        {
	        goto sdk;
        }

        if (type.Namespace == "Oxide.Plugins" && type.Name.EndsWith("Attribute"))
        {
	        type.Namespace = string.Empty;
	        goto sdk;
        }

        if (type.FullName == "Oxide.Plugins.Hash`2")
        {
	        type.Namespace = string.Empty;
	        goto common;
        }

        if (type.FullName is "Oxide.Core.Libraries.Timer" or "Oxide.Plugins.PluginTimers")
        {
	        type.Name = "Timers";
	        type.Namespace = "Oxide.Plugins";
	        goto common;
        }

        if (type.FullName == "Oxide.Core.Plugins.HookMethodAttribute")
        {
	        type.Namespace = string.Empty;
	        goto sdk;
        }

        if (type.FullName is "Oxide.Plugins.CSharpPlugin" or "Oxide.Core.Plugins.CSPlugin")
        {
	        type.Name = "RustPlugin";
	        type.Namespace = "Oxide.Plugins";
	        goto common;
        }

        if (type.FullName == "Oxide.Core.Plugins.PluginManager")
        {
	        type.Namespace = string.Empty;
        }

        common:
        type.Scope = CompatManager.Common.ImportWith(importer);
        return;

        sdk:
        type.Scope = CompatManager.SDK.ImportWith(importer);
    }
}
