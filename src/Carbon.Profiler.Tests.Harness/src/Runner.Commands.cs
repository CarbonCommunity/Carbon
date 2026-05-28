using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Carbon.Profiler.Tests.Harness;

internal sealed partial class Runner
{
	private IEnumerator WaitForCommands()
	{
		var deadline = Time.realtimeSinceStartup + 120f;
		string[] requiredCommands =
		[
			"carbon.profile",
			"carbon.abort_profile",
			"carbon.export_profile",
			"carbon.tracked",
			"carbon.profiler_version",
		];

		while (true)
		{
			var missing = requiredCommands.Where(command => !HasCommand(command)).ToArray();
			if (missing.Length == 0)
			{
				RunnerLog.Info("Profiler commands registered");
				yield break;
			}

			if (Time.realtimeSinceStartup > deadline)
			{
				Fail("Timed out waiting for profiler commands: " + string.Join(", ", missing));
				yield break;
			}

			yield return new WaitForSeconds(0.5f);
		}
	}

	private static bool HasCommand(string command)
	{
		return ServerIndexContains(command) ||
		       ConsoleSystem.Index.All?.Any(x => x.FullName == command) == true;
	}

	// release vs staging branch fix for now
	private static bool ServerIndexContains(string command)
	{
		try
		{
			var dictField = typeof(ConsoleSystem.Index.Server).GetField("Dict", BindingFlags.Public | BindingFlags.Static);
			if (dictField?.GetValue(null) is not IDictionary dict)
			{
				return false;
			}

			foreach (var keyCandidate in dict.Keys)
			{
				if (keyCandidate is string keyString && string.Equals(keyString, command, StringComparison.Ordinal))
				{
					return true;
				}

				if (keyCandidate != null && string.Equals(keyCandidate.ToString(), command, StringComparison.Ordinal))
				{
					return true;
				}
			}

			var keyType = dict.GetType().GetGenericArguments().FirstOrDefault();
			if (keyType == null)
			{
				return false;
			}

			var key = keyType == typeof(string) ? command : Activator.CreateInstance(keyType, command);
			return key != null && dict.Contains(key);
		}
		catch
		{
			return false;
		}
	}
}
