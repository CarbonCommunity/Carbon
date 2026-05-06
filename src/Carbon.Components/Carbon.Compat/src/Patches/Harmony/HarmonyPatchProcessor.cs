using System.Reflection;
using AsmResolver;
using AsmResolver.DotNet.Serialized;
using Carbon.Compat.Converters;
using Facepunch;

namespace Carbon.Compat.Patches.Harmony;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

public class HarmonyPatchProcessor : BaseHarmonyPatch
{
    public override void Apply(ModuleDefinition asm, ReferenceImporter importer, ref BaseConverter.Context context)
    {
        foreach (TypeDefinition type in asm.GetAllTypes())
        {
            bool invalid = false;

            List<CustomAttribute> patches = Pool.Get<List<CustomAttribute>>();

            foreach (CustomAttribute attr in type.CustomAttributes)
            {
                ITypeDefOrRef declaringType = attr.Constructor?.DeclaringType;

                if (declaringType == null)
                {
	                continue;
                }

                CustomAttributeSignature sig = attr.Signature;

                if (sig == null)
                {
	                continue;
                }

                if (declaringType.Name == "HarmonyPatch" && declaringType.Scope is SerializedAssemblyReference aref && aref.Name == HarmonyASM)
                {
                    if (sig.FixedArguments.Count > 1 && sig.FixedArguments[0].Element is TypeDefOrRefSignature tr && sig.FixedArguments[1].Element is Utf8String ats)
                    {
                        RegisterPatch(asm.Name.ToString(), tr.DefinitionAssembly().Name, ats, tr.FullName, $"{asm.Assembly.Name} - {type.FullName}", null);

                        if (!PatchWhitelist.IsPatchAllowed(tr, ats))
                        {
                            invalid = true;
                            patches.Add(attr);
                            break;
                        }
                    }
                    else
                    {
                        if (!PatchWhitelist.IsPatchAllowed(type))
                        {
                            invalid = true;
                            patches.Add(attr);
                            break;
                        }
                    }
                }
            }

            if (invalid)
            {
                type.CustomAttributes.Add(new CustomAttribute(asm.CorLibTypeFactory.CorLibScope.CreateTypeReference("System", "ObsoleteAttribute").CreateMemberReference(".ctor", MethodSignature.CreateInstance(asm.CorLibTypeFactory.Void)).ImportWith(importer)));

                foreach (CustomAttribute attr in patches)
                {
                    type.CustomAttributes.Remove(attr);
                }
            }

			Pool.FreeUnmanaged(ref patches);
        }
    }

    public static void RegisterPatch(string parentAssemblyName, string assemblyName, string methodName, string typeName, string reason, HarmonyLib.Harmony harmony)
    {
	    Components.Harmony.CurrentPatches.Add(new Components.Harmony.PatchInfoEntry(parentAssemblyName, assemblyName, methodName, typeName, reason, harmony));
    }

    public static void RegisterPatch(MethodBase method, string reason, HarmonyLib.Harmony harmony)
    {
	    if(method== null)return;

	    Components.Harmony.CurrentPatches.Add(new Components.Harmony.PatchInfoEntry(method.DeclaringType.Assembly.GetName().Name + ".dll", method, harmony));
    }

    public static class PatchWhitelist
    {
        public static bool IsPatchAllowed(TypeDefOrRefSignature type, Utf8String method)
        {
            if (type.FullName == "ServerMgr" && type.Scope is SerializedAssemblyReference aref &&
                aref.Name == "Assembly-CSharp" && method == "UpdateServerInformation")
            {
                return false;
            }

            return true;
        }

        public static List<string> string_blacklist = new List<string>()
        {
            "Oxide.Core.OxideMod",
            "Oxide.Core"
        };

        public static bool IsPatchAllowed(TypeDefinition type)
        {
            MethodDefinition target = type.Methods.FirstOrDefault(x => x.CustomAttributes.Any(y =>
                y.Constructor.DeclaringType.Name == "HarmonyTargetMethods" &&
                y.Constructor.DeclaringType.Scope is SerializedAssemblyReference asmref &&
                asmref.Name == HarmonyASM));
            if (target?.MethodBody is not CilMethodBody body) goto End;

            for (int index = 0; index < body.Instructions.Count; index++)
            {
                CilInstruction CIL = body.Instructions[index];

                if (CIL.OpCode == CilOpCodes.Ldstr && CIL.Operand is string str)
                {
                    if (string_blacklist.Contains(str))
                    {
	                    return false;
                    }
                }
            }

            End:
            return true;
        }
    }
}
