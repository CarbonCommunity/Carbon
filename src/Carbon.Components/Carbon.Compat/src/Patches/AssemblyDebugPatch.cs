using System.Diagnostics;
using Carbon.Compat.Converters;

namespace Carbon.Compat.Patches;

/*
 *
 * Copyright (c) 2023-2024 Patrette, under the GNU v3 license rights
 * Copyright (c) 2023-2024 Carbon Community, under the GNU v3 license rights
 *
 */

public class AssemblyDebugPatch : IAssemblyPatch
{
	public void Apply(ModuleDefinition assembly, ReferenceImporter importer, ref BaseConverter.Context context)
	{
		if (!Debugger.IsAttached) return;
		foreach (TypeDefinition type in assembly.GetAllTypes())
		{
			foreach (MethodDefinition method in type.Methods)
			{
				CilMethodBody body = method.CilMethodBody;

				if (body == null)
				{
					continue;
				}

				for (int i = 0; i < body.Instructions.Count; i++)
				{
					CilInstruction cil = body.Instructions[i];

					if (cil.OpCode == CilOpCodes.Call && cil.Operand is MemberReference mref && // bruh
					    mref.DeclaringType.DefinitionAssembly().IsCorLib &&
					    mref.Signature is MethodSignature msig &&
					    ((mref.DeclaringType.Name == "Debugger" &&
					      (mref.Name == "get_IsAttached" || mref.Name == "IsLogging")) ||
					     (mref.DeclaringType.Name == "Environment" && mref.Name == "FailFast")))
					{
						for (int pc = 0; pc < msig.ParameterTypes.Count; pc++)
						{
							body.Instructions.Insert(i, new CilInstruction(CilOpCodes.Pop));
							i++;
						}

						if (msig.ReturnType.ElementType == ElementType.Boolean)
						{
							cil.OpCode = CilOpCodes.Ldc_I4_0;
							cil.Operand = null;
							continue;
						}

						if (msig.ReturnsValue)
						{
							if (!msig.ReturnType.IsValueType)
							{
								body.Instructions.Insert(i, new CilInstruction(CilOpCodes.Ldnull));
								i++;
							}
						}

						body.Instructions.RemoveAt(i);
						i--;
					}
				}
			}
		}

		for (int index = 0; index < assembly.Assembly.CustomAttributes.Count; index++)
		{
			CustomAttribute attr = assembly.Assembly.CustomAttributes[index];

			if (attr.Constructor.DeclaringType.FullName == "System.Diagnostics.DebuggableAttribute" &&
			    attr.Constructor.DeclaringType.DefinitionAssembly().IsCorLib)
			{
				assembly.Assembly.CustomAttributes.RemoveAt(index--);
			}
		}

		TypeSignature enumRef = importer.ImportTypeSignature(typeof(DebuggableAttribute.DebuggingModes));
		CustomAttribute debugAttr = new CustomAttribute(importer.ImportType(typeof(DebuggableAttribute))
				.CreateMemberReference(".ctor",
					MethodSignature.CreateInstance(assembly.CorLibTypeFactory.Void,
						importer.ImportTypeSignature(typeof(DebuggableAttribute.DebuggingModes))))
				.ImportWith(importer),
			new CustomAttributeSignature(new CustomAttributeArgument(enumRef,
				(int)(DebuggableAttribute.DebuggingModes.DisableOptimizations |
				      DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints |
				      DebuggableAttribute.DebuggingModes.EnableEditAndContinue))));

		assembly.Assembly.CustomAttributes.Add(debugAttr);
	}
}
