using System.Text;
using System.Text.Json;
using Carbon.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;

namespace Carbon.Output;

internal sealed class HookahOutputWriter(string outputFolder, string managedFolder, bool important, bool deterministic, bool formatOutput)
{
	public void CleanOutputFolder()
	{
		foreach (var file in Directory.EnumerateFiles(outputFolder, "*.cs"))
		{
			File.Delete(file);
		}
	}

	public void Write(HookGenerationReport report, string gameProtocol)
	{
		foreach (var group in report.SuccessfulHooks.GroupBy(x => x.Category).OrderBy(x => x.Min(y => y.Order)))
		{
			var outFile = Path.Combine(outputFolder, $"{group.Key}.cs");
			var source = BuildCategoryHeader(gameProtocol) + string.Concat(group.OrderBy(x => x.Order).Select(x => x.Source));
			File.WriteAllText(outFile, ProcessSource(source));
		}

		File.WriteAllText(Path.Combine(outputFolder, "__example.cs.txt"), ProcessSource(BuildExampleSource(report, gameProtocol)));
		File.WriteAllText(Path.Combine(outputFolder, "__HookahRuntime.cs"), GetRuntimeHelpersSource());
		File.WriteAllText(Path.Combine(outputFolder, "_Meta.cs"), BuildMetaSource());
	}

	public static void WriteSummary(HookGenerationReport report, string gameProtocol, string summaryPath)
	{
		var directory = Path.GetDirectoryName(Path.GetFullPath(summaryPath));
		if (!string.IsNullOrEmpty(directory))
		{
			Directory.CreateDirectory(directory);
		}

		var summary = new
		{
			gameProtocol,
			generatedCount = report.SuccessfulHooks.Count,
			failedCount = report.FailedHooks.Count,
			generated = report.SuccessfulHooks.Select(ToSummaryItem).ToArray(),
			failed = report.FailedHooks.Select(ToSummaryItem).ToArray(),
		};

		File.WriteAllText(summaryPath, JsonSerializer.Serialize(summary, new JsonSerializerOptions { WriteIndented = true }));
	}

	private static object ToSummaryItem(HookGenerationResult result)
	{
		return new
		{
			result.Order,
			result.Name,
			result.HookName,
			result.Category,
			result.Action,
			result.BaseHookName,
			result.Error,
		};
	}

	private string BuildCategoryHeader(string gameProtocol)
	{
		var body = new StringBuilder();
		body.AppendLine("using System;");
		body.AppendLine("using System.Collections.Generic;");
		body.AppendLine("using System.Linq;");
		body.AppendLine("using System.Reflection;");
		body.AppendLine("using System.Reflection.Emit;");
		body.AppendLine("using System.Security.Cryptography;");
		body.AppendLine("using API.Hooks;");
		body.AppendLine("using Carbon;");
		body.AppendLine("using HarmonyLib;");
		body.AppendLine("using Oxide.Core;");
		body.AppendLine("using UnityEngine;");
		body.AppendLine();
		body.AppendLine($"// Using game protocol {gameProtocol}");
		body.AppendLine($"// Auto-generated at {GetGeneratedTimestamp()}");
		body.AppendLine();
		body.AppendLine("namespace Carbon.Hooks;");
		body.AppendLine("#pragma warning disable IDE0051");
		body.AppendLine("#pragma warning disable IDE0060");
		body.AppendLine();
		return body.ToString();
	}

	private string BuildExampleSource(HookGenerationReport report, string gameProtocol)
	{
		var code = new StringBuilder();
		string[] blacklist =
		[
			"OnWireClear",
			"OnPlayerInput",
			"OnPlayerTick",
			"OnTick",
			"OnFrame",
			"OnQueueCycle",
		];

		foreach (var hook in report.SuccessfulHooks.Select(x => x.HookName).Distinct())
		{
			if (!hook.Contains(' ') && !hook.Contains('/'))
			{
				code.Append(!blacklist.Contains(hook) ? $"private void {hook}() => Puts(\"{hook} event was triggered\");\n" : $"private void {hook}() {{ }}\n");
			}
		}

		var example = new StringBuilder();
		example.AppendLine("using System;");
		example.AppendLine("using System.Net;");
		example.AppendLine("using ProtoBuf;");
		example.AppendLine("using UnityEngine;");
		example.AppendLine();
		example.AppendLine($"// Built at {GetGeneratedTimestamp()}");
		example.AppendLine();
		example.AppendLine("namespace Oxide.Plugins;");
		example.AppendLine();
		example.AppendLine($"[Info(\"OxideExample\", \"Carbon Community\", \"{gameProtocol}\")]");
		example.AppendLine("class OxideExample : RustPlugin {");
		example.AppendLine(code.ToString());
		example.AppendLine("}");
		return example.ToString();
	}

	private string BuildMetaSource()
	{
		return $@"namespace Carbon.Hooks;

public class _Meta
{{
	public static readonly string Checksum = ""{File.ReadAllText(Path.Combine(managedFolder, ".hash"))}"";

	public static readonly bool Important = {important.ToString().ToLower()};
}}";
	}

	private string GetGeneratedTimestamp()
	{
		return deterministic ? "1970-01-01 00:00:00" : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
	}

	private string ProcessSource(string source)
	{
		return formatOutput ? Format(source) : source;
	}

	private static string Format(string source)
	{
		return Formatter.Format(SyntaxFactory.ParseSyntaxTree(source).GetCompilationUnitRoot(), new AdhocWorkspace()).ToString();
	}

	private static string GetRuntimeHelpersSource()
	{
		return
			"""
			using System;
			using System.Collections.Generic;
			using System.Reflection;
			using System.Reflection.Emit;
			using System.Runtime.CompilerServices;
			using HarmonyLib;

			namespace Carbon.Hooks;

			internal static class __HookahRuntime
			{
				private static readonly ConditionalWeakTable<ILGenerator, Dictionary<int, LocalBuilder>> SyntheticLocals = new();

				private static int GetOriginalLocalCount(MethodBase method) => method?.GetMethodBody()?.LocalVariables?.Count ?? 0;

				private static LocalBuilder GetOrDeclareSyntheticLocal(ILGenerator generator, int requestedSlot, Type localType)
				{
					var locals = SyntheticLocals.GetOrCreateValue(generator);
					if (locals.TryGetValue(requestedSlot, out var existing))
					{
						return existing;
					}

					var declared = generator.DeclareLocal(localType ?? typeof(object));
					locals[requestedSlot] = declared;
					return declared;
				}

				private static CodeInstruction CreateOriginalLoad(int requestedSlot) => requestedSlot switch
				{
					0 => new CodeInstruction(OpCodes.Ldloc_0),
					1 => new CodeInstruction(OpCodes.Ldloc_1),
					2 => new CodeInstruction(OpCodes.Ldloc_2),
					3 => new CodeInstruction(OpCodes.Ldloc_3),
					_ => new CodeInstruction(OpCodes.Ldloc, requestedSlot)
				};

				private static CodeInstruction CreateOriginalStore(int requestedSlot) => requestedSlot switch
				{
					0 => new CodeInstruction(OpCodes.Stloc_0),
					1 => new CodeInstruction(OpCodes.Stloc_1),
					2 => new CodeInstruction(OpCodes.Stloc_2),
					3 => new CodeInstruction(OpCodes.Stloc_3),
					_ => new CodeInstruction(OpCodes.Stloc, requestedSlot)
				};

				private static CodeInstruction CreateOriginalAddressLoad(int requestedSlot) => requestedSlot switch
				{
					_ => new CodeInstruction(OpCodes.Ldloca, requestedSlot)
				};

				public static CodeInstruction CreateLoadLocalInstruction(ILGenerator generator, MethodBase method, int requestedSlot, Type syntheticLocalType)
				{
					if (requestedSlot < GetOriginalLocalCount(method))
					{
						return CreateOriginalLoad(requestedSlot);
					}

					return new CodeInstruction(OpCodes.Ldloc, GetOrDeclareSyntheticLocal(generator, requestedSlot, syntheticLocalType));
				}

				public static CodeInstruction CreateStoreLocalInstruction(ILGenerator generator, MethodBase method, int requestedSlot, Type syntheticLocalType)
				{
					if (requestedSlot < GetOriginalLocalCount(method))
					{
						return CreateOriginalStore(requestedSlot);
					}

					return new CodeInstruction(OpCodes.Stloc, GetOrDeclareSyntheticLocal(generator, requestedSlot, syntheticLocalType));
				}

				public static CodeInstruction CreateLoadLocalAddressInstruction(ILGenerator generator, MethodBase method, int requestedSlot, Type syntheticLocalType)
				{
					if (requestedSlot < GetOriginalLocalCount(method))
					{
						return CreateOriginalAddressLoad(requestedSlot);
					}

					return new CodeInstruction(OpCodes.Ldloca, GetOrDeclareSyntheticLocal(generator, requestedSlot, syntheticLocalType));
				}
			}
			""";
	}
}
