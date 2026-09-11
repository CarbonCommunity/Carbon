using System.Reflection;
using System.Text.RegularExpressions;
using Carbon.Generation;
using Carbon.Utility;
using Carbon.Validation.Metadata;
using HarmonyLib;
using static Carbon.Projects.Oxide.Oxide;

namespace Carbon.Validation;

/// <summary>
///     Validates every hook of an OPJ project against the current game assemblies: do the referenced
///     methods still exist, do the recorded signatures still match, and do the recorded injection
///     indexes still point at the right instructions. When the previous game build is available
///     (old managed folder), drifted injection indexes are re-anchored by aligning the old and new
///     IL, producing automatic fixes; everything too ambiguous is reported for human review.
/// </summary>
internal sealed partial class OpjChecker(OpjCheckOptions options) : IDisposable
{
	private readonly MetadataAssemblySet _current = new(options.ManagedFolder);
	private readonly MetadataAssemblySet? _old = options.OldManagedFolder == null ? null : new MetadataAssemblySet(options.OldManagedFolder);
	private readonly Dictionary<string, HookDef> _hooksByName = new(StringComparer.Ordinal);
	private readonly Dictionary<string, int> _hooksPerTarget = new(StringComparer.Ordinal);
	private readonly Dictionary<string, OpjHookCheck> _checksByName = new(StringComparer.Ordinal);
	private readonly List<(OpjHookCheck Check, HookDef.Data Hook)> _checkedHooks = [];
	private readonly Dictionary<OpjHookCheck, string?> _anchoredToBase = [];

	public OpjCheckReport Check(Projects.Oxide.Oxide project)
	{
		var report = new OpjCheckReport();

		foreach (var manifest in project.Manifests)
		{
			foreach (var hookDef in manifest.Hooks)
			{
				_hooksByName.TryAdd(hookDef.Hook.Name, hookDef);
				var target = GetTargetKey(hookDef.Hook);
				_hooksPerTarget[target] = _hooksPerTarget.TryGetValue(target, out var count) ? count + 1 : 1;
			}
		}

		for (var manifestIndex = 0; manifestIndex < project.Manifests.Count; manifestIndex++)
		{
			var manifest = project.Manifests[manifestIndex];
			Utility.Program.ResolveAssembly(Path.GetFileNameWithoutExtension(manifest.AssemblyName));

			for (var hookIndex = 0; hookIndex < manifest.Hooks.Count; hookIndex++)
			{
				var hookDef = manifest.Hooks[hookIndex];
				var hook = hookDef.Hook;
				var check = new OpjHookCheck(manifestIndex, hookIndex, hookDef.Type, hook.AssemblyName, hook.Name, hook.HookName,
					hook.TypeName, hook.Signature?.Name ?? string.Empty, hook.Flagged);
				report.Hooks.Add(check);
				_checksByName.TryAdd(hook.Name, check);
				_checkedHooks.Add((check, hook));

				try
				{
					AnalyzeHook(check, hookDef.Type, hook);
				}
				catch (Exception ex)
				{
					check.Add(OpjIssueKind.NotImplemented, OpjIssueSeverity.NeedsHuman, $"analysis failed: {ex.Message}");
				}
			}
		}

		// Cross-hook passes: stacked hooks depend on their base hooks' geometry, so their fixes are
		// only trustworthy once the whole chain has been analyzed.
		GateFixesOnBaseChains();
		FollowBaseReanchors();

		return report;
	}

	/// <summary>
	///     A stacked hook's index translation borrows its base hooks' recorded geometry; when a base
	///     hook itself needs human attention on its indexes, that geometry will change, so the
	///     child's index rewrites must not be applied.
	/// </summary>
	private void GateFixesOnBaseChains()
	{
		foreach (var (check, hook) in _checkedHooks)
		{
			foreach (var baseHook in EnumerateSameMethodBaseChain(hook))
			{
				if (!_checksByName.TryGetValue(baseHook.Name, out var baseCheck)
				    || !baseCheck.Diagnostics.Any(x => x.Severity == OpjIssueSeverity.NeedsHuman
					    && x.Kind is OpjIssueKind.InjectionIndexDrift or OpjIssueKind.InjectionIndexOutOfRange
						    or OpjIssueKind.RemoveRangeInvalid))
				{
					continue;
				}

				DemoteIndexSpaceFixes(check, $"base hook '{baseHook.Name}' needs human attention first");
				break;
			}
		}
	}

	/// <summary>
	///     Hooks anchored into a base hook's inserted instructions use absolute indexes into the
	///     base-patched stream, so when the base hook's injection index is re-anchored the child's
	///     index shifts by the same delta.
	/// </summary>
	private void FollowBaseReanchors()
	{
		foreach (var (check, hook) in _checkedHooks)
		{
			if (!_anchoredToBase.TryGetValue(check, out var currentHash)
			    || string.IsNullOrEmpty(hook.BaseHookName)
			    || !_hooksByName.ContainsKey(hook.BaseHookName)
			    || !_checksByName.TryGetValue(hook.BaseHookName, out var baseCheck))
			{
				continue;
			}

			if (baseCheck.Diagnostics.Any(x => x.Severity == OpjIssueSeverity.NeedsHuman
				    && x.Kind is OpjIssueKind.InjectionIndexDrift or OpjIssueKind.InjectionIndexOutOfRange
					    or OpjIssueKind.RemoveRangeInvalid))
			{
				check.Add(OpjIssueKind.InjectionIndexDrift, OpjIssueSeverity.NeedsHuman,
					$"InjectionIndex {hook.InjectionIndex} anchors into instructions inserted by base hook '{hook.BaseHookName}', which needs human attention; resolve the base hook first");
				continue;
			}

			var baseFix = baseCheck.Diagnostics
				.Where(x => x.Severity == OpjIssueSeverity.AutoFixable)
				.SelectMany(x => x.Fixes)
				.FirstOrDefault(x => x.Path == "InjectionIndex");
			if (baseFix?.OldValue is not int oldIndex || baseFix.NewValue is not int newIndex || oldIndex == newIndex)
			{
				continue;
			}

			var delta = newIndex - oldIndex;
			if (hook.Instructions is { Count: > 0 }
			    && hook.Instructions.Any(x => x.OpType == HookDef.Data.OpType.Instruction && !x.ReferencesNewInstruction))
			{
				check.Add(OpjIssueKind.InjectionIndexDrift, OpjIssueSeverity.NeedsHuman,
					$"base hook '{hook.BaseHookName}' re-anchors by {delta:+#;-#} and this hook targets original instructions; review its indexes together with the base");
				continue;
			}

			var follow = check.Add(OpjIssueKind.InjectionIndexDrift, OpjIssueSeverity.AutoFixable,
				$"InjectionIndex follows base hook '{hook.BaseHookName}' re-anchor: {hook.InjectionIndex} -> {hook.InjectionIndex + delta}");
			follow.Fixes.Add(new OpjFixEdit("InjectionIndex", hook.InjectionIndex, hook.InjectionIndex + delta));
			if (currentHash != null && check.Severity <= OpjIssueSeverity.AutoFixable)
			{
				follow.Fixes.Add(new OpjFixEdit("MSILHash", hook.MsilHash, currentHash));
			}
		}
	}

	private IEnumerable<HookDef.Data> EnumerateSameMethodBaseChain(HookDef.Data hook)
	{
		var visited = new HashSet<string>(StringComparer.Ordinal);
		var current = hook;
		while (!string.IsNullOrEmpty(current.BaseHookName) && visited.Add(current.BaseHookName)
		       && _hooksByName.TryGetValue(current.BaseHookName, out var baseDef))
		{
			var baseHook = baseDef.Hook;
			if (baseHook.TypeName != hook.TypeName || baseHook.Signature?.Name != hook.Signature?.Name)
			{
				yield break;
			}

			yield return baseHook;
			current = baseHook;
		}
	}

	private static void DemoteIndexSpaceFixes(OpjHookCheck check, string reason)
	{
		foreach (var diagnostic in check.Diagnostics)
		{
			if (diagnostic.Severity == OpjIssueSeverity.AutoFixable
			    && diagnostic.Fixes.Any(x => x.Path == "InjectionIndex" || x.Path.StartsWith("Instructions[", StringComparison.Ordinal)))
			{
				diagnostic.Demote(reason);
			}
		}

		// The recorded hash must keep describing the same build as the recorded indexes.
		check.Diagnostics.RemoveAll(x => x.Severity == OpjIssueSeverity.AutoFixable && x.Fixes.Any(f => f.Path == "MSILHash"));
	}

	private static string GetTargetKey(HookDef.Data hook)
	{
		return $"{hook.AssemblyName}|{hook.TypeName}|{hook.Signature?.Name}|{string.Join(",", hook.Signature?.Parameters ?? [])}";
	}

	private void AnalyzeHook(OpjHookCheck check, string action, HookDef.Data hook)
	{
		if (hook.Signature == null)
		{
			check.Add(OpjIssueKind.NotImplemented, OpjIssueSeverity.NeedsHuman, "hook definition has no signature");
			return;
		}

		if (Projects.Oxide.Helper.HookBlacklist.Contains(hook.HookName)
		    || Projects.Oxide.Helper.PatchBlacklist.Contains(hook.Name)
		    || Generator.LocalBlacklist.Contains(hook.HookName))
		{
			check.Add(OpjIssueKind.NotImplemented, OpjIssueSeverity.Info, "hook is blacklisted in the generator; not validated");
			return;
		}

		if (action is not ("Simple" or "Modify" or "InitOxide"))
		{
			check.Add(OpjIssueKind.NotImplemented, OpjIssueSeverity.Warning,
				$"hook type '{action}' is not implemented by the generator; only the target reference is validated");
		}

		if (!string.IsNullOrEmpty(hook.AssemblyName)
		    && Utility.Program.ResolveAssembly(Path.GetFileNameWithoutExtension(hook.AssemblyName)) == null)
		{
			check.Add(OpjIssueKind.MissingAssembly, OpjIssueSeverity.NeedsHuman,
				$"assembly '{hook.AssemblyName}' was not found in the managed folder");
			return;
		}

		var typeName = hook.TypeName;
		var type = Tools.TypeByNameEx(typeName);
		if (type == null)
		{
			type = TryFixCompilerGeneratedTypeName(check, hook, ref typeName);
		}

		WarnOnPolicyCoupling(check, hook);

		if (type == null)
		{
			ReportMissingType(check, hook);
			return;
		}

		var declared = GetDeclaredMembers(type);
		var namesakes = declared.Where(x => x.Name == hook.Signature.Name).ToList();
		var method = namesakes.FirstOrDefault(x => SignatureComparer.ParametersMatch(x.GetParameters(), hook.Signature.Parameters));

		if (method != null)
		{
			AnalyzeReturnAndExposure(check, hook, method);
		}
		else if (namesakes.Count > 0)
		{
			method = SelectBestOverload(namesakes, hook.Signature.Parameters);
			if (method == null)
			{
				var ambiguous = check.Add(OpjIssueKind.AmbiguousMethod, OpjIssueSeverity.NeedsHuman,
					$"{namesakes.Count} overloads of '{hook.Signature.Name}' exist but none matches the recorded signature closely enough to pick one");
				ambiguous.Candidates.AddRange(namesakes.Select(DescribeMethod));
				return;
			}

			AnalyzeSignatureDrift(check, hook, method);
		}
		else
		{
			method = TryDetectRename(check, hook, type);
			if (method == null)
			{
				ReportMissingMethod(check, hook, type, declared);
				return;
			}
		}

		var current = _current.Resolve(method);
		AnalyzeArgumentReferences(check, hook, method, current);
		AnalyzeBody(check, action, hook, method, current);

		var sharedTarget = SharedTargetCount(hook);
		if (sharedTarget > 1 && check.Diagnostics.Any(x => x.Severity >= OpjIssueSeverity.Warning))
		{
			check.Add(OpjIssueKind.BodyDrift, OpjIssueSeverity.Info,
				$"{sharedTarget - 1} other hook(s) patch the same method; injection indexes may interact at patch time");
		}
	}

	private int SharedTargetCount(HookDef.Data hook)
	{
		return _hooksPerTarget.TryGetValue(GetTargetKey(hook), out var count) ? count : 1;
	}

	private static List<MethodBase> GetDeclaredMembers(Type type)
	{
		var members = new List<MethodBase>(AccessTools.GetDeclaredMethods(type));
		members.AddRange(type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static));
		return members;
	}

	private static Type GetReturnType(MethodBase method)
	{
		return method is MethodInfo info ? info.ReturnType : typeof(void);
	}

	private void AnalyzeReturnAndExposure(OpjHookCheck check, HookDef.Data hook, MethodBase method)
	{
		var recordedReturn = hook.Signature.ReturnType;
		var actualReturn = SignatureComparer.PatcherTypeName(GetReturnType(method));
		if (!SignatureComparer.TypeNamesMatch(recordedReturn, actualReturn))
		{
			var diagnostic = check.Add(OpjIssueKind.ReturnTypeDrift, OpjIssueSeverity.AutoFixable,
				$"return type changed: '{recordedReturn}' -> '{actualReturn}' (return-based behaviors may need review)");
			diagnostic.Fixes.Add(new OpjFixEdit("Signature.ReturnType", recordedReturn, actualReturn));
		}

		// Publicized assemblies (Carbon's build publicizes the rust folder in place) lose the
		// vanilla accessibility, so exposure cannot be validated against them.
		if (IsExposureTrustworthy(hook.AssemblyName))
		{
			var actualExposure = SignatureComparer.GetExposure(method);
			if (hook.Signature.Exposure != actualExposure)
			{
				var diagnostic = check.Add(OpjIssueKind.ExposureDrift, OpjIssueSeverity.AutoFixable,
					$"method exposure changed: {hook.Signature.Exposure} -> {actualExposure}");
				diagnostic.Fixes.Add(new OpjFixEdit("Signature.Exposure", (int)hook.Signature.Exposure, (int)actualExposure));
			}
		}
	}

	private readonly HashSet<string> _publicizedWarned = new(StringComparer.OrdinalIgnoreCase);

	private bool IsExposureTrustworthy(string assemblyName)
	{
		var assembly = _current.Get(assemblyName);
		if (assembly is not { IsLikelyPublicized: true })
		{
			return true;
		}

		if (_publicizedWarned.Add(assemblyName))
		{
			Logger.Warning($"'{assemblyName}' in the managed folder is publicized; exposure validation skipped");
		}

		return false;
	}

	private void AnalyzeSignatureDrift(OpjHookCheck check, HookDef.Data hook, MethodBase method)
	{
		var recorded = hook.Signature.Parameters ?? [];
		var actual = method.GetParameters().Select(x => SignatureComparer.PatcherTypeName(x.ParameterType)).ToArray();
		if (SignatureComparer.ParametersMatch(actual, recorded))
		{
			// Different spellings of the same signature; nothing to fix.
			AnalyzeReturnAndExposure(check, hook, method);
			return;
		}

		// The argument-string remap decides the drift's fate: an unmappable reference means the
		// signature fix must stay a proposal, or the old-signature baseline would be erased.
		var remapOk = TryRemapArgumentString(check, hook, method, out var argumentFix);

		var diagnostic = check.Add(OpjIssueKind.SignatureDrift,
			remapOk ? OpjIssueSeverity.AutoFixable : OpjIssueSeverity.NeedsHuman,
			$"signature changed: ({string.Join(", ", recorded)}) -> ({string.Join(", ", actual)})"
			+ (remapOk ? string.Empty : " (argument references no longer map; fix left as a proposal)"));
		diagnostic.Fixes.Add(new OpjFixEdit("Signature.Parameters", recorded, actual));
		if (argumentFix != null)
		{
			diagnostic.Fixes.Add(argumentFix);
		}

		AnalyzeReturnAndExposure(check, hook, method);
	}

	/// <summary>
	///     When a hook addresses method parameters by index ("p0", "a1") and the signature moved,
	///     tries to remap the references using the old build's parameter names/types. Returns false
	///     when a reference no longer maps onto the new signature.
	/// </summary>
	private bool TryRemapArgumentString(OpjHookCheck check, HookDef.Data hook, MethodBase method, out OpjFixEdit? argumentFix)
	{
		argumentFix = null;

		if (string.IsNullOrWhiteSpace(hook.ArgumentString)
		    || (hook.ArgumentBehavior != ArgumentBehavior.UseArgumentString && hook.ReturnBehavior != ReturnBehavior.UseArgumentString))
		{
			return true;
		}

		var oldMethod = ResolveOldMethod(hook);
		var newParameters = method.GetParameters();
		var replacements = new Dictionary<string, string>(StringComparer.Ordinal);
		var failures = new List<string>();

		foreach (var reference in EnumerateArgumentReferences(hook.ArgumentString))
		{
			if (reference.Kind is not ('p' or 'a'))
			{
				continue;
			}

			var mappedIndex = -1;
			if (oldMethod != null && reference.Index < oldMethod.ParameterTypes.Length)
			{
				var oldName = reference.Index < oldMethod.ParameterNames.Length ? oldMethod.ParameterNames[reference.Index] : string.Empty;
				var oldType = oldMethod.ParameterTypes[reference.Index];

				for (var i = 0; i < newParameters.Length; i++)
				{
					if (!string.IsNullOrEmpty(oldName) && newParameters[i].Name == oldName)
					{
						mappedIndex = i;
						break;
					}
				}

				if (mappedIndex < 0)
				{
					var typeMatches = Enumerable.Range(0, newParameters.Length)
						.Where(i => SignatureComparer.TypeNamesMatch(SignatureComparer.PatcherTypeName(newParameters[i].ParameterType), oldType))
						.ToList();
					if (typeMatches.Count == 1)
					{
						mappedIndex = typeMatches[0];
					}
				}
			}
			else if (reference.Index < newParameters.Length)
			{
				// No old build to compare against; trust the reference while it stays in range.
				mappedIndex = reference.Index;
			}

			if (mappedIndex < 0)
			{
				failures.Add(reference.Token);
			}
			else if (mappedIndex != reference.Index)
			{
				replacements[reference.Token] = $"{reference.Kind}{mappedIndex}";
			}
		}

		if (failures.Count > 0)
		{
			check.Add(OpjIssueKind.ArgumentOutOfRange, OpjIssueSeverity.NeedsHuman,
				$"argument string '{hook.ArgumentString}' references parameter(s) [{string.Join(", ", failures)}] that no longer map onto the new signature");
			return false;
		}

		if (replacements.Count == 0)
		{
			return true;
		}

		// Rewrite in one pass over the original string, on token boundaries, so replacements cannot
		// cascade into each other and "p1" never rewrites the "p1" inside "p10".
		var remapped = ArgumentTokenRegex().Replace(hook.ArgumentString,
			match => replacements.TryGetValue(match.Value, out var replacement) ? replacement : match.Value);

		if (!string.Equals(remapped, hook.ArgumentString, StringComparison.Ordinal))
		{
			argumentFix = new OpjFixEdit("ArgumentString", hook.ArgumentString, remapped);
		}

		return true;
	}

	private readonly record struct ArgumentReference(char Kind, int Index, string Token);

	private static IEnumerable<ArgumentReference> EnumerateArgumentReferences(string argumentString)
	{
		Tools.ArgumentParser(argumentString.Trim(), out var arguments, out var returnVariable);
		var tokens = returnVariable == null ? arguments : [.. arguments, returnVariable];

		foreach (var raw in tokens)
		{
			var token = raw.Trim();
			if (token.Contains('.'))
			{
				token = token.Split('.')[0];
			}

			if (token.Length < 2)
			{
				continue;
			}

			var kind = char.ToLowerInvariant(token[0]);
			if (kind is 'p' or 'a' or 'l' or 'v' or 'r' && int.TryParse(token[1..], out var index))
			{
				yield return new ArgumentReference(kind, index, token);
			}
		}
	}

	private void AnalyzeArgumentReferences(OpjHookCheck check, HookDef.Data hook, MethodBase method, MetadataMethod? current)
	{
		if (string.IsNullOrWhiteSpace(hook.ArgumentString)
		    || (hook.ArgumentBehavior != ArgumentBehavior.UseArgumentString && hook.ReturnBehavior != ReturnBehavior.UseArgumentString))
		{
			return;
		}

		var effective = GetEffectiveArgumentString(check, hook);
		var parameterCount = method.GetParameters().Length;
		var localCount = current?.LocalCount ?? method.GetMethodBody()?.LocalVariables.Count ?? 0;

		foreach (var reference in EnumerateArgumentReferences(effective))
		{
			switch (reference.Kind)
			{
				case 'p' or 'a' when reference.Index >= parameterCount:
					check.Add(OpjIssueKind.ArgumentOutOfRange, OpjIssueSeverity.NeedsHuman,
						$"argument '{reference.Token}' references parameter {reference.Index} but the method only has {parameterCount}");
					break;

				case 'l' or 'v' when reference.Index >= localCount:
					check.Add(OpjIssueKind.LocalOutOfRange, OpjIssueSeverity.Warning,
						$"argument '{reference.Token}' references local slot {reference.Index} beyond the method body ({localCount} locals); requires synthetic-local remapping via its base hook");
					break;
			}
		}
	}

	private static string GetEffectiveArgumentString(OpjHookCheck check, HookDef.Data hook)
	{
		foreach (var diagnostic in check.Diagnostics)
		{
			foreach (var fix in diagnostic.Fixes)
			{
				if (fix.Path == "ArgumentString" && fix.NewValue is string remapped)
				{
					return remapped;
				}
			}
		}

		return hook.ArgumentString;
	}

	private void AnalyzeBody(OpjHookCheck check, string action, HookDef.Data hook, MethodBase method, MetadataMethod? current)
	{
		if (current?.Instructions == null)
		{
			if (hook.InjectionIndex > 0 || action == "Modify")
			{
				check.Add(OpjIssueKind.BodyDrift, OpjIssueSeverity.Warning, "method body is unavailable for inspection");
			}

			return;
		}

		var instructions = current.Instructions;
		var currentHash = current.MsilHash;
		var hashMatches = string.Equals(currentHash, hook.MsilHash, StringComparison.Ordinal);

		// When the body drifted and the old build is the authoring baseline, an IL alignment can
		// re-anchor drifted indexes (injection index, Modify branch targets).
		var old = hashMatches ? null : ResolveOldMethod(hook);
		var baselineConfirmed = old?.Instructions != null && string.Equals(old.MsilHash, hook.MsilHash, StringComparison.Ordinal);
		var alignment = baselineConfirmed ? new IlAlignment(old!.Instructions!, instructions) : null;

		// Hooks stacked on a base hook record their indexes against the base-patched instruction
		// stream; translate back into the vanilla body where possible. While the body still matches
		// the recorded hash, an untranslatable index is simply valid as authored.
		var vanillaIndex = ResolveVanillaIndex(hook, hook.InjectionIndex, out var baseShift, out var anchoredToBase);
		if (anchoredToBase)
		{
			// FollowBaseReanchors picks these up once the base hook's own analysis is complete.
			_anchoredToBase[check] = hashMatches ? null : currentHash;

			if (action == "Modify")
			{
				AnalyzeModifyInstructions(check, hook, current, hashMatches, alignment);
			}

			return;
		}

		if (vanillaIndex < 0)
		{
			if (!hashMatches)
			{
				check.Add(OpjIssueKind.BodyDrift, OpjIssueSeverity.Warning,
					$"method body changed and InjectionIndex {hook.InjectionIndex} depends on instructions generated by base hook '{hook.BaseHookName}'; review manually");
			}

			if (action == "Modify")
			{
				AnalyzeModifyInstructions(check, hook, current, hashMatches, alignment);
			}

			return;
		}

		var count = instructions.Length;
		var indexValid = vanillaIndex >= 0 && vanillaIndex < count;
		var removeValid = action != "Modify" || hook.RemoveCount <= 0 || (indexValid && vanillaIndex + hook.RemoveCount <= count);

		if (hashMatches)
		{
			if (!indexValid)
			{
				var flagged = check.Flagged
					? " — the hook is flagged in the patcher; its index was left stale from an older build"
					: string.Empty;
				check.Add(OpjIssueKind.InjectionIndexOutOfRange, OpjIssueSeverity.NeedsHuman,
					$"InjectionIndex {hook.InjectionIndex} is outside the method body (0..{count - 1}) even though the body matches the recorded hash{flagged}");
			}
			else if (!removeValid)
			{
				check.Add(OpjIssueKind.RemoveRangeInvalid, OpjIssueSeverity.NeedsHuman,
					$"removal range {vanillaIndex}..{vanillaIndex + hook.RemoveCount} exceeds the method body ({count} instructions)");
			}

			if (action == "Modify")
			{
				AnalyzeModifyInstructions(check, hook, current, true, null);
			}

			return;
		}

		if (old?.Instructions == null)
		{
			var reason = _old == null ? "provide --old-managed to re-anchor" : "method not found in the old build";
			if (!indexValid)
			{
				check.Add(OpjIssueKind.InjectionIndexOutOfRange, OpjIssueSeverity.NeedsHuman,
					$"InjectionIndex {hook.InjectionIndex} is outside the method body (0..{count - 1}) and cannot be re-anchored ({reason})");
			}
			else
			{
				check.Add(OpjIssueKind.BodyDrift, OpjIssueSeverity.Warning,
					$"method body changed since the hook was last verified; InjectionIndex {hook.InjectionIndex} not verified ({reason})");
			}

			if (action == "Modify")
			{
				AnalyzeModifyInstructions(check, hook, current, false, null);
			}

			return;
		}

		if (alignment == null)
		{
			check.Add(OpjIssueKind.BaselineMismatch, OpjIssueSeverity.Warning,
				"the old build does not match the hook's recorded hash (not the authoring baseline); re-anchoring skipped");
			if (!indexValid)
			{
				check.Add(OpjIssueKind.InjectionIndexOutOfRange, OpjIssueSeverity.NeedsHuman,
					$"InjectionIndex {hook.InjectionIndex} is outside the method body (0..{count - 1})");
			}

			if (action == "Modify")
			{
				AnalyzeModifyInstructions(check, hook, current, false, null);
			}

			return;
		}

		if (vanillaIndex >= old.Instructions!.Length)
		{
			check.Add(OpjIssueKind.InjectionIndexOutOfRange, OpjIssueSeverity.NeedsHuman,
				$"InjectionIndex {hook.InjectionIndex} is outside the recorded baseline body (0..{old.Instructions.Length - 1}); OPJ data inconsistent");
			if (action == "Modify")
			{
				AnalyzeModifyInstructions(check, hook, current, false, alignment);
			}

			return;
		}

		var mapped = alignment.Map(vanillaIndex);
		var confidence = alignment.GetConfidence(vanillaIndex, mapped);

		if (mapped < 0)
		{
			var proposal = alignment.ProposeNearby(vanillaIndex);
			var diagnostic = check.Add(OpjIssueKind.InjectionIndexDrift, OpjIssueSeverity.NeedsHuman,
				$"the instruction anchoring InjectionIndex {hook.InjectionIndex} was removed in the new build"
				+ (proposal >= 0 ? $"; the closest surviving position is {proposal + baseShift}" : string.Empty));
			if (proposal >= 0)
			{
				diagnostic.Fixes.Add(new OpjFixEdit("InjectionIndex", hook.InjectionIndex, proposal + baseShift));
			}
		}
		else if (confidence == AlignmentConfidence.Low)
		{
			var diagnostic = check.Add(OpjIssueKind.InjectionIndexDrift, OpjIssueSeverity.NeedsHuman,
				$"InjectionIndex re-anchors {hook.InjectionIndex} -> {mapped + baseShift} but the surrounding code changed too much (low confidence)");
			diagnostic.Fixes.Add(new OpjFixEdit("InjectionIndex", hook.InjectionIndex, mapped + baseShift));
		}
		else if (mapped != vanillaIndex)
		{
			var diagnostic = check.Add(OpjIssueKind.InjectionIndexDrift, OpjIssueSeverity.AutoFixable,
				$"InjectionIndex re-anchored {hook.InjectionIndex} -> {mapped + baseShift} ({confidence.ToString().ToLowerInvariant()} confidence)");
			diagnostic.Fixes.Add(new OpjFixEdit("InjectionIndex", hook.InjectionIndex, mapped + baseShift));
		}
		else
		{
			check.Add(OpjIssueKind.BodyDrift, OpjIssueSeverity.Info,
				$"method body changed but InjectionIndex {hook.InjectionIndex} verified stable ({confidence.ToString().ToLowerInvariant()} confidence)");
		}

		if (action == "Modify")
		{
			AnalyzeModifyInstructions(check, hook, current, false, alignment);
			ValidateRemovalRange(check, hook, alignment, vanillaIndex, mapped);
		}

		// The recorded hash and the recorded indexes must describe the same build: either the index
		// rewrites and the hash refresh all land, or none of them do.
		if (check.Severity == OpjIssueSeverity.NeedsHuman)
		{
			DemoteIndexSpaceFixes(check, "the hook still needs human attention; index rewrites only land together with a hash refresh");
		}
		else if (currentHash != null)
		{
			var refresh = check.Add(OpjIssueKind.BodyDrift, OpjIssueSeverity.AutoFixable,
				"recorded MSIL hash refreshed to the current build");
			refresh.Fixes.Add(new OpjFixEdit("MSILHash", hook.MsilHash, currentHash));
		}
	}

	private void ValidateRemovalRange(OpjHookCheck check, HookDef.Data hook, IlAlignment alignment, int vanillaIndex, int mappedIndex)
	{
		if (hook.RemoveCount <= 0 || mappedIndex < 0)
		{
			return;
		}

		for (var offset = 0; offset < hook.RemoveCount; offset++)
		{
			if (alignment.Map(vanillaIndex + offset) != mappedIndex + offset)
			{
				check.Add(OpjIssueKind.RemoveRangeInvalid, OpjIssueSeverity.NeedsHuman,
					$"the {hook.RemoveCount} removed instruction(s) at {hook.InjectionIndex} changed in the new build; review the removal range");
				return;
			}
		}
	}

	/// <summary>
	///     Validates the operands of a Modify hook's inserted instructions: member references must
	///     still resolve and branch targets into the original body must still exist (re-anchored via
	///     the alignment when the body drifted).
	/// </summary>
	private void AnalyzeModifyInstructions(OpjHookCheck check, HookDef.Data hook, MetadataMethod current, bool hashMatches,
		IlAlignment? alignment)
	{
		if (hook.Instructions == null)
		{
			return;
		}

		var count = current.Instructions?.Length ?? 0;

		for (var index = 0; index < hook.Instructions.Count; index++)
		{
			var instruction = hook.Instructions[index];
			switch (instruction.OpType)
			{
				case HookDef.Data.OpType.Field:
				case HookDef.Data.OpType.Method:
				case HookDef.Data.OpType.Generic:
					ValidateMemberOperand(check, index, instruction);
					break;

				case HookDef.Data.OpType.Type:
					ValidateTypeOperand(check, index, instruction);
					break;

				case HookDef.Data.OpType.Instruction:
					ValidateInstructionTarget(check, hook, index, instruction, count, hashMatches, alignment);
					break;

				case HookDef.Data.OpType.Variable:
				case HookDef.Data.OpType.VariableIndex:
					if (TryReadIndexOperand(instruction, out var localIndex) && localIndex >= current.LocalCount)
					{
						check.Add(OpjIssueKind.LocalOutOfRange, OpjIssueSeverity.Warning,
							$"instruction {index} ({instruction.OpCode}) references local slot {localIndex} beyond the method body ({current.LocalCount} locals); requires synthetic-local remapping");
					}

					break;
			}

			// leave/leave_s always anchor to an original-body instruction, whatever the OpType says.
			if (instruction.OpCode is "leave" or "leave_s" && instruction.OpType != HookDef.Data.OpType.Instruction)
			{
				ValidateInstructionTarget(check, hook, index, instruction, count, hashMatches, alignment);
			}
		}
	}

	private void ValidateInstructionTarget(OpjHookCheck check, HookDef.Data hook, int index, HookDef.Data.InstructionData instruction,
		int count, bool hashMatches, IlAlignment? alignment)
	{
		if (!TryReadIndexOperand(instruction, out var target))
		{
			return;
		}

		if (instruction.ReferencesNewInstruction)
		{
			if (target < 0 || target >= hook.Instructions.Count)
			{
				check.Add(OpjIssueKind.InstructionTargetOutOfRange, OpjIssueSeverity.NeedsHuman,
					$"instruction {index} ({instruction.OpCode}) targets inserted instruction {target} but only {hook.Instructions.Count} are inserted");
			}

			return;
		}

		// The recorded target lives in the same (possibly base-patched) index space as the
		// injection index; translate it back into the vanilla body first.
		var vanillaTarget = ResolveVanillaIndex(hook, target, out var targetShift, out var anchoredToBase);
		if (anchoredToBase)
		{
			// Targets instructions inserted by the base hook; validated through the base hook.
			return;
		}

		if (vanillaTarget < 0)
		{
			check.Add(OpjIssueKind.InstructionTargetDrift, OpjIssueSeverity.Warning,
				$"instruction {index} ({instruction.OpCode}) target {target} cannot be translated through base hook '{hook.BaseHookName}'; not verified");
			return;
		}

		if (hashMatches)
		{
			if (vanillaTarget >= count)
			{
				var shared = SharedTargetCount(hook);
				if (shared > 1)
				{
					check.Add(OpjIssueKind.InstructionTargetOutOfRange, OpjIssueSeverity.Warning,
						$"instruction {index} ({instruction.OpCode}) targets original instruction {target} outside the body (0..{count - 1}); {shared - 1} other hook(s) patch this method, so the target is likely recorded against the co-patched stream");
				}
				else
				{
					check.Add(OpjIssueKind.InstructionTargetOutOfRange, OpjIssueSeverity.NeedsHuman,
						$"instruction {index} ({instruction.OpCode}) targets original instruction {target} but the method body has {count} instructions");
				}
			}

			return;
		}

		if (alignment == null)
		{
			check.Add(OpjIssueKind.InstructionTargetDrift, OpjIssueSeverity.Warning,
				$"instruction {index} ({instruction.OpCode}) targets original instruction {target}; the body changed and the target cannot be verified");
			return;
		}

		var mapped = alignment.Map(vanillaTarget);
		if (mapped < 0)
		{
			check.Add(OpjIssueKind.InstructionTargetDrift, OpjIssueSeverity.NeedsHuman,
				$"instruction {index} ({instruction.OpCode}) targets original instruction {target}, which was removed in the new build");
			return;
		}

		var confidence = alignment.GetConfidence(vanillaTarget, mapped);
		if (mapped != vanillaTarget)
		{
			var severity = confidence == AlignmentConfidence.Low ? OpjIssueSeverity.NeedsHuman : OpjIssueSeverity.AutoFixable;
			var diagnostic = check.Add(OpjIssueKind.InstructionTargetDrift, severity,
				$"instruction {index} ({instruction.OpCode}) target re-anchored {target} -> {mapped + targetShift} ({confidence.ToString().ToLowerInvariant()} confidence)");
			diagnostic.Fixes.Add(new OpjFixEdit($"Instructions[{index}].Operand", target, mapped + targetShift));
		}
	}

	/// <summary>
	///     Resolves an operand type name, falling back to the generic type definition for
	///     instantiations ("EncryptedValue`1[System.UInt64]") that reflection cannot parse directly.
	/// </summary>
	private static Type? ResolveOperandType(string typeName)
	{
		var type = Tools.TypeByNameEx(typeName);
		if (type != null || !typeName.Contains('`'))
		{
			return type;
		}

		var argumentsStart = typeName.IndexOfAny(['[', '<']);
		return argumentsStart > 0 ? Tools.TypeByNameEx(typeName[..argumentsStart]) : null;
	}

	private static bool TryReadIndexOperand(HookDef.Data.InstructionData instruction, out int value)
	{
		value = -1;
		if (instruction.Operand == null)
		{
			return false;
		}

		try
		{
			value = Convert.ToInt32(instruction.Operand);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private void ValidateMemberOperand(OpjHookCheck check, int index, HookDef.Data.InstructionData instruction)
	{
		var operand = instruction.Operand?.ToString();
		if (string.IsNullOrEmpty(operand))
		{
			return;
		}

		var parts = operand.Split('|');
		if (parts.Length < 3)
		{
			return;
		}

		var assembly = Utility.Program.ResolveAssembly(Path.GetFileNameWithoutExtension(parts[0]));
		var type = ResolveOperandType(parts[1]);
		if (type == null)
		{
			if (assembly == null)
			{
				// Oxide/Carbon runtime assemblies are injected at patch time and are not part of
				// the game's managed folder; nothing to validate against.
				check.Add(OpjIssueKind.OperandMissing, OpjIssueSeverity.Info,
					$"instruction {index} ({instruction.OpCode}) references '{parts[1]}' from '{parts[0]}', which is not in the managed folder (assumed runtime-provided)");
				return;
			}

			check.Add(OpjIssueKind.OperandMissing, OpjIssueSeverity.NeedsHuman,
				$"instruction {index} ({instruction.OpCode}) references missing type '{parts[1]}'");
			return;
		}

		var member = parts[2];
		var memberName = member.Split('(', '[')[0];
		bool found;

		try
		{
			found = instruction.OpType == HookDef.Data.OpType.Field
				? AccessTools.Field(type, memberName) != null
				: AccessTools.Method(type, memberName) != null || type.GetMember(memberName,
					BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Length > 0;
		}
		catch (Exception)
		{
			// An ambiguous lookup still proves the member exists.
			found = true;
		}

		if (!found)
		{
			check.Add(OpjIssueKind.OperandMissing, OpjIssueSeverity.NeedsHuman,
				$"instruction {index} ({instruction.OpCode}) references missing member '{parts[1]}::{memberName}'");
		}
	}

	private void ValidateTypeOperand(OpjHookCheck check, int index, HookDef.Data.InstructionData instruction)
	{
		var operand = instruction.Operand?.ToString();
		if (string.IsNullOrEmpty(operand))
		{
			return;
		}

		var parts = operand.Split('|');
		var typeName = parts.Length >= 2 ? parts[1] : parts[0];
		System.Reflection.Assembly? assembly = null;
		if (parts.Length >= 2)
		{
			assembly = Utility.Program.ResolveAssembly(Path.GetFileNameWithoutExtension(parts[0]));
		}

		if (ResolveOperandType(typeName) == null)
		{
			if (parts.Length >= 2 && assembly == null)
			{
				check.Add(OpjIssueKind.OperandMissing, OpjIssueSeverity.Info,
					$"instruction {index} ({instruction.OpCode}) references '{typeName}' from '{parts[0]}', which is not in the managed folder (assumed runtime-provided)");
				return;
			}

			check.Add(OpjIssueKind.OperandMissing, OpjIssueSeverity.NeedsHuman,
				$"instruction {index} ({instruction.OpCode}) references missing type '{typeName}'");
		}
	}

	/// <summary>
	///     Translates an index recorded against a base-patched instruction stream back into the
	///     vanilla method body. Returns -1 when the chain cannot be resolved;
	///     <paramref name="anchoredToBase" /> is set when the index points into base-inserted code.
	/// </summary>
	private int ResolveVanillaIndex(HookDef.Data hook, int index, out int baseShift, out bool anchoredToBase)
	{
		baseShift = 0;
		anchoredToBase = false;

		var visited = new HashSet<string>(StringComparer.Ordinal);
		var currentHook = hook;
		var vanilla = index;

		while (!string.IsNullOrEmpty(currentHook.BaseHookName))
		{
			if (!visited.Add(currentHook.BaseHookName) || !_hooksByName.TryGetValue(currentHook.BaseHookName, out var baseDef))
			{
				return -1;
			}

			var baseHook = baseDef.Hook;
			if (baseHook.TypeName != hook.TypeName || baseHook.Signature?.Name != hook.Signature.Name)
			{
				// The base hook patches a different method; no index-space interaction.
				break;
			}

			if (baseHook.Instructions == null || baseHook.Instructions.Count == 0)
			{
				if (baseDef.Type == "Modify" && baseHook.RemoveCount > 0)
				{
					// Pure deletion: nothing inserted; indexes at/after the removal shift back.
					if (vanilla >= baseHook.InjectionIndex)
					{
						vanilla += baseHook.RemoveCount;
						baseShift -= baseHook.RemoveCount;
					}

					currentHook = baseHook;
					continue;
				}

				// A Simple base hook inserts a generated, statically unknown number of instructions.
				if (vanilla > baseHook.InjectionIndex)
				{
					return -1;
				}

				currentHook = baseHook;
				continue;
			}

			var inserted = baseHook.Instructions.Count;
			var removed = baseHook.RemoveCount;
			if (vanilla >= baseHook.InjectionIndex && vanilla < baseHook.InjectionIndex + inserted)
			{
				anchoredToBase = true;
				return -1;
			}

			if (vanilla >= baseHook.InjectionIndex + inserted)
			{
				vanilla = vanilla - inserted + removed;
				baseShift += inserted - removed;
			}

			currentHook = baseHook;
		}

		return vanilla;
	}

	private MethodBase? SelectBestOverload(List<MethodBase> namesakes, string[] recordedParameters)
	{
		MethodBase? best = null;
		var bestScore = -1;
		var tie = false;

		foreach (var candidate in namesakes)
		{
			var parameters = candidate.GetParameters();
			var score = parameters.Length == recordedParameters.Length ? 1 : 0;
			var positional = Math.Min(parameters.Length, recordedParameters.Length);
			for (var i = 0; i < positional; i++)
			{
				if (SignatureComparer.TypeNamesMatch(SignatureComparer.PatcherTypeName(parameters[i].ParameterType), recordedParameters[i]))
				{
					score += 2;
				}
			}

			if (score > bestScore)
			{
				bestScore = score;
				best = candidate;
				tie = false;
			}
			else if (score == bestScore)
			{
				tie = true;
			}
		}

		return tie ? null : best;
	}

	/// <summary>
	///     When a method vanished by name, compares the old build's IL against every method of the
	///     current type to spot a rename with an (almost) unchanged body.
	/// </summary>
	private MethodBase? TryDetectRename(OpjHookCheck check, HookDef.Data hook, Type type)
	{
		var old = ResolveOldMethod(hook);
		if (old?.Instructions == null)
		{
			return null;
		}

		var location = type.Assembly.Location;
		if (string.IsNullOrEmpty(location))
		{
			return null;
		}

		var currentType = _current.GetByPath(location)?.FindType(SignatureComparer.PatcherTypeName(type));
		if (currentType == null)
		{
			return null;
		}

		MetadataMethod? best = null;
		var bestSimilarity = 0.0;
		var runnerUpSimilarity = 0.0;

		foreach (var candidate in currentType.Methods)
		{
			if (candidate.Instructions == null)
			{
				continue;
			}

			var similarity = IlAlignment.BodySimilarity(old.Instructions, candidate.Instructions);
			if (similarity > bestSimilarity)
			{
				runnerUpSimilarity = bestSimilarity;
				bestSimilarity = similarity;
				best = candidate;
			}
			else if (similarity > runnerUpSimilarity)
			{
				runnerUpSimilarity = similarity;
			}
		}

		if (best == null || bestSimilarity < 0.85 || runnerUpSimilarity > 0.6)
		{
			// Not confident enough to call it a rename; ReportMissingMethod lists the candidates.
			return null;
		}

		var method = AccessTools.GetDeclaredMethods(type)
			.FirstOrDefault(x => x.Name == best.Name && SignatureComparer.ParametersMatch(x.GetParameters(), best.ParameterTypes));
		if (method == null)
		{
			return null;
		}

		// The argument-string remap decides whether the rename can be applied outright, same as
		// signature drift: unmappable references keep the fix a proposal.
		var remapOk = TryRemapArgumentString(check, hook, method, out var argumentFix);

		var rename = check.Add(OpjIssueKind.MethodRenamed,
			remapOk ? OpjIssueSeverity.AutoFixable : OpjIssueSeverity.NeedsHuman,
			$"method renamed '{hook.Signature.Name}' -> '{best.Name}' (IL similarity {bestSimilarity:0.00})"
			+ (remapOk ? string.Empty : " (argument references no longer map; fix left as a proposal)"));
		rename.Fixes.Add(new OpjFixEdit("Signature.Name", hook.Signature.Name, best.Name));

		var actual = method.GetParameters().Select(x => SignatureComparer.PatcherTypeName(x.ParameterType)).ToArray();
		if (!SignatureComparer.ParametersMatch(method.GetParameters(), hook.Signature.Parameters))
		{
			rename.Fixes.Add(new OpjFixEdit("Signature.Parameters", hook.Signature.Parameters, actual));
		}

		if (argumentFix != null)
		{
			rename.Fixes.Add(argumentFix);
		}

		AnalyzeReturnAndExposure(check, hook, method);
		return method;
	}

	private void ReportMissingMethod(OpjHookCheck check, HookDef.Data hook, Type type, List<MethodBase> declared)
	{
		var old = ResolveOldMethod(hook);
		var existedBefore = old != null ? " (it existed in the old build)" : string.Empty;
		var diagnostic = check.Add(OpjIssueKind.MissingMethod, OpjIssueSeverity.NeedsHuman,
			$"method '{hook.Signature.Name}({string.Join(", ", hook.Signature.Parameters ?? [])})' no longer exists on '{hook.TypeName}'{existedBefore}");

		// Rank likely successors: by body similarity when the old build is available, then by name.
		if (old?.Instructions != null)
		{
			var location = type.Assembly.Location;
			var currentType = string.IsNullOrEmpty(location) ? null : _current.GetByPath(location)?.FindType(SignatureComparer.PatcherTypeName(type));
			if (currentType != null)
			{
				var similar = currentType.Methods
					.Where(x => x.Instructions != null)
					.Select(x => (Method: x, Similarity: IlAlignment.BodySimilarity(old.Instructions, x.Instructions!)))
					.Where(x => x.Similarity >= 0.5)
					.OrderByDescending(x => x.Similarity)
					.Take(3);
				foreach (var (candidate, similarity) in similar)
				{
					diagnostic.Candidates.Add($"{candidate.RenderSignature()} (IL similarity {similarity:0.00})");
				}
			}
		}

		foreach (var candidate in declared
			.Where(x => SignatureComparer.NameDistance(x.Name, hook.Signature.Name) <= 2)
			.OrderBy(x => SignatureComparer.NameDistance(x.Name, hook.Signature.Name))
			.Take(3))
		{
			var described = DescribeMethod(candidate);
			if (!diagnostic.Candidates.Contains(described))
			{
				diagnostic.Candidates.Add($"{described} (similar name)");
			}
		}
	}

	private void ReportMissingType(OpjHookCheck check, HookDef.Data hook)
	{
		var existedBefore = _old?.Get(hook.AssemblyName)?.FindType(hook.TypeName) != null ? " (it existed in the old build)" : string.Empty;
		var diagnostic = check.Add(OpjIssueKind.MissingType, OpjIssueSeverity.NeedsHuman,
			$"type '{hook.TypeName}' no longer exists{existedBefore}");

		var slash = hook.TypeName.LastIndexOf('/');
		if (slash > 0)
		{
			var parent = Tools.TypeByNameEx(hook.TypeName[..slash]);
			if (parent != null)
			{
				foreach (var nested in parent.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic).Take(10))
				{
					diagnostic.Candidates.Add($"{hook.TypeName[..slash]}/{nested.Name}");
				}
			}
		}
	}

	/// <summary>
	///     Compiler-generated nested types (async state machines, lambda display classes) renumber
	///     whenever surrounding members shift; re-resolves them through their stable name parts.
	/// </summary>
	private Type? TryFixCompilerGeneratedTypeName(OpjHookCheck check, HookDef.Data hook, ref string typeName)
	{
		var slash = typeName.LastIndexOf('/');
		if (slash <= 0)
		{
			return null;
		}

		var parentName = typeName[..slash];
		var nestedName = typeName[(slash + 1)..];
		var parent = Tools.TypeByNameEx(parentName);
		if (parent == null)
		{
			return null;
		}

		var stateMachine = StateMachineRegex().Match(nestedName);
		var displayClass = DisplayClassRegex().Match(nestedName);
		List<Type> candidates;

		if (stateMachine.Success)
		{
			var prefix = stateMachine.Groups[1].Value;
			candidates = parent.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
				.Where(x => RenumberedName(x.Name, prefix))
				.ToList();
		}
		else if (displayClass.Success)
		{
			candidates = parent.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
				.Where(x => DisplayClassAnyRegex().IsMatch(x.Name))
				.Where(x => AccessTools.GetDeclaredMethods(x).Any(m => m.Name == hook.Signature.Name
					&& SignatureComparer.ParametersMatch(m.GetParameters(), hook.Signature.Parameters ?? [])))
				.ToList();
		}
		else
		{
			return null;
		}

		if (candidates.Count > 1 && _old != null)
		{
			candidates = DisambiguateByOldBody(hook, candidates);
		}

		if (candidates.Count != 1)
		{
			if (candidates.Count > 1)
			{
				var ambiguous = check.Add(OpjIssueKind.MissingType, OpjIssueSeverity.NeedsHuman,
					$"compiler-generated type '{typeName}' renumbered but {candidates.Count} candidates match");
				ambiguous.Candidates.AddRange(candidates.Select(x => $"{parentName}/{x.Name}"));
			}

			return null;
		}

		var renamed = $"{parentName}/{candidates[0].Name}";
		var diagnostic = check.Add(OpjIssueKind.StateMachineRenumbered, OpjIssueSeverity.AutoFixable,
			$"compiler-generated type renumbered: '{typeName}' -> '{renamed}'");
		diagnostic.Fixes.Add(new OpjFixEdit("TypeName", typeName, renamed));
		typeName = renamed;
		return candidates[0];
	}

	private static bool RenumberedName(string name, string prefix)
	{
		if (!name.StartsWith(prefix, StringComparison.Ordinal))
		{
			return false;
		}

		var suffix = name[prefix.Length..];
		return suffix.Length > 0 && suffix.All(char.IsAsciiDigit);
	}

	private List<Type> DisambiguateByOldBody(HookDef.Data hook, List<Type> candidates)
	{
		var old = ResolveOldMethod(hook);
		if (old?.Instructions == null)
		{
			return candidates;
		}

		var scored = new List<(Type Type, double Similarity)>();
		foreach (var candidate in candidates)
		{
			var location = candidate.Assembly.Location;
			var currentType = string.IsNullOrEmpty(location)
				? null
				: _current.GetByPath(location)?.FindType(SignatureComparer.PatcherTypeName(candidate));
			var method = currentType?.Methods.FirstOrDefault(x => x.Name == hook.Signature.Name);
			if (method?.Instructions == null)
			{
				continue;
			}

			scored.Add((candidate, IlAlignment.BodySimilarity(old.Instructions, method.Instructions)));
		}

		var winners = scored.Where(x => x.Similarity >= 0.85).OrderByDescending(x => x.Similarity).ToList();
		return winners.Count >= 1 && (winners.Count == 1 || winners[0].Similarity - winners[1].Similarity > 0.15)
			? [winners[0].Type]
			: candidates;
	}

	private void WarnOnPolicyCoupling(OpjHookCheck check, HookDef.Data hook)
	{
		if (!HookPolicies.MatchesOnClanCreatedAsyncSuccessRetargetPolicy(hook)
		    && !HookPolicies.MatchesOnTeamMemberInviteSendInviteAnchorPolicy(hook)
		    && !HookPolicies.MatchesOnClanLogoChangedPatchDependencyPolicy(hook))
		{
			return;
		}

		check.Add(OpjIssueKind.PolicyHandled, OpjIssueSeverity.Info,
			"a generator policy in HookPolicies.cs special-cases this hook by its exact OPJ values; if the OPJ entry is changed, update the policy too");

		if (HookPolicies.MatchesOnClanCreatedAsyncSuccessRetargetPolicy(hook))
		{
			var targetType = Tools.TypeByNameEx(HookPolicies.GetEmittedTargetTypeName(hook));
			var targetMethod = Tools.MethodByNameEx(targetType, HookPolicies.GetEmittedTargetMethodName(hook),
				HookPolicies.GetEmittedTargetMethodArgs(hook));
			if (targetMethod == null)
			{
				check.Add(OpjIssueKind.MissingMethod, OpjIssueSeverity.NeedsHuman,
					"the generator policy retarget for this hook no longer resolves; update HookPolicies.cs");
			}
		}
	}

	private MetadataMethod? ResolveOldMethod(HookDef.Data hook)
	{
		if (_old == null || hook.Signature == null)
		{
			return null;
		}

		return _old.Get(hook.AssemblyName)?.FindMethod(hook.TypeName, hook.Signature.Name, hook.Signature.Parameters);
	}

	private static string DescribeMethod(MethodBase method)
	{
		return $"{SignatureComparer.PatcherTypeName(GetReturnType(method))} {method.Name}({string.Join(", ", method.GetParameters().Select(x => SignatureComparer.PatcherTypeName(x.ParameterType)))})";
	}

	public void Dispose()
	{
		_current.Dispose();
		_old?.Dispose();
	}
}

internal sealed partial class OpjChecker
{
	[GeneratedRegex(@"(?<=^|[,\s>])([pPaA]\d+)(?=$|[.,\s=])")]
	private static partial Regex ArgumentTokenRegex();

	[GeneratedRegex(@"^(<\w+>d__)\d+$")]
	private static partial Regex StateMachineRegex();

	[GeneratedRegex(@"^(<>c__DisplayClass)\d+(_\d+)?$")]
	private static partial Regex DisplayClassRegex();

	[GeneratedRegex(@"^<>c__DisplayClass\d+(_\d+)?$")]
	private static partial Regex DisplayClassAnyRegex();
}
