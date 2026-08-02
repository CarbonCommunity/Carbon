using System.Security.Cryptography;
using System.Text;
using Carbon.Validation.Metadata;

namespace Carbon.Validation;

/// <summary>
///     Recomputes the MSILHash the Oxide patcher stores per hook: SHA-256 over the UTF-8 bytes of a
///     Mono.Cecil 0.9.5-style disassembly of the method body, one "IL_xxxx: opcode operand" line per
///     instruction with "\n" escaped, CRLF line breaks and a trailing line break, Base64-encoded.
///     See Oxide.Patcher's ILWeaver.Hash / ILWeaver.ToString().
/// </summary>
internal static class MsilHash
{
	public static string Compute(IlInstruction[] instructions)
	{
		var text = new StringBuilder();
		for (var i = 0; i < instructions.Length; i++)
		{
			text.Append(instructions[i].Text.Replace("\n", "\\n"));
			text.Append("\r\n");
		}

		return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(text.ToString())));
	}
}
