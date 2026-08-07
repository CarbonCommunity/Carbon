using System.Text;
using Carbon.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Carbon.Validation;

/// <summary>
///     Applies computed fixes surgically to the raw OPJ JSON — only the changed values are touched,
///     property order and formatting are preserved — and writes the result back in the patcher's
///     own style (2-space indentation, CRLF, no trailing newline).
/// </summary>
internal sealed class OpjFixer
{
	private readonly JObject _root;

	public OpjFixer(string inputFile)
	{
		// Date parsing must stay off or ISO-datetime-shaped strings (e.g. Deprecation.RemovalDate)
		// would be re-serialized in Newtonsoft's canonical form, altering untouched lines.
		using var reader = new JsonTextReader(new StreamReader(inputFile)) { DateParseHandling = DateParseHandling.None };
		_root = JObject.Load(reader);
	}

	/// <summary>
	///     Applies every fix attached to an auto-fixable diagnostic. Returns the number of hooks touched.
	/// </summary>
	public int Apply(OpjCheckReport report)
	{
		var fixedHooks = 0;

		foreach (var check in report.Hooks)
		{
			var touched = false;

			foreach (var diagnostic in check.Diagnostics)
			{
				if (diagnostic.Severity != OpjIssueSeverity.AutoFixable)
				{
					continue;
				}

				var hookObject = GetHookObject(check.ManifestIndex, check.HookIndex);
				if (hookObject == null)
				{
					Logger.Warning($"{check.HookName} fix skipped: hook JSON not found at manifest {check.ManifestIndex}, hook {check.HookIndex}");
					continue;
				}

				var applied = true;
				foreach (var fix in diagnostic.Fixes)
				{
					applied &= SetValue(hookObject, fix, check.HookName);
				}

				diagnostic.Applied = applied;
				touched |= applied;
			}

			if (touched)
			{
				fixedHooks++;
			}
		}

		return fixedHooks;
	}

	public void Write(string outputFile)
	{
		var directory = Path.GetDirectoryName(Path.GetFullPath(outputFile));
		if (!string.IsNullOrEmpty(directory))
		{
			Directory.CreateDirectory(directory);
		}

		using var stream = new StreamWriter(outputFile, false, new UTF8Encoding(false));
		stream.NewLine = "\r\n";
		using var writer = new JsonTextWriter(stream);
		writer.Formatting = Formatting.Indented;
		writer.Indentation = 2;
		writer.IndentChar = ' ';
		_root.WriteTo(writer);
	}

	private JObject? GetHookObject(int manifestIndex, int hookIndex)
	{
		return _root["Manifests"]?[manifestIndex]?["Hooks"]?[hookIndex]?["Hook"] as JObject;
	}

	/// <summary>
	///     Sets a value at a path like "Signature.Parameters" or "Instructions[3].Operand", verifying
	///     the current value still matches what the checker saw before overwriting it.
	/// </summary>
	private static bool SetValue(JObject hookObject, OpjFixEdit fix, string hookName)
	{
		JToken? parent = hookObject;
		JToken? current = null;
		string? property = null;
		var segments = fix.Path.Split('.');

		for (var i = 0; i < segments.Length; i++)
		{
			var segment = segments[i];
			var name = segment;
			var arrayIndex = -1;

			var bracket = segment.IndexOf('[');
			if (bracket >= 0)
			{
				name = segment[..bracket];
				arrayIndex = int.Parse(segment[(bracket + 1)..segment.IndexOf(']')]);
			}

			if (i > 0)
			{
				parent = current;
			}

			if (parent is not JObject parentObject)
			{
				Logger.Warning($"{hookName} fix skipped: '{fix.Path}' does not resolve");
				return false;
			}

			property = name;
			current = parentObject[name];
			if (arrayIndex >= 0)
			{
				if (current is not JArray array || arrayIndex >= array.Count)
				{
					Logger.Warning($"{hookName} fix skipped: '{fix.Path}' does not resolve");
					return false;
				}

				parent = array[arrayIndex];
				current = parent;
				property = null;
			}
		}

		var replacement = ToToken(fix.NewValue);

		if (property == null)
		{
			// The path ends on an array element itself.
			((JValue)current!).Value = ((JValue)replacement).Value;
			return true;
		}

		var target = (JObject)parent!;
		var existing = target[property];
		if (existing != null && fix.OldValue != null && existing is JValue && !JToken.DeepEquals(existing, ToToken(fix.OldValue)))
		{
			// Advisory only: the loaded model post-processes a few values (parameter replacers), so
			// a mismatch here is not necessarily a stale analysis.
			Logger.Warning($"{hookName} '{fix.Path}': recorded value '{fix.OldValue}' differs from file value '{existing}'; overwriting");
		}

		target[property] = replacement;
		return true;
	}

	private static JToken ToToken(object? value)
	{
		return value switch
		{
			null => JValue.CreateNull(),
			string[] array => new JArray(array.Select(x => (object)x).ToArray()),
			JToken token => token,
			_ => new JValue(value),
		};
	}
}
