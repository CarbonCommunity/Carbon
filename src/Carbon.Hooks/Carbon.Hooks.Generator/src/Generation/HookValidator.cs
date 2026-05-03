using System.Reflection;
using Carbon.Utility;
using static Carbon.Projects.Oxide.Oxide;

namespace Carbon.Generation;

internal sealed class HookValidator(ValidationMode validationMode)
{
	public bool Validate(string action, HookDef.Data hook, MethodInfo targetMethod)
	{
		HookPolicies.WarnOnPolicyHashDrift(hook);

		var diagnostics = new List<string>();
		var metadataReturnType = Tools.TypeNameSanitizerEx(hook.Signature.ReturnType);
		var targetReturnType = Tools.TypeNameSanitizerEx(targetMethod.ReturnType.ToString());

		if (!HookPolicies.MatchesOnClanCreatedAsyncSuccessRetargetPolicy(hook)
		    && !string.Equals(metadataReturnType, targetReturnType, StringComparison.Ordinal))
		{
			diagnostics.Add($"metadata return '{metadataReturnType}' != target return '{targetReturnType}'");
		}

		if (HookUsesSyntheticLocal(hook, targetMethod))
		{
			diagnostics.Add("hook references local slots beyond the target method body and requires synthetic-local remapping");
		}

		if (diagnostics.Count == 0)
		{
			return true;
		}

		var message = $"{hook.HookName} validation: {string.Join("; ", diagnostics)}";
		switch (validationMode)
		{
			case ValidationMode.Warn:
				Logger.Warning(message);
				return true;
			case ValidationMode.Skip:
				Logger.Warning($"{message} [skipped]");
				return false;
			case ValidationMode.Fail:
				throw new InvalidOperationException(message);
			default:
				return true;
		}
	}

	private static bool HookUsesSyntheticLocal(HookDef.Data hook, MethodInfo targetMethod)
	{
		if (HookPolicies.MatchesOnClanCreatedAsyncSuccessRetargetPolicy(hook))
		{
			return false;
		}

		var localCount = targetMethod.GetMethodBody()?.LocalVariables.Count ?? 0;

		if (!string.IsNullOrWhiteSpace(hook.ArgumentString))
		{
			Tools.ArgumentParser(hook.ArgumentString.Trim(), out var argVars, out var retVar);
			for (var i = 0; i < argVars.Length; i++)
			{
				var argument = argVars[i];
				if (TryParseLocalReference(argument, out var localIndex) && localIndex >= localCount)
				{
					return true;
				}
			}

			if (retVar != null && TryParseLocalReference(retVar, out var returnLocalIndex) && returnLocalIndex >= localCount)
			{
				return true;
			}
		}

		if (hook.Instructions == null)
		{
			return false;
		}

		for (var i = 0; i < hook.Instructions.Count; i++)
		{
			var instruction = hook.Instructions[i];
			var localIndex = instruction.OpCode switch
			{
				"ldloc_0" or "stloc_0" => 0,
				"ldloc_1" or "stloc_1" => 1,
				"ldloc_2" or "stloc_2" => 2,
				"ldloc_3" or "stloc_3" => 3,
				"ldloc" or "ldloc_s" or "ldloca_s" or "stloc" or "stloc_s" => Convert.ToInt32(instruction.Operand),
				_ => -1,
			};

			if (localIndex >= localCount)
			{
				return true;
			}
		}

		return false;
	}

	private static bool TryParseLocalReference(string argument, out int index)
	{
		index = -1;
		if (string.IsNullOrWhiteSpace(argument))
		{
			return false;
		}

		var value = argument.Trim();
		if (value.Contains('.'))
		{
			value = value.Split('.')[0];
		}

		return (value.StartsWith("l", StringComparison.OrdinalIgnoreCase) || value.StartsWith("v", StringComparison.OrdinalIgnoreCase))
		       && value.Length > 1
		       && int.TryParse(value[1..], out index);
	}
}
