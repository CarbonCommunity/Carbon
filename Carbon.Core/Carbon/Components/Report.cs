///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carbon.Core;
using Epic.OnlineServices.Sessions;
using Facepunch;
using Harmony;
using Humanlights.Components;
using Humanlights.Extensions;
using Oxide.Plugins;

namespace Carbon
{
	public class Report : IDisposable
	{
		public static Action<string> OnPluginAdded;
		public static Action<Plugin, List<string>> OnPluginCompiled;
		public static Action OnProcessEnded;

		public Dictionary<string, Result> Results;

		public void Init()
		{
			Logger.Log($" Initializing report...");

			Results = new Dictionary<string, Result>(200);

			OnPluginCompiled += (plugin, incompatibleHooks) =>
			{
				Results.Add(plugin.FilePath, new Result
				{
					FilePath = plugin.FilePath,
					FileName = plugin.FileName,
					Plugin = plugin,
					IncompatibleHooks = incompatibleHooks.ToArray()
				});
			};
			OnProcessEnded += () =>
			{
				var report = string.Empty;

				using (var builder = new StringTable("#", "Name", "Author", "Version", "File", "Compile Time", "Hooks", "[HookMethod]s", "[PluginReference]s", "Incompatible Hooks"))
				{
					var counter = 1;
					foreach (var value in Results)
					{
						var result = value.Value;

						builder.AddRow($"{counter:n0}", result.Plugin.Name, result.Plugin.Author, result.Plugin.Version, result.FileName, $"{result.Plugin.CompileTime * 1000:0}ms", $"{result.Plugin.Hooks.ToArray().ToString(", ", " and ").Trim()}", $"{result.Plugin.HookMethods.Select(x => $"{x.Name}").ToArray().ToString(", ", " and ").Trim()}", $"{result.Plugin.PluginReferences.Select(x => $"{x.Field.FieldType.Name} {x.Field.Name}").ToArray().ToString(", ", " and ").Trim()}", $"{result.IncompatibleHooks.ToString(", ", " and ").Trim()}");
						counter++;
					}

					report += $"PLUGIN REPORT:\n{builder.ToStringMinimal()}\n\n";
				}

				using (var builder = new StringTable("#", "Name", "Uses", "Files Affected"))
				{
					var counter = 1;
					var hooks = Pool.GetList<string>();
					foreach (var result in Results) foreach (var hook in result.Value.IncompatibleHooks) if (!hooks.Contains(hook)) hooks.Add(hook);

					foreach (var hook in hooks)
					{
						builder.AddRow($"{counter:n0}", hook, $"{Results.Count(x => x.Value.IncompatibleHooks.Contains(hook)):n0}", $"{Results.Where(x => x.Value.IncompatibleHooks.Contains(hook)).Select(x => $"{x.Value.FileName}").ToArray().ToString(", ", " and ")}");
						counter++;
					}

					Pool.FreeList(ref hooks);

					report += $"INCOMPATIBLE HOOK REPORT:\n{builder.ToStringMinimal()}\n\n";
				}

				var path = Path.Combine(CarbonCore.GetReportsFolder(), $"pluginreport_{DateTime.UtcNow:ddMMyyyyhhmmss}.txt");
				OsEx.File.Create(path, report.Trim());
				Logger.Log($" Report generated with {Results.Count:n0} results at '{path}'");

				Dispose();
			};

			CarbonCore.ReloadPlugins();
		}
		public void Dispose()
		{
			OnPluginAdded = null;
			OnPluginCompiled = null;
			OnProcessEnded = null;

			foreach (var result in Results)
			{
				Array.Clear(result.Value.IncompatibleHooks, 0, result.Value.IncompatibleHooks.Length);
			}

			Results.Clear();
			Results = null;
		}

		public struct Result
		{
			public string FilePath;
			public string FileName;
			public Plugin Plugin;
			public string[] IncompatibleHooks;
		}
	}
}
