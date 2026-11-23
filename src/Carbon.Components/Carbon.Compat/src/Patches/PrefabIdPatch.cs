using System.Reflection;
using AsmResolver.DotNet.Builder;

namespace Carbon.Compat.Patches;

internal static class PrefabIdPatch
{
	private static readonly PropertyInfo PrefabId = typeof(BaseNetworkable).GetProperty("prefabID",
		BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

	internal static byte[] Rewrite(byte[] bytes, ModuleDefinition module = null)
	{
		if (PrefabId == null)
		{
			return bytes;
		}

		module ??= ModuleDefinition.FromBytes(bytes);
		if (!Apply(module))
		{
			return bytes;
		}

		using var output = new MemoryStream();
		module.Write(output, new ManagedPEImageBuilder(MetadataBuilderFlags.PreserveAll));
		return output.ToArray();
	}

	internal static bool Apply(ModuleDefinition module)
	{
		if (PrefabId?.PropertyType != typeof(uint) || !module.GetImportedMemberReferences().Any(IsPrefabIdField))
		{
			return false;
		}

		IMethodDescriptor getter = null;
		IMethodDescriptor setter = null;
		foreach (var type in module.GetAllTypes())
		{
			for (var i = 0; i < type.Methods.Count; i++)
			{
				var instructions = type.Methods[i].CilMethodBody?.Instructions;
				if (instructions == null)
				{
					continue;
				}

				for (var j = 0; j < instructions.Count; j++)
				{
					var instruction = instructions[j];
					if ((instruction.OpCode != CilOpCodes.Ldfld && instruction.OpCode != CilOpCodes.Stfld) ||
					    instruction.Operand is not MemberReference field || !IsPrefabIdField(field) ||
					    (j > 0 && instructions[j - 1].OpCode.OpCodeType == CilOpCodeType.Prefix))
					{
						continue;
					}

					instruction.Operand = instruction.OpCode == CilOpCodes.Ldfld
						? getter ??= module.DefaultImporter.ImportMethod(PrefabId.GetMethod)
						: setter ??= module.DefaultImporter.ImportMethod(PrefabId.SetMethod);
					instruction.OpCode = CilOpCodes.Callvirt;
				}
			}
		}
		return getter != null || setter != null;
	}

	private static bool IsPrefabIdField(MemberReference member) =>
		member.Name == "prefabID" && member.Signature is FieldSignature signature &&
		signature.FieldType.ElementType == ElementType.U4 &&
		member.Parent is TypeReference { Scope: AssemblyReference owner } type &&
		type.FullName == "BaseNetworkable" && owner.Name == "Assembly-CSharp";
}
