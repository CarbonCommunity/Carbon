#nullable disable

using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Carbon.Projects.Oxide;

internal static partial class Helper
{
	internal static bool LoadArgumentEx(
		ref StringBuilder instructions, ParameterInfo parameter, bool hasThis = true, Type type = null, string[] target = null
	)
	{
		// if non-static "this" is at a0, need to inc index by one
		var index = parameter?.Position ?? 0;
		if (parameter != null && hasThis)
		{
			index++;
		}

		return index switch
		{
			0 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldarg_0), type: type, target: target),
			1 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldarg_1), type: type, target: target),
			2 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldarg_2), type: type, target: target),
			3 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldarg_3), type: type, target: target),
			_ => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldarg_S), index, false),
		};
	}

	internal static bool LoadLocalEx(ref StringBuilder instructions, LocalVariableInfo variable, string[] target)
	{
		return variable.LocalIndex switch
		{
			0 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldloc_0), type: variable.LocalType, target: target),
			1 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldloc_1), type: variable.LocalType, target: target),
			2 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldloc_2), type: variable.LocalType, target: target),
			3 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldloc_3), type: variable.LocalType, target: target),
			_ => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldloc_S), variable.LocalIndex, false),
		};
	}

	internal static bool StoreLocalEx(ref StringBuilder instructions, LocalVariableInfo variable)
	{
		return variable.LocalIndex switch
		{
			0 => AddYieldInstruction(ref instructions, nameof(OpCodes.Stloc_0)),
			1 => AddYieldInstruction(ref instructions, nameof(OpCodes.Stloc_1)),
			2 => AddYieldInstruction(ref instructions, nameof(OpCodes.Stloc_2)),
			3 => AddYieldInstruction(ref instructions, nameof(OpCodes.Stloc_3)),
			_ => AddYieldInstruction(ref instructions, nameof(OpCodes.Stloc_S), variable.LocalIndex, false),
		};
	}

	internal static bool LoadLocalIntEx(ref StringBuilder instructions, int index, string typeHint = "System.Object")
	{
		AddGenericInstruction(ref instructions,
			$"yield return __GeneratorRuntime.CreateLoadLocalInstruction(Generator, Method, {index}, typeof({typeHint}));");
		return true;
	}

	internal static bool StoreLocalIntEx(ref StringBuilder instructions, int index, string typeHint = "System.Object")
	{
		AddGenericInstruction(ref instructions,
			$"yield return __GeneratorRuntime.CreateStoreLocalInstruction(Generator, Method, {index}, typeof({typeHint}));");
		return true;
	}

	internal static bool AddLoadLocalInstructionEx(ref StringBuilder instructions, int index, string typeHint = "System.Object")
	{
		AddGenericInstruction(ref instructions,
			$"edit.Add(__GeneratorRuntime.CreateLoadLocalInstruction(Generator, Method, {index}, typeof({typeHint})));");
		return true;
	}

	internal static bool AddStoreLocalInstructionEx(ref StringBuilder instructions, int index, string typeHint = "System.Object")
	{
		AddGenericInstruction(ref instructions,
			$"edit.Add(__GeneratorRuntime.CreateStoreLocalInstruction(Generator, Method, {index}, typeof({typeHint})));");
		return true;
	}

	internal static bool AddLoadLocalAddressInstructionEx(ref StringBuilder instructions, int index, string typeHint = "System.Object")
	{
		AddGenericInstruction(ref instructions,
			$"edit.Add(__GeneratorRuntime.CreateLoadLocalAddressInstruction(Generator, Method, {index}, typeof({typeHint})));");
		return true;
	}

	internal static bool LoadIntegerEx(ref StringBuilder instructions, int index)
	{
		return index switch
		{
			0 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldc_I4_0)),
			1 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldc_I4_1)),
			2 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldc_I4_2)),
			3 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldc_I4_3)),
			4 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldc_I4_4)),
			5 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldc_I4_5)),
			6 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldc_I4_6)),
			7 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldc_I4_7)),
			8 => AddYieldInstruction(ref instructions, nameof(OpCodes.Ldc_I4_8)),
			_ => index is >= -128 and <= 127
				? AddYieldInstruction(ref instructions, nameof(OpCodes.Ldc_I4_S), (sbyte)index, false)
				: AddYieldInstruction(ref instructions, nameof(OpCodes.Ldc_I4), index, false),
		};
	}
}
