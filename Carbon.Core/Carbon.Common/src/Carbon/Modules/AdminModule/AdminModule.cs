using API.Commands;
using Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Game.Rust.Cui;
using ProtoBuf;
using static Carbon.Components.CUI;
using static ConsoleSystem;
using Color = UnityEngine.Color;
using StringEx = Carbon.Extensions.StringEx;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;
#pragma warning disable IDE0051

public partial class AdminModule : CarbonModule<AdminConfig, AdminData>
{
	internal static AdminModule Singleton { get; set; }

	public override string Name => "Admin";
	public override VersionNumber Version => new(1, 7, 0);
	public override Type Type => typeof(AdminModule);
	public override bool EnabledByDefault => true;

	public ImageDatabaseModule ImageDatabase;
	public ColorPickerModule ColorPicker;
	public DatePickerModule DatePicker;
	public ModalModule Modal;

	public readonly Handler Handler = new();

	internal const float OptionWidth = 0.475f;
	internal const float TooltipOffset = 15;
	internal const int RangeCuts = 50;

	internal List<Tab> Tabs = new();
	internal Dictionary<BasePlayer, PlayerSession> AdminPlayers = new();

	const string PanelId = "carbonmodularui";
	const string CursorPanelId = "carbonmodularuicur";
	const string SpectatePanelId = "carbonmodularuispectate";
	const int AccessLevels = 3;

	public AdminModule()
	{
		Singleton = this;
	}

	internal static List<string> _logQueue { get; } = new();
	internal static Dictionary<LogType, string> _logColor { get; } = new()
	{
		[LogType.Log] = "white",
		[LogType.Warning] = "#dbbe2a",
		[LogType.Error] = "#db2a2a"
	};
	public bool HandleEnableNeedsKeyboard(PlayerSession ap)
	{
		return ap.SelectedTab == null || ap.SelectedTab.Dialog == null;
	}
	public bool HandleEnableNeedsKeyboard(BasePlayer player)
	{
		return HandleEnableNeedsKeyboard(GetPlayerSession(player));
	}

	public override void OnServerInit()
	{
		base.OnServerInit();

		ImageDatabase = GetModule<ImageDatabaseModule>();
		ColorPicker = GetModule<ColorPickerModule>();
		DatePicker = GetModule<DatePickerModule>();
		Modal = GetModule<ModalModule>();

		Unsubscribe("OnPluginLoaded");
		Unsubscribe("OnPluginUnloaded");
		Unsubscribe("OnEntityDismounted");
		Unsubscribe("CanDismountEntity");
		Unsubscribe("OnEntityVisibilityCheck");
		Unsubscribe("OnEntityDistanceCheck");

		for (int i = 1; i <= AccessLevels; i++)
		{
			RegisterPermission($"adminmodule.accesslevel{i}");
		}

		Application.logMessageReceived += OnLog;
	}
	public override void OnPostServerInit()
	{
		base.OnPostServerInit();

		GenerateTabs();
	}

	public override void OnEnabled(bool initialized)
	{
		base.OnEnabled(initialized);

		foreach (var command in ConfigInstance.OpenCommands)
		{
			Community.Runtime.CorePlugin.cmd.AddChatCommand(command, this, (player, cmd, args) =>
			{
				if (!CanAccess(player)) return;

				var ap = GetPlayerSession(player);

				ap.SelectedTab = Tabs.FirstOrDefault(x => HasAccessLevel(player, x.AccessLevel));

				var tab = GetTab(player);
				tab.OnChange?.Invoke(ap, tab);

				ap.Clear();

				DrawCursorLocker(player);
				Draw(player);
			}, silent: true);
		}
	}
	public override void OnDisabled(bool initialized)
	{
		if (initialized)
		{
			Community.Runtime.CorePlugin.NextTick(() =>
			{
				foreach (var player in BasePlayer.activePlayerList)
				{
					Close(player);
				}
			});
		}

		base.OnDisabled(initialized);
	}
	public override void Load()
	{
		base.Load();

		if (Community.IsServerFullyInitializedCache) GenerateTabs();
	}
	public override void Save()
	{
		base.Save();

		PluginsTab.ServerOwner.Save();
	}

	public override Dictionary<string, Dictionary<string, string>> GetDefaultPhrases()
	{
		return new Dictionary<string, Dictionary<string, string>>
		{
			["en"] = new()
			{
				["hostname"] = "Host Name",
				["level"] = "Level",
				["info"] = "Info",
				["version"] = "Version",
				["version2"] = "Informational Version",
				["hooks"] = "Hooks",
				["statichooks"] = "Static Hooks",
				["dynamichooks"] = "Dynamic Hooks",
				["plugins"] = "Plugins",
				["mods"] = "Mods",
				["console"] = "Console",
				["execservercmd"] = "Execute Server Command",
				["config"] = "Config",
				["ismodded"] = "Is Modded",
				["ismodded_help"] = "When enabled, it marks the server as modded.",
				["autoupdateexthooks"] = "Auto Update External Hooks",
				["autoupdateexthooks_help"] = "Automatically update the 'Carbon.Hooks.Extra' file on boot. Recommended to be enabled.",
				["general"] = "General",
				["hookvalidation"] = "Hook Validation",
				["hookvalidation_help"] = "Probably obsolete, but when enabled, it prints a list of hooks that are compatible in Oxide, but not Carbon.",
				["entmapbuffersize"] = "Entity Map Buffer Size (restart required)",
				["entmapbuffersize_help"] = "Only change if you're aware what this is used for. Developers-related option.",
				["watchers"] = "Watchers",
				["scriptwatchers"] = "Script Watchers",
				["scriptwatchers_help"] = "When disabled, you must load/unload plugins manually with 'c.load' or 'c.unload'.",
				["scriptwatchersoption"] = "Script Watchers Option",
				["scriptwatchersoption_help"] = "Indicates wether the script watcher (whenever enabled) listens to the 'carbon/plugins' folder only, or its subfolders.",
				["harmonyreference"] = "Harmony Reference (<color=red>!</color>)",
				["harmonyreference_help"] = "Enabling this will allow plugins to patch Harmony patches at runtime. This might create instability and conflict if unmanaged.",
				["filenamecheck"] = "File Name Check",
				["filenamecheck_help"] = "Checks for file names. Otherwise will load the plugins regardless. Recommended to be enabled.",
				["logging"] = "Logging",
				["logfilemode"] = "Log File Mode",
				["logverbosity"] = "Log Verbosity (Debug)",
				["logseverity"] = "Log Severity",
				["misc"] = "Miscellaneous",
				["serverlang"] = "Server Language",
				["webreqip"] = "WebRequest IP",
				["permmode"] = "Permission Mode",
				["nocontent"] = "There are no options available.\nSelect a sub-tab to populate this area (if available)."
			}
		};
	}

	private void OnLog(string condition, string stackTrace, LogType type)
	{
		try
		{
			if (_logQueue.Count >= 7) _logQueue.RemoveAt(0);

			var log = condition.Split('\n');
			var result = log[0];
			Array.Clear(log, 0, log.Length);
			_logQueue.Add($"<color={_logColor[type]}>{result}</color>");
		}
		catch { }
	}

	public bool HasAccessLevel(BasePlayer player, int accessLevel)
	{
		if (accessLevel == 0 || (player != null && player.IsAdmin)) return true;

		for (int i = accessLevel; i <= AccessLevels; i++)
		{
			if (HasPermission(player.UserIDString, $"adminmodule.accesslevel{i}"))
			{
				return true;
			}
		}

		return false;
	}
	public void GenerateTabs()
	{
		UnregisterAllTabs();

		RegisterTab(CarbonTab.Get());
		RegisterTab(PlayersTab.Get());
		if (!ConfigInstance.DisableEntitiesTab) RegisterTab(EntitiesTab.Get());
		RegisterTab(PermissionsTab.Get());
		RegisterTab(ModulesTab.Get());
		if (!ConfigInstance.DisablePluginsTab) RegisterTab(PluginsTab.Get());
	}

	private void OnEntityDismounted(BaseMountable entity, BasePlayer player)
	{
		var ap = GetPlayerSession(player);
		var tab = GetTab(player);
		if (!ap.GetStorage(tab, "wasviewingcam", false)) return;

		entity.Kill();
		Draw(player);

		Unsubscribe("OnEntityDismounted");
	}
	private void OnPlayerLootEnd(PlayerLoot loot)
	{
		if (EntitiesTab.LastContainerLooter != null && loot.baseEntity == EntitiesTab.LastContainerLooter.Player)
		{
			Draw(EntitiesTab.LastContainerLooter.Player);
			EntitiesTab.LastContainerLooter = null;
			Unsubscribe("OnEntityVisibilityCheck");
			Unsubscribe("OnEntityDistanceCheck");
		}
	}
	private object OnEntityDistanceCheck(BaseEntity ent, BasePlayer player, uint id, string debugName, float maximumDistance)
	{
		var ap = GetPlayerSession(player);
		var tab = GetTab(player);
		var lootedEnt = ap.GetStorage<BaseEntity>(tab, "lootedent");

		if (lootedEnt == null) return null;

		return true;
	}
	private object OnEntityVisibilityCheck(BaseEntity ent, BasePlayer player, uint id, string debugName, float maximumDistance)
	{
		var ap = GetPlayerSession(player);
		var tab = GetTab(player);
		var lootedEnt = ap.GetStorage<BaseEntity>(tab, "lootedent");

		if (lootedEnt == null) return null;

		return true;
	}
	private void OnPlayerDisconnected(BasePlayer player)
	{
		if (PlayersTab.BlindedPlayers.Contains(player)) PlayersTab.BlindedPlayers.Remove(player);
	}

	private bool CanAccess(BasePlayer player)
	{
		if (HookCaller.CallStaticHook(3097360729, player) is bool result)
		{
			return result;
		}

		var authLevel = player.Connection.authLevel;
		var minLevel = ConfigInstance.MinimumAuthLevel;
		var hasAccess = authLevel >= minLevel;

		if (!hasAccess)
		{
			if (authLevel == 0)
			{
				player.ChatMessage($"Your auth level is not high enough to use this feature.");
			}
			else if (authLevel < minLevel && authLevel > 0)
			{
				player.ChatMessage($"Your auth level is not high enough to use this feature. Please adjust the minimum level required in your config or give yourself auth level {minLevel}.");
			}
		}

		return hasAccess;
	}

	#region Option Elements

	internal void TabButton(CUI cui, CuiElementContainer container, string parent, string text, string command, float width, float offset, bool highlight = false, bool disabled = false)
	{
		var button = cui.CreateProtectedButton(container, parent: parent, id: null,
			color: highlight ? $"{DataInstance.Colors.SelectedTabColor} 0.7" : "0.3 0.3 0.3 0.1",
			textColor: $"1 1 1 {(disabled ? 0.15 : 0.5)}",
			text: text, 11,
			xMin: offset, xMax: offset + width, yMin: 0, yMax: 1,
			command: disabled ? string.Empty : command,
			font: Handler.FontTypes.RobotoCondensedRegular);

		if (highlight)
		{
			cui.CreatePanel(container, button, null,
				color: "1 1 1 0.4",
				xMin: 0, xMax: 1f, yMin: 0f, yMax: 0.03f,
				OxMax: -0.5f);
		}
	}

	public void TabColumnPagination(CUI cui, CuiElementContainer container, string parent, int column, PlayerSession.Page page, float height, float offset)
	{
		var id = $"{parent}{column}";

		cui.CreatePanel(container, parent, id,
			color: "0.3 0.3 0.3 0.3",
			xMin: 0.02f, xMax: 0.98f, yMin: offset, yMax: offset + height);

		cui.CreateText(container, parent: id, id: null,
			color: "1 1 1 0.5",
			text: $" / {page.TotalPages + 1:n0}", 9,
			xMin: 0.5f, xMax: 1f, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleLeft,
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedInputField(container, parent: id, id: null,
			color: "1 1 1 1",
			text: $"{page.CurrentPage + 1}", 9,
			xMin: 0f, xMax: 0.495f, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleRight,
			command: PanelId + $".changecolumnpage {column} 4 ",
			characterLimit: 0,
			readOnly: false,
			font: Handler.FontTypes.RobotoCondensedRegular);

		#region Left

		cui.CreateProtectedButton(container, parent: id, id: null,
			color: page.CurrentPage > 0 ? "0.8 0.7 0.2 0.7" : "0.3 0.3 0.3 0.1",
			textColor: "1 1 1 0.5",
			text: "<<", 8,
			xMin: 0, xMax: 0.1f, yMin: 0f, yMax: 1f,
			command: page.CurrentPage > 0 ? PanelId + $".changecolumnpage {column} 2" : "",
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedButton(container, parent: id, id: null,
			color: "0.4 0.7 0.2 0.7",
			textColor: "1 1 1 0.5",
			text: "<", 8,
			xMin: 0.1f, xMax: 0.2f, yMin: 0f, yMax: 1f,
			command: PanelId + $".changecolumnpage {column} 0",
			font: Handler.FontTypes.RobotoCondensedRegular);

		#endregion

		#region Right

		cui.CreateProtectedButton(container, parent: id, id: null,
			color: page.CurrentPage < page.TotalPages ? "0.8 0.7 0.2 0.7" : "0.3 0.3 0.3 0.1",
			textColor: "1 1 1 0.5",
			text: ">>", 8,
			xMin: 0.9f, xMax: 1f, yMin: 0f, yMax: 1f,
			command: page.CurrentPage < page.TotalPages ? PanelId + $".changecolumnpage {column} 3" : "",
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedButton(container, parent: id, id: null,
			color: "0.4 0.7 0.2 0.7",
			textColor: "1 1 1 0.5",
			text: ">", 8,
			xMin: 0.8f, xMax: 0.9f, yMin: 0f, yMax: 1f,
			command: PanelId + $".changecolumnpage {column} 1",
			font: Handler.FontTypes.RobotoCondensedRegular);

		#endregion
	}
	public void TabPanelName(CUI cui, CuiElementContainer container, string parent, string text, float height, float offset, TextAnchor align)
	{
		cui.CreateText(container, parent: parent, id: $"{parent}text",
			color: "1 1 1 0.7",
			text: text?.ToUpper(), 12,
			xMin: 0.025f, xMax: 0.98f, yMin: offset, yMax: offset + height,
			align: align,
			font: Handler.FontTypes.RobotoCondensedBold);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreatePanel(container, $"{parent}text", null,
				color: $"1 1 1 {DataInstance.Colors.TitleUnderlineOpacity}",
				xMin: 0, xMax: 1, yMin: 0f, yMax: 0.015f);
		}
	}
	public void TabPanelText(CUI cui, CuiElementContainer container, string parent, string text, int size, string color, float height, float offset, TextAnchor align, Handler.FontTypes font, bool isInput)
	{
		if (isInput)
		{
			cui.CreateInputField(container, parent: parent, id: null,
				color: color,
				text: text, size, characterLimit: 0, readOnly: true,
				xMin: 0.025f, xMax: 0.98f, yMin: offset, yMax: offset + height,
				align: align,
				font: font);
		}
		else
		{
			cui.CreateText(container, parent: parent, id: null,
				color: color,
				text: text, size,
				xMin: 0.025f, xMax: 0.98f, yMin: offset, yMax: offset + height,
				align: align,
				font: font);
		}
	}
	public void TabPanelButton(CUI cui, CuiElementContainer container, string parent, string text, string command, float height, float offset, Tab.OptionButton.Types type = Tab.OptionButton.Types.None, TextAnchor align = TextAnchor.MiddleCenter)
	{
		var color = type switch
		{
			Tab.OptionButton.Types.Selected => "0.4 0.7 0.2 0.7",
			Tab.OptionButton.Types.Warned => "0.8 0.7 0.2 0.7",
			Tab.OptionButton.Types.Important => "0.97 0.2 0.1 0.7",
			_ => "0.2 0.2 0.2 0.5",
		};

		var button = cui.CreateProtectedButton(container, parent: parent, id: $"{parent}btn",
			color: color,
			textColor: "1 1 1 0.5",
			text: text, 11,
			xMin: 0.015f, xMax: 0.985f, yMin: offset, yMax: offset + height,
			command: command,
			align: align,
			font: Handler.FontTypes.RobotoCondensedRegular);
	}
	public void TabPanelToggle(CUI cui, CuiElementContainer container, string parent, string text, string command, float height, float offset, bool isOn)
	{
		var toggleButtonScale = 0.94f;

		cui.CreatePanel(container, parent, $"{parent}panel",
			color: "0.2 0.2 0.2 0",
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreateText(container, parent: $"{parent}panel", id: $"{parent}text",
				color: $"1 1 1 {DataInstance.Colors.OptionNameOpacity}",
				text: $"{text}:", 12,
				xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
				align: TextAnchor.MiddleLeft,
				font: Handler.FontTypes.RobotoCondensedRegular);

			cui.CreatePanel(container, $"{parent}panel", null,
				color: "0.2 0.2 0.2 0.5",
				xMin: 0, xMax: toggleButtonScale, yMin: 0, yMax: 0.015f);
		}

		var button = cui.CreateProtectedButton(container, parent: parent, id: $"{parent}btn",
			color: "0.2 0.2 0.2 0.5",
			textColor: "1 1 1 0.5",
			text: string.Empty, 11,
			xMin: toggleButtonScale, xMax: 0.985f, yMin: offset, yMax: offset + height,
			command: command,
			font: Handler.FontTypes.RobotoCondensedRegular);

		if (isOn)
		{
			cui.CreateImage(container, button, null,
				url: "checkmark",
				color: "0.4 0.7 0.2 0.7",
				xMin: 0.15f, xMax: 0.85f, yMin: 0.15f, yMax: 0.85f);
		}
	}
	public void TabPanelInput(CUI cui, CuiElementContainer container, string parent, string text, string placeholder, string command, int characterLimit, bool readOnly, float height, float offset, PlayerSession session, Tab.OptionButton.Types type = Tab.OptionButton.Types.None)
	{
		var color = type switch
		{
			Tab.OptionButton.Types.Selected => "0.4 0.7 0.2 0.7",
			Tab.OptionButton.Types.Warned => "0.8 0.7 0.2 0.7",
			Tab.OptionButton.Types.Important => "0.97 0.2 0.1 0.7",
			_ => "0.2 0.2 0.2 0.5",
		};

		cui.CreatePanel(container, parent, $"{parent}panel",
			color: "0.2 0.2 0.2 0",
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreateText(container, parent: $"{parent}panel", id: $"{parent}text",
			color: $"1 1 1 {DataInstance.Colors.OptionNameOpacity}",
			text: $"{text}:", 12,
			xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleLeft,
			font: Handler.FontTypes.RobotoCondensedRegular);

			cui.CreatePanel(container, $"{parent}panel", null,
				color: color,
				xMin: 0, xMax: OptionWidth, yMin: 0, yMax: 0.015f);
		}

		var inPanel = cui.CreatePanel(container, $"{parent}panel", $"{parent}inppanel",
			color: color,
			xMin: OptionWidth, xMax: 0.985f, yMin: 0, yMax: 1);

		cui.CreateProtectedInputField(container, parent: inPanel, id: null,
			color: $"1 1 1 {(readOnly ? 0.2f : 1f)}",
			text: placeholder, 11,
			xMin: 0.03f, xMax: 1, yMin: 0, yMax: 1,
			command: command,
			align: TextAnchor.MiddleLeft,
			characterLimit: characterLimit,
			readOnly: readOnly,
			needsKeyboard: Singleton.HandleEnableNeedsKeyboard(session),
			font: Handler.FontTypes.RobotoCondensedRegular);

		if (!readOnly)
		{
			cui.CreatePanel(container, inPanel, null,
				color: $"{DataInstance.Colors.EditableInputHighlight} 0.9",
				xMin: 0, xMax: 1, yMin: 0, yMax: 0.05f,
				OxMax: -0.5f);
		}
	}
	public void TabPanelEnum(CUI cui, CuiElementContainer container, string parent, string text, string value, string command, float height, float offset, Tab.OptionButton.Types type = Tab.OptionButton.Types.Selected)
	{
		var color = type switch
		{
			Tab.OptionButton.Types.Selected => "0.4 0.7 0.2 0.7",
			Tab.OptionButton.Types.Warned => "0.8 0.7 0.2 0.7",
			Tab.OptionButton.Types.Important => "0.97 0.2 0.1 0.7",
			_ => "0.2 0.2 0.2 0.5",
		};

		cui.CreatePanel(container, parent, $"{parent}panel",
			color: "0.2 0.2 0.2 0",
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreateText(container, parent: $"{parent}panel", id: $"{parent}text",
			color: $"1 1 1 {DataInstance.Colors.OptionNameOpacity}",
			text: $"{text}:", 12,
			xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleLeft,
			font: Handler.FontTypes.RobotoCondensedRegular);

			cui.CreatePanel(container, $"{parent}panel", null,
				color: "0.2 0.2 0.2 0.5",
				xMin: 0, xMax: OptionWidth, yMin: 0, yMax: 0.015f);
		}

		cui.CreatePanel(container, $"{parent}panel", $"{parent}inppanel",
			color: "0.2 0.2 0.2 0.5",
			xMin: OptionWidth, xMax: 0.985f, yMin: 0, yMax: 1);

		cui.CreateText(container, parent: $"{parent}inppanel", id: null,
		color: "1 1 1 0.7",
		text: value, 11,
		xMin: 0, xMax: 1, yMin: 0, yMax: 1,
		align: TextAnchor.MiddleCenter,
		font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedButton(container, parent: $"{parent}inppanel", id: null,
			color: color,
			textColor: "1 1 1 0.7",
			text: "<", 10,
			xMin: 0f, xMax: 0.15f, yMin: 0, yMax: 1,
			command: $"{command} true",
			align: TextAnchor.MiddleCenter,
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedButton(container, parent: $"{parent}inppanel", id: null,
			color: color,
			textColor: "1 1 1 0.7",
			text: ">", 10,
			xMin: 0.85f, xMax: 1f, yMin: 0, yMax: 1,
			command: $"{command} false",
			align: TextAnchor.MiddleCenter,
			font: Handler.FontTypes.RobotoCondensedRegular);
	}
	public void TabPanelRadio(CUI cui, CuiElementContainer container, string parent, string text, bool isOn, string command, float height, float offset)
	{
		var toggleButtonScale = 0.93f;

		cui.CreatePanel(container, parent, $"{parent}panel",
			color: "0.2 0.2 0.2 0",
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreateText(container, parent: $"{parent}panel", id: $"{parent}text",
			color: $"1 1 1 {DataInstance.Colors.OptionNameOpacity}",
			text: $"{text}:", 12,
			xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleLeft,
			font: Handler.FontTypes.RobotoCondensedRegular);

			cui.CreatePanel(container, $"{parent}panel", null,
				color: "0.2 0.2 0.2 0.5",
				xMin: 0, xMax: toggleButtonScale, yMin: 0, yMax: 0.015f);
		}

		cui.CreateProtectedButton(container, parent: parent, id: $"{parent}btn",
			color: "0.2 0.2 0.2 0.5",
			textColor: "1 1 1 0.5",
			text: string.Empty, 11,
			xMin: toggleButtonScale, xMax: 0.985f, yMin: offset, yMax: offset + height,
			command: command,
			font: Handler.FontTypes.RobotoCondensedRegular);

		if (isOn)
		{
			cui.CreatePanel(container, $"{parent}btn", null,
				color: "0.4 0.7 0.2 0.7",
				xMin: 0.2f, xMax: 0.8f, yMin: 0.2f, yMax: 0.8f);
		}
	}
	public void TabPanelDropdown(CUI cui, PlayerSession.Page page, CuiElementContainer container, string parent, string text, string command, float height, float offset, int index, string[] options, string[] optionsIcons, float optionsIconsScale, bool display, Tab.OptionButton.Types type = Tab.OptionButton.Types.Selected)
	{
		var color = type switch
		{
			Tab.OptionButton.Types.Selected => "0.4 0.7 0.2",
			Tab.OptionButton.Types.Warned => "0.8 0.7 0.2",
			Tab.OptionButton.Types.Important => "0.97 0.2 0.1",
			_ => "0.2 0.2 0.2",
		};

		cui.CreatePanel(container, parent, $"{parent}panel",
			color: "0.2 0.2 0.2 0",
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreateText(container, parent: $"{parent}panel", id: $"{parent}text",
			color: $"1 1 1 {DataInstance.Colors.OptionNameOpacity}",
				text: $"{text}:", 12,
				xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
				align: TextAnchor.MiddleLeft,
				font: Handler.FontTypes.RobotoCondensedRegular);

			cui.CreatePanel(container, $"{parent}panel", null,
				color: "0.2 0.2 0.2 0.5",
				xMin: 0, xMax: OptionWidth, yMin: 0, yMax: 0.015f);
		}

		var inPanel = cui.CreatePanel(container, $"{parent}panel", $"{parent}inppanel",
			color: "0.2 0.2 0.2 0.5",
			xMin: OptionWidth, xMax: 0.985f, yMin: 0, yMax: 1);

		var icon = optionsIcons != null && index <= optionsIcons.Length - 1 ? optionsIcons[index] : null;
		var iconXmin = 0.015f;
		var iconXmax = 0.072f;
		var iconYmin = 0.2f;
		var iconYmax = 0.8f;

		var button = cui.CreateProtectedButton(container, parent: inPanel, id: null,
			color: $"0.2 0.2 0.2 0.7",
			textColor: "0 0 0 0",
			text: string.Empty, 0,
			xMin: 0f, xMax: 1f, yMin: 0, yMax: 1,
			command: $"{command} false",
			align: TextAnchor.MiddleLeft,
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateText(container, parent: button, null, "1 1 1 0.7", options[index], 10,
			xMin: string.IsNullOrEmpty(icon) ? 0.02f : 0.085f, xMax: 1f, yMin: 0f, yMax: 1f, align: TextAnchor.MiddleLeft);

		if (!string.IsNullOrEmpty(icon))
		{
			cui.CreateImage(container, button, null, icon, optionsIconsScale, "1 1 1 0.7",
				xMin: iconXmin, xMax: iconXmax, yMin: iconYmin, yMax: iconYmax);
		}

		if (display)
		{
			var _spacing = 22;
			var _offset = -_spacing;
			var contentsPerPage = 10;
			var rowPage = options.Skip(contentsPerPage * page.CurrentPage).Take(contentsPerPage);
			var rowPageCount = rowPage.Count();
			var shiftOffset = 15;
			page.TotalPages = (int)Math.Ceiling((double)options.Length / contentsPerPage - 1);
			page.Check();

			for (int i = 0; i < rowPageCount; i++)
			{
				var actualI = i + (page.CurrentPage * contentsPerPage);
				var current = options[actualI];
				var isSelected = actualI == index;

				var subIcon = optionsIcons != null && actualI <= optionsIcons.Length - 1 ? optionsIcons[actualI] : null;

				var subButton = cui.CreateProtectedButton(container, parent: $"{parent}inppanel", id: null,
					color: isSelected ? $"{color} 1" : "0.1 0.1 0.1 1",
					textColor: "0 0 0 0",
					text: string.Empty, 0,
					xMin: 0f, xMax: 1f, yMin: 0, yMax: 1,
					OyMin: _offset, OyMax: _offset,
					OxMin: shiftOffset,
					command: $"{command} true call {actualI}",
					align: TextAnchor.MiddleLeft,
					font: Handler.FontTypes.RobotoCondensedRegular);

				cui.CreateText(container, parent: subButton, null, isSelected ? "1 1 1 0.7" : "1 1 1 0.4", current, 10,
					xMin: string.IsNullOrEmpty(subIcon) ? 0.035f : 0.085f, xMax: 1f, yMin: 0f, yMax: 1f, align: TextAnchor.MiddleLeft);

				if (!string.IsNullOrEmpty(subIcon))
				{
					cui.CreateImage(container, subButton, null, subIcon, optionsIconsScale, isSelected ? "1 1 1 0.7" : "1 1 1 0.4",
						xMin: iconXmin, xMax: iconXmax, yMin: iconYmin, yMax: iconYmax);
				}

				_offset -= _spacing;
			}

			if (page.TotalPages > 0)
			{
				var controls = cui.CreatePanel(container, parent: $"{parent}inppanel", id: null, "0.2 0.2 0.2 0.2",
					OyMin: _offset, OyMax: _offset - 2,
					OxMin: shiftOffset);

				var id = cui.CreatePanel(container, controls, id: $"{parent}dropdown",
					color: "0.3 0.3 0.3 0.3",
					xMin: 0f, xMax: 1f, yMin: 0, yMax: 1);

				cui.CreateText(container, parent: id, id: null,
					color: "1 1 1 0.5",
					text: $"{page.CurrentPage + 1:n0} / {page.TotalPages + 1:n0}", 9,
					xMin: 0.5f, xMax: 1f, yMin: 0, yMax: 1,
					align: TextAnchor.MiddleLeft,
					font: Handler.FontTypes.RobotoCondensedRegular);

				#region Left

				cui.CreateProtectedButton(container, parent: id, id: null,
					color: page.CurrentPage > 0 ? "0.8 0.7 0.2 0.7" : "0.3 0.3 0.3 0.1",
					textColor: "1 1 1 0.5",
					text: "<<", 8,
					xMin: 0, xMax: 0.1f, yMin: 0f, yMax: 1f,
					command: $"{command} true --",
					font: Handler.FontTypes.RobotoCondensedRegular);

				cui.CreateProtectedButton(container, parent: id, id: null,
					color: "0.4 0.7 0.2 0.7",
					textColor: "1 1 1 0.5",
					text: "<", 8,
					xMin: 0.1f, xMax: 0.2f, yMin: 0f, yMax: 1f,
					command: $"{command} true -1",
					font: Handler.FontTypes.RobotoCondensedRegular);

				#endregion

				#region Right

				cui.CreateProtectedButton(container, parent: id, id: null,
					color: page.CurrentPage < page.TotalPages ? "0.8 0.7 0.2 0.7" : "0.3 0.3 0.3 0.1",
					textColor: "1 1 1 0.5",
					text: ">>", 8,
					xMin: 0.9f, xMax: 1f, yMin: 0f, yMax: 1f,
					command: $"{command} true ++",
					font: Handler.FontTypes.RobotoCondensedRegular);

				cui.CreateProtectedButton(container, parent: id, id: null,
					color: "0.4 0.7 0.2 0.7",
					textColor: "1 1 1 0.5",
					text: ">", 8,
					xMin: 0.8f, xMax: 0.9f, yMin: 0f, yMax: 1f,
					command: $"{command} true 1",
					font: Handler.FontTypes.RobotoCondensedRegular);

				#endregion
			}
		}
	}
	public void TabPanelRange(CUI cui, CuiElementContainer container, string parent, string text, string command, string valueText, float min, float max, float value, float height, float offset, Tab.OptionButton.Types type = Tab.OptionButton.Types.None)
	{
		var color = type switch
		{
			Tab.OptionButton.Types.Selected => "0.4 0.7 0.2 0.7",
			Tab.OptionButton.Types.Warned => "0.8 0.7 0.2 0.7",
			Tab.OptionButton.Types.Important => "0.97 0.2 0.1 0.7",
			_ => "0.2 0.2 0.2 0.5",
		};

		cui.CreatePanel(container, parent, $"{parent}panel",
			color: "0.2 0.2 0.2 0",
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreateText(container, parent: $"{parent}panel", id: $"{parent}text",
			color: $"1 1 1 {DataInstance.Colors.OptionNameOpacity}",
			text: $"{text}:", 12,
			xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleLeft,
			font: Handler.FontTypes.RobotoCondensedRegular);

			cui.CreatePanel(container, $"{parent}panel", null,
				color: color,
				xMin: 0, xMax: OptionWidth, yMin: 0, yMax: 0.015f);
		}

		var panel = cui.CreatePanel(container, $"{parent}panel", $"{parent}inppanel",
			color: color,
			xMin: OptionWidth, xMax: 0.985f, yMin: 0, yMax: 1);

		cui.CreatePanel(container, panel, null,
			color: HexToRustColor("#f54242", 0.8f),
			xMin: 0, xMax: value.Scale(min, max, 0f, 1f), yMin: 0, yMax: 1);

		cui.CreateText(container, panel, null, "1 1 1 1", valueText, 8);

		var cuts = max.Clamp(min, RangeCuts);
		var offsetScale = 1f / cuts;
		var currentOffset = 0f;

		for (int i = 0; i < cuts; i++)
		{
			cui.CreateProtectedButton(container, panel, null, "0 0 0 0", "0 0 0 0", string.Empty, 0,
				xMin: currentOffset, xMax: currentOffset + offsetScale, yMin: 0, yMax: 1,
				command: $"{command} {i}");

			currentOffset += offsetScale;
		}
	}
	public void TabPanelButtonArray(CUI cui, CuiElementContainer container, string parent, string command, float spacing, float height, float offset, params Tab.OptionButton[] buttons)
	{
		var panel = cui.CreatePanel(container, parent, $"{parent}panel",
			color: "0.2 0.2 0.2 0",
			xMin: 0.015f, xMax: 0.985f, yMin: offset, yMax: offset + height);

		var cuts = (1f / buttons.Length) - spacing;
		var currentOffset = 0f;

		for (int i = 0; i < buttons.Length; i++)
		{
			var button = buttons[i];
			var color = (button.Type == null ? Tab.OptionButton.Types.None : button.Type(null)) switch
			{
				Tab.OptionButton.Types.Selected => "0.4 0.7 0.2 0.7",
				Tab.OptionButton.Types.Warned => "0.8 0.7 0.2 0.7",
				Tab.OptionButton.Types.Important => "0.97 0.2 0.1 0.7",
				_ => "0.2 0.2 0.2 0.5",
			};
			cui.CreateProtectedButton(container, panel, null, color, "1 1 1 0.5", button.Name, 11,
				xMin: currentOffset, xMax: currentOffset + cuts, yMin: 0, yMax: 1,
				command: $"{command} {i}");

			currentOffset += cuts + spacing;
		}
	}
	public void TabPanelInputButton(CUI cui, CuiElementContainer container, string parent, string text, string command, float buttonPriority, Tab.OptionInput input, Tab.OptionButton button, PlayerSession ap, float height, float offset)
	{
		var color = "0.2 0.2 0.2 0.5";
		var buttonColor = (button.Type == null ? Tab.OptionButton.Types.None : button.Type(null)) switch
		{
			Tab.OptionButton.Types.Selected => "0.4 0.7 0.2 0.7",
			Tab.OptionButton.Types.Warned => "0.8 0.7 0.2 0.7",
			Tab.OptionButton.Types.Important => "0.97 0.2 0.1 0.7",
			_ => "0.2 0.2 0.2 0.5",
		};

		var panel = cui.CreatePanel(container, parent, $"{parent}panel",
			color: "0.2 0.2 0.2 0",
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreateText(container, parent: $"{parent}panel", id: $"{parent}text",
				color: $"1 1 1 {DataInstance.Colors.OptionNameOpacity}",
				text: $"{text}:", 12,
				xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
				align: TextAnchor.MiddleLeft,
				font: Handler.FontTypes.RobotoCondensedRegular);
		}

		var inPanel = cui.CreatePanel(container, $"{parent}panel", $"{parent}inppanel",
			color: color,
			xMin: OptionWidth, xMax: 0.985f, yMin: 0, yMax: 1);

		cui.CreatePanel(container, $"{parent}panel", null,
			color: color,
			xMin: 0, xMax: OptionWidth, yMin: 0, yMax: 0.015f);

		cui.CreateProtectedInputField(container, parent: inPanel, id: null,
			color: $"1 1 1 {(input.ReadOnly ? 0.2f : 1f)}",
			text: input.Placeholder?.Invoke(ap), 11,
			xMin: 0.03f, xMax: 1f - buttonPriority, yMin: 0, yMax: 1,
			command: $"{command} input",
			align: TextAnchor.MiddleLeft,
			characterLimit: input.CharacterLimit,
			readOnly: input.ReadOnly,
			needsKeyboard: Singleton.HandleEnableNeedsKeyboard(ap),
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedButton(container, parent: inPanel, id: null,
			color: buttonColor,
			textColor: "1 1 1 0.5",
			text: button.Name, 11,
			xMin: 1f - buttonPriority, xMax: 1f, yMin: 0f, yMax: 1f,
			command: $"{command} button",
			align: button.Align,
			font: Handler.FontTypes.RobotoCondensedRegular);

		if (!input.ReadOnly)
		{
			cui.CreatePanel(container, inPanel, null,
				color: $"{DataInstance.Colors.EditableInputHighlight} 0.9",
				xMin: 0, xMax: 1f - buttonPriority, yMin: 0, yMax: 0.05f,
				OxMax: -0.5f);
		}
	}
	public void TabPanelColor(CUI cui, CuiElementContainer container, string parent, string text, string color, string command, float height, float offset)
	{
		var toggleButtonScale = 0.825f;

		cui.CreatePanel(container, parent, $"{parent}panel",
			color: "0.2 0.2 0.2 0",
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreateText(container, parent: $"{parent}panel", id: $"{parent}text",
			color: $"1 1 1 {DataInstance.Colors.OptionNameOpacity}",
			text: $"{text}:", 12,
			xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleLeft,
			font: Handler.FontTypes.RobotoCondensedRegular);

			cui.CreatePanel(container, $"{parent}panel", null,
				color: "0.2 0.2 0.2 0.5",
				xMin: 0, xMax: toggleButtonScale, yMin: 0, yMax: 0.015f);
		}

		var split = color.Split(' ');
		cui.CreateProtectedButton(container, parent: parent, id: $"{parent}btn",
			color: color,
			textColor: "1 1 1 1",
			text: split.Length > 1 ? $"#{ColorUtility.ToHtmlStringRGB(new Color(split[0].ToFloat(), split[1].ToFloat(), split[2].ToFloat(), 1))}" : string.Empty, 10,
			xMin: toggleButtonScale, xMax: 0.985f, yMin: offset, yMax: offset + height,
			command: command,
			font: Handler.FontTypes.RobotoCondensedRegular);
		Array.Clear(split, 0, split.Length);
		split = null;
	}
	public void TabTooltip(CUI cui, CuiElementContainer container, string parent, Tab.Option tooltip, string command, PlayerSession admin, float height, float offset)
	{
		if (admin.Tooltip == tooltip)
		{
			var tip = cui.CreatePanel(container, parent, null, "#1a6498",
				xMin: 0.05f, xMax: ((float)admin.Tooltip.Tooltip.Length).Scale(1f, 78f, 0.1f, 0.79f), yMin: offset, yMax: offset + height);

			cui.CreateText(container, tip, null, "#6bc0fc", admin.Tooltip.Tooltip, 10);
		}

		if (!string.IsNullOrEmpty(tooltip.Tooltip))
		{
			cui.CreateProtectedButton(container, parent, null, "0 0 0 0", "0 0 0 0", string.Empty, 0,
				xMin: 0, xMax: OptionWidth, yMin: offset, yMax: offset + height,
				command: $"{command} tooltip");
		}
	}

	#endregion

	#region UI Commands

	[ProtectedCommand(PanelId + ".changetab")]
	private void ChangeTab(Arg args)
	{
		var player = args.Player();
		var ap = GetPlayerSession(player);
		var previous = ap.SelectedTab;

		ap.Clear();

		if (int.TryParse(args.Args[0], out int index))
		{
			SetTab(player, index);
			ap.SelectedTab = Tabs[index];
		}
		else
		{
			var indexOf = Tabs.IndexOf(previous);
			indexOf = args.Args[0] == "up" ? indexOf + 1 : indexOf - 1;

			if (indexOf > Tabs.Count - 1) indexOf = 0;
			else if (indexOf < 0) indexOf = Tabs.Count - 1;

			SetTab(player, indexOf);
		}
	}

	[ProtectedCommand(PanelId + ".callaction")]
	private void CallAction(Arg args)
	{
		var player = args.Player();

		if (CallColumnRow(player, args.Args[0].ToInt(), args.Args[1].ToInt(), args.Args.Skip(2).ToArray()))
			Draw(player);
	}

	[ProtectedCommand(PanelId + ".changecolumnpage")]
	private void ChangeColumnPage(Arg args)
	{
		var player = args.Player();
		var instance = GetPlayerSession(player);
		var page = instance.GetOrCreatePage(args.Args[0].ToInt());
		var type = args.Args[1].ToInt();

		switch (type)
		{
			case 0:
				page.CurrentPage--;
				if (page.CurrentPage < 0) page.CurrentPage = page.TotalPages;
				break;

			case 1:
				page.CurrentPage++;
				if (page.CurrentPage > page.TotalPages) page.CurrentPage = 0;
				break;

			case 2:
				page.CurrentPage = 0;
				break;

			case 3:
				page.CurrentPage = page.TotalPages;
				break;

			case 4:
				page.CurrentPage = (args.Args[2].ToInt() - 1).Clamp(0, page.TotalPages);
				break;
		}

		Draw(player);
	}

	[ProtectedCommand(PanelId + ".close")]
	private void CloseUI(Arg args)
	{
		Close(args.Player());
	}

	[ProtectedCommand(PanelId + ".dialogaction")]
	private void Dialog_Action(Arg args)
	{
		var player = args.Player();
		var admin = GetPlayerSession(player);
		var tab = GetTab(player);
		var dialog = tab?.Dialog;
		if (tab != null) tab.Dialog = null;

		switch (args.Args[0])
		{
			case "confirm":
				try { dialog?.OnConfirm(admin); } catch { }
				break;

			case "decline":
				try { dialog?.OnDecline(admin); } catch { }
				break;
		}

		Draw(player);
	}

	#endregion

	#region Methods

	public const float OptionHeightOffset = 0.0035f;

	public void Draw(BasePlayer player)
	{
		try
		{
			var ap = GetPlayerSession(player);
			var tab = GetTab(player);
			ap.IsInMenu = true;

			if (CanAccess(player) && !DataInstance.ShowedWizard
				&& (tab != null && tab.Id != "setupwizard" && tab.Id != "configeditor") && HasAccessLevel(player, 3))
			{
				tab = ap.SelectedTab = SetupWizard.Make();
			}

			using var cui = new CUI(Handler);

			var container = cui.CreateContainer(PanelId,
				color: "0 0 0 0.75",
				xMin: 0, xMax: 1, yMin: 0, yMax: 1,
				needsCursor: true, destroyUi: PanelId, parent: ClientPanels.HudMenu);

			var shade = cui.CreatePanel(container, parent: PanelId, id: $"{PanelId}color",
				color: "0 0 0 0.6",
				xMin: 0.5f, xMax: 0.5f, yMin: 0.5f, yMax: 0.5f,
				OxMin: -475, OxMax: 475, OyMin: -300, OyMax: 300);
			var main = cui.CreatePanel(container, shade, $"{PanelId}main",
				color: "0 0 0 0.5",
				blur: true);

			using (TimeMeasure.New($"{Name}.Main"))
			{
				if (tab == null || !tab.Fullscreen)
				{
					#region Title

					cui.CreateText(container, parent: main, id: null,
						color: "1 1 1 0.8",
						text: "<b>Admin Settings</b>", 18,
						xMin: 0.0175f, yMin: 0.8f, xMax: 1f, yMax: 0.97f,
						align: TextAnchor.UpperLeft,
						font: Handler.FontTypes.RobotoCondensedBold);

					#endregion

					#region Tabs
					try
					{
						var tabButtons = cui.CreatePanel(container, parent: main, id: null,
							color: "0 0 0 0.6",
							xMin: 0.01f, xMax: 0.99f, yMin: 0.875f, yMax: 0.92f);

						TabButton(cui, container, tabButtons, "<", PanelId + ".changetab down", 0.03f, 0);
						TabButton(cui, container, tabButtons, ">", PanelId + ".changetab up", 0.03f, 0.97f);

						var tabIndex = 0.03f;
						var amount = Tabs.Count;
						var tabWidth = amount == 0 ? 0f : 0.94f / amount;

						for (int i = ap.TabSkip; i < amount; i++)
						{
							var _tab = Tabs[ap.TabSkip + i];
							var plugin = _tab.Plugin.IsCorePlugin ? string.Empty : $"<size=8>\nby {_tab.Plugin?.Name}</size>";
							TabButton(cui, container, tabButtons, $"{(Tabs.IndexOf(ap.SelectedTab) == i ? $"<b>{_tab.Name}</b>" : _tab.Name)}{plugin}", PanelId + $".changetab {i}", tabWidth, tabIndex, Tabs.IndexOf(ap.SelectedTab) == i, !HasAccessLevel(player, _tab.AccessLevel));
							tabIndex += tabWidth;
						}
					}
					catch (Exception ex) { PutsError($"Draw({player}).Tabs", ex); }
					#endregion
				}
			}			

			#region Panels
			try
			{
				using (TimeMeasure.New($"{Name}.Panels/Overrides"))
				{
					var panels = cui.CreatePanel(container, main, "panels",
						color: "0 0 0 0",
						xMin: 0.01f, xMax: 0.99f, yMin: 0.02f, yMax: tab != null && tab.Fullscreen ? 0.98f : 0.86f);

					if (tab != null)
					{
						tab.Under?.Invoke(tab, cui, container, panels, ap);

						if (tab.Override == null)
						{
							#region Columns

							var panelIndex = 0f;
							var spacing = 0.005f;
							var panelWidth = (tab.Columns.Count == 0 ? 0f : 1f / tab.Columns.Count) - spacing;

							for (int i = 0; i < tab.Columns.Count; i++)
							{
								var rows = tab.Columns[i];
								var panel = cui.CreatePanel(container, "panels", $"sub{i}",
									color: "0 0 0 0.5",
									xMin: panelIndex, xMax: panelIndex + panelWidth - spacing, yMin: 0, yMax: 1);

								#region Rows

								var columnPage = ap.GetOrCreatePage(i);
								var contentsPerPage = 19;
								var rowSpacing = 0.01f;
								var rowHeight = 0.04f;
								var rowPage = rows.Skip(contentsPerPage * columnPage.CurrentPage).Take(contentsPerPage);
								var rowPageCount = rowPage.Count();
								columnPage.TotalPages = (int)Math.Ceiling(((double)rows.Count) / contentsPerPage - 1);
								columnPage.Check();
								var rowIndex = (rowHeight + rowSpacing) * (contentsPerPage - (rowPageCount - (columnPage.TotalPages > 0 ? 0 : 1)));

								if (rowPageCount == 0)
								{
									cui.CreateText(container, panel, null, "1 1 1 0.35", GetPhrase("nocontent", player.UserIDString), 8, align: TextAnchor.MiddleCenter);
								}

								if (columnPage.TotalPages > 0)
								{
									rowHeight += OptionHeightOffset;

									TabColumnPagination(cui, container, panel, i, columnPage, rowHeight, rowIndex);

									rowHeight -= OptionHeightOffset;

									rowIndex += rowHeight + rowSpacing;
								}

								for (int r = rowPageCount; r-- > 0;)
								{
									var actualI = r + (columnPage.CurrentPage * contentsPerPage);
									var row = rows.ElementAt(actualI);

									rowHeight += OptionHeightOffset;

									switch (row)
									{
										case Tab.OptionName name:
											TabPanelName(cui, container, panel, name.Name, rowHeight, rowIndex, name.Align);
											break;

										case Tab.OptionButton button:
											TabPanelButton(cui, container, panel, button.Name, PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex, button.Type == null ? Tab.OptionButton.Types.None : button.Type.Invoke(ap), button.Align);
											break;

										case Tab.OptionText text:
											TabPanelText(cui, container, panel, text.Name, text.Size, text.Color, rowHeight, rowIndex, text.Align, text.Font, text.IsInput);
											break;

										case Tab.OptionInput input:
											TabPanelInput(cui, container, panel, input.Name, input.Placeholder?.Invoke(ap), PanelId + $".callaction {i} {actualI}", input.CharacterLimit, input.ReadOnly, rowHeight, rowIndex, ap);
											break;

										case Tab.OptionEnum @enum:
											TabPanelEnum(cui, container, panel, @enum.Name, @enum.Text?.Invoke(ap), PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex);
											break;

										case Tab.OptionToggle toggle:
											TabPanelToggle(cui, container, panel, toggle.Name, PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex, toggle.IsOn != null ? toggle.IsOn.Invoke(ap) : false);
											break;

										case Tab.OptionRadio radio:
											TabPanelRadio(cui, container, panel, radio.Name, radio.Index == tab.Radios[radio.Id].Selected, PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex);
											break;

										case Tab.OptionDropdown dropdown:
											TabPanelDropdown(cui, ap._selectedDropdownPage, container, panel, dropdown.Name, PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex, dropdown.Index.Invoke(ap), dropdown.Options, dropdown.OptionsIcons, dropdown.OptionsIconScale, ap._selectedDropdown == dropdown);
											break;

										case Tab.OptionRange range:
											TabPanelRange(cui, container, panel, range.Name, PanelId + $".callaction {i} {actualI}", range.Text?.Invoke(ap), range.Min, range.Max, range.Value == null ? 0 : range.Value.Invoke(ap), rowHeight, rowIndex);
											break;

										case Tab.OptionButtonArray array:
											TabPanelButtonArray(cui, container, panel, PanelId + $".callaction {i} {actualI}", array.Spacing, rowHeight, rowIndex, array.Buttons);
											break;

										case Tab.OptionInputButton inputButton:
											TabPanelInputButton(cui, container, panel, inputButton.Name, PanelId + $".callaction {i} {actualI}", inputButton.ButtonPriority, inputButton.Input, inputButton.Button, ap, rowHeight, rowIndex);
											break;

										case Tab.OptionColor color:
											TabPanelColor(cui, container, panel, color.Name, color.Color?.Invoke() ?? "0.1 0.1 0.1 0.5", PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex);
											break;

										default:
											break;
									}

									#region Tooltip

									TabTooltip(cui, container, panel, row, PanelId + $".callaction {i} {actualI}", ap, rowHeight, rowIndex);

									#endregion

									rowHeight -= OptionHeightOffset;

									rowIndex += rowHeight + rowSpacing;
								}

								#endregion

								panelIndex += panelWidth + spacing;
							}

							#endregion
						}
						else tab.Override.Invoke(tab, cui, container, panels, ap);

						tab.Over?.Invoke(tab, cui, container, panels, ap);

						if (tab.Dialog != null)
						{
							if (ap.Player.serverInput.IsDown(BUTTON.SPRINT))
							{
								tab.Dialog.OnConfirm?.Invoke(ap);
							}
							else
							{
								var dialog = cui.CreatePanel(container, panels, null, "0.15 0.15 0.15 0.2", blur: true);
								cui.CreatePanel(container, dialog, null, "0 0 0 0.9");

								cui.CreateText(container, dialog, null,
									"1 1 1 1", tab.Dialog.Title, 20, yMin: 0.1f);

								cui.CreateText(container, dialog, null,
									"1 1 1 0.4", "Confirm action".ToUpper().SpacedString(3), 10, yMin: 0.2f);

								cui.CreateText(container, dialog, null,
									"1 1 1 0.2", "<color=red><b>*</b></color> Holding <b>[SPRINT]</b> next time you open this panel will automatically confirm it.", 8, xMin: 0.02f, yMin: 0.03f, align: TextAnchor.LowerLeft);

								cui.CreateProtectedButton(container, dialog, null, "0.9 0.4 0.3 0.8", "1 1 1 0.7", "DECLINE".SpacedString(1), 10,
									xMin: 0.4f, xMax: 0.49f, yMin: 0.425f, yMax: 0.475f, command: $"{PanelId}.dialogaction decline");

								cui.CreateProtectedButton(container, dialog, null, "0.4 0.9 0.3 0.8", "1 1 1 0.7", "CONFIRM".SpacedString(1), 10,
									xMin: 0.51f, xMax: 0.6f, yMin: 0.425f, yMax: 0.475f, command: $"{PanelId}.dialogaction confirm");

								Community.Runtime.CorePlugin.timer.In(0.2f, () =>
								{
									if (ap.Player.serverInput.IsDown(BUTTON.SPRINT))
									{
										try { tab.Dialog.OnConfirm?.Invoke(ap); } catch { }
										tab.Dialog = null;

										Draw(player);
									}
								});
							}
						}
					}
					else
					{
						cui.CreateText(container, panels, null, "1 1 1 0.4", "No tab selected.", 9);
					}
				}
			}
			catch (Exception ex) { PutsError($"Draw({player}).Panels", ex); }
			#endregion

			#region Exit

			using (TimeMeasure.New($"{Name}.Exit"))
			{
				cui.CreateProtectedButton(container, parent: $"{PanelId}main", id: null,
					color: "0.6 0.2 0.2 0.9",
					textColor: "1 0.5 0.5 1",
					text: "X", 9,
					xMin: 0.965f, xMax: 0.99f, yMin: 0.955f, yMax: 0.99f,
					command: PanelId + ".close",
					font: Handler.FontTypes.DroidSansMono);
			}

			#endregion

			using (TimeMeasure.New($"{Name}.Send"))
			{
				cui.Send(container, player);
			}
		}
		catch (Exception ex)
		{
			PutsError($"Draw(player) failed.", ex);
		}

		Subscribe("OnPluginLoaded");
		Subscribe("OnPluginUnloaded");
	}
	public void DrawCursorLocker(BasePlayer player)
	{
		using var cui = new CUI(Handler);

		var container = cui.CreateContainer(CursorPanelId,
			color: "0 0 0 0",
			xMin: 0, xMax: 0, yMin: 0, yMax: 0,
			fadeIn: 0.005f,
			needsCursor: true, destroyUi: CursorPanelId);

		cui.Send(container, player);
	}
	public void Close(BasePlayer player)
	{
		Handler.Destroy(PanelId, player);
		Handler.Destroy(CursorPanelId, player);

		var ap = GetPlayerSession(player);
		ap.IsInMenu = false;

		var noneInMenu = true;
		foreach (var admin in AdminPlayers)
		{
			if (admin.Value.IsInMenu)
			{
				noneInMenu = false;
				break;
			}
		}

		if (noneInMenu)
		{
			Unsubscribe("OnPluginLoaded");
			Unsubscribe("OnPluginUnloaded");
		}
	}

	public void RegisterTab(Tab tab, int? insert = null)
	{
		var existentTab = Tabs.FirstOrDefault(x => x.Id == tab.Id);
		if (existentTab != null)
		{
			var index = Tabs.IndexOf(existentTab);
			Tabs.RemoveAt(index);
			existentTab = null;

			Tabs.Insert(insert ?? index, tab);
		}
		else
		{
			if (insert != null) Tabs.Insert(insert.Value, tab);
			else Tabs.Add(tab);
		}

		Puts($"Registered tab '{tab.Name}'");
	}
	public void UnregisterTab(string id)
	{
		var tab = Tabs.FirstOrDefault(x => x.Id == id);
		tab?.Dispose();

		Tabs.RemoveAll(x => x.Id == id);

		if (tab != null) Puts($"Unregistered tab '{tab.Name}'");
	}
	public void UnregisterAllTabs()
	{
		Tabs.Clear();
	}

	public PlayerSession GetPlayerSession(BasePlayer player)
	{
		if (AdminPlayers.TryGetValue(player, out PlayerSession adminPlayer)) return adminPlayer;

		adminPlayer = new PlayerSession(player);
		AdminPlayers.Add(player, adminPlayer);
		return adminPlayer;
	}
	public void SetTab(BasePlayer player, string id, bool onChange = true)
	{
		var ap = GetPlayerSession(player);
		var previous = ap.SelectedTab;

		var tab = Tabs.FirstOrDefault(x => HasAccessLevel(player, x.AccessLevel) && x.Id == id);
		if (tab != null)
		{
			ap.Tooltip = null;
			ap.SelectedTab = tab;
			if (onChange) try { tab?.OnChange?.Invoke(ap, tab); } catch { }
		}

		if (ap.SelectedTab != previous) Draw(player);
	}
	public void SetTab(BasePlayer player, int index, bool onChange = true)
	{
		var ap = GetPlayerSession(player);
		var previous = ap.SelectedTab;

		var lookupTab = Tabs[index];
		var tab = HasAccessLevel(player, lookupTab.AccessLevel) ? lookupTab : Tabs.FirstOrDefault(x => HasAccessLevel(player, x.AccessLevel));
		if (tab != null)
		{
			ap.Tooltip = null;
			ap.SelectedTab = tab;
			if (onChange) try { tab?.OnChange?.Invoke(ap, tab); } catch { }
		}

		if (ap.SelectedTab != previous) Draw(player);
	}
	public void SetTab(BasePlayer player, Tab tab, bool onChange = true)
	{
		var ap = GetPlayerSession(player);
		var previous = ap.SelectedTab;

		tab = HasAccessLevel(player, tab.AccessLevel) ? tab : Tabs.FirstOrDefault(x => HasAccessLevel(player, x.AccessLevel));
		if (tab != null)
		{
			ap.Tooltip = null;
			ap.SelectedTab = tab;
			if (onChange) try { tab?.OnChange?.Invoke(ap, tab); } catch { }
		}

		if (ap.SelectedTab != previous) Draw(player);
	}
	public Tab GetTab(BasePlayer player)
	{
		if (Tabs.Count == 0) return null;

		var ap = GetPlayerSession(player);
		if (ap.SelectedTab == null) return null;

		return ap.SelectedTab;
	}
	public Tab FindTab(string id)
	{
		return Tabs.FirstOrDefault(x => x.Id == id);
	}
	public bool HasTab(string id)
	{
		return FindTab(id) != null;
	}
	public bool CallColumnRow(BasePlayer player, int column, int row, string[] args)
	{
		var ap = GetPlayerSession(player);
		var tab = GetTab(player);

		ap.LastPressedColumn = column;
		ap.LastPressedRow = row;

		var option = tab.Columns[column][row];
		if (args.Length > 0 && args[0] == "tooltip")
		{
			if (ap.Tooltip != option) ap.Tooltip = option;
			else ap.Tooltip = null;
			return true;
		}

		switch (option)
		{
			case Tab.OptionButton button:
				button.Callback?.Invoke(ap);
				return button.Callback != null;

			case Tab.OptionInput input:
				input.Callback?.Invoke(ap, args);
				return input.Callback != null;

			case Tab.OptionEnum @enum:
				@enum.Callback?.Invoke(ap, args[0].ToBool());
				return @enum.Callback != null;

			case Tab.OptionToggle toggle:
				toggle.Callback?.Invoke(ap);
				return toggle.Callback != null;

			case Tab.OptionRadio radio:
				if (radio.Radio.Selected != radio.Index)
				{
					radio.Radio.Change(radio.Index, ap);
					radio.Callback?.Invoke(true, ap);
					return true;
				}
				break;

			case Tab.OptionDropdown dropdown:
				var page = ap._selectedDropdownPage;
				switch (args[0].ToBool())
				{
					case true:
						switch (args[1])
						{
							case "call":
								ap._selectedDropdown = null;
								dropdown.Callback?.Invoke(ap, args[2].ToInt());
								page.CurrentPage = 0;
								break;

							default:
								switch (args[1])
								{
									case "--":
										page.CurrentPage = 0;
										break;

									case "++":
										page.CurrentPage = page.TotalPages;
										break;

									default:
										page.CurrentPage = args[1].ToInt();
										break;
								}

								if (page.CurrentPage < 0) page.CurrentPage = page.TotalPages;
								else if (page.CurrentPage > page.TotalPages) page.CurrentPage = 0;
								break;
						}

						return true;

					case false:
						page.CurrentPage = 0;

						var oldSelectedDropdown = ap._selectedDropdown;
						if (oldSelectedDropdown == dropdown)
						{
							ap._selectedDropdown = null;
							return true;
						}
						else
						{
							ap._selectedDropdown = dropdown;
							return oldSelectedDropdown != dropdown;
						}
				}

			case Tab.OptionRange range:
				range.Callback?.Invoke(ap, args[0].ToFloat().Scale(0f, range.Max.Clamp(range.Min, RangeCuts) - 1f, range.Min, range.Max));
				return range.Callback != null;

			case Tab.OptionButtonArray array:
				var callback = array.Buttons[args[0].ToInt()].Callback;
				callback?.Invoke(ap);
				return callback != null;

			case Tab.OptionInputButton inputButton:
				switch (args[0])
				{
					case "input":
						inputButton.Input.Callback?.Invoke(ap, args.Skip(1).ToArray());
						return inputButton.Input.Callback != null;

					case "button":
						inputButton.Button.Callback?.Invoke(ap);
						return inputButton.Button.Callback != null;
				}
				break;

			case Tab.OptionColor color:
				if (color.Callback != null)
				{
					ColorPicker.Draw(player, (rustColor, hexColor, alpha) => { color.Callback?.Invoke(ap, rustColor, hexColor, alpha); });
					return false;
				}
				break;
		}

		return false;
	}

	#endregion

	#region Hooks

	private void OnNewSave(string filename)
	{
		DataInstance.ShowedWizard = false;
	}

	#endregion

	#region Custom Hooks

	private object IValidDismountPosition(BaseMountable mountable, BasePlayer player)
	{
		switch (mountable.skinID)
		{
			case 69696:
				return true;
			default:
				break;
		}

		return null;
	}

	#endregion

	public class PlayerSession : IDisposable
	{
		public static PlayerSession Blank { get; } = new PlayerSession(null);

		public BasePlayer Player;
		public bool IsInMenu;
		public Dictionary<int, Page> ColumnPages = new();
		public Dictionary<string, object> LocalStorage = new();

		public Tab SelectedTab;
		public int TabSkip;
		public int LastPressedColumn;
		public int LastPressedRow;

		public Tab.Option Tooltip;

		internal Tab.OptionDropdown _selectedDropdown;
		internal Page _selectedDropdownPage = new();

		public PlayerSession(BasePlayer player)
		{
			Player = player;
		}

		public void SetPage(int column, int page)
		{
			if (ColumnPages.TryGetValue(column, out var pageInstance))
			{
				pageInstance.CurrentPage = page;
				pageInstance.Check();
			}
		}
		public T GetStorage<T>(Tab tab, string id, T @default = default)
		{
			try
			{
				var mainId = id;
				id = $"{tab?.Id}_{id}";

				if (LocalStorage.TryGetValue(id, out var storage)) return (T)storage;

				return SetStorage(tab, mainId, (T)@default);
			}
			catch (Exception ex) { Logger.Warn($"Failed GetStorage<{typeof(T).Name}>({tab?.Id}, {id}): {ex.Message}"); }

			return (T)default;
		}
		public T SetStorage<T>(Tab tab, string id, T value)
		{
			id = $"{tab?.Id}_{id}";

			LocalStorage[id] = value;
			return value;
		}
		public T SetDefaultStorage<T>(Tab tab, string id, T value)
		{
			if (Player == null) return default;

			return GetStorage(tab, id, value);
		}
		public void ClearStorage(Tab tab, string id)
		{
			id = $"{tab?.Id}_{id}";

			LocalStorage[id] = null;
		}
		public bool HasStorage(Tab tab, string id)
		{
			id = $"{tab?.Id}_{id}";

			return LocalStorage.ContainsKey(id);
		}
		public void Clear()
		{
			foreach (var page in ColumnPages)
			{
				var value = ColumnPages[page.Key];
				Facepunch.Pool.Free(ref value);
			}

			ColumnPages.Clear();
			// LocalStorage.Clear();

			_selectedDropdown = null;
			_selectedDropdownPage.CurrentPage = 0;
		}
		public Page GetOrCreatePage(int column)
		{
			if (ColumnPages.TryGetValue(column, out var page)) return page;
			else
			{
				ColumnPages[column] = page = new Page();
				return page;
			}
		}

		public void Dispose()
		{
			Clear();
		}

		public class Page
		{
			public int CurrentPage { get; set; }
			public int TotalPages { get; set; }

			public void Check()
			{
				if (CurrentPage < 0) CurrentPage = TotalPages;
				else if (CurrentPage > TotalPages) CurrentPage = 0;
			}
		}
	}
	public class Tab : IDisposable
	{
		public string Id;
		public string Name;
		public int AccessLevel = 0;
		public RustPlugin Plugin;
		public Action<Tab, CUI, CuiElementContainer, string, PlayerSession> Over, Under, Override;
		public Dictionary<int, List<Option>> Columns = new();
		public Action<PlayerSession, Tab> OnChange;
		public Dictionary<string, Radio> Radios = new();
		public TabDialog Dialog;
		public bool Fullscreen;

		public Tab(string id, string name, RustPlugin plugin, Action<PlayerSession, Tab> onChange = null, int accessLevel = 0)
		{
			Id = id;
			Name = name;
			Plugin = plugin;
			OnChange = onChange;
			AccessLevel = accessLevel;
		}

		public void ClearColumn(int column, bool erase = false)
		{
			if (Columns.TryGetValue(column, out var rows))
			{
				rows.Clear();

				if (erase)
				{
					Columns[column] = null;
					Columns.Remove(column);
				}
			}
		}
		public void ClearAfter(int index, bool erase = false)
		{
			var count = Columns.Count;

			for (int i = 0; i < count; i++)
			{
				if (i >= index) ClearColumn(i, erase);
			}
		}
		public Tab AddColumn(int column, bool clear = false)
		{
			if (!Columns.TryGetValue(column, out var options))
			{
				Columns[column] = options = new List<Option>();
			}

			if (clear)
			{
				options.Clear();
			}

			return this;
		}
		public Tab AddRow(int column, Option row)
		{
			if (Columns.TryGetValue(column, out var options))
			{
				options.Add(row);
			}
			else
			{

				Columns[column] = options = new List<Option>();
				options.Add(row);
			}

			return this;
		}
		public Tab AddName(int column, string name, TextAnchor align = TextAnchor.MiddleLeft)
		{
			return AddRow(column, new OptionName(name, align));
		}
		public Tab AddButton(int column, string name, Action<PlayerSession> callback, Func<PlayerSession, OptionButton.Types> type = null, TextAnchor align = TextAnchor.MiddleCenter)
		{
			return AddRow(column, new OptionButton(name, align, callback, type));
		}
		public Tab AddToggle(int column, string name, Action<PlayerSession> callback, Func<PlayerSession, bool> isOn = null, string tooltip = null)
		{
			return AddRow(column, new OptionToggle(name, callback, ap => { try { return (isOn?.Invoke(ap)).GetValueOrDefault(false); } catch (Exception ex) { Logger.Error($"AddToggle[{column}][{name}] failed", ex); } return false; }, tooltip));
		}
		public Tab AddText(int column, string name, int size, string color, TextAnchor align = TextAnchor.MiddleCenter, Handler.FontTypes font = Handler.FontTypes.RobotoCondensedRegular, bool isInput = false)
		{
			return AddRow(column, new OptionText(name, size, color, align, font, isInput));
		}
		public Tab AddInput(int column, string name, Func<PlayerSession, string> placeholder, int characterLimit, bool readOnly, Action<PlayerSession, string[]> callback = null, string tooltip = null)
		{
			return AddRow(column, new OptionInput(name, placeholder, characterLimit, readOnly, callback, tooltip));
		}
		public Tab AddInput(int column, string name, Func<PlayerSession, string> placeholder, Action<PlayerSession, string[]> callback = null, string tooltip = null)
		{
			return AddInput(column, name, placeholder, 0, callback == null, callback, tooltip);
		}
		public Tab AddEnum(int column, string name, Action<PlayerSession, bool> callback, Func<PlayerSession, string> text, string tooltip = null)
		{
			AddRow(column, new OptionEnum(name, callback, text, tooltip));
			return this;
		}
		public Tab AddRadio(int column, string name, string id, bool wantsOn, Action<bool, PlayerSession> callback = null, string tooltip = null)
		{
			if (!Radios.TryGetValue(id, out var radio))
			{
				Radios[id] = radio = new();
			}

			radio.TemporaryIndex++;
			if (wantsOn) radio.Selected = radio.TemporaryIndex;

			var index = radio.TemporaryIndex;
			var option = new OptionRadio(name, id, index, wantsOn, callback, radio, tooltip);
			radio.Options.Add(option);

			return AddRow(column, option);
		}
		public Tab AddDropdown(int column, string name, Func<PlayerSession, int> index, Action<PlayerSession, int> callback, string[] options, string[] optionsIcons = null, float optionsIconScale = 0f, string tooltip = null)
		{
			return AddRow(column, new OptionDropdown(name, index, callback, options, optionsIcons, optionsIconScale, tooltip));
		}
		public Tab AddRange(int column, string name, float min, float max, Func<PlayerSession, float> value, Action<PlayerSession, float> callback, Func<PlayerSession, string> text = null, string tooltip = null)
		{
			return AddRow(column, new OptionRange(name, min, max, value, callback, text, tooltip));
		}
		public Tab AddButtonArray(int column, float spacing, params OptionButton[] buttons)
		{
			return AddRow(column, new OptionButtonArray(string.Empty, spacing, null, buttons));
		}
		public Tab AddButtonArray(int column, params OptionButton[] buttons)
		{
			return AddRow(column, new OptionButtonArray(string.Empty, 0.01f, null, buttons));
		}
		public Tab AddInputButton(int column, string name, float buttonPriority, OptionInput input, OptionButton button, string tooltip = null)
		{
			return AddRow(column, new OptionInputButton(name, buttonPriority, input, button, tooltip));
		}
		public Tab AddColor(int column, string name, Func<string> color, Action<PlayerSession, string, string, float> callback, string tooltip = null)
		{
			return AddRow(column, new OptionColor(name, color, callback, tooltip));
		}

		public void CreateDialog(string title, Action<PlayerSession> onConfirm, Action<PlayerSession> onDecline)
		{
			Dialog = new TabDialog(title, onConfirm, onDecline);
		}
		public void CreateModal(BasePlayer player, string title, Dictionary<string, ModalModule.Modal.Field> fields, Action<BasePlayer, ModalModule.Modal> onConfirm = null, Action onCancel = null)
		{
			Singleton.Modal.Open(player, title, fields, onConfirm, onCancel);
		}

		public void Dispose()
		{
			foreach (var column in Columns)
			{
				column.Value.Clear();
			}

			Columns.Clear();
			Columns = null;
		}

		public class Radio : IDisposable
		{
			public int Selected;

			public int TemporaryIndex = -1;

			public List<OptionRadio> Options = new();

			public void Change(int index, PlayerSession ap)
			{
				Options[Selected]?.Callback?.Invoke(false, ap);

				Selected = index;
			}

			public void Dispose()
			{
				Options.Clear();
				Options = null;
			}
		}
		public class TabDialog
		{
			public string Title;
			public Action<PlayerSession> OnConfirm, OnDecline;

			public TabDialog(string title, Action<PlayerSession> onConfirm, Action<PlayerSession> onDecline)
			{
				Title = title;
				OnConfirm = onConfirm;
				OnDecline = onDecline;
			}
		}

		public class Option
		{
			public string Name;
			public string Tooltip;

			public Option(string name, string tooltip = null)
			{
				Name = name;
				Tooltip = tooltip;
			}
		}
		public class OptionName : Option
		{
			public TextAnchor Align;

			public OptionName(string name, TextAnchor align, string tooltip = null) : base(name, tooltip) { Align = align; }
		}
		public class OptionText : Option
		{
			public int Size;
			public string Color;
			public TextAnchor Align;
			public Handler.FontTypes Font;
			public bool IsInput;

			public OptionText(string name, int size, string color, TextAnchor align, Handler.FontTypes font, bool isInput, string tooltip = null) : base(name, tooltip) { Align = align; Size = size; Color = color; Font = font; IsInput = isInput; }
		}
		public class OptionInput : Option
		{
			public Func<PlayerSession, string> Placeholder;
			public int CharacterLimit;
			public bool ReadOnly;
			public Action<PlayerSession, string[]> Callback;

			public OptionInput(string name, Func<PlayerSession, string> placeholder, int characterLimit, bool readOnly, Action<PlayerSession, string[]> args, string tooltip = null) : base(name, tooltip)
			{
				Placeholder = ap => { try { return placeholder?.Invoke(ap); } catch (Exception ex) { Logger.Error($"Failed OptionInput.Placeholder callback ({name}): {ex.Message}"); return string.Empty; } };
				Callback = (ap, args2) => { try { args?.Invoke(ap, args2); } catch (Exception ex) { Logger.Error($"Failed OptionInput.Callback callback ({name}): {ex.Message}"); } };
				CharacterLimit = characterLimit;
				ReadOnly = readOnly;
			}
		}
		public class OptionButton : Option
		{
			public Func<PlayerSession, Types> Type;
			public Action<PlayerSession> Callback;
			public TextAnchor Align = TextAnchor.MiddleCenter;

			public enum Types
			{
				None,
				Selected,
				Warned,
				Important
			}

			public OptionButton(string name, TextAnchor align, Action<PlayerSession> callback, Func<PlayerSession, Types> type = null, string tooltip = null) : base(name, tooltip)
			{
				Align = align;
				Callback = (ap) => { try { callback?.Invoke(ap); } catch (Exception ex) { Logger.Error($"Failed OptionButton.Callback callback ({name}): {ex.Message}"); } };
				Type = (ap) => { try { return (type?.Invoke(ap)).GetValueOrDefault(Types.None); } catch (Exception ex) { Logger.Error($"Failed OptionButton.Type callback ({name}): {ex.Message}"); return Types.None; } };
			}
			public OptionButton(string name, Action<PlayerSession> callback, Func<PlayerSession, Types> type = null, string tooltip = null) : base(name, tooltip)
			{
				Callback = callback;
				Type = type;
			}
		}
		public class OptionToggle : Option
		{
			public Func<PlayerSession, bool> IsOn;
			public Action<PlayerSession> Callback;

			public OptionToggle(string name, Action<PlayerSession> callback, Func<PlayerSession, bool> isOn = null, string tooltip = null) : base(name, tooltip)
			{
				Callback = (ap) => { try { callback?.Invoke(ap); } catch (Exception ex) { Logger.Error($"Failed OptionToggle.Callback callback ({name}): {ex.Message}"); } };
				IsOn = (ap) => { try { return (isOn?.Invoke(ap)).GetValueOrDefault(false); } catch (Exception ex) { Logger.Error($"Failed OptionToggle.IsOn callback ({name}): {ex.Message}"); return false; } };
			}
		}
		public class OptionEnum : Option
		{
			public Func<PlayerSession, string> Text;
			public Action<PlayerSession, bool> Callback;

			public OptionEnum(string name, Action<PlayerSession, bool> callback, Func<PlayerSession, string> text, string tooltip = null) : base(name, tooltip)
			{
				Callback = (ap, value) => { try { callback?.Invoke(ap, value); } catch (Exception ex) { Logger.Error($"Failed OptionEnum.Callback callback ({name}): {ex.Message}"); } };
				Text = (ap) => { try { return text?.Invoke(ap); } catch (Exception ex) { Logger.Error($"Failed OptionToggle.Callback callback ({name}): {ex.Message}"); return string.Empty; } };
			}
		}
		public class OptionRange : Option
		{
			public float Min = 0;
			public float Max = 1;
			public Func<PlayerSession, float> Value;
			public Action<PlayerSession, float> Callback;
			public Func<PlayerSession, string> Text;

			public OptionRange(string name, float min, float max, Func<PlayerSession, float> value, Action<PlayerSession, float> callback, Func<PlayerSession, string> text, string tooltip = null) : base(name, tooltip)
			{
				Min = min;
				Max = max;
				Callback = (ap, value) => { try { callback?.Invoke(ap, value); } catch (Exception ex) { Logger.Error($"Failed OptionRange.Callback callback ({name}): {ex.Message}"); } };
				Value = (ap) => { try { return (value?.Invoke(ap)).GetValueOrDefault(0); } catch (Exception ex) { Logger.Error($"Failed OptionRange.Callback callback ({name}): {ex.Message}"); return 0f; } };
				Text = (ap) => { try { return text?.Invoke(ap); } catch (Exception ex) { Logger.Error($"Failed OptionRange.Callback callback ({name}): {ex.Message}"); return string.Empty; } };
			}
		}
		public class OptionRadio : Option
		{
			public string Id;
			public int Index;
			public bool WantsOn;
			public Action<bool, PlayerSession> Callback;

			public Radio Radio;

			public OptionRadio(string name, string id, int index, bool on, Action<bool, PlayerSession> callback, Radio radio, string tooltip = null) : base(name, tooltip)
			{
				Id = id;
				Callback = (value, ap) => { try { callback?.Invoke(value, ap); } catch (Exception ex) { Logger.Error($"Failed OptionRadio.Callback callback ({name}): {ex.Message}"); } };
				WantsOn = on;
				Index = index;
				Radio = radio;
			}
		}
		public class OptionDropdown : Option
		{
			public Func<PlayerSession, int> Index;
			public Action<PlayerSession, int> Callback;
			public string[] Options;
			public string[] OptionsIcons;
			public float OptionsIconScale;

			public OptionDropdown(string name, Func<PlayerSession, int> index, Action<PlayerSession, int> callback, string[] options, string[] optionsIcons, float optionsIconScale, string tooltip = null) : base(name, tooltip)
			{
				Index = (ap) => { try { return (index?.Invoke(ap)).GetValueOrDefault(0); } catch (Exception ex) { Logger.Error($"Failed OptionRange.Callback callback ({name}): {ex.Message}"); return 0; } };
				Callback = (ap, value) => { try { callback?.Invoke(ap, value); } catch (Exception ex) { Logger.Error($"Failed OptionRange.Callback callback ({name}): {ex.Message}"); } };
				Options = options;
				OptionsIcons = optionsIcons;
				OptionsIconScale = optionsIconScale;
			}
		}
		public class OptionInputButton : Option
		{
			public OptionInput Input;
			public OptionButton Button;
			public float ButtonPriority = 0.25f;

			public OptionInputButton(string name, float buttonPriority, OptionInput input, OptionButton button, string tooltip = null) : base(name, tooltip)
			{
				ButtonPriority = buttonPriority;
				Input = input;
				Button = button;
			}
		}
		public class OptionButtonArray : Option
		{
			public OptionButton[] Buttons;
			public float Spacing = 0.01f;

			public OptionButtonArray(string name, float spacing, string tooltip = null, params OptionButton[] buttons) : base(name, tooltip)
			{
				Buttons = buttons;
				Spacing = spacing;
			}
		}
		public class OptionColor : Option
		{
			public Func<string> Color;
			public Action<PlayerSession, string, string, float> Callback;

			public OptionColor(string name, Func<string> color, Action<PlayerSession, string, string, float> callback, string tooltip = null) : base(name, tooltip)
			{
				Color = color;
				Callback = callback;
			}
		}
	}
	public class DynamicTab : Tab
	{
		public DynamicTab(string id, string name, RustPlugin plugin, Action<PlayerSession, Tab> onChange = null) : base(id, name, plugin, onChange) { }
	}

	#region Core Tabs

	#region Administration - Custom Commands

	[ProtectedCommand("carbongg.endspectate")]
	private void EndSpectate(Arg arg)
	{
		StopSpectating(arg.Player());
	}

	#endregion

	private void OnPluginLoaded(RustPlugin plugin)
	{
		PluginsTab.GetVendor(PluginsTab.VendorTypes.Codefling)?.Refresh();
		PluginsTab.GetVendor(PluginsTab.VendorTypes.uMod)?.Refresh();

		foreach (var player in BasePlayer.activePlayerList)
		{
			var ap = Singleton.GetPlayerSession(player);

			if (ap.IsInMenu && Singleton.GetTab(player).Id == "plugins")
			{
				Singleton.Draw(player);
			}
		}
	}
	private void OnPluginUnloaded(RustPlugin plugin)
	{
		Community.Runtime.CorePlugin.NextTick(() =>
		{
			foreach (var player in BasePlayer.activePlayerList)
			{
				var ap = Singleton.GetPlayerSession(player);

				if (ap.IsInMenu && Singleton.GetTab(player).Id == "pluginbrowser")
				{
					Singleton.Draw(player);
				}
			}
		});
	}

	#endregion

	internal static void StartSpectating(BasePlayer player, BaseEntity target)
	{
		if (!string.IsNullOrEmpty(player.spectateFilter))
		{
			StopSpectating(player);
		}

		var targetPlayer = target as BasePlayer;
		player.Teleport(target.transform.position);
		player.SetPlayerFlag(BasePlayer.PlayerFlags.Spectating, b: true);
		player.gameObject.SetLayerRecursive(10);
		player.CancelInvoke(player.InventoryUpdate);
		player.SendEntitySnapshot(target);
		player.gameObject.Identity();
		player.SetParent(target);
		player.viewAngles = target.transform.rotation.eulerAngles;
		player.eyes.NetworkUpdate(target.transform.rotation);
		player.SendNetworkUpdate();
		player.spectateFilter = targetPlayer != null ? targetPlayer.UserIDString : target.net.ID.ToString();

		using var cui = new CUI(Singleton.Handler);
		var container = cui.CreateContainer(SpectatePanelId, color: "0.1 0.1 0.1 0.8", needsCursor: false, parent: ClientPanels.Overlay);
		var panel = cui.CreatePanel(container, SpectatePanelId, null, "0 0 0 0");
		var item = target.GetItem();
		cui.CreateText(container, panel, null, "1 1 1 0.2", $"YOU'RE SPECTATING ".SpacedString(1, false) + $"<b>{(targetPlayer == null ? item != null ? item.info.displayName.english.ToUpper().SpacedString(1) : target.ShortPrefabName.ToUpper().SpacedString(1) : targetPlayer.displayName.ToUpper().SpacedString(1))}</b>", 15);
		cui.CreateProtectedButton(container, panel, null, "#1c6aa0", "1 1 1 0.7", "END SPECTATE".SpacedString(1), 10,
			xMin: 0.45f, xMax: 0.55f, yMin: 0.15f, yMax: 0.19f, command: "carbongg.endspectate");
		cui.Send(container, player);

		Community.Runtime.CorePlugin.NextTick(() => Singleton.Close(player));
	}
	internal static void StopSpectating(BasePlayer player)
	{
		using var cui = new CUI(Singleton.Handler);
		cui.Destroy(SpectatePanelId, player);

		if (string.IsNullOrEmpty(player.spectateFilter))
		{
			return;
		}

		var spectated = player.GetParentEntity();
		player.SetParent(null, true, true);
		player.SetPlayerFlag(BasePlayer.PlayerFlags.Spectating, b: false);
		player.InvokeRepeating(player.InventoryUpdate, 1f, 0.1f * UnityEngine.Random.Range(0.99f, 1.01f));
		player.gameObject.SetLayerRecursive(17);
		if (spectated != null) player.Teleport(spectated.transform.position);
		player.spectateFilter = string.Empty;
		if (!player.IsFlying) player.SendConsoleCommand("noclip");
		player.Teleport(player.transform.position + (Vector3.up * -3f));

		var tab = Singleton.GetTab(player);
		var ap = Singleton.GetPlayerSession(player);
		EntitiesTab.SelectEntity(tab, ap, spectated);
		EntitiesTab.DrawEntitySettings(tab, 1, ap);
		Singleton.Draw(player);
	}

	#region Custom Tabs

	public class ConfigEditor : Tab
	{
		internal JObject Entry { get; set; }
		internal Action<PlayerSession, JObject> OnSave, OnSaveAndReload, OnCancel;
		internal const string Spacing = " ";

		public ConfigEditor(string id, string name, RustPlugin plugin, Action<PlayerSession, Tab> onChange = null) : base(id, name, plugin, onChange)
		{
		}

		public static ConfigEditor Make(string json, Action<PlayerSession, JObject> onCancel, Action<PlayerSession, JObject> onSave, Action<PlayerSession, JObject> onSaveAndReload)
		{
			var tab = new ConfigEditor("configeditor", "Config Editor", Community.Runtime.CorePlugin)
			{
				Entry = JObject.Parse(json),
				OnSave = onSave,
				OnSaveAndReload = onSaveAndReload,
				OnCancel = onCancel
			};

			tab._draw();
			return tab;
		}

		internal void _draw()
		{
			AddColumn(0);
			AddColumn(1);

			var list = Facepunch.Pool.GetList<OptionButton>();
			if (OnCancel != null) list.Add(new OptionButton("Cancel", ap => { OnCancel?.Invoke(ap, Entry); }));
			if (OnSave != null) list.Add(new OptionButton("Save", ap => { OnSave?.Invoke(ap, Entry); }));
			if (OnSaveAndReload != null) list.Add(new OptionButton("Save & Reload", ap => { OnSaveAndReload?.Invoke(ap, Entry); }));

			AddButtonArray(0, list.ToArray());
			Facepunch.Pool.FreeList(ref list);

			foreach (var token in Entry)
			{
				if (token.Value is JObject) AddName(0, $"{token.Key}");

				_recurseBuild(token.Key, token.Value, 0, 0);
			}
		}
		internal void _recurseBuild(string name, JToken token, int level, int column, bool removeButtons = false)
		{
			switch (token)
			{
				case JArray array:
					{
						AddName(column, $"{StringEx.SpacedString(Spacing, level, false)}{name}");
						AddButton(column, $"Edit", ap =>
						{
							_drawArray(name, array, level, column, ap);
						});
					}
					break;

				default:
					var usableToken = token is JProperty property ? property.Value : token;
					switch (usableToken?.Type)
					{
						case JTokenType.String:
							var value = usableToken.ToObject<string>();
							var valueSplit = value.Split(' ');
							if (value.StartsWith("#") || (valueSplit.Length >= 3 && valueSplit.All(x => float.TryParse(x, out _))))
							{
								AddColor(column, name, () => value.StartsWith("#") ? HexToRustColor(value) : value, (ap, hex, rust, alpha) =>
								{
									value = value.StartsWith("#") ? hex : rust;
									usableToken.Replace(usableToken = $"#{value}");
									Community.Runtime.CorePlugin.NextFrame(() => Singleton.SetTab(ap.Player, Make(Entry.ToString(), OnCancel, OnSave, OnSaveAndReload), false));
								}, tooltip: $"The color value of the '{name.Trim()}' property.");
							}
							else AddInput(column, name, ap => usableToken.ToObject<string>(), (ap, args) => { usableToken.Replace(usableToken = args.ToString(" ")); });
							Array.Clear(valueSplit, 0, valueSplit.Length);
							valueSplit = null;
							break;

						case JTokenType.Integer:
							AddInput(column, name, ap => usableToken.ToObject<long>().ToString(), (ap, args) => { usableToken.Replace(usableToken = args.ToString(" ").ToLong()); }, tooltip: $"The integer/long value of the '{name.Trim()}' property.");
							break;

						case JTokenType.Float:
							AddInput(column, name, ap => usableToken.ToObject<float>().ToString(), (ap, args) => { usableToken.Replace(usableToken = args.ToString(" ").ToFloat()); }, tooltip: $"The float value of the '{name.Trim()}' property.");
							break;

						case JTokenType.Boolean:
							AddToggle(column, name,
								ap => { usableToken.Replace(usableToken = !usableToken.ToObject<bool>()); },
								ap => usableToken.ToObject<bool>(), tooltip: $"The boolean value of the '{name.Trim()}' property.");
							break;

						case JTokenType.Array:
							{
								var array2 = usableToken as JArray;
								AddName(column, $"{StringEx.SpacedString(Spacing, level, false)}{name}");
								AddButton(column, $"Edit", ap =>
								{
									_drawArray(name, array2, level, column, ap);
								});
							}
							break;

						case JTokenType.Object:
							var newLevel = level + 1;
							if (token.Parent is JArray array)
							{
								AddInputButton(column, null, 0.2f,
									new OptionInput(null, ap => $"{array.IndexOf(token)}", 0, true, null),
									new OptionButton("Remove", TextAnchor.MiddleCenter, ap =>
									{
										array.Remove(token);
										ClearColumn(column);
										// DrawArray(name, array, 0, true);
										_drawArray(name, array, level, column, ap);
									}, ap => OptionButton.Types.Important));

							}
							DrawArray(name, token, newLevel);

							break;
					}
					break;
			}

			void DrawArray(string title, JToken tok, int ulevel, bool editRefresh = false)
			{
				if (editRefresh)
				{
					AddName(column, $"Editing '{(tok.Parent as JProperty)?.Name}'");
				}

				foreach (var subToken in tok)
				{
					if (subToken is JObject && !editRefresh)
						AddName(column, $"{StringEx.SpacedString(Spacing, ulevel, false)}{(subToken.Parent as JProperty)?.Name}");
					_recurseBuild($"{StringEx.SpacedString(Spacing, ulevel + 1, false)}{(subToken as JProperty)?.Name}", subToken, ulevel + 1, column);

					if (removeButtons)
					{
						var jproperty = (subToken as JProperty);
						AddButton(column, $"Remove '{jproperty?.Name.Trim()}'", ap2 =>
						{
							(tok as JObject).Remove(jproperty.Name);
							_drawArray(name, tok.Parent as JArray, ulevel, column, ap2);
						}, ap2 => OptionButton.Types.Important);
					}
				}
			}
		}
		internal void _drawArray(string name, JArray array, int level, int column, PlayerSession ap)
		{
			var index = 0;
			var subColumn = column + 1;
			ClearAfter(subColumn, true);
			AddName(subColumn, $"Editing '{name.Trim()}'");
			foreach (var element in array)
			{
				_recurseBuild($"{StringEx.SpacedString(Spacing, level, false)}{index:n0}", element, 0, subColumn, array.Count == 1);

				AddButton(subColumn, $"Remove", ap2 =>
				{
					array.Remove(element);
					_drawArray(name, array, level, column, ap);
				}, ap2 => OptionButton.Types.Important);

				index++;
			}
			if (array.Count <= 1)
			{
				var sample = array.FirstOrDefault() as JObject;
				var newPropertyName = ap.GetStorage(this, "jsonprop", "New Property");

				if (array.Count == 1)
				{
					AddButton(subColumn, $"Duplicate", ap2 =>
					{
						array.Add(array.LastOrDefault());
						_drawArray(name, array, level, column, ap);
					}, ap2 => OptionButton.Types.Warned);
				}
				else if (array.Count == 0) AddText(subColumn, $"{StringEx.SpacedString(Spacing, 0, false)}No entries", 10, "1 1 1 0.6", TextAnchor.MiddleLeft);

				AddInput(subColumn, "Property Name", ap => ap.GetStorage(this, "jsonprop", "New Property"), (ap, args) => { ap.SetStorage(this, "jsonprop", newPropertyName = args.ToString(" ")); });
				AddButtonArray(subColumn, 0.01f,
					new OptionButton("Add Label", ap => { if (sample == null) array.Add(sample = JObject.Parse("{ }")); if (!(sample as IDictionary<string, JToken>).ContainsKey(newPropertyName)) { sample.Add(newPropertyName, string.Empty); _drawArray(name, array, level, column, ap); } }),
					new OptionButton("Add Toggle", ap => { if (sample == null) array.Add(sample = JObject.Parse("{ }")); if (!(sample as IDictionary<string, JToken>).ContainsKey(newPropertyName)) { sample.Add(newPropertyName, false); _drawArray(name, array, level, column, ap); } }),
					new OptionButton("Add Int", ap => { if (sample == null) array.Add(sample = JObject.Parse("{ }")); if (!(sample as IDictionary<string, JToken>).ContainsKey(newPropertyName)) { sample.Add(newPropertyName, 0); _drawArray(name, array, level, column, ap); } }),
					new OptionButton("Add Float", ap => { if (sample == null) array.Add(sample = JObject.Parse("{ }")); if (!(sample as IDictionary<string, JToken>).ContainsKey(newPropertyName)) { sample.Add(newPropertyName, 0.0f); _drawArray(name, array, level, column, ap); } }));
			}
			else
			{
				AddButton(subColumn, $"Duplicate", ap2 =>
				{
					array.Add(array.LastOrDefault());
					_drawArray(name, array, level, column, ap);
				}, ap2 => OptionButton.Types.Selected);
			}
		}
	}
	public class SetupWizard : Tab
	{
		internal List<Page> Pages = new();

		public SetupWizard(string id, string name, RustPlugin plugin, Action<PlayerSession, Tab> onChange = null) : base(id, name, plugin, onChange)
		{
		}

		public static SetupWizard Make()
		{
			var pluginsTab = Singleton.FindTab("plugins");

			var tab = new SetupWizard("setupwizard", "Setup Wizard", Community.Runtime.CorePlugin) { Fullscreen = true };
			tab.Override = tab.Draw;

			tab.Pages.Add(new Page("Main", (cui, t, container, panel, ap) =>
			{
				cui.CreateImage(container, panel, null, "carbonws", "1 1 1 0.7", 0.2f, 0.8f, yMin: 0.52f, yMax: 0.71f, OyMin: -20, OyMax: -20);
				cui.CreateText(container, panel, null, "1 1 1 0.5", "Welcome to <b>Carbon</b> setup wizard!", 13, yMax: 0.495f, OyMin: -20, OyMax: -20, align: TextAnchor.UpperCenter);
				tab.DisplayArrows(cui, tab, container, panel, ap, true);
			}));

			tab.Pages.Add(new Page("Getting Started", (cui, t, container, panel, ap) =>
			{
				tab.InfoTemplate(cui, t, container, panel, ap, "What is Carbon?",
					$"Carbon is a robust self-updating framework designed with performance and stability in mind. It has complete backward compatibility with all Oxide plugins and uses the <b>latest C#</b> and <b>Harmony</b> libraries to give developers the tools to let their creativity flourish!\n\n" +
					$"\n{Header("Features", 1)}" +
					$"\n<b>AUTO-UPDATES</b>" +
					$"\nCarbon updates itself and all dynamic hook DLLs at runtime, so you do not need to restart your Rust server when new builds come out.\n\n" +
					$"<b>C# 10</b>" +
					$"\nCarbon natively supports the latest C# version, with many improvements and optimizations. All plugins get compiled to the latest C# version.\n\n" +
					$"<b>HIGH PERFORMANCE</b>" +
					$"\nThe goal is to make the servers as performant as possible, learn from the community about new ways to do things, and high profiling for tracking any hiccups.\n\n" +
					$"<b>DYNAMIC HOOKS</b>" +
					$"\nA crucial distinction that makes Carbon so much different from other frameworks is that although we support all Oxide hooks (appx. +600), they are not called unless a plugin subscribes to them.\n\n" +
					$"<b>HARMONY 2.0</b>" +
					$"\nCarbon runs on Harmony v2.0, which introduces higher performance Pre-&-Post-fixes and even faster transpiler calls, which 99% of running hooks use.", "");
			}));
			tab.Pages.Add(new Page("Modules", (cui, t, container, panel, ap) =>
			{
				tab.InfoTemplate(cui, t, container, panel, ap,
					"Modules",
					"Modules are built-in and out of the box ready-to-use systems that are useful for most servers." +
					"\nThey're heavily inspired by public free plugins on the Rust modding market providing convenience and maintenance for your most important needs.", "");
			}));
			tab.Pages.Add(new Page("Gather Manager", (cui, t, container, panel, ap) =>
			{
				tab.ModuleInfoTemplate(cui, t, container, panel, ap,
					"Gather Manager Module",
					"The module allows you to modify the processing modifiers of Quarries, Excavators, Pickup and Gather amounts, globally set or item-specific. This module comes with a variety of useful tools, including custom recycler speed.",
					"", FindModule("GatherManagerModule"));
			}));
			tab.Pages.Add(new Page("Stack Manager", (cui, t, container, panel, ap) =>
			{
				tab.ModuleInfoTemplate(cui, t, container, panel, ap,
					"Stack Manager Module",
					"High performance will allow to set custom item stacks based on:\n" +
					"\n• Item name" +
					"\n• Item category" +
					"\n• Blacklisted items (useful when using categories)", "", FindModule("StackManagerModule"));
			}));
			tab.Pages.Add(new Page("Vanish", (cui, t, container, panel, ap) =>
			{
				tab.ModuleInfoTemplate(cui, t, container, panel, ap,
					"Vanish Module",
					"A very lightweight auth-level based system allowing you to become invisible, with various settings in the config file.", "", FindModule("VanishModule"));
			}));
			tab.Pages.Add(new Page("Moderation Tools", (cui, t, container, panel, ap) =>
			{
				tab.ModuleInfoTemplate(cui, t, container, panel, ap,
					"Moderation Tools Module",
					"This module is a bundle of very helpful and often usable moderation tools that can grant the ability to players with regular authority level to use noclip and god-mode and nothing else (use the 'carbon.admin' permission to allow the use of the '/cadmin' command).\n" +
					"There's also a permission ('carbon.cmod') that allows players to kick, ban or mute players with defined reasons.", "", FindModule("ModerationToolsModule"));
			}));
			tab.Pages.Add(new Page("Optimisations", (cui, t, container, panel, ap) =>
			{
				tab.ModuleInfoTemplate(cui, t, container, panel, ap,
					"Optimisations Module",
					"A Carbon built-in version of the Circular Network Distance from Codefling.", "", FindModule("OptimisationsModule"));

			}));
			tab.Pages.Add(new Page("Plugin Browser", (cui, t, container, panel, ap) =>
			{
				cui.CreateClientImage(container, panel, null, "https://i.imgur.com/mFxaU99.png", "1 1 1 0.7",
					xMin: 0.35f, xMax: 0.65f, yMin: 0.1f, yMax: 0.425f, fadeIn: 1f);

				var cfPaidPluginCount = PluginsTab.CodeflingInstance?.FetchedPlugins.Count(x => x.IsPaid());
				var cfFreePluginCount = PluginsTab.CodeflingInstance?.FetchedPlugins.Count(x => !x.IsPaid());
				var uModFreePluginCount = PluginsTab.uModInstance?.FetchedPlugins.Count(x => !x.IsPaid());

				tab.InternalFeatureInfoTemplate(cui, t, container, panel, ap,
					"Plugin Browser",
					"The plugin browser allows you to explore Codefling and uMod plugins all within the game. Manage, stay up-to-date and download new plugins in the very intuitive UI. Filter all plugins by searching, using tags or sort the lists based on your liking." +
					$"\n\n{Header("Codefling", 1)} ({cfFreePluginCount:n0} free, {cfPaidPluginCount:n0} paid)" +
					$"\nThe Codefling integration allows you to download free available files and it supports an in-game login system which allows you to download or update your paid files in-game." +
					$"\nPurchasing will not be available but you may browse new files you're interested in and add them to cart through the game as well." +
					$"\n\n{Header("uMod", 1)} ({uModFreePluginCount:n0} free)" +
					$"\nBrowse the 1.5K catalogue full of free, Rust- and covalence supported plugins.",
					"A very minimum amount of plugins currently are not compatible, due to them being out of date (on Oxide too) or requiring external DLLs that are Oxide-only compatible; meaning that it's the author's responsability to add Carbon support.",
					"plugins");
			}));
			tab.Pages.Add(new Page("Whitelist", (cui, t, container, panel, ap) =>
			{
				tab.ModuleInfoTemplate(cui, t, container, panel, ap,
					"Whitelist Module",
					"A very basic system that only grants players access to a server based on the 'whitelist.bypass' permission or 'whitelisted' group.", "",
					FindModule("WhitelistModule"));
			}));
			tab.Pages.Add(new Page("DRM", (cui, t, container, panel, ap) =>
			{
				tab.ModuleInfoTemplate(cui, t, container, panel, ap,
					"DRM Module",
					"A system that allows server hosts to bind endpoints that deliver plugin information with respect to the public and private keys.\n" +
					"For more information, check out the documentation page over on https://docs.carbonmod.gg.", "",
					FindModule("DRMModule"));
			}));

			tab.Pages.Add(new Page("Finalize", (cui, t, container, panel, ap) =>
			{
				Singleton.DataInstance.ShowedWizard = true;
				Singleton.GenerateTabs();
				Community.Runtime.CorePlugin.NextTick(() => Singleton.SetTab(ap.Player, 0));
			}));

			return tab;
		}

		internal void Draw(Tab tab, CUI cui, CuiElementContainer container, string panel, PlayerSession ap)
		{
			var page = ap.GetStorage(tab, "page", 0);
			Pages[page].Draw?.Invoke(cui, tab, container, panel, ap);
		}

		internal void InfoTemplate(CUI cui, Tab tab, CuiElementContainer container, string panel, PlayerSession ap, string title, string content, string hint)
		{
			cui.CreateImage(container, panel, null, "carbonws", "0 0 0 0.1", xMin: 0.75f, xMax: 0.95f, yMin: 0.875f, yMax: 0.95f);

			var mainTitle = cui.CreatePanel(container, panel, null, "0 0 0 0.5", xMin: 0.05f, xMax: ((float)title.Length).Scale(0, 7, 0.075f, 0.18f), yMin: 0.875f, yMax: 0.95f);
			cui.CreateText(container, mainTitle, null, "1 1 1 1", $"<b>{title.ToUpper()}</b>", 25, align: TextAnchor.MiddleCenter, fadeIn: 2f);

			cui.CreatePanel(container, panel, null, "0 0 0 0.5", xMin: 0.05f, xMax: 0.875f, yMin: 0.1f, yMax: 0.86f, fadeIn: 1f);
			cui.CreateText(container, panel, null, "1 1 1 0.5", content +
				$"\n\n{(string.IsNullOrEmpty(hint) ? "" : Header("Hint", 2))}" +
				$"\n{hint}", 12, xMin: 0.06f, xMax: 0.85f, yMax: 0.84f, align: TextAnchor.UpperLeft);

			DisplayArrows(cui, tab, container, panel, ap);
		}
		internal void ModuleInfoTemplate(CUI cui, Tab tab, CuiElementContainer container, string panel, PlayerSession player, string title, string content, string hint, BaseModule module)
		{
			var consoleCommands = Community.Runtime.CommandManager.ClientConsole.Where(x => x.Reference == module && !x.HasFlag(CommandFlags.Hidden));
			var chatCommands = Community.Runtime.CommandManager.Chat.Where(x => x.Reference == module && !x.HasFlag(CommandFlags.Hidden));
			var consoleCommandCount = consoleCommands.Count();
			var chatCommandCount = chatCommands.Count();

			content = $"The <b>{module.Name}</b> uses <b>{module.Hooks.Count:n0}</b> total {module.Hooks.Count.Plural("hook", "hooks")}, with currently <b>{module.IgnoredHooks.Count:n0}</b> ignored {module.IgnoredHooks.Count.Plural("hook", "hooks")}, " +
				$"and so far has used {module.TotalHookTime:0.000}ms of server time during those hook calls. " +
				$"This module is {(module.EnabledByDefault ? "enabled" : "disabled")} by default. " +
				$"This module has <b>{consoleCommandCount:n0}</b> console and <b>{chatCommandCount:n0}</b> chat {(consoleCommandCount == 1 && chatCommandCount == 1 ? "command" : "commands")} and will{(!module.ForceModded ? " <b>not</b>" : "")} enforce this server to modded when enabled.{((consoleCommandCount + chatCommandCount) == 0 ? "" : "\n\n")}" +
				((consoleCommandCount > 0 ? $"<b>Console commands:</b> {consoleCommands.Select(x => $"{x.Name}").ToArray().ToString(", ")}\n" : "") +
				(chatCommandCount > 0 ? $"<b>Chat commands:</b> {chatCommands.Select(x => $"{x.Name}").ToArray().ToString(", ")}\n" : "") +
				$"\n\n{(string.IsNullOrEmpty(content) ? "" : Header("About", 1))}" +
				$"\n{content}");

			InfoTemplate(cui, tab, container, panel, player, title, content, hint);

			cui.CreateProtectedButton(container, panel, null, "0.3 0.3 0.3 0.5", "1 1 1 1", "OPEN FOLDER".SpacedString(1), 10,
				xMin: 0.9f, yMin: 0.075f, yMax: 0.125f, OyMin: 60, OyMax: 60, OxMin: 8, OxMax: 8, command: $"wizard.openmodulefolder {module.Type.Name}");

			cui.CreateProtectedButton(container, panel, null, "0.3 0.3 0.3 0.5", "1 1 1 1", "EDIT CONFIG".SpacedString(1), 10,
				xMin: 0.9f, yMin: 0.075f, yMax: 0.125f, OyMin: 30, OyMax: 30, OxMin: 8, OxMax: 8, command: $"wizard.editmoduleconfig {module.Type.Name}");

			cui.CreateProtectedButton(container, panel, null, module.GetEnabled() ? "0.4 0.9 0.3 0.5" : "0.1 0.1 0.1 0.5", "1 1 1 1", module.GetEnabled() ? "ENABLED".SpacedString(1) : "DISABLED".SpacedString(1), 10,
				xMin: 0.9f, yMin: 0.075f, yMax: 0.125f, OxMin: 8, OxMax: 8, command: $"wizard.togglemodule {module.Type.Name}");
		}
		internal void InternalFeatureInfoTemplate(CUI cui, Tab tab, CuiElementContainer container, string panel, PlayerSession player, string title, string content, string hint, string feature)
		{
			InfoTemplate(cui, tab, container, panel, player, title, content, hint);

			var isEnabled = IsFeatureEnabled(feature);
			cui.CreateProtectedButton(container, panel, null, isEnabled ? "0.4 0.9 0.3 0.5" : "0.1 0.1 0.1 0.5", "1 1 1 1", isEnabled ? "ENABLED".SpacedString(1) : "DISABLED".SpacedString(1), 10,
				xMin: 0.9f, yMin: 0.075f, yMax: 0.125f, OxMin: 8, OxMax: 8, command: $"wizard.togglefeature {feature}");
		}
		internal void DisplayArrows(CUI cui, Tab tab, CuiElementContainer container, string panel, PlayerSession ap, bool centerNext = false)
		{
			var page = ap.GetStorage(tab, "page", 0);

			if (page < Pages.Count - 1)
			{
				var nextPage = Pages[page + 1];

				if (centerNext)
				{
					cui.CreateProtectedButton(container, panel, null, "#7d8f32", "1 1 1 1", $"{nextPage.Title} ▶", 9,
						xMin: 0.9f, yMin: 0f, yMax: 0.055f, OxMin: -395, OxMax: -395, OyMin: 145f, OyMax: 145f, command: $"wizard.changepage 1");
				}
				else
				{
					cui.CreateProtectedButton(container, panel, null, "#7d8f32", "1 1 1 1", $"{nextPage.Title} ▶", 9,
						xMin: 0.9f, yMin: 0f, yMax: 0.055f, OxMin: 8, OxMax: 8, command: $"wizard.changepage 1");
				}
			}

			if (page >= 1)
			{
				var backPage = Pages[page - 1];
				cui.CreateProtectedButton(container, panel, null, "#7d8f32", "1 1 1 1", $"◀ {backPage.Title}", 9,
					xMin: 0, xMax: 0.1f, yMin: 0f, yMax: 0.055f, OxMin: -9, OxMax: -9, command: $"wizard.changepage -1");
			}
		}

		internal static string Header(string value, int level)
		{
			switch (level)
			{
				case 1: return $"<size=20>{value}</size>";
				case 2: return $"<size=17>{value}</size>";
				case 3: return $"<size=14>{value}</size>";
				default:
					break;
			}

			return value;
		}

		internal bool IsFeatureEnabled(string feature)
		{
			switch (feature)
			{
				case "plugins":
					return !Singleton.ConfigInstance.DisablePluginsTab;
			}

			return false;
		}

		public class Page
		{
			public string Title;
			public Action<CUI, Tab, CuiElementContainer, string, PlayerSession> Draw;

			public Page() { }
			public Page(string title, Action<CUI, Tab, CuiElementContainer, string, PlayerSession> draw)
			{
				Title = title;
				Draw = draw;
			}
		}
	}

	#region Setup Wizard - Custom Commands

	[ProtectedCommand("wizard.changepage")]
	private void ChangePage(Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());
		var tab = GetTab(ap.Player);

		var currentPage = ap.GetStorage(tab, "page", 0);
		currentPage += arg.Args[0].ToInt();
		ap.SetStorage(tab, "page", currentPage);

		Community.Runtime.CorePlugin.NextFrame(() => Draw(ap.Player));
	}

	[ProtectedCommand("wizard.togglemodule")]
	private void ToggleModule(Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());
		var tab = GetTab(ap.Player) as SetupWizard;

		var module = FindModule(arg.Args[0]);
		var enabled = module.GetEnabled();
		module.SetEnabled(!enabled);

		Draw(ap.Player);
	}

	[ProtectedCommand("wizard.togglefeature")]
	private void ToggleFeature(Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());

		var feature = arg.Args[0];
		switch (feature)
		{
			case "plugins":
				ConfigInstance.DisablePluginsTab = !ConfigInstance.DisablePluginsTab;
				break;
		}

		Draw(ap.Player);
	}

	[ProtectedCommand("wizard.editmoduleconfig")]
	private void EditModuleConfig(Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());
		var tab = GetTab(ap.Player) as SetupWizard;

		var module = FindModule(arg.Args[0]);
		var moduleConfigFile = Path.Combine(Core.Defines.GetModulesFolder(), module.Name, "config.json");
		ap.SelectedTab = ConfigEditor.Make(OsEx.File.ReadText(moduleConfigFile),
			(ap, jobject) =>
			{
				SetTab(ap.Player, SetupWizard.Make());
				Draw(ap.Player);
			},
			(ap, jobject) =>
			{
				OsEx.File.Create(moduleConfigFile, jobject.ToString(Formatting.Indented));
				module.Load();
				SetTab(ap.Player, SetupWizard.Make());
				Draw(ap.Player);
			}, null);

		Draw(ap.Player);
	}

	[ProtectedCommand("wizard.openmodulefolder")]
	private void OpenModuleFolder(Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());
		var tab = GetTab(ap.Player) as SetupWizard;

		var module = FindModule(arg.Args[0]);
		Application.OpenURL(Path.Combine(Core.Defines.GetModulesFolder(), module.Name));

		Draw(ap.Player);
	}

	#endregion

	#endregion
}

public class AdminConfig
{
	[JsonProperty("OpenCommands")]
	public string[] OpenCommands = new string[] { "cp", "cpanel" };
	public int MinimumAuthLevel = 2;
	public bool DisableEntitiesTab = false;
	public bool DisablePluginsTab = true;
}
public class AdminData
{
	[JsonProperty("ShowedWizard v3")]
	public bool ShowedWizard = false;
	public DataColors Colors = new();

	public class DataColors
	{
		public string SelectedTabColor = "0.4 0.7 0.2";
		public string EditableInputHighlight = "0.259 0.529 0.961";
		public float OptionNameOpacity = 0.7f;
		public float TitleUnderlineOpacity = 0.9f;

	}
}
