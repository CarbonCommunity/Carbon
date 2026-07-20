using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Carbon.Base;
using Carbon.Components;
using Carbon.Extensions;
using Cronos;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Plugins;
using Random = UnityEngine.Random;

namespace Carbon.Modules;

public partial class AutoWipeModule : CarbonModule<AutoWipeConfig, AutoWipeData>
{
	public static AutoWipeModule Singleton;

	public override string Name => "AutoWipe";
	public override VersionNumber Version => new(2, 0, 0);
	public override Type Type => typeof(AutoWipeModule);
	public override bool EnabledByDefault => false;

	private readonly char[] splitter = new[] { '|' };
	private readonly float wipeCooldown = 60 * 60;
	private readonly float wipeTick = 30;
	private Timer wipeTimer;

	public bool InCooldown() => (DateTime.UtcNow - new DateTime(DataInstance.LastWipeTime)).TotalSeconds <= wipeCooldown;

	public override void Init()
	{
		base.Init();
		Singleton = this;
	}

	public override void Load()
	{
		base.Load();

		if (!IsEnabled() || Community.IsServerInitialized)
		{
			return;
		}

		if (InCooldown())
		{
			Singleton.PutsWarn($"Initialized world config [WIPE_COOLDOWN]");
			DataInstance.Wipe?.InitWorld(ConfigInstance.Maps, DataInstance.LastWipeTime);
			return;
		}

		if (DataInstance.NextWipe == null)
		{
			DataInstance.NextWipe = GetUpcomingAvailableWipeImpl();
		}

		if (!string.IsNullOrEmpty(ConfigInstance.WipeChatCommand))
		{
			UpdateWipeChatCommand(null, ConfigInstance.WipeChatCommand);
		}

		var currentWipe = DataInstance.Wipe;
		var wipe = DataInstance.NextWipe ?? currentWipe;
		var justWiped = wipe != null && !wipe.Equals(currentWipe);

		if (justWiped)
		{
			var config = ConfigInstance.GetWipeConfig(wipe);
			DataInstance.LastWipeTime = DateTime.UtcNow.Ticks;
			DataInstance.NextWipe = null;
			ConVar.Server.autoUploadMap = false;

			if (wipe.Temp)
			{
				ConfigInstance.AvailableWipes.Remove(wipe);
				PutsWarn($"Removed map from list");
			}

			DataInstance.Wipe ??= new();
			wipe.CopyTo(DataInstance.Wipe);
			PutsWarn($"New wipe detected!");
			DataInstance.Wipe?.InitWorld(ConfigInstance.Maps, DataInstance.LastWipeTime);

			if (config.PostWipeCommands != null)
			{
				for(int i = 0; i < config.PostWipeCommands.Length; i++)
				{
					var command = config.PostWipeCommands[i];
					if (string.IsNullOrEmpty(command))
						continue;
					ConsoleSystem.Run(ConsoleSystem.Option.Server.Quiet(), command);
				}
			}

			if (config.PostWipeDeletes != null)
			{
				for(int i = 0; i < config.PostWipeDeletes.Length; i++)
				{
					var delete = config.PostWipeDeletes[i];
					if (string.IsNullOrEmpty(delete))
						continue;

					if (delete.Contains("*"))
					{
						var directoryPath = Path.GetDirectoryName(delete);
						var searchPattern = Path.GetFileName(delete);

						if (string.IsNullOrEmpty(directoryPath))
						{
							directoryPath = ".";
						}

						if (Directory.Exists(directoryPath))
						{
							try
							{
								var matchingFiles = Directory.GetFiles(directoryPath, searchPattern);
								for (int o = 0; o < matchingFiles.Length; o++)
								{
									var file = matchingFiles[o];
									File.Delete(file);
									PutsWarn($"Deleting scheduled file '{file}'");
								}
							}
							catch (Exception ex)
							{
								PutsError($"Error deleting files matching pattern '{delete}'", ex);
							}
						}
						continue;
					}

					if (OsEx.File.Exists(delete))
					{
						OsEx.File.Delete(delete);
						PutsWarn($"Deleting scheduled file '{delete}'");
						continue;
					}

					if (OsEx.Folder.Exists(delete))
					{
						OsEx.Folder.Delete(delete);
						PutsWarn($"Deleting scheduled directory '{delete}'");
					}
				}
			}

			Save();
		}
		else
		{
			Singleton.PutsWarn($"Initialized world config");
			DataInstance.Wipe?.InitWorld(ConfigInstance.Maps, DataInstance.LastWipeTime);
		}
	}

	public bool UpdateWipeChatCommand(string old, string current)
	{
		if (old == current)
		{
			return false;
		}

		var hasChanged = false;

		if (!string.IsNullOrEmpty(old))
		{
			Community.Runtime.Core.cmd.RemoveChatCommand(old, this);
			hasChanged = true;
		}

		if (!string.IsNullOrEmpty(current))
		{
			Community.Runtime.Core.cmd.AddChatCommand(current, this, nameof(WipeChat));
			hasChanged = true;
		}

		return hasChanged;
	}

	private void WipeChat(BasePlayer player, string cmd, string[] args)
	{
		var nextWipe = GetUpcomingWipeImpl();
		if (nextWipe.wipe == null)
		{
			player.ChatMessage($"No available wipe found");
			return;
		}

		var result = (nextWipe.next.GetValueOrDefault() - DateTime.UtcNow).TotalSeconds;
		player.ChatMessage($"Next wipe happens in <color=orange>{TimeEx.Format(result, false).ToLower()}</color>");
	}

	public override void OnServerInit(bool initial)
	{
		base.OnServerInit(initial);
		OnEnableStatus();
	}

	public override bool PreLoadShouldSave(bool newConfig, bool newData)
	{
		var invalidConfigCorrected = false;

		if (ConfigInstance.Maps == null)
		{
			ConfigInstance.Maps = new();
			invalidConfigCorrected = true;
		}

		return invalidConfigCorrected;
	}

	public override void OnEnabled(bool initialized)
	{
		base.OnEnabled(initialized);

		if (initialized)
		{
			if (wipeTimer != null)
			{
				wipeTimer.Destroy();
			}
			wipeTimer = Community.Runtime.Core.timer.Every(wipeTick, WipeTickImpl);
		}
	}

	public override void OnDisabled(bool initialized)
	{
		base.OnDisabled(initialized);
		if (initialized)
		{
			if (wipeTimer != null)
			{
				wipeTimer.Destroy();
				wipeTimer = null;
			}
		}
	}

	public override void OnUnload()
	{
		if (wipeTimer != null)
		{
			wipeTimer.Destroy();
			wipeTimer = null;
		}
		base.OnUnload();
	}

	private void RefreshHostName()
	{
		if (DataInstance == null)
		{
			return;
		}

		var lastWipeDate = new DateTime(DataInstance.LastWipeTime);

		if (!string.IsNullOrEmpty(ConVar.Server.hostname) && HasReplacements(ConVar.Server.hostname))
		{
			ConVar.Server.hostname = ProcessString(ConVar.Server.hostname, lastWipeDate);
			PutsWarn("Updated server hostname replacements");
		}
		if (!string.IsNullOrEmpty(ConVar.Server.description) && HasReplacements(ConVar.Server.description))
		{
			ConVar.Server.description = ProcessString(ConVar.Server.description, lastWipeDate);
			PutsWarn("Updated server description replacements");
		}

		return;

		static string ProcessString(string source, DateTime time)
		{
			return source
				.Replace("[WIPE_DAY]", $"{time.Day}")
				.Replace("[WIPE_MONTH]", $"{time.Month}")
				.Replace("[WIPE_YEAR]", $"{time.Year}")
				.Replace("[WIPE_HOUR]", $"{time.Hour}")
				.Replace("[WIPE_MINUTE]", $"{time.Minute}");
		}

		static bool HasReplacements(string source)
		{
			return source.Contains("[WIPE_DAY]") ||
			       source.Contains("[WIPE_MONTH]") ||
			       source.Contains("[WIPE_YEAR]") ||
			       source.Contains("[WIPE_HOUR]") ||
			       source.Contains("[WIPE_MINUTE]");
		}
	}

	private void OnServerInformationUpdated()
	{
		RefreshHostName();
	}

	private void WipeTickImpl()
	{
		if (!IsEnabled() || InCooldown())
		{
			return;
		}

		DataInstance.NextWipe = GetUpcomingAvailableWipeImpl();

		if (DataInstance.NextWipe == null)
		{
			return;
		}

		if (DataInstance.NextWipe.Commands != null)
		{
			for(int i = 0; i < DataInstance.NextWipe.Commands.Length; i++)
			{
				var command = DataInstance.NextWipe.Commands[i];
				if (string.IsNullOrEmpty(command))
					continue;
				ConsoleSystem.Run(ConsoleSystem.Option.Server.Quiet(), command);
			}
		}

		wipeTimer.Destroy();
		Save();
	}

	private Wipe GetUpcomingAvailableWipeImpl()
	{
		for (int i = 0; i < ConfigInstance.AvailableWipes.Count; i++)
		{
			var wipe = ConfigInstance.AvailableWipes[i];
			if (wipe.ShouldWipe())
			{
				return wipe;
			}
		}
		return null;
	}

	private (Wipe wipe, DateTime? next) GetUpcomingWipeImpl()
	{
		var now = DateTime.UtcNow;
		var nextRun = ConfigInstance.AvailableWipes.Select(job => (job, CronExpression.Parse(job.Cron).GetNextOccurrence(now, TimeZoneInfo.Utc)))
			.Where(x => x.Item2.HasValue)
			.OrderBy(x => x.Item2)
			.FirstOrDefault();

		return nextRun;
	}

	[ConsoleCommand("autowipe.wipes", "Prints all available wipes present in the Wipes config property.")]
	[AuthLevel(2)]
	private void print_wipes(ConsoleSystem.Arg arg)
	{
		using var table = new StringTable("#", "wipe name", "mapurl", "mapsize", "serverseed", "type", "temp", "nextwipe", "wipecommands");
		for (int i = 0; i < ConfigInstance.AvailableWipes.Count; i++)
		{
			var wipe = ConfigInstance.AvailableWipes[i];
			table.AddRow(i + 1, wipe.WipeName, wipe.MapUrl, wipe.MapSize,
				wipe.ServerSeed == 0 ? "random" : wipe.ServerSeed,
				wipe.Type, wipe.Temp ? "yes" : "no", wipe.Cron, wipe.Commands?.ToString("->"));
		}

		arg.ReplyWith(table.ToStringMinimal());
	}

	[ConsoleCommand("autowipe.delete", "Deletes an existent wipe present in the Wipes config property.")]
	[AuthLevel(2)]
	private void delete_wipe(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs())
		{
			arg.ReplyWith("Provide an index from 'autowipe.wipes'");
			return;
		}

		var i = arg.GetInt(0) - 1;
		if (i < 0 || i >= ConfigInstance.AvailableWipes.Count)
		{
			arg.ReplyWith("Went above or below indexes available. Use numbers from 'autowipe.wipes`'");
			return;
		}

		ConfigInstance.AvailableWipes.RemoveAt(i);
		Save();
		arg.ReplyWith("Removed wipe");
	}

	[ConsoleCommand("autowipe.add", "Adds a new wipe to the list.")]
	[AuthLevel(2)]
	private void add_wipe(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs(8))
		{
			arg.ReplyWith("You've got missing arguments. Please make sure to follow the following syntax:\n" +
			              "eg. autowipe.add \"<WipeName>\" \"<MapBrowserName>\" \"<MapUrl>\" \"<MapSize>\" \"<ServerSeed|0=random>\" \"<Type|0=fullwipe 1=mapwipe>\" \"<Temp|True/False>\" \"<Cron>\" \"<Commands>\"");
			return;
		}

		ConfigInstance.AvailableWipes.Add(new()
		{
			WipeName = arg.GetString(0),
			MapBrowserName = arg.GetString(1),
			MapUrl = arg.GetString(2),
			MapSize = arg.GetInt(3),
			ServerSeed = arg.GetInt(4),
			Type = (WipeTypes)arg.GetInt(5),
			Temp = arg.GetBool(6),
			Cron = arg.GetString(7),
			Commands = arg.GetString(8).Split(splitter, StringSplitOptions.RemoveEmptyEntries)
		});
		Save();
		arg.ReplyWith("Added wipe");
	}

	[ConsoleCommand("autowipe.maps", "Prints all available map urls present in the MapPool config property.")]
	[AuthLevel(2)]
	private void print_maps(ConsoleSystem.Arg arg)
	{
		using var table = new StringTable("", "map url", "temporary");
		for (int i = 0; i < ConfigInstance.Maps.Count; i++)
		{
			var wipe = ConfigInstance.Maps[i];
			table.AddRow(i + 1, wipe.Url, wipe.Temp ? "temp" : "standard");
		}

		arg.ReplyWith(table.ToStringMinimal());
	}

	[ConsoleCommand("autowipe.deletemap", "Deletes an existent map url present in the MapPool config property.")]
	[AuthLevel(2)]
	private void delete_map(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs())
		{
			arg.ReplyWith("Provide an index from 'autowipe.maps'");
			return;
		}

		var i = arg.GetInt(0);
		if (i < 0 || i >= ConfigInstance.AvailableWipes.Count)
		{
			arg.ReplyWith("Went above or below indexes available. Use numbers from 'autowipe.maps`'");
			return;
		}

		ConfigInstance.AvailableWipes.RemoveAt(i);
		Save();
		arg.ReplyWith("Removed map URL");
	}

	[ConsoleCommand("autowipe.addmap", "Adds a new map URLs to the list.")]
	[AuthLevel(2)]
	private void add_map(ConsoleSystem.Arg arg)
	{
		if (!arg.HasArgs())
		{
			arg.ReplyWith("You've got missing arguments. Please make sure to follow the following syntax:\n" +
			              "eg. autowipe.addmap \"<MapUrl>\" \"<Temp|True/False>\"");
			return;
		}

		var url = arg.GetString(0);
		var temp = arg.GetBool(1);

		if (ConfigInstance.Maps.Any(x => x.Url.Equals(url, StringComparison.OrdinalIgnoreCase)))
		{
			arg.ReplyWith($"Map url '{url}' already exists in the pool");
			return;
		}

		ConfigInstance.Maps.Add(new WipeMap
		{
			Url = url,
			Temp = temp
		});
		Save();
		arg.ReplyWith("Added map url");
	}

	[ConsoleCommand("autowipe.wipechat", "Updates the wipe chat command.")]
	[AuthLevel(2)]
	private void wipe_chat(ConsoleSystem.Arg arg)
	{
		var command = arg.GetString(0);
		var hasChanged = UpdateWipeChatCommand(ConfigInstance.WipeChatCommand, command);
		arg.ReplyWith(hasChanged ? $"Updated Wipe chat command to '{command}'" : $"Wipe chat command has not been changed.");
		if (hasChanged)
		{
			Save();
		}
	}

	public class Wipe
	{
		public string WipeName;
		public string[] Commands;
		public string MapBrowserName;
		public string MapUrl;
		public int MapSize;
		public int ServerSeed;
		public string Cron;
		public bool Temp;

		[JsonProperty("Type (0=fullwipe 1=mapwipe)")]
		public WipeTypes Type;

		public void CopyTo(Wipe other)
		{
			other.WipeName = WipeName;
			other.Commands = Commands.ToArray();
			other.MapBrowserName = MapBrowserName;
			other.MapUrl = MapUrl;
			other.MapSize = MapSize;
			other.ServerSeed = ServerSeed;
			other.Cron = Cron;
			other.Temp = Temp;
			other.Type = Type;
		}

		public void InitWorld(List<WipeMap> maps, long lastWipe)
		{
#if !MINIMAL
			Community.Runtime.Core.CustomMapName = string.IsNullOrEmpty(MapBrowserName) ? "-1" : MapBrowserName;
#endif
			if (MapUrl == "POOL")
			{
				var randomIndex = Random.Range(0, maps.Count);
				var map = maps[randomIndex];
				MapUrl = map.Url;
				if (map.Temp)
				{
					maps.RemoveAt(randomIndex);
				}
			}

			Singleton.RefreshHostName();
			World.Url = ConVar.Server.levelurl = MapUrl;
			if (MapSize != 0)
				World.InitSize(ConVar.Server.worldsize = MapSize);
			if (ServerSeed == 0)
				ServerSeed = Random.Range(1, int.MaxValue);
			World.InitSeed(ConVar.Server.seed = ServerSeed);

			using var table = new StringTable("wipe_name", "seed", "size", "url");
			table.AddRow(WipeName, ServerSeed, MapSize, MapUrl);
			Logger.Warn(table.Write(StringTable.FormatTypes.None));
		}

		public override bool Equals(object other)
		{
			if (other is Wipe otherVal)
			{
				return GetHashCode() == otherVal.GetHashCode();
			}

			return false;
		}

		public override int GetHashCode()
		{
			return (WipeName, MapBrowserName, MapUrl, MapSize, ServerSeed, Type, Temp, Cron, UpcomingWipeCommands: Commands).GetHashCode();
		}

		public bool ShouldWipe()
		{
			if (string.IsNullOrEmpty(Cron))
			{
				return false;
			}

			var now = DateTime.UtcNow;
			var cron = CronExpression.Parse(Cron);
			var nextOccurrence = cron.GetNextOccurrence(now.AddMinutes(-1), TimeZoneInfo.Utc);
			if (!nextOccurrence.HasValue)
			{
				return false;
			}

			static DateTime RoundDownTo10Minutes(DateTime dt)
			{
				int roundedMinutes = dt.Minute - (dt.Minute % 10);
				return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, roundedMinutes, 0, dt.Kind);
			}

			var nowRounded = RoundDownTo10Minutes(now);
			var occurrenceRounded = RoundDownTo10Minutes(nextOccurrence.Value);

			return nowRounded == occurrenceRounded;
		}
	}

	public struct WipeConfig
	{
		public string[] PostWipeCommands;
		public string[] PostWipeDeletes;
	}

	public struct WipeMap
	{
		public string Url;
		public bool Temp;
	}

	public enum WipeTypes
	{
		FullWipe,
		MapWipe
	}
}

public class AutoWipeConfig
{
	public string WipeChatCommand;
	public AutoWipeModule.WipeConfig FullWipe;
	public AutoWipeModule.WipeConfig MapWipe;
	public List<AutoWipeModule.WipeMap> Maps = new();
	public List<AutoWipeModule.Wipe> AvailableWipes = new();

	public AutoWipeModule.WipeConfig GetWipeConfig(AutoWipeModule.Wipe wipe)
	{
		return wipe.Type switch
		{
			AutoWipeModule.WipeTypes.FullWipe => FullWipe,
			AutoWipeModule.WipeTypes.MapWipe => MapWipe,
			_ => default
		};
	}
}

public class AutoWipeData
{
	public long LastWipeTime;
	public AutoWipeModule.Wipe Wipe;
	public AutoWipeModule.Wipe NextWipe;
}
