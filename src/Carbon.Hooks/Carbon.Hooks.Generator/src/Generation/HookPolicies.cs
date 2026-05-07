using System.Text;
using Carbon.Projects.Oxide;
using Carbon.Utility;
using static Carbon.Projects.Oxide.Oxide;

namespace Carbon.Generation;

internal static class HookPolicies
{
	private const string OnClanCreatedKnownMsilHash = "635jwYcCfWiJfWKJFGaT8v7zfRJT0VND5GqMKxQMUVE=";
	private const string OnClanLogoChangedPatchKnownMsilHash = "KuTX1u22EO8+4GjS+SACsxt4g298bNL2E6omZgUevf8=";

	public static bool MatchesOnClanCreatedAsyncSuccessRetargetPolicy(HookDef.Data hook)
	{
		return hook.HookName == "OnClanCreated"
		       && hook.Name == "OnClanCreated"
		       && hook.TypeName == "LocalClanBackend/<Create>d__11"
		       && hook.Signature.Name == "MoveNext";
	}

	public static bool MatchesOnTeamMemberInviteSendInviteAnchorPolicy(HookDef.Data hook)
	{
		return hook.HookName == "OnTeamMemberInvite"
		       && hook.Name == "OnTeamMemberInvite [sendinvite]"
		       && hook.TypeName == "RelationshipManager"
		       && hook.Signature.Name == "sendinvite";
	}

	public static bool MatchesOnClanLogoChangedPatchDependencyPolicy(HookDef.Data hook)
	{
		return hook.HookName == "OnClanLogoChanged [patch]"
		       && hook.Name == "OnClanLogoChanged [patch]"
		       && hook.TypeName == "LocalClan/<SetLogo>d__60"
		       && hook.Signature.Name == "MoveNext";
	}

	public static void WarnOnPolicyHashDrift(HookDef.Data hook)
	{
		if (MatchesOnClanCreatedAsyncSuccessRetargetPolicy(hook))
		{
			WarnOnPolicyHashDrift(hook, OnClanCreatedKnownMsilHash, nameof(TryGenerateOnClanCreatedAsyncSuccessRetargetPolicy));
		}

		if (MatchesOnClanLogoChangedPatchDependencyPolicy(hook))
		{
			WarnOnPolicyHashDrift(hook, OnClanLogoChangedPatchKnownMsilHash, nameof(MatchesOnClanLogoChangedPatchDependencyPolicy));
		}
	}

	public static bool ShouldEmitDependencyAttribute(HookDef.Data hook)
	{
		return !MatchesOnTeamMemberInviteSendInviteAnchorPolicy(hook)
		       && !MatchesOnClanLogoChangedPatchDependencyPolicy(hook);
	}

	public static string GetEmittedTargetTypeName(HookDef.Data hook)
	{
		return MatchesOnClanCreatedAsyncSuccessRetargetPolicy(hook) ? "LocalClanBackend" : hook.TypeName;
	}

	public static string GetEmittedTargetMethodName(HookDef.Data hook)
	{
		return MatchesOnClanCreatedAsyncSuccessRetargetPolicy(hook) ? "Create" : hook.Signature.Name;
	}

	public static string[] GetEmittedTargetMethodArgs(HookDef.Data hook)
	{
		return MatchesOnClanCreatedAsyncSuccessRetargetPolicy(hook) ? ["System.UInt64", "System.String"] : hook.Signature.Parameters;
	}

	public static bool TryGeneratePolicyBody(StringBuilder body, HookDef.Data hook)
	{
		return TryGenerateOnClanCreatedAsyncSuccessRetargetPolicy(body, hook);
	}

	public static bool TryEmitModifyAnchorPrelude(StringBuilder body, HookDef.Data hook)
	{
		if (!MatchesOnTeamMemberInviteSendInviteAnchorPolicy(hook))
		{
			return false;
		}

		body.AppendLine(
			"var sendInvite = AccessTools.Method(Carbon.Extensions.AccessToolsEx.TypeByName(\"RelationshipManager+PlayerTeam\"), \"SendInvite\", new System.Type[] { Carbon.Extensions.AccessToolsEx.TypeByName(\"BasePlayer\") });");
		body.AppendLine("int anchorIndex = -1;");
		body.AppendLine("for (int i = 0; i < original.Count - 2; i++) {");
		body.AppendLine("if (original[i].opcode != OpCodes.Ldloc_1) { continue; }");
		body.AppendLine("if (original[i + 1].opcode != OpCodes.Ldloc_3) { continue; }");
		body.AppendLine("if (!Equals(original[i + 2].operand, sendInvite)) { continue; }");
		body.AppendLine("anchorIndex = i;");
		body.AppendLine("break;");
		body.AppendLine("}");
		body.AppendLine("if (anchorIndex < 0) return original.AsEnumerable();");
		return true;
	}

	public static void ConfigureModifyAnchorPolicy(HookDef.Data hook)
	{
		if (MatchesOnTeamMemberInviteSendInviteAnchorPolicy(hook))
		{
			Helper.UseModifyAnchor("anchorIndex", hook.InjectionIndex);
			return;
		}

		Helper.ClearModifyAnchor();
	}

	private static void WarnOnPolicyHashDrift(HookDef.Data hook, string expectedHash, string policyName)
	{
		if (string.IsNullOrWhiteSpace(hook.MsilHash) || hook.MsilHash == expectedHash)
		{
			return;
		}

		Logger.Warning($"{hook.HookName} policy '{policyName}' is applying with a drifted OPJ hash ({hook.MsilHash} != {expectedHash})");
	}

	private static bool TryGenerateOnClanCreatedAsyncSuccessRetargetPolicy(StringBuilder body, HookDef.Data hook)
	{
		if (!MatchesOnClanCreatedAsyncSuccessRetargetPolicy(hook))
		{
			return false;
		}

		Helper.Parameters.Add(("leaderSteamId", typeof(ulong)));
		body.AppendLine(
			"public static void Postfix(ulong leaderSteamId, ref System.Threading.Tasks.ValueTask<ClanValueResult<IClan>> __result) {");
		body.AppendLine("__result = AwaitHookResult(__result, leaderSteamId);");
		body.AppendLine("}");
		body.AppendLine(
			"private static async System.Threading.Tasks.ValueTask<ClanValueResult<IClan>> AwaitHookResult(System.Threading.Tasks.ValueTask<ClanValueResult<IClan>> original, ulong leaderSteamId) {");
		body.AppendLine("ClanValueResult<IClan> result = await original;");
		body.AppendLine("if (result.IsSuccess && result.Value is LocalClan clan) {");
		body.AppendLine($"HookCaller.CallStaticHook({HookStringPool.GetOrAdd(hook.HookName)}u, clan, leaderSteamId);");
		body.AppendLine("}");
		body.AppendLine("return result;");
		body.AppendLine("}");
		body.AppendLine("}");
		body.AppendLine("}");
		body.AppendLine("}");
		body.AppendLine();
		return true;
	}
}
