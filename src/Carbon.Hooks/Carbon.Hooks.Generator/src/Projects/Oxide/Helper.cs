#nullable disable

using System.Reflection;
using System.Text;
using static Carbon.Projects.Oxide.Oxide;

namespace Carbon.Projects.Oxide;

internal static partial class Helper
{
	public static readonly string[] HookBlacklist =
	[
		"IOnPlayerChat",
		"IOnRconMessage",
		"OnWireClear",
		"OnWireClear [patch]", // to fix
	];

	[ThreadStatic] public static Type CurrentField;
	[ThreadStatic] public static List<(string, Type)> Parameters;
	[ThreadStatic] public static List<string> ParametersTemp;
	[ThreadStatic] public static Type ReturnType;
	[ThreadStatic] public static Type RuntimeType;
	[ThreadStatic] public static HookDef.Data Metadata;
	[ThreadStatic] public static bool IsReturning;
	[ThreadStatic] public static string PendingInstructionStateSource;
	[ThreadStatic] public static bool PendingInstructionStateIncludesBlocks;
	[ThreadStatic] public static string ModifyAnchorExpression;
	[ThreadStatic] public static List<string> PendingOriginalLabelAssignments;
	[ThreadStatic] public static int ModifyAnchorBaseIndex;
	[ThreadStatic] private static bool DeterministicNames;
	[ThreadStatic] private static int GeneratedNameIndex;

	internal static void ResetGenerationState(bool deterministicNames)
	{
		CurrentField = null;
		Parameters = [];
		ParametersTemp = [];
		ReturnType = null;
		RuntimeType = null;
		Metadata = null;
		IsReturning = false;
		PendingInstructionStateSource = null;
		PendingInstructionStateIncludesBlocks = true;
		ModifyAnchorExpression = null;
		PendingOriginalLabelAssignments = [];
		ModifyAnchorBaseIndex = 0;
		DeterministicNames = deterministicNames;
		GeneratedNameIndex = 0;
	}

	private static string CreateGeneratedName(string prefix) => DeterministicNames ? $"{prefix}_{GeneratedNameIndex++:0000}" : $"{prefix}_{Guid.NewGuid():N}";
	
	internal static void ArmInstructionStateTransfer(string source, bool includeBlocks = true)
	{
		PendingInstructionStateSource = source;
		PendingInstructionStateIncludesBlocks = includeBlocks;
	}

	internal static void UseModifyAnchor(string expression, int baseIndex)
	{
		ModifyAnchorExpression = expression;
		ModifyAnchorBaseIndex = baseIndex;
	}

	internal static void ClearModifyAnchor()
	{
		ModifyAnchorExpression = null;
		ModifyAnchorBaseIndex = 0;
		PendingOriginalLabelAssignments ??= [];
		PendingOriginalLabelAssignments.Clear();
	}

	internal static string GetOriginalIndexExpression(int index)
	{
		if (string.IsNullOrEmpty(ModifyAnchorExpression))
		{
			return index.ToString();
		}

		var delta = index - ModifyAnchorBaseIndex;
		if (delta == 0)
		{
			return ModifyAnchorExpression;
		}

		return delta > 0 ? $"{ModifyAnchorExpression} + {delta}" : $"{ModifyAnchorExpression} - {Math.Abs(delta)}";
	}

	internal static bool AddYieldInstruction(ref StringBuilder instructions, string opcode, object operand = null, bool useQuotes = true, Type type = null, string[] target = null, bool ignoreValueTypes = false)
	{
		RuntimeType = null;

		string BuildInstruction(string expression)
		{
			if (string.IsNullOrEmpty(PendingInstructionStateSource))
			{
				return expression;
			}

			var source = PendingInstructionStateSource;
			PendingInstructionStateSource = null;
			var moveBlocks = PendingInstructionStateIncludesBlocks;
			PendingInstructionStateIncludesBlocks = true;
			return moveBlocks ? $"{expression}.MoveLabelsFrom({source}).MoveBlocksFrom({source})" : $"{expression}.MoveLabelsFrom({source})";
		}

		if (operand == null)
		{
			instructions.AppendLine($"yield return {BuildInstruction($"new CodeInstruction(OpCodes.{opcode})")};");

			Handle(ref instructions);
			return true;
		}

		if (useQuotes)
		{
			operand = $"\"{operand}\"";
		}

		instructions.AppendLine($"yield return {BuildInstruction($"new CodeInstruction(OpCodes.{opcode}, {operand})")};");

		Handle(ref instructions);

		return true;

		void Handle(ref StringBuilder instructions)
		{
			if (type == null)
			{
				return;
			}

			if (target is { Length: > 0 })
			{
				const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
				var currentType = type;

				for (var i = 0; i < target.Length; i++)
				{
					var value = target[i];
					var isMethod = value.Contains("()");
					value = value.Replace("()", string.Empty);
					var isProperty = target.Length == 1 ? type.GetRuntimeProperty(value) != null : currentType.GetProperty(value) != null;
					var runtimeType = target.Length == 1
						? type.GetRuntimeField(value)?.FieldType ?? type.GetRuntimeProperty(value)?.PropertyType ??
						type.GetRuntimeMethod(value, [])?.ReturnType
						: currentType.GetField(value, flags)?.FieldType ?? currentType.GetProperty(value, flags)?.PropertyType;

					if (runtimeType == null)
					{
						continue;
					}

					CurrentField = currentType;
					instructions.AppendLine($"// Set {CurrentField}");

					instructions.AppendLine(
						$"// value:{value} isProperty:{isProperty} runtimeType:{runtimeType} currentType:{currentType} type:{type}");

					if (isProperty)
					{
						instructions.AppendLine(
							$"yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName(\"{currentType.FullName}\"), \"get_{value}\"));");
					}
					else
					{
						if (isMethod)
						{
							instructions.AppendLine(
								$"yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName(\"{currentType.FullName}\"), \"{value}\"));");
						}
						else
						{
							instructions.AppendLine(
								$"yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(Carbon.Extensions.AccessToolsEx.TypeByName(\"{currentType.FullName}\"), \"{value}\"));");
						}
					}

					if (!ignoreValueTypes && runtimeType is { IsValueType: true })
					{
						instructions.AppendLine(
							$"yield return new CodeInstruction(OpCodes.Box, Carbon.Extensions.AccessToolsEx.TypeByName(\"{runtimeType.FullName}\"));");
					}

					currentType = RuntimeType = runtimeType;
				}
			}
		}
	}

	internal static bool AddYieldInstructionMoveLabels(
		ref StringBuilder instructions, string opcode, object operand = null, bool useQuotes = true
	)
	{
		string BuildInstruction(string expression)
		{
			if (string.IsNullOrEmpty(PendingInstructionStateSource))
			{
				return expression;
			}

			var source = PendingInstructionStateSource;
			PendingInstructionStateSource = null;
			var moveBlocks = PendingInstructionStateIncludesBlocks;
			PendingInstructionStateIncludesBlocks = true;
			return moveBlocks
				? $"{expression}.MoveLabelsFrom({source}).MoveBlocksFrom({source})"
				: $"{expression}.MoveLabelsFrom({source})";
		}

		if (operand == null)
		{
			instructions.AppendLine($"yield return {BuildInstruction($"new CodeInstruction(OpCodes.{opcode})")};");
			return true;
		}

		if (useQuotes)
		{
			operand = $"\"{operand}\"";
		}

		instructions.AppendLine($"yield return {BuildInstruction($"new CodeInstruction(OpCodes.{opcode}, {operand})")};");
		return true;
	}

	internal static bool AddGenericInstruction(ref StringBuilder instructions, string line)
	{
		instructions.AppendLine(line);
		return true;
	}

	internal static void AddOpCodeWithLabel(
		ref StringBuilder instructions, ref Dictionary<int, string> existingInstructions, ref Dictionary<int, string> newInstructions, string opcode, object operand, bool referencesNewInstruction)
	{
		var index = Convert.ToInt32(operand);

		if (!existingInstructions.TryGetValue(index, out var label))
		{
			label = CreateGeneratedName("label");
			AddGenericInstruction(ref instructions, $"Label {label} = Generator.DefineLabel();");
			existingInstructions.Add(index, label);

			if (referencesNewInstruction)
			{
				newInstructions.Add(index, label);
			}
			else
			{
				var targetInstructionVar = CreateGeneratedName("targetInstruction");
				AddGenericInstruction(ref instructions, $"var {targetInstructionVar} = original[{GetOriginalIndexExpression(index)}];");
				PendingOriginalLabelAssignments.Add($"{targetInstructionVar}.labels.Add({label});");
			}
		}

		AddGenericInstruction(ref instructions, $"edit.Add(new CodeInstruction(OpCodes.{opcode}, {label}));");
	}

	internal static void FlushPendingOriginalLabelAssignments(ref StringBuilder instructions)
	{
		PendingOriginalLabelAssignments ??= [];
		if (PendingOriginalLabelAssignments.Count == 0)
		{
			return;
		}

		for (var i = 0; i < PendingOriginalLabelAssignments.Count; i++)
		{
			var line = PendingOriginalLabelAssignments[i];
			AddGenericInstruction(ref instructions, line);
		}

		PendingOriginalLabelAssignments.Clear();
	}

	internal static string LookAheadForTypeHint(List<HookDef.Data.InstructionData> instructions, int index)
	{
		// ffs.. the need for this method kills me
		// they qualify everything.. why don't they qualify the type of the newly created local var ?

		try
		{
			var instruction =
				instructions.Where((t, x) => t.OpCode == "box" && instructions[x - 1].OpCode == $"ldloc_{index}").Single();

			if (instruction.Operand != null)
			{
				return instruction.Operand.ToString().Split(['|'], 3)[1];
			}
		}
		catch
		{
			// ignore
		}

		return "System.Object";
	}
}
