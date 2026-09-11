namespace Carbon.Validation;

/// <summary>
///     What a single diagnostic is about.
/// </summary>
internal enum OpjIssueKind
{
	MissingAssembly,
	MissingType,
	MissingMethod,
	MethodRenamed,
	StateMachineRenumbered,
	SignatureDrift,
	ReturnTypeDrift,
	ExposureDrift,
	AmbiguousMethod,
	InjectionIndexOutOfRange,
	InjectionIndexDrift,
	RemoveRangeInvalid,
	InstructionTargetOutOfRange,
	InstructionTargetDrift,
	OperandMissing,
	BodyDrift,
	ArgumentOutOfRange,
	LocalOutOfRange,
	BaselineMismatch,
	PolicyHandled,
	NotImplemented,
}

/// <summary>
///     How actionable a diagnostic is. Ordering matters: higher values are worse.
/// </summary>
internal enum OpjIssueSeverity
{
	/// <summary>Informational only, nothing to do.</summary>
	Info,

	/// <summary>Something drifted but the hook still works; worth a look eventually.</summary>
	Warning,

	/// <summary>A concrete fix was computed and can be applied automatically.</summary>
	AutoFixable,

	/// <summary>The hook is broken or too ambiguous to fix without human input.</summary>
	NeedsHuman,
}

internal enum AlignmentConfidence
{
	None,
	Low,
	Medium,
	High,
}

/// <summary>
///     A concrete, mechanical edit to the hook's JSON. <see cref="Path" /> is relative to the hook's
///     "Hook" object and uses the raw OPJ property names (e.g. "Signature.Parameters", "MSILHash",
///     "Instructions[3].Operand").
/// </summary>
internal sealed record OpjFixEdit(string Path, object? OldValue, object? NewValue);

/// <summary>
///     A single finding for a hook, optionally carrying the edits that would resolve it.
/// </summary>
internal sealed class OpjDiagnostic(OpjIssueKind kind, OpjIssueSeverity severity, string message)
{
	public OpjIssueKind Kind { get; } = kind;

	public OpjIssueSeverity Severity { get; private set; } = severity;

	public string Message { get; private set; } = message;

	/// <summary>
	///     Escalates an auto-fixable diagnostic to needs-human; its fixes become printed proposals
	///     instead of applied edits.
	/// </summary>
	public void Demote(string reason)
	{
		if (Severity != OpjIssueSeverity.AutoFixable)
		{
			return;
		}

		Severity = OpjIssueSeverity.NeedsHuman;
		Message += $" [not auto-applied: {reason}]";
	}

	/// <summary>Edits that resolve this diagnostic. Only applied when severity is AutoFixable.</summary>
	public List<OpjFixEdit> Fixes { get; } = [];

	/// <summary>Ranked human-readable alternatives when a fix could not be chosen automatically.</summary>
	public List<string> Candidates { get; } = [];

	/// <summary>Set once the fixer wrote the edits into the OPJ.</summary>
	public bool Applied { get; set; }
}

/// <summary>
///     All findings for a single hook definition, addressed by manifest/hook index so fixes can be
///     applied to the raw JSON without relying on hook names being unique.
/// </summary>
internal sealed class OpjHookCheck(
	int manifestIndex,
	int hookIndex,
	string action,
	string assemblyName,
	string name,
	string hookName,
	string typeName,
	string methodName,
	bool flagged)
{
	public int ManifestIndex { get; } = manifestIndex;

	public int HookIndex { get; } = hookIndex;

	public string Action { get; } = action;

	public string AssemblyName { get; } = assemblyName;

	public string Name { get; } = name;

	public string HookName { get; } = hookName;

	public string TypeName { get; } = typeName;

	public string MethodName { get; } = methodName;

	/// <summary>Whether the patcher itself has this hook flagged for review.</summary>
	public bool Flagged { get; } = flagged;

	public List<OpjDiagnostic> Diagnostics { get; } = [];

	public OpjIssueSeverity Severity => Diagnostics.Count == 0 ? OpjIssueSeverity.Info : Diagnostics.Max(x => x.Severity);

	public OpjDiagnostic Add(OpjIssueKind kind, OpjIssueSeverity severity, string message)
	{
		var diagnostic = new OpjDiagnostic(kind, severity, message);
		Diagnostics.Add(diagnostic);
		return diagnostic;
	}
}

internal sealed class OpjCheckReport
{
	public List<OpjHookCheck> Hooks { get; } = [];

	public IEnumerable<OpjHookCheck> WithIssues => Hooks.Where(x => x.Diagnostics.Count > 0);

	public int CountBySeverity(OpjIssueSeverity severity)
	{
		return Hooks.Count(x => x.Diagnostics.Count > 0 && x.Severity == severity);
	}

	/// <summary>Diagnostic-level count; hooks whose worst issue needs a human can still carry fixes.</summary>
	public int AutoFixableDiagnostics => Hooks.Sum(x => x.Diagnostics.Count(d => d.Severity == OpjIssueSeverity.AutoFixable));

	public int CleanCount => Hooks.Count(x => x.Diagnostics.Count == 0);
}

internal sealed record OpjCheckOptions(
	string ManagedFolder,
	string? OldManagedFolder
);
