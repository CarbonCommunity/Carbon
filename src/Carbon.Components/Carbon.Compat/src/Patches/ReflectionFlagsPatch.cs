using System.Reflection;
using Carbon.Compat.Converters;

namespace Carbon.Compat.Patches;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

public class ReflectionFlagsPatch : IAssemblyPatch
{
    public static List<string> ReflectionTypeMethods = new List<string>()
    {
        "GetMethod",
        "GetField",
        "GetProperty",
        "GetMember"
    };

    public void Apply(ModuleDefinition assembly, ReferenceImporter importer, ref BaseConverter.Context context)
    {
        foreach (TypeDefinition type in assembly.GetAllTypes())
        {
            foreach (MethodDefinition method in type.Methods)
            {
	            if (method.MethodBody is not CilMethodBody body)
	            {
		            continue;
	            }

                for (int index = 0; index < body.Instructions.Count; index++)
                {
                    CilInstruction CIL = body.Instructions[index];

                    if (CIL.OpCode == CilOpCodes.Callvirt &&
                        CIL.Operand is MemberReference mref &&
                        mref.Signature is MethodSignature msig &&
                        mref.DeclaringType is TypeReference tref &&
                        tref.Scope is AssemblyReference aref &&
                        aref.IsCorLib &&
                        tref.Name == "Type" &&
                        ReflectionTypeMethods.Contains(mref.Name) &&
                        msig.ParameterTypes.Any(x=>x.Scope is AssemblyReference { IsCorLib: true } && x.Name == "BindingFlags")
                       )
                    {
                        for (int li = index - 1; li >= Math.Max(index-5, 0); li--)
                        {
                            CilInstruction xil = body.Instructions[li];

                            if (!xil.IsLdcI4())
                            {
                                continue;
                            }

                            BindingFlags flags = (BindingFlags)xil.GetLdcI4Constant() | BindingFlags.Public | BindingFlags.NonPublic;

                            xil.Operand = (object)(int)flags;
                            xil.OpCode = CilOpCodes.Ldc_I4;

                            goto exit;
                        }
                        Logger.Error($"Failed to find binding flags for {method.FullName} at #IL_{CIL.Offset:X}:{index} in {assembly.Name}");
                    }

                    exit: ;
                }
            }
        }
    }
}
