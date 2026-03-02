using API.Analytics;
using API.Assembly;
using API.Commands;
using API.Contracts;
using API.Events;
using Carbon.Profiler;
using Facepunch;
using Newtonsoft.Json;

namespace Carbon;

public partial class Community
{
	public Config Config { get; set; }
	public MonoProfilerConfig MonoProfilerConfig => MonoProfilerConfig.Instance;

	public void ForceEnsurePublicizedAssembly(string value, ref bool needsSave)
	{
		if (Config.Publicizer.PublicizedAssemblies.Contains(value))
		{
			return;
		}
		Config.Publicizer.PublicizedAssemblies.Add(value);
		needsSave = true;
	}

	/// <summary>
	/// Load Carbon config from disk.
	/// </summary>
	/// <param name="postProcess">Ensures that default mandatory values are properly present in the config after it's been loaded.</param>
	public void LoadConfig(bool postProcess = true)
	{
		var needsSave = false;

		if (!OsEx.File.Exists(Defines.GetConfigFile()))
		{
			needsSave = true;
			Config ??= new();
		}
		else
		{
			Config = JsonConvert.DeserializeObject<Config>(OsEx.File.ReadText(Defines.GetConfigFile()));
		}

		if (postProcess)
		{
			if (Config.Compiler.ConditionalCompilationSymbols == null)
			{
				Config.Compiler.ConditionalCompilationSymbols = new();
				needsSave = true;
			}

			if (string.IsNullOrEmpty(Config.Permissions.AdminDefaultGroup))
				Config.Permissions.AdminDefaultGroup = "admin";

			if (string.IsNullOrEmpty(Config.Permissions.PlayerDefaultGroup))
				Config.Permissions.PlayerDefaultGroup = "default";

			if (string.IsNullOrEmpty(Config.Permissions.ModeratorDefaultGroup))
				Config.Permissions.ModeratorDefaultGroup = "moderator";

			if (!Config.Compiler.ConditionalCompilationSymbols.Contains("CARBON"))
				Config.Compiler.ConditionalCompilationSymbols.Add("CARBON");

			if (!Config.Compiler.ConditionalCompilationSymbols.Contains("RUST"))
				Config.Compiler.ConditionalCompilationSymbols.Add("RUST");

			if (!Config.Compiler.ConditionalCompilationSymbols.Contains("OXIDE_PUBLICIZED"))
				Config.Compiler.ConditionalCompilationSymbols.Add("OXIDE_PUBLICIZED");

			Config.Compiler.ConditionalCompilationSymbols =
				Config.Compiler.ConditionalCompilationSymbols.Distinct().ToList();

			if (Config.Prefixes == null)
			{
				Config.Prefixes = new();
				needsSave = true;
			}

			if (Config.Aliases == null)
			{
				Config.Aliases = new();
				needsSave = true;
			}
			else
			{
				var invalidAliases = Pool.Get<List<string>>();
				invalidAliases.AddRange(from alias in Config.Aliases
										where !Config.IsValidAlias(alias.Key, out _) || alias.Key == alias.Value
										select alias.Key);

				foreach (var invalidAlias in invalidAliases)
				{
					Config.Aliases.Remove(invalidAlias);
					Logger.Warn($" Removed invalid alias: {invalidAlias}");
				}

				if (invalidAliases.Count > 0)
				{
					needsSave = true;
				}

				Pool.FreeUnmanaged(ref invalidAliases);

				foreach (var alias in Config.Aliases)
				{
					if (!ConsoleSystem.Index.All.Any(x =>
						    x.FullName.Equals(alias.Key, StringComparison.OrdinalIgnoreCase)))
					{
						continue;
					}

					Logger.Warn($" Alias '{alias.Key}' overrides an already existing Rust command");
				}
			}

			if (Config.Prefixes.Count == 0)
			{
				Config.Prefixes.Add(new()
				{
					Value = "/",
					PrintToChat = false,
					PrintToConsole = false,
					SuggestionAuthLevel = 2
				});
			}

			Config.Publicizer.PublicizedAssemblies ??= new();
			ForceEnsurePublicizedAssembly("Assembly-CSharp.dll", ref needsSave);
			ForceEnsurePublicizedAssembly("Facepunch.Console.dll", ref needsSave);
			ForceEnsurePublicizedAssembly("Facepunch.Network.dll", ref needsSave);
			ForceEnsurePublicizedAssembly("Facepunch.Nexus.dll", ref needsSave);
			ForceEnsurePublicizedAssembly("Facepunch.Ping.dll", ref needsSave);
			ForceEnsurePublicizedAssembly("Facepunch.Unity.dll", ref needsSave);
			ForceEnsurePublicizedAssembly("Rust.Localization.dll", ref needsSave);
			ForceEnsurePublicizedAssembly("Rust.Clans.Local.dll", ref needsSave);
			ForceEnsurePublicizedAssembly("Rust.FileSystem.dll", ref needsSave);
			ForceEnsurePublicizedAssembly("Rust.Harmony.dll", ref needsSave);
			ForceEnsurePublicizedAssembly("Rust.Global.dll", ref needsSave);
			ForceEnsurePublicizedAssembly("Rust.Data.dll", ref needsSave);

			Config.Publicizer.PublicizerMemberIgnores ??=
			[
				"^HiddenValueBase$",
				"^HiddenValue`1$",
				"^Pool$"
			];

			if (Config.Aliases.Count == 0)
			{
				Config.Aliases["carbon"] = "c.version";
				Config.Aliases["plugins"] = "c.plugins";
				Config.Aliases["reload"] = "c.reload";
				Config.Aliases["load"] = "c.load";
				Config.Aliases["unload"] = "c.unload";
				needsSave = true;
			}
			else if (Config.Aliases.Remove("harmony.load") ||
			         Config.Aliases.Remove("harmony.unload"))
			{
				needsSave = true;
			}

			// Mandatory for across the board access
			API.Commands.Command.Prefixes = Config.Prefixes;

			Logger.CoreLog ??= new("Carbon.Core");
			Logger.CoreLog.SplitSize = (int)(Config.Logging.LogSplitSize * 1000000f);

			if (needsSave)
			{
				SaveConfig();
			}
		}

		if (Config.Analytics.Enabled)
		{
			Logger.Warn($"Carbon Analytics are ON. They're entirely anonymous and help us to further improve.");
		}
		else
		{
			Logger.Error($"Carbon Analytics are OFF.");
		}
	}

	/// <summary>
	/// Save Carbon config to disk.
	/// </summary>
	public void SaveConfig()
	{
		Config ??= new Config();

		OsEx.File.Create(Defines.GetConfigFile(), JsonConvert.SerializeObject(Config, Formatting.Indented));
	}

	/// <summary>
	/// Load Carbon MonoProfiler config from disk.
	/// </summary>
	public void LoadMonoProfilerConfig()
	{
		MonoProfilerConfig.Load(Defines.GetMonoProfilerConfigFile());
	}

	/// <summary>
	/// Save Carbon MonoProfiler config to disk.
	/// </summary>
	public void SaveMonoProfilerConfig()
	{
		MonoProfilerConfig.Save(Defines.GetMonoProfilerConfigFile());
	}
}
