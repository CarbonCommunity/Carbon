using Carbon.Utility;
using static Carbon.Projects.Oxide.Oxide;

namespace Carbon.Generation;

internal sealed record HookWorkItem(Manifest Manifest, string Action, HookDef.Data Hook, int Order);

internal sealed record HookGenerationResult(
	int Order,
	string Name,
	string HookName,
	string Category,
	string Source,
	bool Success,
	string Action,
	string? BaseHookName,
	string? Error
)
{
	public static HookGenerationResult Generated(int order, HookDef.Data hook, string source)
	{
		return new HookGenerationResult(order, hook.Name, hook.HookName, hook.HookCategory, source, true, string.Empty, hook.BaseHookName,
			null);
	}

	public static HookGenerationResult Failed(int order, HookDef.Data hook, string action, string? error = null)
	{
		return new HookGenerationResult(order, hook.Name, hook.HookName, hook.HookCategory ?? "None", string.Empty, false, action,
			hook.BaseHookName, error);
	}
}

internal sealed record HookGenerationReport(
	IReadOnlyList<HookGenerationResult> SuccessfulHooks,
	IReadOnlyList<HookGenerationResult> FailedHooks
);

internal sealed record HookahGenerationOptions(int Jobs, ValidationMode ValidationMode, bool Deterministic);
