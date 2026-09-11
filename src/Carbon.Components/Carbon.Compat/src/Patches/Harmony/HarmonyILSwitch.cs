using Carbon.Compat.Converters;
using Carbon.Compat.Lib;
using HarmonyLib;

namespace Carbon.Compat.Patches.Harmony;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

public class HarmonyILSwitch : BaseHarmonyPatch
{
    public override void Apply(ModuleDefinition asm, ReferenceImporter importer, ref BaseConverter.Context context)
    {
        IMethodDescriptor PatchProcessorCompatRef = importer.ImportMethod(AccessTools.Method(typeof(HarmonyCompat), nameof(HarmonyCompat.PatchProcessorCompat)));

        foreach (TypeDefinition type in asm.GetAllTypes())
        {
            foreach (MethodDefinition method in type.Methods)
            {
	            if (method.MethodBody is not CilMethodBody body)
	            {
		            continue;
	            }

                for (int i = 0; i < body.Instructions.Count; i++)
                {
                    CilInstruction CIL = body.Instructions[i];

                    // IL Patches
                    if (CIL.OpCode == CilOpCodes.Call && CIL.Operand is MemberReference { FullName: $"{Harmony2NS}.{HarmonyStr} {Harmony2NS}.{HarmonyStr}::Create(System.String)" })
                    {
                        CIL.OpCode = CilOpCodes.Newobj;
                        CIL.Operand = importer.ImportMethod(AccessTools.Constructor(typeof(HarmonyLib.Harmony), new Type[]{typeof(string)}));
                    }

                    if ((CIL.OpCode == CilOpCodes.Newobj) && CIL.Operand is MemberReference bref &&
                        bref.DeclaringType.DefinitionAssembly().Name == HarmonyASM &&
                        bref.DeclaringType.Name == "PatchProcessor" &&
                        bref.Name == ".ctor")
                    {
                        CIL.OpCode = CilOpCodes.Call;
                        CIL.Operand = PatchProcessorCompatRef;
                        continue;
                    }

                    if (CIL.OpCode == CilOpCodes.Callvirt && CIL.Operand is MemberReference cref &&
                        cref.DeclaringType.DefinitionAssembly().Name == HarmonyASM &&
                        cref.DeclaringType.Name == "PatchProcessor" &&
                        cref.Name == "Patch")
                    {
                        if (i != 0)
                        {
                            CilInstruction ccall = body.Instructions[i - 1];

                            if (ccall.OpCode == CilOpCodes.Call && ccall.Operand == PatchProcessorCompatRef)
                            {
                                body.Instructions.RemoveAt(i);

                                CilInstruction pop = body.Instructions[i];

                                if (pop.OpCode == CilOpCodes.Pop)
                                {
                                    body.Instructions.RemoveAt(i);
                                }
                            }
                        }
                    }


                    if ((CIL.OpCode == CilOpCodes.Callvirt || CIL.OpCode ==  CilOpCodes.Call) && CIL.Operand is MemberReference dref &&
                        dref.DeclaringType.DefinitionAssembly().Name == HarmonyASM &&
                        dref.Name == "Patch")
                    {
	                    CIL.Operand = importer.ImportMethod(AccessTools.Method(typeof(HarmonyCompat),
		                    nameof(HarmonyCompat.InstancePatchCompat)));
	                    CIL.OpCode = CilOpCodes.Call;
                    }
                }
            }
        }
    }
}
