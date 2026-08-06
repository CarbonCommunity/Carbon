using System.Globalization;
using System.Reflection.Emit;

namespace Carbon.Validation.Metadata;

/// <summary>
///     A decoded IL instruction. <see cref="Text" /> matches Mono.Cecil 0.9.5's Instruction.ToString()
///     operand rendering (what the Oxide patcher hashes); <see cref="AlignmentKey" /> is the same
///     content with branch targets normalized to relative distances so two builds of a method can be
///     compared position-independently.
/// </summary>
internal sealed class IlInstruction
{
	public int Index { get; init; }

	public int Offset { get; init; }

	public required string Name { get; init; }

	/// <summary>Cecil-style operand text, or null when the opcode has no operand.</summary>
	public string? Operand { get; set; }

	/// <summary>Position-independent comparison key ("opcode:operand").</summary>
	public string AlignmentKey { get; set; } = string.Empty;

	/// <summary>True when the opcode is a branch (including unresolved targets).</summary>
	public bool IsBranch { get; set; }

	/// <summary>Target instruction index for branch opcodes, -1 otherwise.</summary>
	public int BranchTarget { get; set; } = -1;

	public int[]? SwitchTargets { get; set; }

	public string Text => Operand == null ? $"IL_{Offset:x4}: {Name}" : $"IL_{Offset:x4}: {Name} {Operand}";

	public override string ToString()
	{
		return Text;
	}
}

/// <summary>
///     Decodes raw IL bytes into <see cref="IlInstruction" />s using the System.Reflection.Emit
///     opcode table, rendering operands the way Mono.Cecil 0.9.5 does.
/// </summary>
internal static class IlDecoder
{
	private static readonly OpCode?[] SingleByte = new OpCode?[0x100];
	private static readonly OpCode?[] TwoByte = new OpCode?[0x100];

	// Cecil 0.9.5 names a few opcodes differently than System.Reflection.Emit.
	private static readonly Dictionary<string, string> NameOverrides = new(StringComparer.Ordinal)
	{
		["ldelem"] = "ldelem.any",
		["stelem"] = "stelem.any",
	};

	static IlDecoder()
	{
		foreach (var field in typeof(OpCodes).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
		{
			if (field.GetValue(null) is not OpCode opCode)
			{
				continue;
			}

			if (opCode.Size == 1)
			{
				SingleByte[opCode.Value & 0xFF] = opCode;
			}
			else
			{
				TwoByte[opCode.Value & 0xFF] = opCode;
			}
		}
	}

	public static IlInstruction[] Decode(byte[] il, MetadataMethod method)
	{
		var instructions = new List<IlInstruction>();
		var offsetToIndex = new Dictionary<int, int>();
		var branchTargets = new List<(int InstructionIndex, int TargetOffset)>();
		var switchTargets = new List<(int InstructionIndex, int[] TargetOffsets)>();
		var position = 0;

		while (position < il.Length)
		{
			var offset = position;
			var code = il[position++];
			OpCode? opCode;
			if (code == 0xFE)
			{
				opCode = TwoByte[il[position++]];
			}
			else
			{
				opCode = SingleByte[code];
			}

			if (opCode == null)
			{
				throw new BadImageFormatException($"unknown opcode 0x{code:x2} at IL_{offset:x4}");
			}

			var name = NameOverrides.TryGetValue(opCode.Value.Name!, out var overridden) ? overridden : opCode.Value.Name!;
			var instruction = new IlInstruction { Index = instructions.Count, Offset = offset, Name = name };

			switch (opCode.Value.OperandType)
			{
				case OperandType.InlineNone:
					break;

				case OperandType.ShortInlineBrTarget:
				{
					var delta = (sbyte)il[position];
					position += 1;
					branchTargets.Add((instruction.Index, position + delta));
					break;
				}

				case OperandType.InlineBrTarget:
				{
					var delta = BitConverter.ToInt32(il, position);
					position += 4;
					branchTargets.Add((instruction.Index, position + delta));
					break;
				}

				case OperandType.InlineSwitch:
				{
					var count = BitConverter.ToInt32(il, position);
					position += 4;
					var targets = new int[count];
					var baseOffset = position + count * 4;
					for (var i = 0; i < count; i++)
					{
						targets[i] = baseOffset + BitConverter.ToInt32(il, position);
						position += 4;
					}

					switchTargets.Add((instruction.Index, targets));
					break;
				}

				case OperandType.InlineString:
					instruction.Operand = "\"" + method.RenderUserString(BitConverter.ToInt32(il, position)) + "\"";
					position += 4;
					break;

				case OperandType.InlineMethod:
				case OperandType.InlineField:
				case OperandType.InlineType:
				case OperandType.InlineTok:
				case OperandType.InlineSig:
					instruction.Operand = method.RenderTokenOperand(BitConverter.ToInt32(il, position));
					position += 4;
					break;

				case OperandType.ShortInlineI:
					// Cecil types ldc.i4.s as sbyte and unaligned.'s operand as byte.
					instruction.Operand = name == "unaligned."
						? il[position].ToString(CultureInfo.InvariantCulture)
						: ((sbyte)il[position]).ToString(CultureInfo.InvariantCulture);
					position += 1;
					break;

				case OperandType.InlineI:
					instruction.Operand = BitConverter.ToInt32(il, position).ToString(CultureInfo.InvariantCulture);
					position += 4;
					break;

				case OperandType.InlineI8:
					instruction.Operand = BitConverter.ToInt64(il, position).ToString(CultureInfo.InvariantCulture);
					position += 8;
					break;

				case OperandType.ShortInlineR:
				{
					// "G7" matches .NET Framework's default float formatting, which the patcher
					// used; negative zero also rendered "0" there (changed in .NET Core 3.0).
					var value = BitConverter.ToSingle(il, position);
					instruction.Operand = value == 0f ? "0" : value.ToString("G7", CultureInfo.InvariantCulture);
					position += 4;
					break;
				}

				case OperandType.InlineR:
				{
					var value = BitConverter.ToDouble(il, position);
					instruction.Operand = value == 0d ? "0" : value.ToString("G15", CultureInfo.InvariantCulture);
					position += 8;
					break;
				}

				case OperandType.ShortInlineVar:
					instruction.Operand = RenderVariableOperand(method, name, il[position]);
					position += 1;
					break;

				case OperandType.InlineVar:
					instruction.Operand = RenderVariableOperand(method, name, BitConverter.ToUInt16(il, position));
					position += 2;
					break;

				default:
					throw new BadImageFormatException($"unsupported operand type {opCode.Value.OperandType} at IL_{offset:x4}");
			}

			offsetToIndex.Add(offset, instruction.Index);
			instructions.Add(instruction);
		}

		foreach (var (instructionIndex, targetOffset) in branchTargets)
		{
			var instruction = instructions[instructionIndex];
			instruction.IsBranch = true;
			instruction.BranchTarget = offsetToIndex.TryGetValue(targetOffset, out var targetIndex) ? targetIndex : -1;
			instruction.Operand = $"IL_{targetOffset:x4}";
		}

		foreach (var (instructionIndex, targetOffsets) in switchTargets)
		{
			var instruction = instructions[instructionIndex];
			instruction.SwitchTargets = targetOffsets
				.Select(x => offsetToIndex.TryGetValue(x, out var targetIndex) ? targetIndex : -1)
				.ToArray();
			instruction.Operand = string.Join(",", targetOffsets.Select(x => $"IL_{x:x4}"));
		}

		for (var i = 0; i < instructions.Count; i++)
		{
			var instruction = instructions[i];
			instruction.AlignmentKey = BuildAlignmentKey(instruction);
		}

		return [.. instructions];
	}

	private static string RenderVariableOperand(MetadataMethod method, string name, int index)
	{
		// Locals print as "V_<n>" (no symbols are loaded); arguments print as the parameter name.
		return name.StartsWith("ldloc", StringComparison.Ordinal) || name.StartsWith("stloc", StringComparison.Ordinal)
			? "V_" + index.ToString(CultureInfo.InvariantCulture)
			: method.RenderArgumentOperand(index);
	}

	private static string BuildAlignmentKey(IlInstruction instruction)
	{
		if (instruction.IsBranch)
		{
			return $"{instruction.Name}:b{instruction.BranchTarget - instruction.Index}";
		}

		if (instruction.SwitchTargets != null)
		{
			return $"{instruction.Name}:sw{string.Join(",", instruction.SwitchTargets.Select(x => x - instruction.Index))}";
		}

		return instruction.Operand == null ? instruction.Name : $"{instruction.Name}:{instruction.Operand}";
	}
}
