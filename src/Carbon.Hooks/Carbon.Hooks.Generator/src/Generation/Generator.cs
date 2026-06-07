using System.Collections.Concurrent;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Carbon.Projects.Oxide;
using Carbon.Utility;
using static Carbon.Projects.Oxide.Oxide;

namespace Carbon.Generation;

internal sealed partial class Generator(GeneratorOptions options)
{
	private static readonly HashSet<string> LocalBlacklist = new(StringComparer.Ordinal)
	{
		"ConsumptionAmountField [AutoTurret]",
		"SplashThresholdField [SprayCanSpray]",
		"IOnPlayerChat[patch]",
	};

	private readonly HookValidator _validator = new(options.ValidationMode);

	public HookGenerationReport Generate(Oxide project)
	{
		ConcurrentBag<HookGenerationResult> generatedHooks = [];
		ConcurrentBag<HookGenerationResult> failedHooks = [];
		var order = 0;
		var parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = options.Jobs > 0 ? options.Jobs : Environment.ProcessorCount,
		};

		for (var manifestIndex = 0; manifestIndex < project.Manifests.Count; manifestIndex++)
		{
			var manifest = project.Manifests[manifestIndex];
			Utility.Program.ResolveAssembly(Path.GetFileNameWithoutExtension(manifest.AssemblyName));

			var actions = manifest.Hooks.Select(x => x.Type).Distinct().ToList();
			for (var actionIndex = 0; actionIndex < actions.Count; actionIndex++)
			{
				var action = actions[actionIndex];
				var hooks = manifest.Hooks.Where(x => x.Type == action).Select(x => x.Hook).ToList();
				switch (action)
				{
					case "Simple":
					case "Modify":
						var hookWork = hooks
							.Select(hook => new HookWorkItem(manifest, action, hook, Interlocked.Increment(ref order)))
							.ToArray();

						Parallel.ForEach(hookWork, parallelOptions, workItem =>
						{
							var result = GenerateHookSafely(workItem);
							if (result.Success)
							{
								generatedHooks.Add(result);
							}
							else
							{
								failedHooks.Add(result);
							}
						});
						break;

					case "InitOxide":
						break;

					default:
						for (var hookIndex = 0; hookIndex < hooks.Count; hookIndex++)
						{
							var hook = hooks[hookIndex];
							Logger.Warning($"{hook.HookName} '{action} {hook.TypeName}' not implemented");
							failedHooks.Add(HookGenerationResult.Failed(Interlocked.Increment(ref order), hook, action));
						}

						break;
				}
			}
		}

		var successfulResults = generatedHooks.OrderBy(x => x.Order).ToList();
		var failedResults = failedHooks.OrderBy(x => x.Order).ToList();
		WarnOnMissingDependencies(successfulResults);

		return new HookGenerationReport(successfulResults, failedResults);
	}

	private HookGenerationResult GenerateHookSafely(HookWorkItem workItem)
	{
		try
		{
			return GenerateHook(workItem);
		}
		catch (Exception) when (options.ValidationMode == ValidationMode.Fail)
		{
			throw;
		}
		catch (Exception ex)
		{
			Logger.Error($"{workItem.Hook.HookName} unexpected generation error: {ex.Message}");
			return HookGenerationResult.Failed(workItem.Order, workItem.Hook, workItem.Action, ex.Message);
		}
	}

	private static void WarnOnMissingDependencies(IReadOnlyCollection<HookGenerationResult> successfulResults)
	{
		var generatedNames = successfulResults
			.Select(x => x.Name)
			.ToHashSet(StringComparer.Ordinal);

		foreach (var result in successfulResults)
		{
			if (string.IsNullOrEmpty(result.BaseHookName) || generatedNames.Contains(result.BaseHookName))
			{
				continue;
			}

			Logger.Warning(
				$"{result.HookName} validation: dependency '{result.BaseHookName}' was not generated; injection index may be invalid");
		}
	}

	private HookGenerationResult GenerateHook(HookWorkItem workItem)
	{
		var hook = workItem.Hook;
		Helper.ResetGenerationState(options.Deterministic);

		if (Helper.HookBlacklist.Contains(hook.HookName))
		{
			return HookGenerationResult.Failed(workItem.Order, hook, workItem.Action);
		}

		if (string.IsNullOrEmpty(hook.HookCategory))
		{
			hook.HookCategory = "None";
		}

		if (LocalBlacklist.Contains(hook.HookName))
		{
			Logger.Warning($"{hook.HookName} is blacklisted");
			return HookGenerationResult.Failed(workItem.Order, hook, workItem.Action);
		}

		Type? targetType;
		MethodInfo? targetMethod;

		try
		{
			var resolvedTypeName = HookPolicies.GetEmittedTargetTypeName(hook);
			var resolvedMethodName = HookPolicies.GetEmittedTargetMethodName(hook);
			var resolvedMethodArgs = HookPolicies.GetEmittedTargetMethodArgs(hook);

			targetType = Tools.TypeByNameEx(resolvedTypeName);
			var targetName = Tools.TypeNameSanitizerEx(resolvedTypeName);
			targetMethod = Tools.MethodByNameEx(targetType, resolvedMethodName, resolvedMethodArgs);
			if (targetType == null || targetMethod == null || targetName == null)
			{
				throw new Exception("is null");
			}

			GenericArityRegex().Replace(targetName, "<>");
		}
		catch (Exception e)
		{
			Logger.Warning($"{hook.HookName} type '{hook.TypeName}' {e.Message}");
			return HookGenerationResult.Failed(workItem.Order, hook, workItem.Action);
		}

		if (!_validator.Validate(workItem.Action, hook, targetMethod))
		{
			return HookGenerationResult.Failed(workItem.Order, hook, workItem.Action);
		}

		var body = new StringBuilder();
		var hookCategory = KeepLettersAndDigits(hook.HookCategory);
		var hookTypeName = KeepLettersAndDigits(hook.TypeName);
		var emittedTypeName = HookPolicies.GetEmittedTargetTypeName(hook);
		var emittedMethodName = HookPolicies.GetEmittedTargetMethodName(hook);
		var emittedMethodArgs = HookPolicies.GetEmittedTargetMethodArgs(hook);
		var hookMethodArgs = string.Join(",", emittedMethodArgs.Select(x => $"\"{x}\"")).Trim();

		var identifier = GetHookIdentifier(hook);
		var className = $"{hookCategory}_{hookTypeName}_{identifier}";

		body.AppendLine($"public partial class Category_{hookCategory} {{");
		body.AppendLine($"public partial class {hookCategory}_{hookTypeName} {{");
		body.AppendLine(
			$"[HookAttribute.Patch(\"{hook.HookName}\", \"{hook.Name}\", \"{emittedTypeName}\", \"{emittedMethodName}\", [{hookMethodArgs}])]");
		body.AppendLine($"[HookAttribute.Identifier(\"{identifier}\")]");

		if (hook.BaseHookName != null && HookPolicies.ShouldEmitDependencyAttribute(hook))
		{
			body.AppendLine($"[HookAttribute.Dependencies(new System.String[] {{ \"{hook.BaseHookName}\" }})]");
		}

		if (hook.HookName.Contains(' '))
		{
			body.AppendLine("[HookAttribute.Options(HookFlags.Patch)]");
		}
		else if (hook.HookName.StartsWith("I"))
		{
			body.AppendLine("[HookAttribute.Options(HookFlags.Static | HookFlags.Hidden)]");
		}
		else
		{
			body.AppendLine("[HookAttribute.Options(HookFlags.None)]");
		}

		var metadataIndex = body.Length;
		Helper.Parameters.Clear();
		body.AppendLine($"""[MetadataAttribute.Category("{hook.HookCategory}")]""");
		body.AppendLine($"""[MetadataAttribute.Assembly("{hook.AssemblyName}")]""");
		body.AppendLine($"public class {className} : API.Hooks.Patch {{");

		if (!HookPolicies.TryGeneratePolicyBody(body, hook))
		{
			body.AppendLine(
				"public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator, MethodBase Method) {");

			switch (workItem.Action)
			{
				case "Simple":
					if (!GenerateSimpleTranspiler(body, hook, targetType, targetMethod))
					{
						return HookGenerationResult.Failed(workItem.Order, hook, workItem.Action);
					}

					break;
				case "Modify":
					if (!GenerateModifyTranspiler(body, hook, targetType, targetMethod))
					{
						return HookGenerationResult.Failed(workItem.Order, hook, workItem.Action);
					}

					break;
			}
		}

		EmitMetadata(body, metadataIndex);
		return HookGenerationResult.Generated(workItem.Order, hook, body.ToString());
	}

	private static bool GenerateSimpleTranspiler(StringBuilder body, HookDef.Data hook, Type targetType, MethodInfo targetMethod)
	{
		var offset = 0;

		if (hook.ArgumentBehavior == ArgumentBehavior.UseArgumentString)
		{
			Tools.ArgumentParser(hook.ArgumentString.Trim(), out var argvar, out var retvar);
			var match = ReturnLocalRegex().Match(string.Join(",", argvar));
			if (match.Success && int.TryParse(match.Groups[1].Value, out var index))
			{
				offset = 2;
				body.AppendLine("List<CodeInstruction> edit = new List<CodeInstruction>();");
				body.AppendLine("List<CodeInstruction> original = new List<CodeInstruction>(Instructions);");
				body.AppendLine(string.Empty);
				body.AppendLine($"Type rettype = ((MethodInfo)original[{index - 1}].operand).ReturnType;");
				body.AppendLine("object retvar = Generator.DeclareLocal(rettype);");
				body.AppendLine(string.Empty);
				body.AppendLine("edit.Add(new CodeInstruction(OpCodes.Stloc_S, retvar));");
				body.AppendLine("edit.Add(new CodeInstruction(OpCodes.Ldloc_S, retvar));");
				body.AppendLine(string.Empty);
				body.AppendLine($"original.InsertRange({index}, edit);");
				body.AppendLine("Instructions = original.AsEnumerable();");
				body.AppendLine(string.Empty);
			}
		}

		var useExitLeave = (targetMethod.GetMethodBody()?.ExceptionHandlingClauses.Count ?? 0) > 0
			&& targetMethod.ReturnType == typeof(void)
			&& (hook.ReturnBehavior == ReturnBehavior.ExitWhenNonNull || hook.ReturnBehavior == ReturnBehavior.ExitWhenValidType);
		Helper.PendingExitLeaveLabel = useExitLeave ? "lastLabel" : null;

		body.AppendLine("int x = 0;");
		body.AppendLine("foreach (CodeInstruction instruction in Instructions) {");
		body.AppendLine($"if (x++ != {hook.InjectionIndex + offset}) {{");
		body.AppendLine("yield return instruction;");
		body.AppendLine("continue;");
		body.AppendLine("}");
		Helper.ArmInstructionStateTransfer("instruction", targetMethod.GetMethodBody()?.ExceptionHandlingClauses.Count == 0);

		if (!Helper.GenerateSimpleBodyEx(ref body, targetType, targetMethod, hook))
		{
			Logger.Warning($"GenerateSimpleBodyEx for '{hook.TypeName}' failed");
			return false;
		}

		body.AppendLine(string.Empty);
		body.AppendLine("yield return instruction;");
		body.AppendLine("} } } } }");
		body.AppendLine();
		return true;
	}

	private static bool GenerateModifyTranspiler(StringBuilder body, HookDef.Data hook, Type targetType, MethodInfo targetMethod)
	{
		body.AppendLine("List<CodeInstruction> edit = new List<CodeInstruction>();");
		body.AppendLine("List<CodeInstruction> original = new List<CodeInstruction>(Instructions);");
		body.AppendLine(string.Empty);
		HookPolicies.TryEmitModifyAnchorPrelude(body, hook);
		HookPolicies.ConfigureModifyAnchorPolicy(hook);

		if (!Helper.GenerateModifyBodyEx(ref body, targetType, targetMethod, hook))
		{
			Helper.ClearModifyAnchor();
			Logger.Warning($"GenerateModifyBodyEx for '{hook.TypeName}' failed");
			return false;
		}

		body.AppendLine(string.Empty);
		var injectionIndexExpression = Helper.GetOriginalIndexExpression(hook.InjectionIndex);
		body.AppendLine(
			$"if (edit.Count > 0) edit[0].MoveLabelsFrom(original[{injectionIndexExpression}]).MoveBlocksFrom(original[{injectionIndexExpression}]);");
		if (hook.RemoveCount > 0)
		{
			var removalIndexExpression = Helper.GetOriginalIndexExpression(hook.InjectionIndex + hook.RemoveCount);
			for (var index = hook.InjectionIndex + 1; index < hook.InjectionIndex + hook.RemoveCount; index++)
			{
				var removedIndexExpression = Helper.GetOriginalIndexExpression(index);
				body.AppendLine(
					$"if (edit.Count > 0) edit[0].labels.AddRange(original[{removedIndexExpression}].labels); else original[{removalIndexExpression}].labels.AddRange(original[{removedIndexExpression}].labels);");
				body.AppendLine($"original[{removedIndexExpression}].labels.Clear();");
			}

			body.AppendLine(
				$"if (edit.Count == 0) original[{removalIndexExpression}].MoveLabelsFrom(original[{injectionIndexExpression}]).MoveBlocksFrom(original[{injectionIndexExpression}]);");
			body.AppendLine($"original.RemoveRange({injectionIndexExpression}, {hook.RemoveCount});");
		}

		body.AppendLine($"original.InsertRange({injectionIndexExpression}, edit);");
		Helper.FlushPendingOriginalLabelAssignments(ref body);
		body.AppendLine("return original.AsEnumerable();");
		body.AppendLine("} } } }");
		body.AppendLine();
		Helper.ClearModifyAnchor();
		return true;
	}

	private static void EmitMetadata(StringBuilder body, int metadataIndex)
	{
		Helper.ParametersTemp.Clear();
		for (var parameterIndex = 0; parameterIndex < Helper.Parameters.Count; parameterIndex++)
		{
			var parameter = Helper.Parameters[parameterIndex];
			try
			{
				var fixedName = parameter.Item1.Replace("`", string.Empty).Replace("[]", string.Empty);
				var name = fixedName[..1].ToLower() + fixedName[1..];
				var multiCount = Helper.ParametersTemp.Count(x => x == name);
				var finalName = multiCount > 0 ? $"{name}{multiCount}" : name;
				var index = $"""[MetadataAttribute.Parameter("{finalName}", "{parameter.Item2}")]""";
				body.Insert(metadataIndex, index);
				metadataIndex += index.Length;
				Helper.ParametersTemp.Add(name);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}

		if (Helper.ReturnType != null)
		{
			body.Insert(metadataIndex, $"[MetadataAttribute.Return(typeof({PrettyName(Helper.ReturnType)}))]");
		}
	}

	private static string PrettyName(Type type)
	{
		if (type == typeof(void))
		{
			return "void";
		}

		try
		{
			if (type.GetGenericArguments().Length == 0)
			{
				return Tools.TypeNameSanitizerEx(type.FullName ?? type.Name);
			}

			var genericArguments = type.GetGenericArguments();
			var typeDefinition = type.FullName ?? type.Name;
			var unmangledName = typeDefinition.Contains('`') ? typeDefinition[..typeDefinition.IndexOf('`')] : typeDefinition;
			return unmangledName + "<" + string.Join(",", genericArguments.Select(PrettyName)) + ">";
		}
		catch (Exception ex)
		{
			Logger.Error(ex);
		}

		return string.Empty;
	}

	private string GetHookIdentifier(HookDef.Data hook)
	{
		if (!options.Deterministic)
		{
			return $"{Guid.NewGuid():N}";
		}

		var identity = string.Join("|",
			hook.AssemblyName,
			hook.TypeName,
			hook.Signature?.Name,
			string.Join(",", hook.Signature?.Parameters ?? []),
			hook.Name,
			hook.HookName,
			hook.HookCategory);
		return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(identity))).ToLowerInvariant()[..32];
	}

	[GeneratedRegex(@"(`\d)")]
	private static partial Regex GenericArityRegex();

	private static string KeepLettersAndDigits(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}

		var buffer = new char[value.Length];
		var count = 0;
		for (var i = 0; i < value.Length; i++)
		{
			var c = value[i];
			if (char.IsLetterOrDigit(c))
			{
				buffer[count++] = c;
			}
		}

		return new string(buffer, 0, count);
	}

	[GeneratedRegex(@"(?i)r(\d+)", RegexOptions.Compiled, "")]
	private static partial Regex ReturnLocalRegex();
}
