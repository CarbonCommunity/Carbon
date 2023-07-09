/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Components;

public class Report : IDisposable
{
	public static Action<string> OnPluginAdded;
	public static Action<Plugin> OnPluginCompiled;
	public static Action OnProcessEnded;

	public Dictionary<string, Result> Results;

	public void Init()
	{
		Logger.Log($" Initializing report...");

		Results = new Dictionary<string, Result>(200);

		OnPluginCompiled = (plugin) =>
		{
			if (Results.ContainsKey(plugin.FilePath)) return;

			Results.Add(plugin.FilePath, new Result
			{
				FilePath = plugin.FilePath,
				FileName = plugin.FileName,
				Plugin = plugin
			});
		};
		OnProcessEnded = () =>
		{
			var report = string.Empty;

			using (var builder = new StringTable("#", "Name", "Author", "Version", "File", "Compile Time", "Hooks", "[HookMethod]s", "[PluginReference]s"))
			{
				var counter = 1;
				foreach (var value in Results)
				{
					var result = value.Value;

					builder.AddRow($"{counter:n0}", result.Plugin.Name, result.Plugin.Author, result.Plugin.Version, result.FileName, $"{result.Plugin.CompileTime:0}ms", $"{result.Plugin.Hooks.Where(x => !string.IsNullOrEmpty(HookStringPool.GetOrAdd(x.Key))).Select(x => HookStringPool.GetOrAdd(x.Key)).ToArray().ToString(", ", " and ").Trim()}", $"{result.Plugin.HookMethods.Select(x => $"{x.Name}").ToArray().ToString(", ", " and ").Trim()}", $"{result.Plugin.PluginReferences.Select(x => $"{x.Field.FieldType.Name} {x.Field.Name}").ToArray().ToString(", ", " and ").Trim()}");
					counter++;
				}

				report += $"PLUGIN REPORT:\n{builder.ToStringMinimal()}\n\n";
			}

			// Failed plugins
			{
				var result = "";
				var count = 1;

				foreach (var mod in ModLoader.FailedMods)
				{
					result += $"{count:n0}. {mod.File}\n";

					foreach (var error in mod.Errors)
					{
						result += $" {error}\n";
					}

					result += "\n";
					count++;
				}

				report += $"COMPILATION FAILED PLUGINS:\n{result}\n\n";
			}

			var path = Path.Combine(Defines.GetReportsFolder(), $"pluginreport_{DateTime.UtcNow:ddMMyyyyhhmmss}.txt");
			OsEx.File.Create(path, report.Trim());
			Logger.Log($" Report generated with {Results.Count:n0} results at '{path}'");

			Dispose();
		};

		Community.Runtime.ReloadPlugins();
	}
	public void Dispose()
	{
		OnPluginAdded = null;
		OnPluginCompiled = null;
		OnProcessEnded = null;

		Results.Clear();
		Results = null;
	}

	public struct Result
	{
		public string FilePath;
		public string FileName;
		public Plugin Plugin;
	}
}
