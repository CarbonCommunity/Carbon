using API.Analytics;
using API.Assembly;
using API.Commands;
using API.Contracts;
using API.Events;
using Carbon.Client;
using Carbon.Profiler;
using Facepunch;
using Newtonsoft.Json;

namespace Carbon;

public partial class Community
{
	public Config Config { get; set; }
	public MonoProfilerConfig MonoProfilerConfig { get; set; }
	public ClientConfig ClientConfig { get; set; }

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
										where !Config.IsValidAlias(alias.Key, out _)
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

			if (Config.Aliases.Count == 0)
			{
				Config.Aliases["carbon"] = "c.version";
				Config.Aliases["harmony.load"] = "c.harmonyload";
				Config.Aliases["harmony.unload"] = "c.harmonyunload";
			}

			// Mandatory for across the board access
			API.Commands.Command.Prefixes = Config.Prefixes;

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
		var needsSave = false;

		if (!OsEx.File.Exists(Defines.GetMonoProfilerConfigFile()))
		{
			MonoProfilerConfig ??= new();
			needsSave = true;
		}
		else
		{
			MonoProfilerConfig = JsonConvert.DeserializeObject<MonoProfilerConfig>(OsEx.File.ReadText(Defines.GetMonoProfilerConfigFile()));
		}

		if (needsSave)
		{
			SaveMonoProfilerConfig();
		}
	}

	/// <summary>
	/// Save Carbon MonoProfiler config to disk.
	/// </summary>
	public void SaveMonoProfilerConfig()
	{
		MonoProfilerConfig ??= new();

		OsEx.File.Create(Defines.GetMonoProfilerConfigFile(), JsonConvert.SerializeObject(MonoProfilerConfig, Formatting.Indented));
	}

	/// <summary>
	/// Load Carbon MonoProfiler config from disk.
	/// </summary>
	public void LoadClientConfig()
	{
		var needsSave = false;

		if (!OsEx.File.Exists(Defines.GetClientConfigFile()))
		{
			ClientConfig ??= new();
			needsSave = true;
		}
		else
		{
			ClientConfig = JsonConvert.DeserializeObject<ClientConfig>(OsEx.File.ReadText(Defines.GetClientConfigFile()));
			ClientConfig.RefreshAddonCache();
		}

		if (needsSave)
		{
			SaveClientConfig();
		}
	}

	/// <summary>
	/// Save Carbon MonoProfiler config to disk.
	/// </summary>
	public void SaveClientConfig()
	{
		ClientConfig ??= new();

		OsEx.File.Create(Defines.GetClientConfigFile(), JsonConvert.SerializeObject(ClientConfig, Formatting.Indented));
	}
}
