using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using API.Commands;
using Carbon.Base;
using Carbon.Components;
using Carbon.Core;
using Carbon.Extensions;
using Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Game.Rust.Cui;
using Oxide.Plugins;
using ProtoBuf;
using UnityEngine;
using static Carbon.Components.CUI;
using static ConsoleSystem;
using Color = UnityEngine.Color;
using Pool = Facepunch.Pool;
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

	public readonly CUI.Handler Handler = new();

	internal const float OptionWidth = 0.475f;
	internal const float TooltipOffset = 15;
	internal const int RangeCuts = 50;

	internal List<Tab> Tabs = new();
	internal Dictionary<BasePlayer, PlayerSession> AdminPlayers = new();
	internal ImageDatabaseModule ImageDatabase;

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
	internal bool HandleEnableNeedsKeyboard(PlayerSession ap)
	{
		return ap.SelectedTab == null || ap.SelectedTab.Dialog == null;
	}
	internal bool HandleEnableNeedsKeyboard(BasePlayer player)
	{
		return HandleEnableNeedsKeyboard(GetPlayerSession(player));
	}

	public override void OnServerInit()
	{
		base.OnServerInit();

		ImageDatabase = GetModule<ImageDatabaseModule>();

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
	}
	public override void OnPostServerInit()
	{
		base.OnPostServerInit();

		GenerateTabs();
	}

	public override void OnEnabled(bool initialized)
	{
		base.OnEnabled(initialized);

		if (initialized)
		{
			Application.logMessageReceived += OnLog;
		}

		foreach (var command in ConfigInstance.OpenCommands)
		{
			Community.Runtime.CorePlugin.cmd.AddChatCommand(command, this, (player, cmd, args) =>
			{
				if (!CanAccess(player)) return;

				var ap = GetPlayerSession(player);
				ap.SelectedTab = Tabs.FirstOrDefault(x => HasAccessLevel(player, x.AccessLevel));

				var tab = GetTab(player);
				tab?.OnChange?.Invoke(ap, tab);

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

			Application.logMessageReceived -= OnLog;
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
				["autoupdate"] = "Auto Update",
				["autoupdate_help"] = "Automatically update the 'Carbon.Hooks.Extra' file on boot. Recommended to be enabled.",
				["general"] = "General",
				["hooktimetracker"] = "Hook Time Tracker",
				["hooktimetracker_help"] = "Tracks the time taken for hooks to be executed.",
				["hookvalidation"] = "Hook Validation",
				["hookvalidation_help"] = "Probably obsolete, but when enabled, it prints a list of hooks that are compatible in Oxide, but not Carbon.",
				["entmapbuffersize"] = "Entity Map Buffer Size (restart required)",
				["entmapbuffersize_help"] = "Only change if you're aware what this is used for. Developers-related option.",
				["watchers"] = "Watchers",
				["scriptwatchers"] = "Script Watchers",
				["scriptwatchers_help"] = "When disabled, you must load/unload plugins manually with 'c.load' or 'c.unload'.",
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
				["permmode"] = "Permission Mode"
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
		if (accessLevel == 0 || player.IsAdmin) return true;

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
		if (!ap.GetStorage<bool>(tab, "wasviewingcam", false)) return;

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
		if (HookCaller.CallStaticHook("CanAccessAdminModule", player) is bool result)
		{
			return result;
		}

		var minLevel = ConfigInstance.MinimumAuthLevel;
		var userLevel = ServerUsers.Is(player.userID, ServerUsers.UserGroup.Moderator) ? 1 : ServerUsers.Is(player.userID, ServerUsers.UserGroup.Owner) ? 2 : 0;

		if (userLevel < minLevel && userLevel > 0)
		{
			player.ChatMessage($"Your auth level is not high enough to use this feature. Please adjust the minimum level required in your config or give yourself auth level {minLevel}.");
		}

		return userLevel >= minLevel;
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
	public void TabPanelText(CUI cui, CuiElementContainer container, string parent, string text, int size, string color, float height, float offset, TextAnchor align, CUI.Handler.FontTypes font, bool isInput)
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

		cui.CreateProtectedButton(container, parent: parent, id: $"{parent}btn",
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

		if (ap.SelectedTab != previous) Draw(player);
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

			using (TimeMeasure.New($"{Name}.Main"))
			{
				cui.CreatePanel(container, parent: PanelId, id: $"{PanelId}color",
					color: "0 0 0 0.6",
					xMin: 0.15f, xMax: 0.85f, yMin: 0.1f, yMax: 0.9f);
				cui.CreatePanel(container, $"{PanelId}color", $"{PanelId}main",
					color: "0 0 0 0.5",
					blur: true);

				if (tab == null || !tab.Fullscreen)
				{
					#region Title

					cui.CreateText(container, parent: $"{PanelId}main", id: null,
						color: "1 1 1 0.8",
						text: "<b>Admin Settings</b>", 18,
						xMin: 0.0175f, yMin: 0.8f, xMax: 1f, yMax: 0.97f,
						align: TextAnchor.UpperLeft,
						font: Handler.FontTypes.RobotoCondensedBold);

					#endregion

					#region Tabs
					try
					{
						cui.CreatePanel(container, parent: $"{PanelId}main", id: "tab_buttons",
							color: "0 0 0 0.6",
							xMin: 0.01f, xMax: 0.99f, yMin: 0.875f, yMax: 0.92f);

						TabButton(cui, container, "tab_buttons", "<", PanelId + ".changetab down", 0.03f, 0);
						TabButton(cui, container, "tab_buttons", ">", PanelId + ".changetab up", 0.03f, 0.97f);

						var tabIndex = 0.03f;
						var amount = Tabs.Count;
						var tabWidth = amount == 0 ? 0f : 0.94f / amount;

						for (int i = ap.TabSkip; i < amount; i++)
						{
							var _tab = Tabs[ap.TabSkip + i];
							var plugin = _tab.Plugin.IsCorePlugin ? string.Empty : $"<size=8>\nby {_tab.Plugin?.Name}</size>";
							TabButton(cui, container, "tab_buttons", $"{(Tabs.IndexOf(ap.SelectedTab) == i ? $"<b>{_tab.Name}</b>" : _tab.Name)}{plugin}", PanelId + $".changetab {i}", tabWidth, tabIndex, Tabs.IndexOf(ap.SelectedTab) == i, !HasAccessLevel(player, _tab.AccessLevel));
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
					var panels = cui.CreatePanel(container, $"{PanelId}main", "panels",
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
								var panel = $"sub{i}";
								var rows = tab.Columns[i];

								cui.CreatePanel(container, "panels", panel,
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

								if (columnPage.TotalPages > 0)
								{
									TabColumnPagination(cui, container, panel, i, columnPage, rowHeight, rowIndex);

									rowIndex += rowHeight + rowSpacing;
								}

								for (int r = rowPageCount; r-- > 0;)
								{
									var actualI = r + (columnPage.CurrentPage * contentsPerPage);
									var row = rows.ElementAt(actualI);

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
			needsCursor: true);

		cui.Destroy(CursorPanelId, player);
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

		AdminPlayers.Clear();
	}
	public void UnregisterTab(string id)
	{
		AdminPlayers.Clear();

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
					ColorPicker.Draw(player, (rustColor, hexColor) => { color.Callback?.Invoke(ap, rustColor, hexColor); });
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
		public T GetStorage<T>(Tab tab, string id, object @default = null)
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

			return GetStorage<T>(tab, id, value);
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
				Pool.Free(ref value);
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
		public Tab AddColumn(int column)
		{
			if (!Columns.TryGetValue(column, out var options))
			{
				Columns[column] = new List<Option>();
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
		public Tab AddText(int column, string name, int size, string color, TextAnchor align = TextAnchor.MiddleCenter, CUI.Handler.FontTypes font = Handler.FontTypes.RobotoCondensedRegular, bool isInput = false)
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
		public Tab AddColor(int column, string name, Func<string> color, Action<PlayerSession, string, string> callback, string tooltip = null)
		{
			return AddRow(column, new OptionColor(name, color, callback, tooltip));
		}

		public void CreateDialog(string title, Action<PlayerSession> onConfirm, Action<PlayerSession> onDecline)
		{
			Dialog = new TabDialog(title, onConfirm, onDecline);
		}
		public void CreateModal(BasePlayer player, string title, Dictionary<string, Modal.Field> fields, Action<BasePlayer, Modal> onConfirm = null, Action onCancel = null)
		{
			Modal.Open(player, title, fields, onConfirm, onCancel);
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
			public CUI.Handler.FontTypes Font;
			public bool IsInput;

			public OptionText(string name, int size, string color, TextAnchor align, CUI.Handler.FontTypes font, bool isInput, string tooltip = null) : base(name, tooltip) { Align = align; Size = size; Color = color; Font = font; IsInput = isInput; }
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
			public Action<PlayerSession, string, string> Callback;

			public OptionColor(string name, Func<string> color, Action<PlayerSession, string, string> callback, string tooltip = null) : base(name, tooltip)
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

	public class CarbonTab
	{
		public static Core.Config Config => Community.Runtime.Config;

		internal static string[] LogFileModes = new string[]
		{
			"Disabled",
			"Save every 5 min.",
			"Save immediately"
		};
		internal static string[] LogVerbosity = new string[]
		{
			"Normal",
			"Level 1",
			"Level 2",
			"Level 3",
			"Level 4",
			"Level 5",
			"Level 6"
		};

		public static Tab Get()
		{
			var tab = new Tab("carbon", "Carbon", Community.Runtime.CorePlugin, (ap, t) => { Refresh(t, ap); }, 2);
			tab.AddColumn(0);
			tab.AddColumn(1);

			return tab;
		}

		public static void Refresh(Tab tab, PlayerSession ap)
		{
			tab.ClearColumn(0);
			tab.ClearColumn(1);

			if (Singleton.HasAccessLevel(ap.Player, 2))
			{
				tab.AddInput(0, Singleton.GetPhrase("hostname", ap.Player.UserIDString), ap => $"{ConVar.Server.hostname}", Singleton.HasAccessLevel(ap.Player, 3) ? (ap2, args) => { ConVar.Server.hostname = args.ToString(" "); } : null);
				tab.AddInput(0, Singleton.GetPhrase("level", ap.Player.UserIDString), ap => $"{ConVar.Server.level}", null);

				tab.AddName(0, Singleton.GetPhrase("info", ap.Player.UserIDString), TextAnchor.MiddleLeft);
				{
					tab.AddInput(0, Singleton.GetPhrase("version", ap.Player.UserIDString), ap => $"{Community.Runtime.Analytics.Version}", null);
					tab.AddInput(0, Singleton.GetPhrase("version2", ap.Player.UserIDString), ap => $"{Community.Runtime.Analytics.InformationalVersion}", null);

					var loadedHooks = Community.Runtime.HookManager.LoadedDynamicHooks.Count(x => x.IsInstalled) + Community.Runtime.HookManager.LoadedStaticHooks.Count(x => x.IsInstalled);
					var totalHooks = Community.Runtime.HookManager.LoadedDynamicHooks.Count() + Community.Runtime.HookManager.LoadedStaticHooks.Count();
					tab.AddInput(0, Singleton.GetPhrase("hooks", ap.Player.UserIDString), ap => $"<b>{loadedHooks:n0}</b> / {totalHooks:n0} loaded", null);
					tab.AddInput(0, Singleton.GetPhrase("statichooks", ap.Player.UserIDString), ap => $"{Community.Runtime.HookManager.LoadedStaticHooks.Count():n0}", null);
					tab.AddInput(0, Singleton.GetPhrase("dynamichooks", ap.Player.UserIDString), ap => $"{Community.Runtime.HookManager.LoadedDynamicHooks.Count():n0}", null);

					tab.AddName(0, Singleton.GetPhrase("plugins", ap.Player.UserIDString), TextAnchor.MiddleLeft);
					tab.AddInput(0, Singleton.GetPhrase("mods", ap.Player.UserIDString), ap => $"{Community.Runtime.Plugins.Plugins.Count:n0}", null);

					if (Singleton.HasAccessLevel(ap.Player, 3))
					{
						tab.AddName(0, Singleton.GetPhrase("console", ap.Player.UserIDString), TextAnchor.MiddleLeft);
						foreach (var log in _logQueue)
						{
							tab.AddText(0, log, 10, "1 1 1 0.85", TextAnchor.MiddleLeft, Handler.FontTypes.RobotoCondensedRegular, isInput: true);
						}
						tab.AddInputButton(0, Singleton.GetPhrase("execservercmd", ap.Player.UserIDString), 0.2f, new Tab.OptionInput(null, null, 0, false, (ap, args) =>
						{
							Run(Option.Server, args.ToString(" "), null);
							Refresh(tab, ap);
						}), new Tab.OptionButton("Refresh", ap =>
						{
							Refresh(tab, ap);
						}));
					}
				}
			}

			if (Singleton.HasAccessLevel(ap.Player, 3))
			{
				tab.AddName(1, Singleton.GetPhrase("config", ap.Player.UserIDString), TextAnchor.MiddleLeft);
				{
					tab.AddToggle(1, Singleton.GetPhrase("ismodded", ap.Player.UserIDString), ap => { Config.IsModded = !Config.IsModded; Community.Runtime.SaveConfig(); }, ap => Config.IsModded, Singleton.GetPhrase("ismodded_help", ap.Player.UserIDString));
					tab.AddToggle(1, Singleton.GetPhrase("autoupdate", ap.Player.UserIDString), ap => { Config.AutoUpdate = !Config.AutoUpdate; Community.Runtime.SaveConfig(); }, ap => Config.AutoUpdate, Singleton.GetPhrase("autoupdate_help", ap.Player.UserIDString));

					tab.AddName(1, Singleton.GetPhrase("general", ap.Player.UserIDString), TextAnchor.MiddleLeft);
					tab.AddToggle(1, Singleton.GetPhrase("hooktimetracker", ap.Player.UserIDString), ap => { Config.HookTimeTracker = !Config.HookTimeTracker; Community.Runtime.SaveConfig(); }, ap => Config.HookTimeTracker, Singleton.GetPhrase("hooktimetracker_help", ap.Player.UserIDString));
					tab.AddToggle(1, Singleton.GetPhrase("hookvalidation", ap.Player.UserIDString), ap => { Config.HookValidation = !Config.HookValidation; Community.Runtime.SaveConfig(); }, ap => Config.HookValidation, Singleton.GetPhrase("hookvalidation_help", ap.Player.UserIDString));
					tab.AddInput(1, Singleton.GetPhrase("entmapbuffersize", ap.Player.UserIDString), ap => Config.EntityMapBufferSize.ToString(), (ap, args) => { Config.EntityMapBufferSize = args[0].ToInt().Clamp(10000, 500000); Community.Runtime.SaveConfig(); }, Singleton.GetPhrase("entmapbuffersize_help", ap.Player.UserIDString));

					tab.AddName(1, Singleton.GetPhrase("watchers", ap.Player.UserIDString), TextAnchor.MiddleLeft);
					tab.AddToggle(1, Singleton.GetPhrase("scriptwatchers", ap.Player.UserIDString), ap => { Config.ScriptWatchers = !Config.ScriptWatchers; Community.Runtime.SaveConfig(); }, ap => Config.ScriptWatchers, Singleton.GetPhrase("scriptwatchers_help", ap.Player.UserIDString));
					tab.AddToggle(1, Singleton.GetPhrase("harmonyreference", ap.Player.UserIDString), ap => { Config.HarmonyReference = !Config.HarmonyReference; Community.Runtime.SaveConfig(); }, ap => Config.HarmonyReference, Singleton.GetPhrase("harmonyreference_help", ap.Player.UserIDString));
					tab.AddToggle(1, Singleton.GetPhrase("filenamecheck", ap.Player.UserIDString), ap => { Config.FileNameCheck = !Config.FileNameCheck; Community.Runtime.SaveConfig(); }, ap => Config.FileNameCheck, Singleton.GetPhrase("filenamecheck_help", ap.Player.UserIDString));

					tab.AddName(1, Singleton.GetPhrase("logging", ap.Player.UserIDString), TextAnchor.MiddleLeft);
					tab.AddDropdown(1, Singleton.GetPhrase("logfilemode", ap.Player.UserIDString), ap => Config.LogFileMode, (ap, index) => { Config.LogFileMode = index; Community.Runtime.SaveConfig(); }, LogFileModes);
					tab.AddDropdown(1, Singleton.GetPhrase("logverbosity", ap.Player.UserIDString), ap => Config.LogVerbosity, (ap, index) => { Config.LogVerbosity = index; Community.Runtime.SaveConfig(); }, LogVerbosity);
					tab.AddDropdown(1, Singleton.GetPhrase("logseverity", ap.Player.UserIDString), ap => (int)Config.LogSeverity, (ap, index) => { Config.LogSeverity = (API.Logger.Severity)index; Community.Runtime.SaveConfig(); }, Enum.GetNames(typeof(API.Logger.Severity)));

					tab.AddName(1, Singleton.GetPhrase("misc", ap.Player.UserIDString), TextAnchor.MiddleLeft);
					tab.AddInput(1, Singleton.GetPhrase("serverlang", ap.Player.UserIDString), ap => Config.Language, (ap, args) => { Config.Language = args[0]; Community.Runtime.SaveConfig(); });
					tab.AddInput(1, Singleton.GetPhrase("webreqip", ap.Player.UserIDString), ap => Config.WebRequestIp, (ap, args) => { Config.WebRequestIp = args[0]; Community.Runtime.SaveConfig(); });
					tab.AddEnum(1, Singleton.GetPhrase("permmode", ap.Player.UserIDString), (ap, back) => { var e = Enum.GetNames(typeof(Permission.SerializationMode)); Config.PermissionSerialization += back ? -1 : 1; if (Config.PermissionSerialization < 0) Config.PermissionSerialization = Permission.SerializationMode.SQL; else if (Config.PermissionSerialization > Permission.SerializationMode.SQL) Config.PermissionSerialization = Permission.SerializationMode.Protobuf; Community.Runtime.SaveConfig(); }, ap => Config.PermissionSerialization.ToString());
				}
			}
		}
	}
	public class PlayersTab
	{
		internal static RustPlugin Core = Community.Runtime.CorePlugin;
		internal static List<BasePlayer> BlindedPlayers = new();

		public static Tab Get()
		{
			var players = new Tab("players", "Players", Community.Runtime.CorePlugin, (instance, tab) =>
			{
				tab.ClearColumn(1);
				RefreshPlayers(tab, instance);
			}, 1);

			players.AddColumn(0);
			players.AddColumn(1);

			return players;
		}

		public static void RefreshPlayers(Tab tab, PlayerSession ap)
		{
			tab.ClearColumn(0);

			if (Singleton.HasAccessLevel(ap.Player, 1))
			{
				tab.AddInput(0, "Search", ap => ap?.GetStorage<string>(tab, "playerfilter"), (ap2, args) => { ap2.SetStorage(tab, "playerfilter", args.ToString(" ")); RefreshPlayers(tab, ap2); });

				tab.AddName(0, "Online");
				var onlinePlayers = BasePlayer.allPlayerList.Where(x => x.userID.IsSteamId() && x.IsConnected);
				foreach (var player in onlinePlayers)
				{
					AddPlayer(tab, ap, player);
				}
				if (onlinePlayers.Count() == 0) tab.AddText(0, "No online players found.", 10, "1 1 1 0.4");

				tab.AddName(0, "Offline");
				var offlinePlayers = BasePlayer.allPlayerList.Where(x => x.userID.IsSteamId() && !x.IsConnected);
				foreach (var player in offlinePlayers)
				{
					AddPlayer(tab, ap, player);
				}
				if (offlinePlayers.Count() == 0) tab.AddText(0, "No offline players found.", 10, "1 1 1 0.4");
			}
		}
		public static void AddPlayer(Tab tab, PlayerSession ap, BasePlayer player)
		{
			if (ap != null)
			{
				var filter = ap.GetStorage<string>(tab, "playerfilter");

				if (!string.IsNullOrEmpty(filter) && !(player.displayName.ToLower().Contains(filter.ToLower()) || player.UserIDString.Contains(filter))) return;
			}

			tab.AddButton(0, $"{player.displayName}", aap =>
			{
				ap.SetStorage(tab, "playerfilterpl", player);
				ShowInfo(tab, ap, player);
			}, aap => aap == null || !(aap.GetStorage<BasePlayer>(tab, "playerfilterpl", null) == player) ? Tab.OptionButton.Types.None : Tab.OptionButton.Types.Selected);
		}
		public static void ShowInfo(Tab tab, PlayerSession aap, BasePlayer player)
		{
			tab.ClearColumn(1);

			tab.AddName(1, $"Player Information", TextAnchor.MiddleLeft);
			tab.AddInput(1, "Name", ap => player.displayName, null);
			tab.AddInput(1, "Steam ID", ap => player.UserIDString, null);
			tab.AddInput(1, "Net ID", ap => $"{player.net?.ID}", null);
			try
			{
				var position = player.transform.position;
				tab.AddInput(1, "Position", ap => $"{player.transform.position}", null);
				tab.AddInput(1, "Rotation", ap => $"{player.transform.rotation}", null);
			}
			catch { }

			if (Singleton.HasAccessLevel(aap.Player, 3))
			{
				tab.AddName(1, $"Permissions", TextAnchor.MiddleLeft);
				{
					tab.AddButton(1, "View Permissions", ap =>
					{
						var perms = Singleton.FindTab("permissions");
						var permission = Community.Runtime.CorePlugin.permission;
						Singleton.SetTab(ap.Player, "permissions");

						ap.SetStorage(tab, "player", player);
						PermissionsTab.GeneratePlayers(perms, permission, ap);
						PermissionsTab.GeneratePlugins(perms, ap, permission, ap.Player, null);
					}, (ap) => Singleton.HasAccessLevel(player, 3) ? Tab.OptionButton.Types.None : Tab.OptionButton.Types.Important);
				}
			}

			if (aap != null)
			{
				tab.AddName(1, $"Actions", TextAnchor.MiddleLeft);

				tab.AddButtonArray(1,
					new Tab.OptionButton("TeleportTo", ap => { ap.Player.Teleport(player.transform.position); }),
					new Tab.OptionButton("Teleport2Me", ap =>
					{
						tab.CreateDialog($"Are you sure about that?", ap =>
						{
							player.transform.position = ap.Player.transform.position;
							player.SendNetworkUpdateImmediate();
						}, null);
					}));

				tab.AddButtonArray(1,
					new Tab.OptionButton("Loot", ap =>
					{
						EntitiesTab.LastContainerLooter = ap;
						ap.SetStorage(tab, "lootedent", player);
						EntitiesTab.SendEntityToPlayer(ap.Player, player);

						Core.timer.In(0.2f, () => Singleton.Close(ap.Player));
						Core.timer.In(0.5f, () =>
						{
							EntitiesTab.SendEntityToPlayer(ap.Player, player);

							ap.Player.inventory.loot.Clear();
							ap.Player.inventory.loot.PositionChecks = false;
							ap.Player.inventory.loot.entitySource = RelationshipManager.ServerInstance;
							ap.Player.inventory.loot.itemSource = null;
							ap.Player.inventory.loot.AddContainer(player.inventory.containerMain);
							ap.Player.inventory.loot.AddContainer(player.inventory.containerWear);
							ap.Player.inventory.loot.AddContainer(player.inventory.containerBelt);
							ap.Player.inventory.loot.MarkDirty();
							ap.Player.inventory.loot.SendImmediate();

							ap.Player.ClientRPCPlayer(null, ap.Player, "RPC_OpenLootPanel", "player_corpse");
						});
					}),
					new Tab.OptionButton("Respawn", ap =>
					{
						tab.CreateDialog($"Are you sure about that?", ap =>
						{
							player.Hurt(player.MaxHealth());
							player.Respawn();
							player.EndSleeping();
						}, null);
					}));

				if (Singleton.HasTab("entities"))
				{
					tab.AddButton(1, "Select Entity", ap2 =>
					{
						Singleton.SetTab(ap2.Player, "entities");
						var tab = Singleton.GetTab(ap2.Player);
						EntitiesTab.SelectEntity(tab, ap2, player);
						EntitiesTab.DrawEntities(tab, ap2);
						EntitiesTab.DrawEntitySettings(tab, 1, ap2);
					});
				}

				tab.AddInput(1, "PM", null, (ap, args) => { player.ChatMessage($"[{ap.Player.displayName}]: {args.ToString(" ")}"); });
				if (aap.Player != player && aap.Player.spectateFilter != player.UserIDString)
				{
					tab.AddButton(1, "Spectate", ap =>
					{
						StartSpectating(ap.Player, player);
						ShowInfo(tab, ap, player);
					});
				}
				if (!string.IsNullOrEmpty(aap.Player.spectateFilter) && (aap.Player.UserIDString == player.UserIDString || aap.Player.spectateFilter == player.UserIDString))
				{
					tab.AddButton(1, "End Spectating", ap =>
					{
						StopSpectating(ap.Player);
						ShowInfo(tab, ap, player);
					}, ap => Tab.OptionButton.Types.Selected);
				}
				if (!BlindedPlayers.Contains(player))
				{
					tab.AddButton(1, "Blind Player", ap =>
					{
						tab.CreateDialog("Are you sure you want to blind the player?", ap =>
						{
							using var cui = new CUI(Singleton.Handler);
							var container = cui.CreateContainer("blindingpanel", "0 0 0 1", needsCursor: true, needsKeyboard: Singleton.HandleEnableNeedsKeyboard(ap));
							cui.CreateClientImage(container, "blindingpanel", null, "https://carbonmod.gg/assets/media/cui/bsod.png", "1 1 1 1");
							cui.Send(container, player);
							BlindedPlayers.Add(player);
							ShowInfo(tab, ap, player);

							if (ap.Player == player) Core.timer.In(1, () => { Singleton.Close(player); });
						}, null);
					});
				}
				else
				{
					tab.AddButton(1, "Unblind Player", ap =>
					{
						using var cui = new CUI(Singleton.Handler);
						cui.Destroy("blindingpanel", player);
						BlindedPlayers.Remove(player);
						ShowInfo(tab, ap, player);
					}, ap => Tab.OptionButton.Types.Selected);
				}

				tab.AddName(1, "Stats");
				tab.AddName(1, "Combat", TextAnchor.MiddleLeft);
				tab.AddRange(1, "Health", 0, player.MaxHealth(), ap => player.health, (ap, value) => player.SetHealth(value), ap => $"{player.health:0}");

				tab.AddRange(1, "Thirst", 0, player.metabolism.hydration.max, ap => player.metabolism.hydration.value, (ap, value) => player.metabolism.hydration.SetValue(value), ap => $"{player.metabolism.hydration.value:0}");
				tab.AddRange(1, "Hunger", 0, player.metabolism.calories.max, ap => player.metabolism.calories.value, (ap, value) => player.metabolism.calories.SetValue(value), ap => $"{player.metabolism.calories.value:0}");
				tab.AddRange(1, "Radiation", 0, player.metabolism.radiation_poison.max, ap => player.metabolism.radiation_poison.value, (ap, value) => player.metabolism.radiation_poison.SetValue(value), ap => $"{player.metabolism.radiation_poison.value:0}");
				tab.AddRange(1, "Bleeding", 0, player.metabolism.bleeding.max, ap => player.metabolism.bleeding.value, (ap, value) => player.metabolism.bleeding.SetValue(value), ap => $"{player.metabolism.bleeding.value:0}");
			}
		}
	}
	public class PermissionsTab
	{
		internal static Permission permission;

		public static Tab Get()
		{
			permission = Community.Runtime.CorePlugin.permission;

			var tab = new Tab("permissions", "Permissions", Community.Runtime.CorePlugin, (ap, tab) =>
			{
				tab.ClearColumn(1);
				tab.ClearColumn(2);
				tab.ClearColumn(3);
				GeneratePlayers(tab, permission, ap);
			}, 3);

			tab.AddName(0, "Options", TextAnchor.MiddleLeft);

			tab.AddButton(0, "Players", ap =>
			{
				ap.SetStorage(tab, "groupedit", false);

				tab.ClearColumn(1);
				tab.ClearColumn(2);
				tab.ClearColumn(3);
				ap.Clear();

				ap.SetStorage(tab, "option", 0);

				GeneratePlayers(tab, permission, ap);
			}, type: (ap) => ap.GetStorage<int>(tab, "option", 0) == 0 ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None);

			GeneratePlayers(tab, permission, PlayerSession.Blank);

			tab.AddButton(0, "Groups", ap =>
			{
				ap.SetStorage(tab, "groupedit", false);

				tab.ClearColumn(1);
				tab.ClearColumn(2);
				tab.ClearColumn(3);
				ap.Clear();

				ap.ClearStorage(tab, "player");
				ap.ClearStorage(tab, "plugin");

				ap.SetStorage(tab, "option", 1);
				GenerateGroups(tab, permission, ap);
			}, type: (ap) => ap.GetStorage<int>(tab, "option", 0) == 1 ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None);
			tab.AddColumn(1);
			tab.AddColumn(2);
			tab.AddColumn(3);

			return tab;
		}

		public static void GeneratePlayers(Tab tab, Permission perms, PlayerSession ap)
		{
			var filter = ap.GetStorage<string>(tab, "playerfilter", string.Empty)?.Trim().ToLower();
			var players = BasePlayer.allPlayerList.Where(x =>
			{
				if (!x.userID.IsSteamId()) return false;

				if (!string.IsNullOrEmpty(filter))
				{
					return x.displayName.ToLower().Contains(filter) || x.UserIDString.Contains(filter);
				}

				return true;
			});

			tab.ClearColumn(1);
			tab.AddName(1, "Players", TextAnchor.MiddleLeft);
			{
				tab.AddInput(1, "Search", ap => ap.GetStorage<string>(tab, "playerfilter", string.Empty), (ap, args) =>
				{
					ap.SetStorage(tab, "playerfilter", args.ToString(" "));
					GeneratePlayers(tab, perms, ap);
				});

				foreach (var player in players)
				{
					tab.AddRow(1, new Tab.OptionButton($"{player.displayName} ({player.userID})", instance2 =>
					{
						ap.SetStorage(tab, "player", player);

						ap.ClearStorage(tab, "plugin");

						tab.ClearColumn(3);

						GeneratePlugins(tab, ap, perms, player, null);
					}, type: (_instance) => ap.GetStorage<BasePlayer>(tab, "player", null) == player ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
				}
			}
		}
		public static void GeneratePlugins(Tab tab, PlayerSession ap, Permission permission, BasePlayer player, string selectedGroup)
		{
			var groupEdit = ap.GetStorage<bool>(tab, "groupedit");
			var filter = ap.GetStorage<string>(tab, "pluginfilter", string.Empty)?.Trim().ToLower();
			var plugins = ModLoader.LoadedPackages.SelectMany(x => x.Plugins).Where(x =>
			{
				if (!string.IsNullOrEmpty(filter))
				{
					return x.IsCorePlugin || x.permission.GetPermissions().Any(y => y.StartsWith(x.Name.ToLower()) && x.Name.Trim().ToLower().Contains(filter));
				}

				return x.IsCorePlugin || x.permission.GetPermissions().Any(y => y.StartsWith(x.Name.ToLower()));
			});

			tab.ClearColumn(2);
			if (string.IsNullOrEmpty(selectedGroup))
			{
				tab.AddName(2, $"{player.displayName}");
				tab.AddButtonArray(2,
					new Tab.OptionButton("Select Player", (ap2) =>
				{
					Singleton.SetTab(ap.Player, "players");
					var tab = Singleton.GetTab(ap.Player);
					ap.SetStorage(tab, "playerfilterpl", player);
					PlayersTab.RefreshPlayers(tab, ap);
					PlayersTab.ShowInfo(tab, ap, player);
				}, ap => Tab.OptionButton.Types.Warned),
					new Tab.OptionButton(groupEdit ? "Edit Plugins" : "Edit Groups", (ap2) =>
				{
					ap.SetStorage(tab, "groupedit", !groupEdit);
					GeneratePlugins(tab, ap, permission, player, null);
				}));
			}
			else
			{
				tab.AddName(2, $"{selectedGroup}");
				tab.AddButtonArray(2, new Tab.OptionButton("Delete", ap =>
				{
					tab.CreateDialog($"Are you sure you want to delete the '{selectedGroup}' group?", ap2 =>
					{
						permission.RemoveGroup(selectedGroup);

						tab.ClearColumn(1);
						tab.ClearColumn(2);
						tab.ClearColumn(3);
						GenerateGroups(tab, permission, ap);
					}, null);
				}, (ap) => Tab.OptionButton.Types.Important), new Tab.OptionButton("Edit", ap =>
				{
					var temp = Pool.GetList<string>();
					var groups = Community.Runtime.CorePlugin.permission.GetGroups();
					temp.Add("None");
					temp.AddRange(groups);
					temp.Remove(selectedGroup);

					var array = temp.ToArray();
					Pool.FreeList(ref temp);

					var parent = permission.GetGroupParent(selectedGroup);
					var parentIndex = Array.IndexOf(array, parent);
					tab.CreateModal(ap.Player, $"Editing '{selectedGroup}'", new Dictionary<string, Modal.Field>()
					{
						["name"] = Modal.Field.Make("Name", Modal.Field.FieldTypes.String, true, selectedGroup, true),
						["dname"] = Modal.Field.Make("Display Name", Modal.Field.FieldTypes.String, @default: permission.GetGroupTitle(selectedGroup)),
						["rank"] = Modal.Field.Make("Rank", Modal.Field.FieldTypes.Integer, @default: permission.GetGroupRank(selectedGroup)),
						["parent"] = Modal.EnumField.MakeEnum("Parent", array, @default: string.IsNullOrEmpty(parent) ? 0 : Array.IndexOf(array, parent), customIsInvalid: field => permission.GetGroupParent(array[field.Get<int>()]) == selectedGroup ? $"Circular parenting detected with '{array[field.Get<int>()]}'." : null)
					}, (ap2, modal) =>
					{
						var parentIndex = modal.Get<int>("parent");
						permission.SetGroupTitle(selectedGroup, modal.Get<string>("dname"));
						permission.SetGroupRank(selectedGroup, modal.Get<int>("rank"));
						if (parentIndex != 0) permission.SetGroupParent(selectedGroup, array[parentIndex]);
						else permission.SetGroupParent(selectedGroup, null);

						tab.ClearColumn(1);
						tab.ClearColumn(2);
						tab.ClearColumn(3);
						GenerateGroups(tab, permission, ap);
						GeneratePlugins(tab, ap, permission, ap.Player, selectedGroup);
					});
				}));
				tab.AddButton(2, "Duplicate Group", ap =>
				{
					var temp = Pool.GetList<string>();
					var groups = Community.Runtime.CorePlugin.permission.GetGroups();
					temp.Add("None");
					temp.AddRange(groups);

					var array = temp.ToArray();
					Pool.FreeList(ref temp);

					Modal.Open(player, "Duplicate Group", new Dictionary<string, Modal.Field>
					{
						["name"] = Modal.Field.Make("Name", Modal.Field.FieldTypes.String, true, customIsInvalid: (field) => permission.GetGroups().Any(x => x == field.Get<string>()) ? "Group with that name already exists." : null),
						["dname"] = Modal.Field.Make("Display Name", Modal.Field.FieldTypes.String, @default: string.Empty),
						["rank"] = Modal.Field.Make("Rank", Modal.Field.FieldTypes.Integer, @default: 0),
						["parent"] = Modal.EnumField.MakeEnum("Parent", array, @default: 0)
					}, onConfirm: (p, modal) =>
					{
						var name = modal.Get<string>("name");
						var parentIndex = modal.Get<int>("parent");
						permission.CreateGroup(name, modal.Get<string>("dname"), modal.Get<int>("rank"));
						if (parentIndex != 0) permission.SetGroupParent(modal.Get<string>("name"), array[parentIndex]);

						var perms = permission.GetGroupPermissions(selectedGroup);
						foreach (var perm in perms)
						{
							permission.GrantGroupPermission(name, perm, null);
						}

						tab.ClearColumn(1);
						tab.ClearColumn(2);
						tab.ClearColumn(3);
						GenerateGroups(tab, permission, ap);
					});
				}, ap => Tab.OptionButton.Types.None);
			}

			if (groupEdit)
			{
				tab.ClearColumn(3);

				tab.AddName(2, "Groups", TextAnchor.MiddleLeft);
				{
					tab.AddInput(2, "Search", ap => ap.GetStorage<string>(tab, "groupfilter", string.Empty), (ap, args) =>
					{
						ap.SetStorage(tab, "groupfilter", args.ToString(" "));
						GeneratePlugins(tab, ap, permission, player, selectedGroup);
					});

					var groupFilter = ap.GetStorage<string>(tab, "groupfilter");

					foreach (var group in permission.GetGroups())
					{
						if (!string.IsNullOrEmpty(groupFilter) && !group.Contains(groupFilter)) continue;

						tab.AddButton(2, $"{group}", ap =>
						{
							if (permission.UserHasGroup(player.UserIDString, group))
							{
								permission.RemoveUserGroup(player.UserIDString, group);
							}
							else permission.AddUserGroup(player.UserIDString, group);

							GeneratePlugins(tab, ap, permission, player, selectedGroup);
						}, type: (_instance) => permission.UserHasGroup(player.UserIDString, group) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None);
					}
				}
			}
			else
			{
				tab.AddName(2, "Plugins", TextAnchor.MiddleLeft);
				{
					tab.AddInput(2, "Search", ap => ap.GetStorage<string>(tab, "pluginfilter", string.Empty), (ap, args) =>
					{
						ap.SetStorage(tab, "pluginfilter", args.ToString(" "));
						GeneratePlugins(tab, ap, permission, player, selectedGroup);
					});

					foreach (var plugin in plugins)
					{
						tab.AddRow(2, new Tab.OptionButton($"{plugin.Name} ({plugin.Version})", instance3 =>
						{
							ap.SetStorage(tab, "plugin", plugin);
							ap.SetStorage(tab, "pluginr", instance3.LastPressedRow);
							ap.SetStorage(tab, "pluginc", instance3.LastPressedColumn);

							GeneratePermissions(tab, permission, plugin, player, selectedGroup);
						}, type: (_instance) => ap.GetStorage<RustPlugin>(tab, "plugin", null) == plugin ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
					}
				}
			}
		}
		public static void GeneratePermissions(Tab tab, Permission perms, RustPlugin plugin, BasePlayer player, string selectedGroup)
		{
			tab.ClearColumn(3);
			tab.AddName(3, "Permissions", TextAnchor.MiddleLeft);
			foreach (var perm in perms.GetPermissions(plugin))
			{
				if (string.IsNullOrEmpty(selectedGroup))
				{
					var isInherited = false;
					var list = "";

					foreach (var group in perms.GetUserGroups(player.UserIDString))
						if (perms.GroupHasPermission(group, perm))
						{
							isInherited = true;
							list += $"<b>{group}</b>, ";
						}

					tab.AddRow(3, new Tab.OptionButton($"{perm}", instance5 =>
					{
						if (perms.UserHasPermission(player.UserIDString, perm))
							perms.RevokeUserPermission(player.UserIDString, perm);
						else perms.GrantUserPermission(player.UserIDString, perm, plugin);
					}, type: (_instance) => isInherited ? Tab.OptionButton.Types.Important : perms.UserHasPermission(player.UserIDString, perm) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));

					if (isInherited)
					{
						tab.AddText(3, $"Inherited by the following groups: {list.TrimEnd(',', ' ')}", 8, "1 1 1 0.6", TextAnchor.UpperLeft, Handler.FontTypes.RobotoCondensedRegular);
					}
				}
				else
				{
					tab.AddRow(3, new Tab.OptionButton($"{perm}", instance5 =>
					{
						if (permission.GroupHasPermission(selectedGroup, perm))
							permission.RevokeGroupPermission(selectedGroup, perm);
						else permission.GrantGroupPermission(selectedGroup, perm, plugin);
					}, type: (_instance) => permission.GroupHasPermission(selectedGroup, perm) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
				}
			}

		}
		public static void GenerateGroups(Tab tab, Permission perms, PlayerSession ap)
		{
			tab.ClearColumn(1);
			tab.AddName(1, "Groups", TextAnchor.MiddleLeft);
			{
				tab.AddInput(1, "Search", ap => ap.GetStorage<string>(tab, "groupfilter", string.Empty), (ap, args) =>
				{
					ap.SetStorage(tab, "groupfilter", args.ToString(" "));
					GenerateGroups(tab, perms, ap);
				});

				var groupFilter = ap.GetStorage<string>(tab, "groupfilter");

				tab.AddButton(1, "Add Group", ap =>
				{
					var temp = Pool.GetList<string>();
					var groups = Community.Runtime.CorePlugin.permission.GetGroups();
					temp.Add("None");
					temp.AddRange(groups);

					var array = temp.ToArray();
					Pool.FreeList(ref temp);

					Modal.Open(ap.Player, "Create Group", new Dictionary<string, Modal.Field>()
					{
						["name"] = Modal.Field.Make("Name", Modal.Field.FieldTypes.String, true, customIsInvalid: (field) => perms.GetGroups().Any(x => x == field.Get<string>()) ? "Group with that name already exists." : null),
						["dname"] = Modal.Field.Make("Display Name", Modal.Field.FieldTypes.String, @default: string.Empty),
						["rank"] = Modal.Field.Make("Rank", Modal.Field.FieldTypes.Integer, @default: 0),
						["parent"] = Modal.EnumField.MakeEnum("Parent", array, @default: 0)
					}, onConfirm: (player, modal) =>
					{
						var parentIndex = modal.Get<int>("parent");
						perms.CreateGroup(modal.Get<string>("name"), modal.Get<string>("dname"), modal.Get<int>("rank"));
						if (parentIndex != 0) perms.SetGroupParent(modal.Get<string>("name"), array[parentIndex]);

						tab.ClearColumn(1);
						tab.ClearColumn(2);
						tab.ClearColumn(3);
						GenerateGroups(tab, perms, ap);
					});
				}, (_instance) => Tab.OptionButton.Types.Warned);

				foreach (var group in permission.GetGroups())
				{
					if (!string.IsNullOrEmpty(groupFilter) && !group.Contains(groupFilter)) continue;

					tab.AddButton(1, $"{group}", instance2 =>
					{
						ap.SetStorage(tab, "group", group);
						ap.ClearStorage(tab, "plugin");

						tab.ClearColumn(2);
						tab.ClearColumn(3);

						GeneratePlugins(tab, ap, permission, ap.Player, group);
					}, type: (_instance) => ap.GetStorage<string>(tab, "group", string.Empty) == group ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None);
				}
			}
		}
	}
	public class EntitiesTab
	{
		internal static int EntityCount = 0;

		internal static RustPlugin Core = Community.Runtime.CorePlugin;
		internal static AdminModule Admin = GetModule<AdminModule>();
		internal static PlayerSession LastContainerLooter;
		internal static string[] BuildingGrades = new string[]
		{
			"Twig",
			"Wood",
			"Stone",
			"Metal",
			"Top Tier"
		};
		internal const string MultiselectionReplacement = "-";

		public static Tab Get()
		{
			var tab = new Tab("entities", "Entities", Community.Runtime.CorePlugin, (ap, tab2) => { tab2.ClearColumn(1); ResetSelection(tab2, ap); DrawEntities(tab2, ap); }, 2);
			tab.AddColumn(0);
			tab.AddColumn(1);

			return tab;
		}

		internal static void SelectEntity(Tab tab, PlayerSession session, BaseEntity entity)
		{
			var selectedEntitites = (List<BaseEntity>)null;

			if (!session.HasStorage(tab, "selectedentities"))
			{
				selectedEntitites = session.SetStorage(tab, "selectedentities", new List<BaseEntity>());
			}
			else
			{
				selectedEntitites = session.GetStorage<List<BaseEntity>>(tab, "selectedentities");
			}

			if (!session.GetStorage<bool>(tab, "multi", false)) selectedEntitites.Clear();
			if (!selectedEntitites.Contains(entity)) selectedEntitites.Add(entity);
		}
		internal static void ResetSelection(Tab tab, PlayerSession session)
		{
			var selectedEntitites = (List<BaseEntity>)null;

			if (!session.HasStorage(tab, "selectedentities"))
			{
				selectedEntitites = session.SetStorage(tab, "selectedentities", new List<BaseEntity>());
			}
			else
			{
				selectedEntitites = session.GetStorage<List<BaseEntity>>(tab, "selectedentities");
				selectedEntitites.Clear();
			}
		}

		internal static void DrawEntities(Tab tab, PlayerSession ap3)
		{
			tab.ClearColumn(0);
			tab.AddName(0, "Entities");

			var selectedEntitites = ap3.GetStorage<List<BaseEntity>>(tab, "selectedentities");

			if (!ap3.HasStorage(tab, "selectedentities"))
			{
				selectedEntitites = ap3.SetStorage(tab, "selectedentities", new List<BaseEntity>());
			}

			tab.AddInputButton(0, "Search Entity", 0.3f,
				new Tab.OptionInput(null, ap => ap.GetStorage<string>(tab, "filter", string.Empty), 0, false, (ap, args) => { ap.SetStorage(tab, "filter", args.ToString(" ")); DrawEntities(tab, ap); }),
				new Tab.OptionButton($"Refresh", ap => { DrawEntities(tab, ap); }));

			var isMulti = ap3.GetStorage<bool>(tab, "multi");
			tab.AddToggle(0, "Multi-selection", ap =>
			{
				isMulti = ap.SetStorage(tab, "multi", !isMulti);
				selectedEntitites.Clear();
				tab.ClearColumn(1);
				DrawEntities(tab, ap3);
			}, ap => isMulti);

			var pool = Pool.GetList<BaseEntity>();
			EntityCount = 0;

			var usedFilter = ap3.GetStorage<string>(tab, "filter", string.Empty)?.ToLower()?.Trim();
			using var map = Entities.Get<BaseEntity>(true);
			var validateFilter = ap3.GetStorage<Func<BaseEntity, bool>>(tab, "validatefilter");
			var maximumRange = ((int)World.Size).Clamp(1, int.MaxValue) / 2;
			var range = ap3.GetStorage<int>(tab, "range", maximumRange);
			map.Each(entity =>
			{
				pool.Add(entity);
				EntityCount++;
			}, entity => entity != null && entity.transform != null && (validateFilter == null || validateFilter.Invoke(entity)) && entity.transform.position != Vector3.zero
				&& (string.IsNullOrEmpty(usedFilter) || entity.ToString().ToLower().Contains(usedFilter) || entity.name.ToLower().Contains(usedFilter) || entity.GetType().Name?.ToLower() == usedFilter)
				&& (range == -1 || ap3 == null || (ap3.Player != null && Vector3.Distance(ap3.Player.transform.position, entity.transform.position) <= range)));

			tab.AddRange(0, "Range", 0, maximumRange, ap => range, (ap, value) => { try { ap.SetStorage(tab, "range", (int)value); DrawEntities(tab, ap); } catch (Exception ex) { Logger.Error($"Oof", ex); } }, ap => $"{range:0.0}m");
			tab.AddName(0, $"Entities  ({EntityCount:n0})", TextAnchor.MiddleLeft);

			var filter = ap3.GetStorage<string>(tab, "filter", string.Empty);
			tab.AddButtonArray(0,
				new Tab.OptionButton("Players", ap => { ap.SetStorage(tab, "filter", nameof(BasePlayer)); DrawEntities(tab, ap); }, ap => filter == nameof(BasePlayer) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
				new Tab.OptionButton("Containers", ap => { ap.SetStorage(tab, "filter", nameof(StorageContainer)); validateFilter = null; DrawEntities(tab, ap); }, ap => filter == nameof(StorageContainer) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
				new Tab.OptionButton("Deployables", ap => { ap.SetStorage(tab, "filter", nameof(Deployable)); validateFilter = null; DrawEntities(tab, ap); }, ap => filter == nameof(Deployable) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
				new Tab.OptionButton("Collectibles", ap => { ap.SetStorage(tab, "filter", nameof(CollectibleEntity)); validateFilter = null; DrawEntities(tab, ap); }, ap => filter == nameof(CollectibleEntity) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
				new Tab.OptionButton("NPCs", ap => { ap.SetStorage(tab, "filter", nameof(NPCPlayer)); validateFilter = null; DrawEntities(tab, ap); }, ap => filter == nameof(NPCPlayer) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None),
				new Tab.OptionButton("I/O", ap => { ap.SetStorage(tab, "filter", nameof(IOEntity)); validateFilter = null; DrawEntities(tab, ap); }, ap => filter == nameof(IOEntity) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));

			switch (ap3.GetStorage<string>(tab, "filter", string.Empty))
			{
				case nameof(BasePlayer):
					tab.AddButtonArray(0,
						new Tab.OptionButton("Online", ap => { validateFilter = entity => entity is BasePlayer player && player.IsConnected; DrawEntities(tab, ap); }),
						new Tab.OptionButton("Offline", ap => { validateFilter = entity => entity is BasePlayer player && !player.IsConnected; DrawEntities(tab, ap); }),
						new Tab.OptionButton("Dead", ap => { validateFilter = entity => entity is BasePlayer player && player.IsDead(); DrawEntities(tab, ap); }));
					break;
			}

			foreach (var entity in pool)
			{
				var name = entity.ToString();

				switch (entity)
				{
					case BasePlayer player:
						name = $"{player.displayName}";
						break;
				}

				tab.AddButton(0, name, ap =>
				{
					if (selectedEntitites.Contains(entity))
					{
						selectedEntitites.Remove(entity);
						tab.ClearColumn(1);
					}
					else
					{
						SelectEntity(tab, ap, entity);
					}

					DrawEntitySettings(tab, 1, ap);
				}, ap => selectedEntitites.Contains(entity) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None);
			}

			if (EntityCount == 0)
			{
				tab.AddText(0, "No entities found with that filter", 9, "1 1 1 0.2", TextAnchor.MiddleCenter, Handler.FontTypes.RobotoCondensedRegular);
			}
		}
		internal static void DrawEntitySettings(Tab tab, int column = 1, PlayerSession ap3 = null)
		{
			var selectedEntitites = ap3.GetStorage<List<BaseEntity>>(tab, "selectedentities");
			tab.ClearColumn(column);

			if (selectedEntitites.Count == 0) return;

			var entity = selectedEntitites[0];
			var multiSelection = selectedEntitites.Count > 1;
			var sameTypeSelection = selectedEntitites.All(x => x.GetType() == entity.GetType());

			tab.AddName(column, "Hierarchy");

			if (column != 1) tab.AddButton(column, "<", ap => { DrawEntities(tab, ap); DrawEntitySettings(tab, 1, ap); }, ap => Tab.OptionButton.Types.Warned);

			if (entity != null && !entity.IsDestroyed)
			{
				var player = entity as BasePlayer;
				var owner = BasePlayer.FindByID(entity.OwnerID);

				if (player != ap3?.Player)
				{
					tab.AddButtonArray(column,
						new Tab.OptionButton("Kill", ap =>
						{
							tab.CreateDialog($"Are you sure about that?", ap =>
							{
								DoAll<BaseEntity>(e => e.Kill());
								DrawEntities(tab, ap);
								tab.ClearColumn(column);
							}, null);
						}, ap => Tab.OptionButton.Types.Important),
						new Tab.OptionButton("Kill (Gibbed)", ap =>
						{
							tab.CreateDialog($"Are you sure about that?", ap =>
							{
								DoAll<BaseEntity>(e => e.Kill(BaseNetworkable.DestroyMode.Gib));
								DrawEntities(tab, ap);
								tab.ClearColumn(column);
							}, null);
						}));
				}

				tab.AddInput(column, "Id", ap => multiSelection ? MultiselectionReplacement : $"{entity.net.ID} [{entity.GetType().FullName}]", null);
				tab.AddInput(column, "Name", ap => multiSelection ? MultiselectionReplacement : $"{entity.ShortPrefabName}", null);

				if (!multiSelection)
				{
					tab.AddInputButton(column, "Owner", 0.3f,
						new Tab.OptionInput(null, ap => $"{(entity.OwnerID.IsSteamId() ? $"{BasePlayer.FindByID(entity.OwnerID).displayName}" : "None")}", 0, true, null),
						new Tab.OptionButton("Select", ap =>
						{
							if (owner == null) return;

							SelectEntity(tab, ap, owner);
							DrawEntities(tab, ap);
							DrawEntitySettings(tab, 1, ap);
						}, ap => owner == null ? Tab.OptionButton.Types.None : Tab.OptionButton.Types.Selected));
				}

				tab.AddInput(column, "Prefab", ap => multiSelection ? MultiselectionReplacement : $"{entity.PrefabName}", null);
				tab.AddInput(column, "Flags", ap => multiSelection ? MultiselectionReplacement : entity.flags == 0 ? "None" : $"{entity.flags}", null);
				tab.AddButton(column, "Edit Flags", ap => { DrawEntitySettings(tab, 0, ap); DrawEntityFlags(tab, ap, 1); });
				tab.AddInput(column, "Position", ap => multiSelection ? MultiselectionReplacement : $"{entity.transform.position}", null);
				tab.AddInput(column, "Rotation", ap => multiSelection ? MultiselectionReplacement : $"{entity.transform.rotation}", null);

				if (sameTypeSelection)
				{
					if (!multiSelection)
					{
						tab.AddButtonArray(column,
							new Tab.OptionButton("TeleportTo", ap => { ap.Player.Teleport(entity.transform.position); }),
							new Tab.OptionButton("Teleport2Me", ap =>
							{
								tab.CreateDialog($"Are you sure about that?", ap =>
								{
									entity.transform.position = ap.Player.transform.position;
									entity.SendNetworkUpdateImmediate();
								}, null);
							}));
					}

					if (entity is StorageContainer storage)
					{
						if (!multiSelection)
						{
							tab.AddButton(column, "Loot Container", ap =>
							{
								LastContainerLooter = ap;

								ap.SetStorage(tab, "lootedent", entity);
								Admin.Subscribe("OnEntityVisibilityCheck");
								Admin.Subscribe("OnEntityDistanceCheck");

								Core.timer.In(0.2f, () => Admin.Close(ap.Player));
								Core.timer.In(0.5f, () =>
								{
									SendEntityToPlayer(ap.Player, entity);

									ap.Player.inventory.loot.Clear();
									ap.Player.inventory.loot.PositionChecks = false;
									ap.Player.inventory.loot.entitySource = storage;
									ap.Player.inventory.loot.itemSource = null;
									ap.Player.inventory.loot.AddContainer(storage.inventory);
									ap.Player.inventory.loot.MarkDirty();
									ap.Player.inventory.loot.SendImmediate();

									ap.Player.ClientRPCPlayer(null, ap.Player, "RPC_OpenLootPanel", storage.panelName);
								});
							});
						}
					}

					if (entity is BasePlayer)
					{
						tab.AddInput(column, "Display Name", ap => multiSelection ? MultiselectionReplacement : player.displayName);
						tab.AddInput(column, "Steam ID", ap => multiSelection ? MultiselectionReplacement : player.UserIDString);

						tab.AddButtonArray(column,
							new Tab.OptionButton("Loot", ap =>
							{
								if (multiSelection) return;

								LastContainerLooter = ap;
								ap.SetStorage(tab, "lootedent", entity);
								SendEntityToPlayer(ap.Player, entity);

								Core.timer.In(0.2f, () => Admin.Close(ap.Player));
								Core.timer.In(0.5f, () =>
								{
									SendEntityToPlayer(ap.Player, entity);

									ap.Player.inventory.loot.Clear();
									ap.Player.inventory.loot.PositionChecks = false;
									ap.Player.inventory.loot.entitySource = RelationshipManager.ServerInstance;
									ap.Player.inventory.loot.itemSource = null;
									ap.Player.inventory.loot.AddContainer(player.inventory.containerMain);
									ap.Player.inventory.loot.AddContainer(player.inventory.containerWear);
									ap.Player.inventory.loot.AddContainer(player.inventory.containerBelt);
									ap.Player.inventory.loot.MarkDirty();
									ap.Player.inventory.loot.SendImmediate();

									ap.Player.ClientRPCPlayer(null, ap.Player, "RPC_OpenLootPanel", "player_corpse");
								});
							}),
							new Tab.OptionButton("Respawn", ap =>
							{
								tab.CreateDialog($"Are you sure about that?", ap =>
								{
									DoAll<BasePlayer>(e =>
									{
										e.Hurt(player.MaxHealth());
										e.Respawn();
										e.EndSleeping();
									});
								}, null);
							}));
						tab.AddInput(column, "PM", null, (ap, args) =>
						{
							DoAll<BasePlayer>(e =>
							{
								e.ChatMessage($"[{ap.Player.displayName}]: {args.ToString(" ")}");
							});
						});

						if (!multiSelection && ap3 != null)
						{
							if (!PlayersTab.BlindedPlayers.Contains(player))
							{
								tab.AddButton(1, "Blind Player", ap =>
								{
									tab.CreateDialog("Are you sure you want to blind the player?", ap =>
									{
										using var cui = new CUI(Singleton.Handler);
										var container = cui.CreateContainer("blindingpanel", "0 0 0 1", needsCursor: true, needsKeyboard: Singleton.HandleEnableNeedsKeyboard(ap));
										cui.CreateClientImage(container, "blindingpanel", null, "https://carbonmod.gg/assets/media/cui/bsod.png", "1 1 1 1");
										cui.Send(container, player);
										PlayersTab.BlindedPlayers.Add(player);
										EntitiesTab.SelectEntity(tab, ap, entity);
										DrawEntitySettings(tab, column, ap3);

										if (ap.Player == player) Core.timer.In(1, () => { Singleton.Close(player); });
									}, null);
								});
							}
							else
							{
								tab.AddButton(1, "Unblind Player", ap =>
								{
									using var cui = new CUI(Singleton.Handler);
									cui.Destroy("blindingpanel", player);
									PlayersTab.BlindedPlayers.Remove(player);
									EntitiesTab.SelectEntity(tab, ap, entity);
									DrawEntitySettings(tab, column, ap3);
								}, ap => Tab.OptionButton.Types.Selected);
							}
						}
					}

					if (!multiSelection && ap3.Player != player && (ap3.Player.spectateFilter != player?.UserIDString && ap3.Player.spectateFilter != entity.net.ID.ToString()))
					{
						tab.AddButton(1, "Spectate", ap =>
						{
							StartSpectating(ap.Player, entity);
							SelectEntity(tab, ap, entity);
							DrawEntitySettings(tab, column, ap3);
						});
					}
					if (!multiSelection && !string.IsNullOrEmpty(ap3.Player.spectateFilter) && (ap3.Player.UserIDString == player?.UserIDString || ap3.Player.spectateFilter == entity.net.ID.ToString()))
					{
						tab.AddButton(1, "End Spectating", ap =>
						{
							StopSpectating(ap.Player);
							SelectEntity(tab, ap, entity);
							DrawEntitySettings(tab, column, ap3);
						}, ap => Tab.OptionButton.Types.Selected);
					}

					if (!multiSelection && entity.parentEntity.IsValid(true)) tab.AddButton(column, $"Parent: {entity.parentEntity.Get(true)}", ap => { DrawEntities(tab, ap); SelectEntity(tab, ap, entity.parentEntity.Get(true)); DrawEntitySettings(tab, 1, ap); });

					if (!multiSelection && entity.children.Count > 0)
					{
						tab.AddName(column, "Children", TextAnchor.MiddleLeft);
						foreach (var child in entity.children)
						{
							tab.AddButton(column, $"{child}", ap => { SelectEntity(tab, ap, child); DrawEntities(tab, ap); DrawEntitySettings(tab, 1, ap); });
						}
					}

					switch (entity)
					{
						case CCTV_RC cctv:
							{
								tab.AddName(column, "CCTV", TextAnchor.MiddleLeft);
								tab.AddInput(column, "Identifier", ap => multiSelection ? MultiselectionReplacement : cctv.GetIdentifier(), (ap, args) => { cctv.UpdateIdentifier(args.ToString(""), true); });
								if (!multiSelection)
								{
									tab.AddButton(column, "View CCTV", ap =>
									{
										Core.timer.In(0.1f, () => { Admin.Close(ap.Player); ap.SetStorage(tab, "wasviewingcam", true); });
										Core.timer.In(0.3f, () =>
										{
											Admin.Subscribe("OnEntityDismounted");
											Admin.Subscribe("CanDismountEntity");

											var station = GameManager.server.CreateEntity("assets/prefabs/deployable/computerstation/computerstation.deployed.prefab", ap.Player.transform.position) as ComputerStation;
											station.skinID = 69696;
											station.SendControlBookmarks(ap.Player);
											station.Spawn();
											station.checkPlayerLosOnMount = false;
											station.legacyDismount = true;

											station.MountPlayer(ap.Player);
											ViewCamera(ap.Player, station, cctv);
										});
									});
								}
								break;
							}
						case CodeLock codeLock:
							{
								tab.AddName(column, "Code Lock", TextAnchor.MiddleLeft);
								tab.AddInput(column, "Code", ap => multiSelection ? MultiselectionReplacement : codeLock.code, (ap, args) =>
								{
									var code = args.ToString(" ");

									foreach (var character in code)
										if (char.IsLetter(character)) return;

									DoAll<CodeLock>(e => e.code = StringEx.Truncate(code, 4));
								});
								break;
							}
						case MiniCopter minicopter:
							{
								tab.AddName(column, "Minicopter", TextAnchor.MiddleLeft);

								if (!minicopter)
								{
									tab.AddButton(column, "Open Fuel", ap => { LastContainerLooter = ap; Core.timer.In(0.2f, () => Admin.Close(ap.Player)); Core.timer.In(0.5f, () => { minicopter.engineController.FuelSystem.GetFuelContainer().PlayerOpenLoot(ap.Player, doPositionChecks: false); }); });
								}
								break;
							}
						case BuildingBlock block:
							{
								tab.AddName(column, "Building Block", TextAnchor.MiddleLeft);
								tab.AddDropdown(column, "Grade", (ap) => (int)block.grade, (ap, index) =>
								{
									DoAll<BuildingBlock>(e =>
									{
										e.ChangeGrade((BuildingGrade.Enum)index, true);
										e.skinID = 0;
									});

									DrawEntitySettings(tab, column, ap);
								}, BuildingGrades);
							}
							break;
					}

					if (entity is BaseCombatEntity combat)
					{
						tab.AddName(column, "Combat", TextAnchor.MiddleLeft);
						tab.AddRange(column, "Health", 0, combat.MaxHealth(), ap => combat.health, (ap, value) =>
						{
							DoAll<BaseCombatEntity>(e => e.SetHealth(value));
						}, ap => $"{combat.health:0}");

						if (entity is BasePlayer)
						{
							tab.AddRange(column, "Thirst", 0, player.metabolism.hydration.max, ap => player.metabolism.hydration.value, (ap, value) => DoAll<BasePlayer>(e => e.metabolism.hydration.SetValue(value)), ap => $"{player.metabolism.hydration.value:0}");
							tab.AddRange(column, "Hunger", 0, player.metabolism.calories.max, ap => player.metabolism.calories.value, (ap, value) => DoAll<BasePlayer>(e => e.metabolism.calories.SetValue(value)), ap => $"{player.metabolism.calories.value:0}");
							tab.AddRange(column, "Radiation", 0, player.metabolism.radiation_poison.max, ap => player.metabolism.radiation_poison.value, (ap, value) => DoAll<BasePlayer>(e => e.metabolism.radiation_poison.SetValue(value)), ap => $"{player.metabolism.radiation_poison.value:0}");
							tab.AddRange(column, "Bleeding", 0, player.metabolism.bleeding.max, ap => player.metabolism.bleeding.value, (ap, value) => DoAll<BasePlayer>(e => e.metabolism.bleeding.SetValue(value)), ap => $"{player.metabolism.bleeding.value:0}");
						}
					}
				}
			}
			else
			{
				tab.ClearColumn(1);
				DrawEntities(tab, ap3);
			}

			void DoAll<T>(Action<T> callback) where T : BaseEntity
			{
				foreach (var selectedEntity in selectedEntitites)
				{
					if (selectedEntity == null) continue;

					callback?.Invoke((T)selectedEntity);
				}
			}
		}
		internal static void DrawEntityFlags(Tab tab, PlayerSession session, int column = 1)
		{
			var selectedEntitites = session.GetStorage<List<BaseEntity>>(tab, "selectedentities", new List<BaseEntity>());

			tab.ClearColumn(column);
			if (selectedEntitites.Count == 0) return;

			var entity = selectedEntitites[0];

			var counter = 0;
			var currentButtons = Pool.GetList<Tab.OptionButton>();

			tab.ClearColumn(column);

			tab.AddName(column, "Entity Flags", TextAnchor.MiddleLeft);
			foreach (var flag in Enum.GetNames(typeof(BaseEntity.Flags)).OrderBy(x => x))
			{
				var flagValue = (BaseEntity.Flags)Enum.Parse(typeof(BaseEntity.Flags), flag);
				var isDifferent = selectedEntitites.All(x => x.HasFlag(flagValue));
				var hasFlag = entity.HasFlag(flagValue);

				currentButtons.Add(new Tab.OptionButton(flag, ap =>
				{
					DoAll<BaseEntity>(e => e.SetFlag(flagValue, !hasFlag));
					DrawEntitySettings(tab, 0, ap);
					DrawEntityFlags(tab, ap, column);
				}, ap => isDifferent ? Tab.OptionButton.Types.Warned : hasFlag ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
				counter++;

				if (counter >= 5)
				{
					tab.AddButtonArray(column, currentButtons.ToArray());
					currentButtons.Clear();
					counter = 0;
				}
			}

			Pool.FreeList(ref currentButtons);

			void DoAll<T>(Action<T> callback) where T : BaseEntity
			{
				foreach (var selectedEntity in selectedEntitites)
				{
					if (selectedEntity == null) continue;

					callback?.Invoke((T)selectedEntity);
				}
			}
		}

		internal static void ViewCamera(BasePlayer player, ComputerStation station, CCTV_RC camera)
		{
			player.net.SwitchSecondaryGroup(camera.net.group);
			station.currentlyControllingEnt.uid = camera.net.ID;
			station.currentPlayerID = player.userID;
			var b = camera.InitializeControl(new CameraViewerId(station.currentPlayerID, 0L));
			station.SetFlag(BaseEntity.Flags.Reserved2, b, recursive: false, networkupdate: false);
			station.SendNetworkUpdateImmediate();
			station.SendControlBookmarks(player);
		}
		internal static void SendEntityToPlayer(BasePlayer player, BaseEntity entity)
		{
			var connection = player.Connection;

			if (connection == null)
			{
				return;
			}

			var netWrite = Net.sv.StartWrite();

			if (netWrite == null)
			{
				return;
			}

			++connection.validate.entityUpdates;

			netWrite.PacketID(Message.Type.Entities);
			netWrite.UInt32(connection.validate.entityUpdates);

			entity.ToStreamForNetwork(netWrite, new BaseNetworkable.SaveInfo() { forConnection = connection, forDisk = false });
			netWrite.Send(new SendInfo(connection));
		}
	}
	public class ModulesTab
	{
		public static Tab Get()
		{
			var tab = new Tab("modules", "Modules", Community.Runtime.CorePlugin, accessLevel: 3);
			tab.AddColumn(0);
			tab.AddColumn(1);

			tab.AddName(0, "Modules");
			foreach (var hookable in Community.Runtime.ModuleProcessor.Modules)
			{
				if (hookable is BaseModule module)
				{
					tab.AddButton(0, $"{hookable.Name}", ap =>
					{
						DrawModuleSettings(tab, module);
					});
				}
			}

			return tab;
		}

		internal static void DrawModuleSettings(Tab tab, BaseModule module)
		{
			tab.ClearColumn(1);

			var carbonModule = module.GetType();
			tab.AddInput(1, "Name", ap => module.Name, null);


			tab.AddToggle(1, "Enabled", ap2 => { module.SetEnabled(!module.GetEnabled()); module.Save(); DrawModuleSettings(tab, module); }, ap2 => module.GetEnabled());

			tab.AddButtonArray(1,
				new Tab.OptionButton("Save", ap => { module.Save(); }),
				new Tab.OptionButton("Load", ap => { module.Load(); }));

			tab.AddButton(1, "Edit Config", ap =>
			{
				var moduleConfigFile = Path.Combine(Core.Defines.GetModulesFolder(), module.Name, "config.json");
				ap.SelectedTab = ConfigEditor.Make(OsEx.File.ReadText(moduleConfigFile),
					(ap, jobject) =>
					{
						Singleton.SetTab(ap.Player, "modules");
						Singleton.Draw(ap.Player);
					},
					(ap, jobject) =>
					{
						OsEx.File.Create(moduleConfigFile, jobject.ToString(Formatting.Indented));
						module.SetEnabled(false);
						module.Load();

						Singleton.SetTab(ap.Player, "modules");
						Singleton.Draw(ap.Player);
					}, null);
			});
		}
	}
	public class PluginsTab
	{
		public enum VendorTypes
		{
			Local,
			Codefling,
			uMod
		}
		public enum FilterTypes
		{
			None,
			Price,
			Author,
			Installed,
			OutOfDate,
			Favourites,
			Owned
		}

		public static bool DropdownShow { get; set; }
		public static string[] DropdownOptions { get; } = new string[] { "A-Z", "Price", "Author", "Installed", "Needs Update", "Favourites", "Owned" };
		public static PlayerSession.Page PlaceboPage { get; } = new PlayerSession.Page();
		public static List<string> TagFilter { get; set; } = new();
		public static string[] PopularTags { get; } = new string[]
		{
				"gui",
				"admin",
				"moderation",
				"chat",
				"building",
				"discord",
				"libraries",
				"loot",
				"pve",
				"event",
				"logging",
				"anti-cheat",
				"economics",
				"npc",
				"info",
				"limitations",
				"statistics",
				"monuments",
				"seasonal",
				"banan",
				"peanus"
		};

		public static IVendorDownloader CodeflingInstance { get; set; }
		public static IVendorDownloader uModInstance { get; set; }
		public static IVendorDownloader Lone_DesignInstance { get; set; }
		public static IVendorDownloader LocalInstance { get; set; }

		public static IVendorDownloader GetVendor(VendorTypes vendor)
		{
			switch (vendor)
			{
				case VendorTypes.Codefling:
					return CodeflingInstance;

				case VendorTypes.uMod:
					return uModInstance;

				// case VendorTypes.Lone_Design:
				// 	return Lone_DesignInstance;

				case VendorTypes.Local:
					return LocalInstance;
			}

			return default;
		}

		public static Tab Get()
		{
			OsEx.Folder.Create(Path.Combine(Core.Defines.GetScriptFolder(), "backups"));

			var tab = new Tab("plugins", "Plugins", Community.Runtime.CorePlugin, (ap, t) =>
			{
				ap.SetStorage(t, "selectedplugin", (Plugin)null);
				LocalInstance?.Refresh();
			}, 2)
			{
				Override = (t, cui, container, parent, ap) => Draw(cui, container, parent, t, ap)
			};

			CodeflingInstance = new Codefling();
			if (!CodeflingInstance.Load())
			{
				CodeflingInstance.FetchList();
				CodeflingInstance.Refresh();
			}

			uModInstance = new uMod();
			if (!uModInstance.Load())
			{
				uModInstance.FetchList();
				uModInstance.Refresh();
			}

			// Lone_DesignInstance = new Lone_Design();
			// if (!Lone_DesignInstance.Load())
			// {
			// 	Lone_DesignInstance.FetchList();
			// 	Lone_DesignInstance.Refresh();
			// }

			LocalInstance = new Local();
			LocalInstance.Refresh();

			ServerOwner.Load();

			return tab;
		}

		public static List<Plugin> GetPlugins(IVendorDownloader vendor, Tab tab, PlayerSession ap)
		{
			return GetPlugins(vendor, tab, ap, out _);
		}
		public static List<Plugin> GetPlugins(IVendorDownloader vendor, Tab tab, PlayerSession ap, out int maxPages)
		{
			maxPages = 0;

			var resultList = Pool.GetList<Plugin>();
			var customList = Pool.GetList<Plugin>();

			using (TimeMeasure.New("GetPluginsFromVendor", 1))
			{
				try
				{
					var plugins = vendor.FetchedPlugins.ToArray();
					var filter = ap.GetStorage<FilterTypes>(tab, "filter", FilterTypes.None);
					switch (filter)
					{
						case FilterTypes.Price:
							plugins = vendor.PriceData;
							break;

						case FilterTypes.Author:
							plugins = vendor.AuthorData;
							break;

						case FilterTypes.Installed:
							plugins = vendor.InstalledData;
							break;

						case FilterTypes.OutOfDate:
							plugins = vendor.OutOfDateData;
							break;
					}

					// if (FlipFilter)
					// {
					// 	var reverse = plugins.Reverse().ToArray();
					// 	Array.Clear(plugins, 0, plugins.Length);
					// 	plugins = reverse;
					// }

					var search = ap.GetStorage<string>(tab, "search");
					if (!string.IsNullOrEmpty(search))
					{
						foreach (var plugin in plugins)
						{
							if (filter == FilterTypes.Favourites)
							{
								if (ServerOwner.Singleton.FavouritePlugins.Contains(plugin.File))
								{
									customList.Add(plugin);
									continue;
								}

								continue;
							}

							if (plugin.Id == search)
							{
								customList.Add(plugin);
								continue;
							}

							if (plugin.Name.ToLower().Trim().Contains(search.ToLower().Trim()))
							{
								customList.Add(plugin);
								continue;
							}

							if (plugin.Author.ToLower().Trim().Contains(search.ToLower().Trim()))
							{
								customList.Add(plugin);
								continue;
							}

							if (TagFilter.Count > 0 && plugin.Tags != null)
							{
								var hasTag = false;
								foreach (var tag in plugin.Tags)
								{
									if (TagFilter.Contains(tag))
									{
										hasTag = true;
										break;
									}
								}

								if (hasTag)
								{
									customList.Add(plugin);
									continue;
								}
							}
						}
					}
					else
					{
						foreach (var plugin in plugins)
						{
							if (filter == FilterTypes.Favourites)
							{
								if (ServerOwner.Singleton.FavouritePlugins.Contains(plugin.File))
								{
									customList.Add(plugin);
									continue;
								}

								continue;
							}

							if (TagFilter.Count > 0 && plugin.Tags != null)
							{
								var hasTag = false;
								foreach (var tag in plugin.Tags)
								{
									if (TagFilter.Contains(tag))
									{
										hasTag = true;
										break;
									}
								}

								if (hasTag)
								{
									customList.Add(plugin);
									continue;
								}
							}
							else customList.Add(plugin);
						}
					}

					maxPages = (customList.Count - 1) / 15;

					var page2 = ap.GetStorage<int>(tab, "page", 0);
					if (page2 > maxPages) ap.SetStorage(tab, "page", maxPages);

					var page = 15 * page2;
					var count = (page + 15).Clamp(0, customList.Count);

					if (count > 0)
					{
						for (int i = page; i < count; i++)
						{
							try { resultList.Add(customList[i]); } catch { }
						}
					}
				}
				catch (Exception ex)
				{
					Pool.FreeList(ref resultList);

					Logger.Error($"Failed getting plugins.", ex);
				}

				Pool.FreeList(ref customList);
			}

			return resultList;
		}

		public static void DownloadThumbnails(IVendorDownloader vendor, Tab tab, PlayerSession ap)
		{
			var plugins = GetPlugins(vendor, tab, ap);

			var images = Pool.GetList<string>();
			var imagesSafe = Pool.GetList<string>();

			foreach (var element in plugins)
			{
				if (element.NoImage()) continue;

				if (element.HasInvalidImage())
				{
					imagesSafe.Add(element.Image);
					continue;
				}

				images.Add(element.Image);
			}

			var eraseAllBeforehand = false;

			if (images.Count > 0) Singleton.ImageDatabase.QueueBatchCallback(vendor.IconScale, eraseAllBeforehand, result => { }, images.ToArray());
			if (imagesSafe.Count > 0) Singleton.ImageDatabase.QueueBatch(vendor.SafeIconScale, eraseAllBeforehand, imagesSafe.ToArray());

			Pool.FreeList(ref plugins);
			Pool.FreeList(ref images);
			Pool.FreeList(ref imagesSafe);
		}

		public static void Draw(CUI cui, CuiElementContainer container, string parent, Tab tab, PlayerSession ap)
		{
			ap.SetDefaultStorage(tab, "vendor", "Local");

			var header = cui.CreatePanel(container, parent, null, "0.2 0.2 0.2 0.5",
				xMin: 0f, xMax: 1f, yMin: 0.95f, yMax: 1f);

			var vendorName = ap.GetStorage<string>(tab, "vendor", "Local");
			var vendor = GetVendor((VendorTypes)Enum.Parse(typeof(VendorTypes), vendorName));

			var vendors = Enum.GetNames(typeof(VendorTypes));
			var cuts = 1f / vendors.Length;
			var offset = 0f;
			foreach (var value in vendors)
			{
				var v = GetVendor((VendorTypes)Enum.Parse(typeof(VendorTypes), value));
				cui.CreateProtectedButton(container, header, null, vendorName == value ? "0.3 0.72 0.25 0.8" : "0.2 0.2 0.2 0.3", "1 1 1 0.7", $"{value.Replace("_", ".")}{(v == null ? "" : $" ({v?.BarInfo})")}", 11,
					xMin: offset, xMax: offset + cuts, command: $"pluginbrowser.changetab {value}");
				offset += cuts;
			}

			var grid = cui.CreatePanel(container, parent, null, "0.2 0.2 0.2 0.3",
				xMin: 0f, xMax: 0.8f, yMin: 0, yMax: 0.94f);

			var spacing = 0.015f;
			var columnSize = 0.195f - spacing;
			var rowSize = 0.3f - spacing;
			var column = 0.02f;
			var row = 0f;
			var yOffset = 0.05f;

			var plugins = GetPlugins(vendor, tab, ap, out var maxPages);

			for (int i = 0; i < 15; i++)
			{
				if (i > plugins.Count() - 1) continue;

				var plugin = plugins.ElementAt(i);

				var card = cui.CreatePanel(container, grid, null, "0.2 0.2 0.2 0.4",
					xMin: column, xMax: column + columnSize, yMin: 0.69f + row - yOffset, yMax: 0.97f + row - yOffset);

				if (plugin.NoImage()) cui.CreateImage(container, card, null, vendor.Logo, "0.2 0.2 0.2 0.4", xMin: 0.2f, xMax: 0.8f, yMin: 0.2f + vendor.LogoRatio, yMax: 0.8f - vendor.LogoRatio);
				else
				{
					if (Singleton.ImageDatabase.GetImage(plugin.Image) != 0) cui.CreateImage(container, card, null, plugin.Image, plugin.HasInvalidImage() ? vendor.SafeIconScale : vendor.IconScale, "1 1 1 1");
					else cui.CreateClientImage(container, card, null, plugin.Image, "1 1 1 1");
				}

				var cardTitle = cui.CreatePanel(container, card, null, "0 0 0 0.9", yMax: 0.25f);
				cui.CreatePanel(container, cardTitle, null, "0 0 0 0.2", blur: true);

				cui.CreateText(container, cardTitle, null, "1 1 1 1", plugin.Name, 11, xMin: 0.05f, yMax: 0.87f, align: TextAnchor.UpperLeft);
				cui.CreateText(container, cardTitle, null, "0.6 0.6 0.3 0.8", $"by <b>{plugin.Author}</b>", 9, xMin: 0.05f, yMin: 0.15f, align: TextAnchor.LowerLeft);
				cui.CreateText(container, cardTitle, null, "0.6 0.75 0.3 0.8", $"<b>{plugin.OriginalPrice}</b>", 11, xMax: 0.95f, yMin: 0.1f, align: TextAnchor.LowerRight);

				var shadowShift = -0.003f;
				cui.CreateText(container, card, null, "0 0 0 0.9", $"v{plugin.Version}", 9, xMax: 0.97f, yMax: 0.95f, OyMin: shadowShift, OxMin: shadowShift, OyMax: shadowShift, OxMax: shadowShift, align: TextAnchor.UpperRight);
				cui.CreateText(container, card, null, "0.6 0.75 0.3 1", $"v{plugin.Version}", 9, xMax: 0.97f, yMax: 0.95f, align: TextAnchor.UpperRight);

				var shadowShift2 = 0.1f;
				if (plugin.IsInstalled())
				{
					cui.CreateImage(container, card, null, "installed", "0 0 0 0.9", xMin: 0.04f, xMax: 0.16f, yMin: 0.83f, yMax: 0.95f, OyMin: shadowShift2, OxMin: -shadowShift2, OyMax: shadowShift2, OxMax: -shadowShift2);
					cui.CreateImage(container, card, null, "installed", plugin.IsUpToDate() ? "0.6 0.75 0.3 1" : "0.85 0.4 0.3 1", xMin: 0.04f, xMax: 0.16f, yMin: 0.83f, yMax: 0.95f);
				}

				cui.CreateProtectedButton(container, card, null, "0 0 0 0", "0 0 0 0", string.Empty, 0, command: $"pluginbrowser.selectplugin {plugin.Id}");

				var favouriteButton = cui.CreateProtectedButton(container, card, null, "0 0 0 0", "0 0 0 0", string.Empty, 0, xMin: 0.84f, xMax: 0.97f, yMin: 0.73f, yMax: 0.86f, command: $"pluginbrowser.interact 10 {plugin.File}");
				cui.CreateImage(container, favouriteButton, null, "star", ServerOwner.Singleton.FavouritePlugins.Contains(plugin.File) ? "0.9 0.8 0.4 0.95" : "0.2 0.2 0.2 0.4");

				column += columnSize + spacing;

				if (i % 5 == 4)
				{
					row -= rowSize + spacing;
					column = 0.02f;
				}
			}

			if (plugins.Count == 0)
			{
				cui.CreateText(container, parent, null, "1 1 1 0.5", "No plugins found for that query", 10, align: TextAnchor.MiddleCenter, yMax: 0.95f);
			}

			var sidebar = cui.CreatePanel(container, parent, null, "0.2 0.2 0.2 0.3",
				xMin: 0.81f, xMax: 1f, yMax: 0.93f);

			var topbar = cui.CreatePanel(container, parent, null, "0.1 0.1 0.1 0.7",
				xMin: 0f, xMax: 0.8f, yMin: 0.89f, yMax: 0.94f);

			var drop = cui.CreatePanel(container, sidebar, null, "0 0 0 0", yMin: 0.96f, OxMin: -155);
			Singleton.TabPanelDropdown(cui, PlaceboPage, container, drop, null, $"pluginbrowser.changesetting filter_dd", 1, 0, (int)ap.GetStorage<FilterTypes>(tab, "filter", FilterTypes.None), DropdownOptions, null, 0, DropdownShow);

			var topbarYScale = 0.1f;
			cui.CreateText(container, topbar, null, "1 1 1 1", plugins.Count > 0 ? $"/ {maxPages + 1:n0}" : "NONE", plugins.Count > 0 ? 10 : 8, xMin: plugins.Count > 0 ? 0.925f : 0.92f, xMax: 0.996f, align: TextAnchor.MiddleLeft);
			if (plugins.Count != 0) cui.CreateProtectedInputField(container, topbar, null, "1 1 1 1", $"{ap.GetStorage<int>(tab, "page", 0) + 1}", 10, 3, false, xMin: 0.8f, xMax: 0.92f, align: TextAnchor.MiddleRight, command: $"pluginbrowser.page ");
			cui.CreateProtectedButton(container, topbar, null, "0.4 0.7 0.3 0.8", "1 1 1 0.6", "<", 10, xMin: 0.86f, xMax: 0.886f, yMin: topbarYScale, yMax: 1f - topbarYScale, command: "pluginbrowser.page -1");
			cui.CreateProtectedButton(container, topbar, null, "0.4 0.7 0.3 0.8", "1 1 1 0.6", ">", 10, xMin: 0.97f, xMax: 0.996f, yMin: topbarYScale, yMax: 1f - topbarYScale, command: "pluginbrowser.page +1");

			var filterSectionOffset = 0.45f;
			var filterSectionOffsetSize = 0.1f;
			var tagsOption = cui.CreatePanel(container, sidebar, null, "0 0 0 0", yMin: filterSectionOffset - filterSectionOffsetSize, yMax: filterSectionOffset - 0.05f);
			var rowNumber = 0;
			var count = 0;
			var countPerRow = 3;
			var buttonSize = 1f / countPerRow;
			var buttonOffset = 0f;
			var rows = 7;
			foreach (var tag in PopularTags.Take(countPerRow * rows))
			{
				count++;

				cui.CreateProtectedButton(container, tagsOption, null, TagFilter.Contains(tag) ? "#b08d2e" : "0.2 0.2 0.2 0.5", "1 1 1 0.6", tag.ToUpper(), 8, buttonOffset, buttonOffset += buttonSize, OyMin: rowNumber * -25, OyMax: rowNumber * -25, command: $"pluginbrowser.tagfilter {tag}");

				if (count % countPerRow == 0)
				{
					buttonOffset = 0;
					rowNumber++;
				}
			}

			var isLocal = vendor is Local;
			var searchQuery = ap.GetStorage<string>(tab, "search");
			var search = cui.CreatePanel(container, topbar, null, "0 0 0 0", xMin: 0.6f, xMax: 0.855f, yMin: 0f, OyMax: -0.5f);
			cui.CreateProtectedInputField(container, search, null, string.IsNullOrEmpty(searchQuery) ? "0.8 0.8 0.8 0.6" : "1 1 1 1", string.IsNullOrEmpty(searchQuery) ? "Search..." : searchQuery, 10, 20, false, xMin: 0.06f, align: TextAnchor.MiddleLeft, needsKeyboard: Singleton.HandleEnableNeedsKeyboard(ap), command: "pluginbrowser.search  ");
			cui.CreateProtectedButton(container, search, null, string.IsNullOrEmpty(searchQuery) ? "0.2 0.2 0.2 0.8" : "#d43131", "1 1 1 0.6", "X", 10, xMin: 0.95f, yMin: 0.05f, yMax: 0.95f, OxMin: -30, OxMax: -22.5f, command: "pluginbrowser.search  ");

			var reloadButton = cui.CreateProtectedButton(container, search, null, isLocal ? "0.2 0.2 0.2 0.4" : "0.2 0.2 0.2 0.8", "1 1 1 0.6", string.Empty, 0, xMin: 0.875f, xMax: 1, yMin: 0.075f, yMax: 0.925f, command: "pluginbrowser.refreshvendor");
			cui.CreateImage(container, reloadButton, null, "reload", "1 1 1 0.4", xMin: 0.25f, xMax: 0.75f, yMin: 0.25f, yMax: 0.75f);

			if (TagFilter.Contains("peanus")) cui.CreateClientImage(container, grid, null, "https://media.discordapp.net/attachments/1078801277565272104/1085062151221293066/15ox1d_1.jpg?width=827&height=675", "1 1 1 1", xMax: 0.8f);
			if (TagFilter.Contains("banan")) cui.CreateClientImage(container, grid, null, "https://cf-images.us-east-1.prod.boltdns.net/v1/static/507936866/2cd498e2-da08-4305-a86e-f9711ac41615/eac8316f-0061-40ed-b289-aac0bab35da0/1280x720/match/image.jpg", "1 1 1 1", xMax: 0.8f);

			var selectedPlugin = ap.GetStorage<Plugin>(tab, "selectedplugin");

			if (selectedPlugin != null)
			{
				vendor.CheckMetadata(selectedPlugin.Id, () => { Singleton.Draw(ap.Player); });

				var mainPanel = cui.CreatePanel(container, parent, null, "0.15 0.15 0.15 0.35", blur: true);
				cui.CreatePanel(container, mainPanel, null, "0 0 0 0.9");

				var image = cui.CreatePanel(container, parent, null, "0 0 0 0.5", xMin: 0.08f, xMax: 0.45f, yMin: 0.15f, yMax: 0.85f);

				if (selectedPlugin.NoImage()) cui.CreateImage(container, image, null, vendor.Logo, "0.2 0.2 0.2 0.4", xMin: 0.2f, xMax: 0.8f, yMin: 0.2f + vendor.LogoRatio, yMax: 0.8f - vendor.LogoRatio);
				{
					if (Singleton.ImageDatabase.GetImage(selectedPlugin.Image) == 0) cui.CreateClientImage(container, image, null, selectedPlugin.Image, "1 1 1 1", xMin: 0.05f, xMax: 0.95f, yMin: 0.05f, yMax: 0.95f);
					else cui.CreateImage(container, image, null, selectedPlugin.Image, selectedPlugin.HasInvalidImage() ? vendor.SafeIconScale : vendor.IconScale, "1 1 1 1", xMin: 0.05f, xMax: 0.95f, yMin: 0.05f, yMax: 0.95f);
				}
				cui.CreateText(container, mainPanel, null, "1 1 1 1", selectedPlugin.Name, 25, xMin: 0.505f, yMax: 0.8f, align: TextAnchor.UpperLeft, font: Handler.FontTypes.RobotoCondensedBold);
				cui.CreateText(container, mainPanel, null, "1 1 1 0.5", $"by <b>{selectedPlugin.Author}</b>  <b>•</b>  v{selectedPlugin.Version}  <b>•</b>  Updated on {selectedPlugin.UpdateDate}  <b>•</b>  {selectedPlugin.DownloadCount:n0} downloads", 11, xMin: 0.48f, yMax: 0.74f, align: TextAnchor.UpperLeft);
				cui.CreateText(container, mainPanel, null, "1 1 1 0.3", $"{(string.IsNullOrEmpty(selectedPlugin.Description) ? "Fetching metdata..." : $"{selectedPlugin.Description}")}", 11, xMin: 0.48f, xMax: 0.85f, yMax: 0.635f, align: TextAnchor.UpperLeft);

				cui.CreateProtectedButton(container, mainPanel, null, "0 0 0 0", "0 0 0 0", string.Empty, 0, align: TextAnchor.MiddleCenter, command: "pluginbrowser.deselectplugin");

				var favouriteButton = cui.CreateProtectedButton(container, mainPanel, null, "0 0 0 0", "0 0 0 0", string.Empty, 0, xMin: 0.48f, xMax: 0.495f, yMin: 0.755f, yMax: 0.785f, command: $"pluginbrowser.interact 10 {selectedPlugin.File}");
				cui.CreateImage(container, favouriteButton, null, "star", ServerOwner.Singleton.FavouritePlugins.Contains(selectedPlugin.File) ? "0.9 0.8 0.4 0.95" : "0.2 0.2 0.2 0.4");

				#region Tags

				var tagOffset = 0f;
				var tagSpacing = 0.012f;
				var tags = cui.CreatePanel(container, mainPanel, null, "0 0 0 0", xMin: 0.48f, xMax: 0.8f, yMin: 0.66f, yMax: 0.7f);
				var tempTags = Pool.GetList<string>();
				var counter = 0;

				foreach (var tag in selectedPlugin.Tags)
				{
					counter += tag.Length;
					tempTags.Add(tag);

					if (counter > 50)
					{
						break;
					}
				}

				foreach (var tag in tempTags)
				{
					if (string.IsNullOrEmpty(tag)) continue;

					var size = ((float)tag.Length).Scale(0, 5, 0.03f, 0.125f);
					var bg = cui.CreateProtectedButton(container, tags, null, TagFilter.Contains(tag) ? "0.8 0.3 0.3 0.8" : "0.2 0.2 0.2 0.4", "1 1 1 1", tag.ToUpper(), 8, xMin: tagOffset, xMax: tagOffset + size, command: $"pluginbrowser.tagfilter {tag}");

					tagOffset += size + tagSpacing;
				}

				Pool.FreeList(ref tempTags);

				#endregion

				cui.CreateProtectedButton(container, mainPanel, null, "#2f802f", "1 1 1 1", "<", 10, align: TextAnchor.MiddleCenter, command: "pluginbrowser.changeselectedplugin -1", xMin: 0f, xMax: 0.02f, yMin: 0.45f, yMax: 0.55f);
				cui.CreateProtectedButton(container, mainPanel, null, "#2f802f", "1 1 1 1", ">", 10, align: TextAnchor.MiddleCenter, command: "pluginbrowser.changeselectedplugin 1", xMin: 0.98f, xMax: 1f, yMin: 0.45f, yMax: 0.55f);

				cui.CreateProtectedButton(container, parent: mainPanel, id: null,
					color: "0.6 0.2 0.2 0.9",
					textColor: "1 0.5 0.5 1",
					text: "X", 9,
					xMin: 0.965f, xMax: 0.99f, yMin: 0.955f, yMax: 0.99f,
					command: "pluginbrowser.deselectplugin",
					font: Handler.FontTypes.DroidSansMono);


				if (Singleton.HasAccessLevel(ap.Player, 3))
				{
					var buttonColor = string.Empty;
					var elementColor = string.Empty;
					var icon = string.Empty;
					var status = string.Empty;
					var scale = 0f;
					var callMode = 0;

					if (!selectedPlugin.IsInstalled())
					{
						if (selectedPlugin.IsPaid())
						{
							buttonColor = "#2f802f";
							elementColor = "#75f475";
							icon = "shopping";
							status = "BUY NOW";
							scale = 0.572f;
							callMode = 3;
						}
						else
						{
							if (!selectedPlugin.IsBusy)
							{
								buttonColor = "#2f802f";
								elementColor = "#75f475";
								status = "DOWNLOAD";
								icon = "clouddl";
								scale = 0.595f;
								callMode = 0;
							}
							else
							{
								buttonColor = "#78772e";
								elementColor = "#c3bd5b";
								icon = "clouddl";
								status = "IN PROGRESS";
								scale = 0.595f;
							}
						}
					}
					else
					{
						if (selectedPlugin.IsUpToDate())
						{
							buttonColor = "#802f2f";
							elementColor = "#c35b5b";
							icon = "trashcan";
							status = "REMOVE";
							scale = 0.564f;
							callMode = 2;
						}
						else
						{
							buttonColor = "#2f802f";
							elementColor = "#75f475";
							icon = "clouddl";
							status = "UPDATE";
							scale = 0.564f;
							callMode = 1;
						}
					}

					if (!selectedPlugin.IsPaid() || selectedPlugin.IsInstalled())
					{
						var button = cui.CreateProtectedButton(container, mainPanel, null, buttonColor, "0 0 0 0", string.Empty, 0, xMin: 0.48f, xMax: scale, yMin: 0.175f, yMax: 0.235f, align: TextAnchor.MiddleRight, command: selectedPlugin.IsBusy ? "" : $"pluginbrowser.interact {callMode} {selectedPlugin.Id}");
						cui.CreateText(container, button, null, "1 1 1 0.7", status, 11, xMax: 0.88f, align: TextAnchor.MiddleRight);
						cui.CreateImage(container, button, null, icon, elementColor, xMin: 0.1f, xMax: 0.3f, yMin: 0.2f, yMax: 0.8f);
					}
					if (selectedPlugin.IsInstalled())
					{
						var path = Path.Combine(Core.Defines.GetConfigsFolder(), selectedPlugin.ExistentPlugin.Config.Filename);

						if (OsEx.File.Exists(path)) cui.CreateProtectedButton(container, mainPanel, null, "0.1 0.1 0.1 0.8", "1 1 1 0.7", "EDIT CONFIG", 11, xMin: 0.48f, xMax: 0.564f, yMin: 0.175f, yMax: 0.235f, OyMin: 35, OyMax: 35, command: selectedPlugin.IsBusy ? "" : $"pluginbrowser.interact 3 {selectedPlugin.Id}");
					}
				}
			}

			Pool.FreeList(ref plugins);
		}

		#region Vendor

		public interface IVendorStored
		{
			bool Load();
			void Save();
		}
		public interface IVendorAuthenticated
		{

		}
		public interface IVendorDownloader : IVendorStored
		{
			string Type { get; }
			string Url { get; }
			string Logo { get; }
			float LogoRatio { get; }

			string ListEndpoint { get; }
			string DownloadEndpoint { get; }
			string BarInfo { get; }

			float IconScale { get; }
			float SafeIconScale { get; }

			Plugin[] PriceData { get; set; }
			Plugin[] AuthorData { get; set; }
			Plugin[] InstalledData { get; set; }
			Plugin[] OutOfDateData { get; set; }
			Plugin[] OwnedData { get; set; }
			string[] PopularTags { get; set; }

			List<Plugin> FetchedPlugins { get; set; }

			void FetchList(Action<IVendorDownloader> callback = null);
			void Download(string id, Action onTimeout = null);
			void Uninstall(string id);
			void Refresh();
			void CheckMetadata(string id, Action callback);
		}

		[ProtoContract]
		public class Codefling : IVendorDownloader, IVendorAuthenticated
		{
			public string Type => "Codefling";
			public string Url => "https://codefling.com";
			public string Logo => "cflogo";
			public float LogoRatio => 0f;

			public float IconScale => 0.4f;
			public float SafeIconScale => 0.2f;

			public Plugin[] PriceData { get; set; }
			public Plugin[] AuthorData { get; set; }
			public Plugin[] InstalledData { get; set; }
			public Plugin[] OutOfDateData { get; set; }
			public Plugin[] OwnedData { get; set; }
			public string[] PopularTags { get; set; }

			public string BarInfo => $"{FetchedPlugins.Count(x => !x.IsPaid()):n0} free, {FetchedPlugins.Count(x => x.IsPaid()):n0} paid";

			public string ListEndpoint => "https://codefling.com/capi/category-2/?do=apicall";
			public string DownloadEndpoint => "https://codefling.com/files/file/[ID]-a?do=download";

			[ProtoMember(1)]
			public List<Plugin> FetchedPlugins { get; set; } = new();

			[ProtoMember(2)]
			public long LastTick { get; set; }

			public void Refresh()
			{
				if (FetchedPlugins == null) return;

				foreach (var plugin in FetchedPlugins)
				{
					var name = plugin.File;
					var length = 0;
					if ((length = name.LastIndexOf('.')) != 0)
					{
						name = name.Substring(0, length);
					}

					foreach (var existentPlugin in Community.Runtime.CorePlugin.plugins.GetAll())
					{
						if (existentPlugin.FileName == name)
						{
							plugin.ExistentPlugin = (RustPlugin)existentPlugin;
							break;
						}
					}
				}

				if (PriceData != null)
				{
					Array.Clear(PriceData, 0, PriceData.Length);
					Array.Clear(AuthorData, 0, AuthorData.Length);
					Array.Clear(InstalledData, 0, InstalledData.Length);
					PriceData = AuthorData = InstalledData = null;
				}

				PriceData = FetchedPlugins.OrderBy(x => x.OriginalPrice).ToArray();
				AuthorData = FetchedPlugins.OrderBy(x => x.Author).ToArray();
				InstalledData = FetchedPlugins.Where(x => x.IsInstalled()).ToArray();
				OutOfDateData = FetchedPlugins.Where(x => x.IsInstalled() && !x.IsUpToDate()).ToArray();

				var tags = Pool.GetList<string>();
				foreach (var plugin in FetchedPlugins)
				{
					foreach (var tag in plugin.Tags)
					{
						var processedTag = tag.ToLower().Trim();

						if (!tags.Contains(processedTag))
						{
							tags.Add(processedTag);
						}
					}
				}
				PopularTags = tags.ToArray();
				Pool.FreeList(ref tags);
			}
			public void FetchList(Action<IVendorDownloader> callback = null)
			{
				Community.Runtime.CorePlugin.webrequest.Enqueue(ListEndpoint, null, (error, data) =>
				{
					var list = JObject.Parse(data);

					FetchedPlugins.Clear();

					var file = list["file"];
					foreach (var plugin in file)
					{
						var p = new Plugin
						{
							Id = plugin["file_id"]?.ToString(),
							Name = plugin["file_name"]?.ToString(),
							Author = plugin["file_author"]?.ToString(),
							Description = plugin["file_description"]?.ToString(),
							Version = plugin["file_version"]?.ToString(),
							OriginalPrice = plugin["file_price"]?.ToString(),
							UpdateDate = plugin["file_updated"]?.ToString(),
							Changelog = plugin["file_changelogs"]?.ToString(),
							File = plugin["file_file_1"]?.ToString(),
							Image = plugin["file_image"]["url"]?.ToString(),
							ImageSize = (plugin["file_image"]["size"]?.ToString().ToInt()).GetValueOrDefault(),
							Tags = plugin["file_tags"]?.ToString().Split(','),
							DownloadCount = (plugin["file_downloads"]?.ToString().ToInt()).GetValueOrDefault()
						};

						var date = DateTimeOffset.FromUnixTimeSeconds(p.UpdateDate.ToLong());
						p.UpdateDate = date.UtcDateTime.ToString();

						try { p.Description = p.Description.TrimStart('\t').Replace("\t", "\n").Split('\n')[0]; } catch { }

						if (p.OriginalPrice == "{}") p.OriginalPrice = "FREE";
						try { p.ExistentPlugin = Community.Runtime.CorePlugin.plugins.GetAll().FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.FilePath) == Path.GetFileNameWithoutExtension(p.File)) as RustPlugin; } catch { }

						FetchedPlugins.Add(p);
					}

					callback?.Invoke(this);
					Logger.Log($"[{Type}] Downloaded JSON");

					OwnedData = InstalledData;

					Save();
				}, Community.Runtime.CorePlugin);
			}
			public void Download(string id, Action onTimeout = null)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				plugin.IsBusy = true;
				plugin.DownloadCount++;

				var path = Path.Combine(Core.Defines.GetScriptFolder(), plugin.File);
				var url = DownloadEndpoint.Replace("[ID]", id);

				Community.Runtime.CorePlugin.timer.In(2f, () =>
				{
					if (plugin.IsBusy)
					{
						plugin.IsBusy = false;
						onTimeout?.Invoke();
					}
				});

				Community.Runtime.CorePlugin.webrequest.Enqueue(url, null, (error, source) =>
				{
					plugin.IsBusy = false;

					if (!source.StartsWith("<!DOCTYPE html>"))
					{
						Singleton.Puts($"Downloaded {plugin.Name}");
						OsEx.File.Create(path, source);
					}
				}, Community.Runtime.CorePlugin, headers: new Dictionary<string, string>
				{
					["user-agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.63",
					["accept"] = "ext/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7"
				});
			}
			public void Uninstall(string id)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				OsEx.File.Move(plugin.ExistentPlugin.FilePath, Path.Combine(Core.Defines.GetScriptFolder(), "backups", $"{plugin.ExistentPlugin.FileName}.cs"), true);
				plugin.ExistentPlugin = null;
			}
			public void CheckMetadata(string id, Action onMetadataRetrieved)
			{

			}

			public bool Load()
			{
				try
				{
					var path = Path.Combine(Core.Defines.GetDataFolder(), "vendordata_cf.db");
					if (!OsEx.File.Exists(path)) return false;

					using var file = File.OpenRead(path);
					var value = Serializer.Deserialize<Codefling>(file);

					LastTick = value.LastTick;
					FetchedPlugins.Clear();
					FetchedPlugins.AddRange(value.FetchedPlugins);

					if ((DateTime.Now - new DateTime(value.LastTick)).TotalHours >= 24)
					{
						Singleton.Puts($"Invalidated {Type} database. Fetching...");
						return false;
					}

					OwnedData = InstalledData;

					Singleton.Puts($"Loaded {Type} from file: {path}");
					Refresh();
				}
				catch { Save(); }
				return true;
			}
			public void Save()
			{
				try
				{
					var path = Path.Combine(Core.Defines.GetDataFolder(), "vendordata_cf.db");
					using var file = File.OpenWrite(path);
					LastTick = DateTime.Now.Ticks;
					Serializer.Serialize(file, this);
					Singleton.Puts($"Stored {Type} to file: {path}");
				}
				catch { }
			}
		}

		[ProtoContract]
		public class uMod : IVendorDownloader
		{
			public string Type => "uMod";
			public string Url => "https://umod.org";
			public string Logo => "umodlogo";
			public float LogoRatio => 0.2f;

			public float IconScale => 1f;
			public float SafeIconScale => 1f;

			public Plugin[] PriceData { get; set; }
			public Plugin[] AuthorData { get; set; }
			public Plugin[] InstalledData { get; set; }
			public Plugin[] OutOfDateData { get; set; }
			public Plugin[] OwnedData { get; set; }
			public string[] PopularTags { get; set; }

			public string BarInfo => $"{FetchedPlugins.Count:n0} free";

			public string ListEndpoint => "https://umod.org/plugins/search.json?page=[ID]&sort=title&sortdir=asc&categories%5B0%5D=universal&categories%5B1%5D=rust";
			public string DownloadEndpoint => "https://umod.org/plugins/[ID].cs";
			public string PluginLookupEndpoint => "https://umod.org/plugins/[ID]/latest.json";

			[ProtoMember(1)]
			public List<Plugin> FetchedPlugins { get; set; } = new();

			[ProtoMember(2)]
			public long LastTick { get; set; }

			public void Refresh()
			{
				if (FetchedPlugins == null) return;

				foreach (var plugin in FetchedPlugins)
				{
					var name = plugin.File.Substring(0, plugin.File.IndexOf(".cs"));
					foreach (var existentPlugin in Community.Runtime.CorePlugin.plugins.GetAll())
					{
						if (existentPlugin.FileName == name)
						{
							plugin.ExistentPlugin = (RustPlugin)existentPlugin;
							break;
						}
					}
				}

				if (PriceData != null)
				{
					Array.Clear(PriceData, 0, PriceData.Length);
					Array.Clear(AuthorData, 0, AuthorData.Length);
					Array.Clear(InstalledData, 0, InstalledData.Length);
					PriceData = AuthorData = InstalledData = null;
				}

				PriceData = FetchedPlugins.OrderBy(x => x.OriginalPrice).ToArray();
				AuthorData = FetchedPlugins.OrderBy(x => x.Author).ToArray();
				InstalledData = FetchedPlugins.Where(x => x.IsInstalled()).ToArray();
				OutOfDateData = FetchedPlugins.Where(x => x.IsInstalled() && !x.IsUpToDate()).ToArray();

				var tags = Pool.GetList<string>();
				foreach (var plugin in FetchedPlugins)
				{
					foreach (var tag in plugin.Tags)
					{
						var processedTag = tag.ToLower().Trim();

						if (!tags.Contains(processedTag))
						{
							tags.Add(processedTag);
						}
					}
				}
				PopularTags = tags.ToArray();
				Pool.FreeList(ref tags);
			}
			public void FetchList(Action<IVendorDownloader> callback = null)
			{
				FetchedPlugins.Clear();

				Community.Runtime.CorePlugin.webrequest.Enqueue(ListEndpoint.Replace("[ID]", "0"), null, (error, data) =>
				{
					var list = JObject.Parse(data);

					FetchPage(0, (list["last_page"]?.ToString().ToInt()).GetValueOrDefault(), callback);
					list = null;
				}, Community.Runtime.CorePlugin);
			}
			public void Download(string id, Action onTimeout = null)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				var path = Path.Combine(Core.Defines.GetScriptFolder(), plugin.File);
				var url = DownloadEndpoint.Replace("[ID]", plugin.Name);

				plugin.IsBusy = true;

				Community.Runtime.CorePlugin.timer.In(2f, () =>
				{
					if (plugin.IsBusy)
					{
						plugin.IsBusy = false;
						onTimeout?.Invoke();
					}
				});

				Community.Runtime.CorePlugin.webrequest.Enqueue(url, null, (error, source) =>
				{
					Singleton.Puts($"Downloaded {plugin.Name}");
					OsEx.File.Create(path, source);

					plugin.IsBusy = false;
					plugin.DownloadCount++;

				}, Community.Runtime.CorePlugin, headers: new Dictionary<string, string>
				{
					["user-agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.63",
					["accept"] = "ext/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7"
				});
			}
			public void Uninstall(string id)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				OsEx.File.Move(plugin.ExistentPlugin.FilePath, Path.Combine(Core.Defines.GetScriptFolder(), "backups", $"{plugin.ExistentPlugin.FileName}.cs"), true);
				plugin.ExistentPlugin = null;
			}
			public void CheckMetadata(string id, Action onMetadataRetrieved)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				if (plugin.HasLookup) return;

				Community.Runtime.CorePlugin.webrequest.Enqueue(PluginLookupEndpoint.Replace("[ID]", plugin.Name.ToLower().Trim()), null, (error, data) =>
				{
					var list = JObject.Parse(data);
					var description = list["description_md"]?.ToString();

					plugin.Changelog = description
						.Replace("<div>", "").Replace("</div>", "")
						.Replace("\\n", "")
						.Replace("<br />", "\n")
						.Replace("<pre>", "")
						.Replace("<p>", "")
						.Replace("</p>", "")
						.Replace("<span class=\"documentation\">", "")
						.Replace("</span>", "")
						.Replace("<code>", "<b>")
						.Replace("</code>", "</b>")
						.Replace("<ul>", "").Replace("</ul>", "")
						.Replace("<li>", "").Replace("</li>", "")
						.Replace("<em>", "").Replace("</em>", "")
						.Replace("<h1>", "<b>").Replace("</h1>", "</b>")
						.Replace("<h2>", "<b>").Replace("</h2>", "</b>")
						.Replace("<h3>", "<b>").Replace("</h3>", "</b>")
						.Replace("<h4>", "<b>").Replace("</h4>", "</b>")
						.Replace("<strong>", "<b>").Replace("</strong>", "</b>");

					if (!string.IsNullOrEmpty(plugin.Changelog) && !plugin.Changelog.EndsWith(".")) plugin.Changelog = plugin.Changelog.Trim() + ".";

					plugin.HasLookup = true;
					onMetadataRetrieved?.Invoke();
				}, Community.Runtime.CorePlugin);
			}

			public void FetchPage(int page, int maxPage, Action<IVendorDownloader> callback = null)
			{
				if (page > maxPage)
				{
					Save();
					callback?.Invoke(this);
					return;
				}

				Community.Runtime.CorePlugin.webrequest.Enqueue(ListEndpoint.Replace("[ID]", $"{page}"), null, (error, data) =>
				{
					var list = JObject.Parse(data);

					var file = list["data"];
					foreach (var plugin in file)
					{
						var p = new Plugin
						{
							Id = plugin["url"]?.ToString(),
							Name = plugin["name"]?.ToString(),
							Author = plugin["author"]?.ToString(),
							Version = plugin["latest_release_version"]?.ToString(),
							Description = plugin["description"]?.ToString(),
							OriginalPrice = "FREE",
							File = $"{plugin["name"]?.ToString()}.cs",
							Image = plugin["icon_url"]?.ToString(),
							ImageSize = 0,
							DownloadCount = (plugin["downloads"]?.ToString().ToInt()).GetValueOrDefault(),
							UpdateDate = plugin["updated_at"]?.ToString(),
							Tags = plugin["tags_all"]?.ToString().Split(',')
						};

						if (!string.IsNullOrEmpty(p.Description) && !p.Description.EndsWith(".")) p.Description += ".";

						if (string.IsNullOrEmpty(p.Author.Trim())) p.Author = "Unmaintained";
						if (p.OriginalPrice == "{}") p.OriginalPrice = "FREE";
						try { p.ExistentPlugin = Community.Runtime.CorePlugin.plugins.GetAll().FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.FilePath) == Path.GetFileNameWithoutExtension(p.File)) as RustPlugin; } catch { }

						if (!FetchedPlugins.Any(x => x.Name == p.Name)) FetchedPlugins.Add(p);
					}

					if (page % (maxPage / 4) == 0 || page == maxPage - 1)
					{
						Logger.Log($"[{Type}] Downloaded {page} out of {maxPage}");
					}

					OwnedData = InstalledData;

					list = null;
				}, Community.Runtime.CorePlugin);
				Community.Runtime.CorePlugin.timer.In(5f, () => FetchPage(page + 1, maxPage, callback));
			}

			public bool Load()
			{
				try
				{
					var path = Path.Combine(Core.Defines.GetDataFolder(), "vendordata_umod.db");
					if (!OsEx.File.Exists(path)) return false;

					using var file = File.OpenRead(path);
					var value = Serializer.Deserialize<uMod>(file);

					LastTick = value.LastTick;
					FetchedPlugins.Clear();
					FetchedPlugins.AddRange(value.FetchedPlugins);

					if ((DateTime.Now - new DateTime(value.LastTick)).TotalHours >= 24)
					{
						Singleton.Puts($"Invalidated {Type} database. Fetching...");
						return false;
					}

					OwnedData = InstalledData;

					Singleton.Puts($"Loaded {Type} from file: {path}");
					Refresh();
				}
				catch { Save(); }
				return true;
			}
			public void Save()
			{
				try
				{
					var path = Path.Combine(Core.Defines.GetDataFolder(), "vendordata_umod.db");
					using var file = File.OpenWrite(path);

					LastTick = DateTime.Now.Ticks;
					Serializer.Serialize(file, this);
					Singleton.Puts($"Stored {Type} to file: {path}");
				}
				catch { }
			}
		}

		[ProtoContract]
		public class Lone_Design : IVendorDownloader, IVendorAuthenticated
		{
			public string Type => "Lone.Design";
			public string Url => "https://lone.design";
			public string Logo => "lonelogo";
			public float LogoRatio => 0f;

			public float IconScale => 0.4f;
			public float SafeIconScale => 0.2f;

			public Plugin[] PriceData { get; set; }
			public Plugin[] AuthorData { get; set; }
			public Plugin[] InstalledData { get; set; }
			public Plugin[] OutOfDateData { get; set; }
			public Plugin[] OwnedData { get; set; }
			public string[] PopularTags { get; set; }

			public string BarInfo => $"{FetchedPlugins.Count(x => !x.IsPaid()):n0} free, {FetchedPlugins.Count(x => x.IsPaid()):n0} paid";

			public string ListEndpoint => "https://api.lone.design/carbon.json";
			public string DownloadEndpoint => "https://codefling.com/files/file/[ID]-a?do=download";

			[ProtoMember(1)]
			public List<Plugin> FetchedPlugins { get; set; } = new();

			[ProtoMember(2)]
			public long LastTick { get; set; }

			public void Refresh()
			{
				if (FetchedPlugins == null) return;

				foreach (var plugin in FetchedPlugins)
				{
					foreach (var existentPlugin in Community.Runtime.CorePlugin.plugins.GetAll())
					{
						if (existentPlugin.FileName == plugin.File)
						{
							plugin.ExistentPlugin = (RustPlugin)existentPlugin;
							break;
						}
					}
				}

				if (PriceData != null)
				{
					Array.Clear(PriceData, 0, PriceData.Length);
					Array.Clear(AuthorData, 0, AuthorData.Length);
					Array.Clear(InstalledData, 0, InstalledData.Length);
					PriceData = AuthorData = InstalledData = null;
				}

				PriceData = FetchedPlugins.OrderBy(x => x.OriginalPrice).ToArray();
				AuthorData = FetchedPlugins.OrderBy(x => x.Author).ToArray();
				InstalledData = FetchedPlugins.Where(x => x.IsInstalled()).ToArray();
				OutOfDateData = FetchedPlugins.Where(x => x.IsInstalled() && !x.IsUpToDate()).ToArray();

				var tags = Pool.GetList<string>();
				foreach (var plugin in FetchedPlugins)
				{
					if (plugin.Tags == null) continue;

					foreach (var tag in plugin.Tags)
					{
						var processedTag = tag?.ToLower().Trim();

						if (!tags.Contains(processedTag))
						{
							tags.Add(processedTag);
						}
					}
				}
				PopularTags = tags.ToArray();
				Pool.FreeList(ref tags);
			}
			public void FetchList(Action<IVendorDownloader> callback = null)
			{
				Community.Runtime.CorePlugin.webrequest.Enqueue(ListEndpoint, null, (error, data) =>
				{
					var list = JToken.Parse(data);

					FetchedPlugins.Clear();

					foreach (var plugin in list)
					{
						var p = new Plugin
						{
							Id = plugin["url"]?.ToString(),
							Name = plugin["name"]?.ToString(),
							Author = plugin["author"]?.ToString(),
							Description = plugin["description"]?.ToString(),
							Version = plugin["version"]?.ToString(),
							OriginalPrice = $"${plugin["price"]?.ToString()}",
							SalePrice = $"${plugin["salePrice"]?.ToString()}",
							File = plugin["filename"]?.ToString(),
							Image = plugin["images"][0]["src"]?.ToString(),
							Tags = plugin["tags"]?.Select(x => x["name"]?.ToString())?.ToArray(),
						};

						if (p.OriginalPrice == "$" || p.OriginalPrice == "$0") p.OriginalPrice = "FREE";
						if (p.SalePrice == "$" || p.SalePrice == "$0") p.SalePrice = "FREE";

						var date = DateTimeOffset.FromUnixTimeSeconds(p.UpdateDate.ToLong());
						p.UpdateDate = date.UtcDateTime.ToString();

						try { p.Description = p.Description.TrimStart('\t').Replace("\t", "\n").Split('\n')[0]; } catch { }

						try { p.ExistentPlugin = Community.Runtime.CorePlugin.plugins.GetAll().FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.FilePath) == Path.GetFileNameWithoutExtension(p.File)) as RustPlugin; } catch { }

						FetchedPlugins.Add(p);
					}

					callback?.Invoke(this);
					Logger.Log($"[{Type}] Downloaded JSON");

					OwnedData = InstalledData;

					Save();
				}, Community.Runtime.CorePlugin);
			}
			public void Download(string id, Action onTimeout = null)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				plugin.IsBusy = true;
				plugin.DownloadCount++;

				var path = Path.Combine(Core.Defines.GetScriptFolder(), plugin.File);
				var url = DownloadEndpoint.Replace("[ID]", id);

				Community.Runtime.CorePlugin.timer.In(2f, () =>
				{
					if (plugin.IsBusy)
					{
						plugin.IsBusy = false;
						onTimeout?.Invoke();
					}
				});

				Community.Runtime.CorePlugin.webrequest.Enqueue(url, null, (error, source) =>
				{
					plugin.IsBusy = false;

					if (!source.StartsWith("<!DOCTYPE html>"))
					{
						Singleton.Puts($"Downloaded {plugin.Name}");
						OsEx.File.Create(path, source);
					}
				}, Community.Runtime.CorePlugin, headers: new Dictionary<string, string>
				{
					["user-agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/110.0.0.0 Safari/537.36 Edg/110.0.1587.63",
					["accept"] = "ext/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7"
				});
			}
			public void Uninstall(string id)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				OsEx.File.Move(plugin.ExistentPlugin.FilePath, Path.Combine(Core.Defines.GetScriptFolder(), "backups", $"{plugin.ExistentPlugin.FileName}.cs"), true);
				plugin.ExistentPlugin = null;
			}
			public void CheckMetadata(string id, Action onMetadataRetrieved)
			{

			}

			public bool Load()
			{
				try
				{
					var path = Path.Combine(Core.Defines.GetDataFolder(), "vendordata_lone.db");
					if (!OsEx.File.Exists(path)) return false;

					using var file = File.OpenRead(path);
					var value = Serializer.Deserialize<Codefling>(file);

					LastTick = value.LastTick;
					FetchedPlugins.Clear();
					FetchedPlugins.AddRange(value.FetchedPlugins);

					if ((DateTime.Now - new DateTime(value.LastTick)).TotalHours >= 24)
					{
						Singleton.Puts($"Invalidated {Type} database. Fetching...");
						return false;
					}

					OwnedData = InstalledData;

					Singleton.Puts($"Loaded {Type} from file: {path}");
					Refresh();
				}
				catch { Save(); }
				return true;
			}
			public void Save()
			{
				try
				{
					var path = Path.Combine(Core.Defines.GetDataFolder(), "vendordata_lone.db");
					using var file = File.OpenWrite(path);
					LastTick = DateTime.Now.Ticks;
					Serializer.Serialize(file, this);
					Singleton.Puts($"Stored {Type} to file: {path}");
				}
				catch { }
			}
		}

		[ProtoContract]
		public class Local : IVendorDownloader
		{
			public string Type => "All";
			public string Url => "none";
			public string Logo => "carbonw";

			public float LogoRatio => 0.23f;
			public string ListEndpoint => string.Empty;
			public string DownloadEndpoint => string.Empty;
			public string BarInfo => $"{FetchedPlugins.Count:n0} loaded";
			public float IconScale => 0.4f;
			public float SafeIconScale => 0.2f;

			public List<Plugin> FetchedPlugins { get; set; } = new();

			internal string[] _defaultTags = new string[] { "carbon", "oxide" };

			public Plugin[] PriceData { get; set; }
			public Plugin[] AuthorData { get; set; }
			public Plugin[] InstalledData { get; set; }
			public Plugin[] OutOfDateData { get; set; }
			public Plugin[] OwnedData { get; set; }
			public string[] PopularTags { get; set; }

			public void CheckMetadata(string id, Action callback)
			{
			}

			public void Download(string id, Action onTimeout = null)
			{
			}

			public void FetchList(Action<IVendorDownloader> callback = null)
			{
			}

			public bool Load()
			{
				return true;
			}

			public void Refresh()
			{
				FetchedPlugins.Clear();

				foreach (var package in ModLoader.LoadedPackages)
				{
					foreach (var plugin in package.Plugins)
					{
						if (plugin.IsCorePlugin) continue;

						var existent = FetchedPlugins.FirstOrDefault(x => x.ExistentPlugin == plugin);

						if (existent == null) FetchedPlugins.Add(CodeflingInstance.FetchedPlugins.FirstOrDefault(x => x.ExistentPlugin == plugin) ??
							uModInstance.FetchedPlugins.FirstOrDefault(x => x.ExistentPlugin == plugin)
							?? (existent = new Plugin
							{
								Name = plugin.Name,
								Author = plugin.Author,
								Version = plugin.Version.ToString(),
								ExistentPlugin = plugin,
								Description = "This is an unlisted plugin.",
								Tags = _defaultTags,
								File = plugin.FileName,
								Id = plugin.Name,
								UpdateDate = DateTime.UtcNow.ToString()
							}));
					}
				}

				FetchedPlugins = FetchedPlugins.OrderBy(x => x.Name).ToList();
				PriceData = FetchedPlugins.OrderBy(x => x.OriginalPrice).ToArray();
				AuthorData = FetchedPlugins.OrderBy(x => x.Author).ToArray();
				InstalledData = FetchedPlugins.Where(x => x.IsInstalled()).ToArray();
				OutOfDateData = FetchedPlugins.Where(x => x.IsInstalled() && !x.IsUpToDate()).ToArray();
			}

			public void Save()
			{
			}

			public void Uninstall(string id)
			{
				var plugin = FetchedPlugins.FirstOrDefault(x => x.Id == id);
				OsEx.File.Move(plugin.ExistentPlugin.FilePath, Path.Combine(Core.Defines.GetScriptFolder(), "backups", $"{plugin.ExistentPlugin.FileName}.cs"), true);
				plugin.ExistentPlugin = null;
			}
		}

		[ProtoContract]
		public class ServerOwner
		{
			public static ServerOwner Singleton { get; internal set; } = new ServerOwner();

			[ProtoMember(1)]
			public List<string> FavouritePlugins { get; set; } = new List<string>();

			[ProtoMember(2)]
			public List<string> AutoUpdate { get; set; } = new();

			public static void Load()
			{
				try
				{
					var path = Path.Combine(Core.Defines.GetDataFolder(), "vendordata_owner.db");
					if (!OsEx.File.Exists(path))
					{
						Save();
						return;
					}

					using var file = File.OpenRead(path);
					Singleton = Serializer.Deserialize<ServerOwner>(file);

					Singleton.FavouritePlugins ??= new();
					Singleton.AutoUpdate ??= new();
				}
				catch
				{
					Singleton = new();
					Save();
				}
			}
			public static void Save()
			{
				try
				{
					var path = Path.Combine(Core.Defines.GetDataFolder(), "vendordata_owner.db");
					using var file = File.OpenWrite(path);

					Serializer.Serialize(file, Singleton);
				}
				catch { }
			}
		}

		[ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
		public class Plugin
		{
			public string Id { get; set; }
			public string Name { get; set; }
			public string Author { get; set; }
			public string Version { get; set; }
			public string Description { get; set; }
			public string Changelog { get; set; }
			public string OriginalPrice { get; set; }
			public string SalePrice { get; set; }
			public string[] Dependencies { get; set; }
			public string File { get; set; }
			public string Image { get; set; }
			public int ImageSize { get; set; }
			public string[] Tags { get; set; }
			public int DownloadCount { get; set; }
			public string UpdateDate { get; set; }
			public bool HasLookup { get; set; } = false;

			internal RustPlugin ExistentPlugin { get; set; }
			internal bool IsBusy { get; set; }

			public bool HasInvalidImage()
			{
				return ImageSize >= 2504304 || Image.EndsWith(".gif");
			}
			public bool NoImage()
			{
				return string.IsNullOrEmpty(Image) || Image.EndsWith(".gif");
			}
			public bool IsInstalled()
			{
				return ExistentPlugin != null;
			}
			public string CurrentVersion()
			{
				if (ExistentPlugin == null) return "N/A";

				return ExistentPlugin.Version.ToString();
			}
			public bool IsPaid()
			{
				return OriginalPrice != "FREE";
			}
			public bool IsUpToDate()
			{
				if (!IsInstalled()) return false;

				return ExistentPlugin.Version.ToString() == Version;
			}
		}

		#endregion
	}

	#region Administration - Custom Commands

	[ProtectedCommand("carbongg.endspectate")]
	private void EndSpectate(Arg arg)
	{
		StopSpectating(arg.Player());
	}

	#endregion

	#region Plugins - Custom Commands

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

	[ProtectedCommand("pluginbrowser.changetab")]
	private void PluginBrowserChange(Arg args)
	{
		var ap = GetPlayerSession(args.Player());
		var tab = GetTab(ap.Player);
		var vendor2 = ap.SetStorage(tab, "vendor", args.Args[0]);

		var vendor = PluginsTab.GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), vendor2));
		vendor.Refresh();
		PluginsTab.TagFilter.Clear();

		PluginsTab.DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));
		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.interact")]
	private void PluginBrowserInteract(Arg args)
	{
		var ap = GetPlayerSession(args.Player());
		var tab = GetTab(ap.Player);

		var vendor = PluginsTab.GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage<string>(tab, "vendor", "Local")));

		switch (args.Args[0])
		{
			case "0":
				vendor.Download(args.Args[1], () => Singleton.Draw(args.Player()));
				break;
			case "1":
				tab.CreateDialog($"Are you sure you want to update '{ap.GetStorage<PluginsTab.Plugin>(tab, "selectedplugin").Name}'?", ap =>
				{
					vendor.Download(args.Args[1], () => Singleton.Draw(args.Player()));
				}, null);
				break;

			case "2":
				tab.CreateDialog($"Are you sure you want to uninstall '{ap.GetStorage<PluginsTab.Plugin>(tab, "selectedplugin").Name}'?", ap =>
				{
					vendor.Uninstall(args.Args[1]);
				}, null);
				break;

			case "3":
				var plugin = vendor.FetchedPlugins.FirstOrDefault(x => x.Id == args.Args[1]).ExistentPlugin;
				var path = Path.Combine(Core.Defines.GetConfigsFolder(), plugin.Config.Filename);
				Singleton.SetTab(ap.Player, ConfigEditor.Make(OsEx.File.ReadText(path),
					(ap, jobject) =>
					{
						Community.Runtime.CorePlugin.NextTick(() => SetTab(ap.Player, "plugins", false));
					},
					(ap, jobject) =>
					{
						OsEx.File.Create(path, jobject.ToString(Formatting.Indented));
						Community.Runtime.CorePlugin.NextTick(() => SetTab(ap.Player, "plugins", false));
					},
					(ap, jobject) =>
					{
						OsEx.File.Create(path, jobject.ToString(Formatting.Indented));
						plugin.ProcessorInstance.SetDirty();
						Community.Runtime.CorePlugin.NextTick(() => SetTab(ap.Player, "plugins", false));
					}));
				break;

			case "10":
				var pluginName = args.Args.Skip(1).ToArray().ToString(" ");
				if (PluginsTab.ServerOwner.Singleton.FavouritePlugins.Contains(pluginName))
					PluginsTab.ServerOwner.Singleton.FavouritePlugins.Remove(pluginName);
				else PluginsTab.ServerOwner.Singleton.FavouritePlugins.Add(pluginName);
				break;
		}

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.page")]
	private void PluginBrowserPage(Arg args)
	{
		var ap = GetPlayerSession(args.Player());
		var tab = GetTab(ap.Player);

		var vendor = PluginsTab.GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage<string>(tab, "vendor", "Local")));
		vendor.Refresh();
		PluginsTab.GetPlugins(vendor, tab, ap, out var maxPages);

		var page = ap.GetStorage<int>(tab, "page", 0);

		switch (args.Args[0])
		{
			case "+1":
				page++;
				break;
			case "-1":
				page--;
				break;

			default:
				page = args.Args[0].ToInt() - 1;
				break;
		}

		if (page < 0) page = maxPages;
		else if (page > maxPages) page = 0;

		ap.SetStorage(tab, "page", page);

		PluginsTab.DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.filter")]
	private void PluginBrowserFilter(Arg args)
	{
		var ap = GetPlayerSession(args.Player());
		var tab = GetTab(ap.Player);

		var vendor = PluginsTab.GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage<string>(tab, "vendor", "Local")));
		vendor.Refresh();

		var filter = ap.GetStorage<PluginsTab.FilterTypes>(tab, "filter");
		var flip = ap.GetStorage<bool>(tab, "flipfilter");
		if ((int)filter == args.Args[0].ToInt()) ap.SetStorage(tab, "flipfilter", !flip); else { ap.SetStorage(tab, "flipfilter", false); }

		ap.SetStorage(tab, "page", 0);
		ap.SetStorage(tab, "filter", (PluginsTab.FilterTypes)args.Args[0].ToInt());

		PluginsTab.DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.tagfilter")]
	private void PluginBrowserTagFilter(Arg args)
	{
		var ap = GetPlayerSession(args.Player());
		var tab = GetTab(ap.Player);

		var vendor = PluginsTab.GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage<string>(tab, "vendor", "Local")));
		vendor.Refresh();

		var filter = args.Args.ToString(" ");

		if (PluginsTab.TagFilter.Contains(filter)) PluginsTab.TagFilter.Remove(filter);
		else PluginsTab.TagFilter.Add(filter);

		PluginsTab.DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.search")]
	private void PluginBrowserSearch(Arg args)
	{
		var ap = GetPlayerSession(args.Player());
		var tab = GetTab(ap.Player);

		var vendor = PluginsTab.GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage<string>(tab, "vendor", "Local")));
		vendor.Refresh();

		var search = ap.SetStorage(tab, "search", args.Args.ToString(" "));
		var page = ap.SetStorage(tab, "page", 0);

		if (search == "Search...") ap.SetStorage(tab, "search", string.Empty);

		PluginsTab.DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.refreshvendor")]
	private void PluginBrowserRefreshVendor(Arg args)
	{
		var ap = GetPlayerSession(args.Player());
		var tab = GetTab(ap.Player);

		var vendor = PluginsTab.GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage<string>(tab, "vendor", "Local")));

		if (vendor is PluginsTab.Local) return;

		tab.CreateDialog("Are you sure you want to redownload the plugin list?\nThis might take a while.", ap =>
		{
			var id = string.Empty;
			switch (vendor)
			{
				case PluginsTab.Codefling:
					id = "cf";
					break;

				case PluginsTab.uMod:
					id = "umod";
					break;
			}

			var dataPath = Path.Combine(Core.Defines.GetDataFolder(), $"vendordata_{id}.db");
			OsEx.File.Delete(dataPath);

			if (!vendor.Load())
			{
				vendor.FetchList();
				vendor.Refresh();
			}

			Singleton.Draw(args.Player());
		}, null);

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.selectplugin")]
	private void PluginBrowserSelectPlugin(Arg args)
	{
		var ap = GetPlayerSession(args.Player());
		var tab = GetTab(ap.Player);

		var vendor = PluginsTab.GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage<string>(tab, "vendor", "Local")));
		vendor.Refresh();

		ap.SetStorage(tab, "selectedplugin", vendor.FetchedPlugins.FirstOrDefault(x => x.Id == args.Args[0]));

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.deselectplugin")]
	private void PluginBrowserDeselectPlugin(Arg args)
	{
		var ap = GetPlayerSession(args.Player());
		var tab = GetTab(ap.Player);

		var vendor = PluginsTab.GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage<string>(tab, "vendor", "Local")));
		vendor.Refresh();

		ap.SetStorage(tab, "selectedplugin", (PluginsTab.Plugin)null);

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.changeselectedplugin")]
	private void PluginBrowserChangeSelected(Arg args)
	{
		var ap = GetPlayerSession(args.Player());
		var tab = GetTab(ap.Player);

		var vendor = PluginsTab.GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage<string>(tab, "vendor", "Local")));
		vendor.Refresh();

		var plugins = PluginsTab.GetPlugins(vendor, tab, ap);
		var nextPage = plugins.IndexOf(ap.GetStorage<PluginsTab.Plugin>(tab, "selectedplugin")) + args.Args[0].ToInt();
		ap.SetStorage(tab, "selectedplugin", plugins[nextPage > plugins.Count - 1 ? 0 : nextPage < 0 ? plugins.Count - 1 : nextPage]);
		Pool.FreeList(ref plugins);

		PluginsTab.DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));

		Singleton.Draw(args.Player());
	}
	[ProtectedCommand("pluginbrowser.changesetting")]
	private void PluginBrowserChangeSetting(Arg args)
	{
		var ap = GetPlayerSession(args.Player());
		var tab = GetTab(ap.Player);
		var vendor = PluginsTab.GetVendor((PluginsTab.VendorTypes)Enum.Parse(typeof(PluginsTab.VendorTypes), ap.GetStorage<string>(tab, "vendor", "Local")));

		switch (args.Args[0])
		{
			case "filter_dd":
				PluginsTab.DropdownShow = !PluginsTab.DropdownShow;

				if (args.HasArgs(4))
				{
					if ((int)ap.GetStorage<PluginsTab.FilterTypes>(tab, "filter", PluginsTab.FilterTypes.None) == args.Args[3].ToInt()) ap.SetStorage(tab, "flipstorage", !ap.GetStorage<bool>(tab, "flipstorage", false)); else { ap.SetStorage(tab, "flipstorage", false); }

					ap.SetStorage(tab, "page", 0);
					ap.SetStorage(tab, "filter", (PluginsTab.FilterTypes)args.Args[3].ToInt());
				}
				break;
		}

		PluginsTab.DownloadThumbnails(vendor, tab, Singleton.GetPlayerSession(args.Player()));

		vendor.Refresh();
		Singleton.Draw(args.Player());
	}

	[ConsoleCommand("adminmodule.downloadplugin", "Downloads a plugin from a vendor (if available). Syntax: adminmodule.downloadplugin <codefling|umod> <plugin>")]
	[AuthLevel(2)]
	private void DownloadPlugin(Arg args)
	{
		var vendor = PluginsTab.GetVendor(args.Args[0] == "codefling" ? PluginsTab.VendorTypes.Codefling : PluginsTab.VendorTypes.uMod);
		if (vendor == null)
		{
			PutsWarn($"Couldn't find that vendor.");
			return;
		}
		var plugin = vendor.FetchedPlugins.FirstOrDefault(x => x.Name.ToLower().Trim().Contains(args.Args[1].ToLower().Trim()));
		if (plugin == null)
		{
			PutsWarn($"Cannot find that plugin.");
			return;
		}
		vendor.Download(plugin.Id, () => { PutsWarn($"Couldn't download {plugin.Name}."); });
	}

	[ConsoleCommand("adminmodule.updatevendor", "Downloads latest vendor information. Syntax: adminmodule.updatevendor <codefling|umod>")]
	[AuthLevel(2)]
	private void UpdateVendor(Arg args)
	{
		var vendor = PluginsTab.GetVendor(args.Args[0] == "codefling" ? PluginsTab.VendorTypes.Codefling : PluginsTab.VendorTypes.uMod);
		if (vendor == null)
		{
			PutsWarn($"Couldn't find that vendor.");
			return;
		}

		var id = string.Empty;
		switch (vendor)
		{
			case PluginsTab.Codefling:
				id = "cf";
				break;

			case PluginsTab.uMod:
				id = "umod";
				break;
		}

		var dataPath = Path.Combine(Core.Defines.GetDataFolder(), $"vendordata_{id}.db");
		OsEx.File.Delete(dataPath);

		if (!vendor.Load())
		{
			vendor.FetchList();
			vendor.Refresh();
		}
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
		var container = cui.CreateContainer(SpectatePanelId, color: "0.1 0.1 0.1 0.8", needsCursor: true, parent: ClientPanels.Overlay);
		var panel = cui.CreatePanel(container, SpectatePanelId, null, "0 0 0 0");
		cui.CreatePanel(container, panel, null, "0 0 0 1", yMax: 0.075f);
		cui.CreatePanel(container, panel, null, "0 0 0 1", yMin: 0.925f);
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

		var tab = Singleton.GetTab(player);
		var ap = Singleton.GetPlayerSession(player);
		EntitiesTab.SelectEntity(tab, ap, spectated);
		EntitiesTab.DrawEntitySettings(tab, 1, ap);
		Singleton.Draw(player);
	}

	#endregion

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

			var list = Pool.GetList<OptionButton>();
			if (OnCancel != null) list.Add(new OptionButton("Cancel", ap => { OnCancel?.Invoke(ap, Entry); }));
			if (OnSave != null) list.Add(new OptionButton("Save", ap => { OnSave?.Invoke(ap, Entry); }));
			if (OnSaveAndReload != null) list.Add(new OptionButton("Save & Reload", ap => { OnSaveAndReload?.Invoke(ap, Entry); }));

			AddButtonArray(0, list.ToArray());
			Pool.FreeList(ref list);

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
								AddColor(column, name, () => value.StartsWith("#") ? HexToRustColor(value) : value, (ap, hex, rust) =>
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
				var newPropertyName = ap.GetStorage<string>(this, "jsonprop", "New Property");

				if (array.Count == 1)
				{
					AddButton(subColumn, $"Duplicate", ap2 =>
					{
						array.Add(array.LastOrDefault());
						_drawArray(name, array, level, column, ap);
					}, ap2 => OptionButton.Types.Warned);
				}
				else if (array.Count == 0) AddText(subColumn, $"{StringEx.SpacedString(Spacing, 0, false)}No entries", 10, "1 1 1 0.6", TextAnchor.MiddleLeft);

				AddInput(subColumn, "Property Name", ap => ap.GetStorage<string>(this, "jsonprop", "New Property"), (ap, args) => { ap.SetStorage(this, "jsonprop", newPropertyName = args.ToString(" ")); });
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
			tab.Pages.Add(new Page("RustEdit.Ext", (cui, t, container, panel, ap) =>
			{
				tab.ModuleInfoTemplate(cui, t, container, panel, ap,
					"RustEdit.Ext Module",
					"An extension to allow further customisation in Rust maps.\n" +
					"\n<b>Features</b>" +
					"\n• Establishes IO connections made in the editor" +
					"\n• Populates custom loot containers and ensures they respawn/refresh loot at the rates set in the associated loot profile" +
					"\n• Creates spawn handlers for all loot containers placed in the editor without a loot profile so they respawn/refresh loot at default rates" +
					"\n• Creates spawn handlers for all resource entities placed in the editor so manually placed resources will respawn" +
					"\n• Creates spawn handlers for all junk piles placed in the editor so manually placed junk piles will respawn" +
					"\n• Creates spawn handlers for NPC Spawners placed in the editor" +
					"\n• Creates spawn handlers for vehicles placed in the editor" +
					"\n• Populates custom vending machines using the vending profile associated with them in the editor" +
					"\n• Overrides OceanPatrolPath generation with a custom path created in the editor" +
					"\n• Creates and manages custom APC paths created in the editor" +
					"\n• Fixes the spawn point prefab and ensures players will only spawn on them" +
					"\n• Fixes the rotation of the excavator arm on map placed excavator monuments that have been rotated" +
					"\n• Ensures desk keycard spawners actually respawn keycards" +
					"\n• Disables damage and decay on all editor placed entities" +
					"\n• Prevents deployable entities from killing themselves" +
					"\n• Updates itself automatically", "", FindModule("RustEditModule"));
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
			var page = ap.GetStorage<int>(tab, "page", 0);
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
			var page = ap.GetStorage<int>(tab, "page", 0);

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

		var currentPage = ap.GetStorage<int>(tab, "page", 0);
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

	#region Color Picker

	internal class ColorPicker
	{
		public const string Brightness = "colorpicker_brightness";
		public const string BrightnessIndicator = "colorpicker_brightnessindicator";
		public const string FirstOpen = "colorpicker_firstopen";
		public const string OnColorPicked = "colorpicker_oncolorpicked";

		public const string PanelId = "carbonuicolorpicker";
		public const string PanelCursorLockId = "carbonuicolorpickercurlock";

		internal static float AnimationLength = 0.005f;
		internal static float CurrentAnimation = 0f;

		public static void Open(BasePlayer player, Action<string, string> onColorPicked)
		{
			var ap = Singleton.GetPlayerSession(player);

			if (!Singleton.ModuleConfiguration.Enabled)
			{
				var empty = string.Empty;
				onColorPicked?.Invoke(empty, empty);
				return;
			}

			DrawCursorLocker(player);
			Draw(player, onColorPicked);
			ap.SetStorage(ap.SelectedTab, FirstOpen, true);
		}
		public static void Close(BasePlayer player)
		{
			var ap = Singleton.GetPlayerSession(player);

			Singleton.Handler.Destroy(PanelId, player);
			Singleton.Handler.Destroy(PanelCursorLockId, player);

			ap.SetStorage(ap.SelectedTab, FirstOpen, false);
		}

		internal static void Draw(BasePlayer player, Action<string, string> onColorPicked)
		{
			if (player == null) return;

			var ap = Singleton.GetPlayerSession(player);

			ap.SetStorage(ap.SelectedTab, OnColorPicked, onColorPicked);

			var brightness = ap.GetStorage<float>(ap.SelectedTab, Brightness, 1f);
			var firstOpen = ap.GetStorage<bool>(ap.SelectedTab, FirstOpen, false);

			using var cui = new CUI(Singleton.Handler);

			var container = cui.CreateContainer(PanelId,
				color: "0 0 0 0.75",
				xMin: 0, xMax: 1, yMin: 0, yMax: 1,
				needsCursor: true, destroyUi: PanelId);

			var color = cui.CreatePanel(container, parent: PanelId, id: PanelId + ".color",
				color: "0 0 0 0.6",
				xMin: 0.3f, xMax: 0.7f, yMin: 0.275f, yMax: 0.825f);
			var main = cui.CreatePanel(container, parent: PanelId + ".color", id: PanelId + ".main",
				color: "0 0 0 0.5",
				blur: true);

			cui.CreateText(container, parent: main, id: null,
				color: "1 1 1 0.8",
				text: "<b>Color Picker</b>", 18,
				xMin: 0f, yMin: 0.8f, xMax: 1f, yMax: 0.98f,
				align: TextAnchor.UpperCenter,
				font: Handler.FontTypes.RobotoCondensedBold);

			#region Main

			var scale = 20f;
			var offset = scale * 0.770f;
			var total = (scale * 2) - 8f;

			var topRightColor = Color.blue;
			var bottomRightColor = Color.green;
			var topLeftColor = Color.red;
			var bottomLeftColor = Color.yellow;

			cui.CreateText(container, parent: main, id: null,
				color: "1 1 1 0.3",
				text: "------------------------------------------------------------------------------------------------------------------------------------- BRIGHTNESS", 8,
				xMin: 0f, xMax: 0.775f, yMin: 0.01f, yMax: 0.98f,
				align: TextAnchor.LowerRight,
				font: Handler.FontTypes.RobotoCondensedRegular);

			cui.CreateText(container, parent: main, id: null,
				color: "1 1 1 0.3",
				text: "SHADES ---------------", 8,
				xMin: 0.805f, xMax: 1, yMin: 0.085f, yMax: 1f,
				align: TextAnchor.LowerLeft,
				font: Handler.FontTypes.RobotoCondensedRegular);

			var input = cui.CreatePanel(container, parent: main, id: null, "0.1 0.1 0.1 0.5",
				xMin: 0.805f, xMax: 0.94f, yMin: 0.085f, yMax: 0.15f, OyMin: -30, OyMax: -30);
			cui.CreateProtectedInputField(container, input, null, "1 1 1 1", "#", 10, 0, false,
				xMin: 0.075f, command: PanelId + ".pickhexcolor ", align: TextAnchor.MiddleLeft, needsKeyboard: Singleton.HandleEnableNeedsKeyboard(ap));

			var picker = cui.CreatePanel(container, parent: main, id: PanelId + ".picker",
				color: "0 0 0 0",
				xMin: 0.175f, xMax: 0.8f, yMin: 0.1f, yMax: 0.9f);

			for (var y = 0f; y < scale; y += 1f)
			{
				var heightColor = Color.Lerp(topRightColor, bottomRightColor, y.Scale(0f, scale, 0f, 1f));

				for (float x = 0; x < scale; x += 1f)
				{
					var widthColor = Color.Lerp(topLeftColor, bottomLeftColor, (x + y).Scale(0f, total, 0f, 1f));
					var _color = Color.Lerp(widthColor, heightColor, x.Scale(0f, scale, 0f, 1f)) * brightness;
					DrawColor(cui, container, ap, scale, _color, picker, offset * x, -(offset * y), fade: !firstOpen ? CurrentAnimation : 0);

					CurrentAnimation += AnimationLength;
				}
			}

			//
			// Brightness
			//
			var counter = 0;
			for (var y = 0f; y < scale; y += 1f)
			{
				var _color = Color.Lerp(Color.black, Color.white, y.Scale(0f, scale, 0f, 1f));
				DrawColor(cui, container, ap, scale, _color, picker, offset * y, -(offset * (scale + 1f)), "brightness", fade: !firstOpen ? CurrentAnimation : 0, index: counter);

				CurrentAnimation += AnimationLength;
				counter++;
			}

			//
			// Saturation
			//
			for (var y = 0f; y < scale; y += 1f)
			{
				var _color = Color.Lerp(Color.white, Color.black, y.Scale(0f, scale, 0f, 1f));
				DrawColor(cui, container, ap, scale, _color, picker, offset * (scale + 1f), -(offset * y), fade: !firstOpen ? CurrentAnimation : 0);

				CurrentAnimation += AnimationLength;
			}

			#endregion

			cui.CreateProtectedButton(container, parent: main, id: null,
				color: "0.6 0.2 0.2 0.9",
				textColor: "1 0.5 0.5 1",
				text: "X", 8,
				xMin: 0.96f, xMax: 0.99f, yMin: 0.95f, yMax: 0.99f,
				command: PanelId + ".close",
				font: Handler.FontTypes.DroidSansMono);

			cui.Send(container, player);


			CurrentAnimation = 0;
		}
		internal static void DrawColor(CUI cui, CuiElementContainer container, PlayerSession ap, float scale, Color color, string parent, float xOffset, float yOffset, string mode = "color", float fade = 0f, int index = -1)
		{
			var size = Extensions.MathEx.Scale(1f, 0, scale, 0f, 1f);

			var id = cui.CreateProtectedButton(container, parent, null,
				color: $"{color.r} {color.g} {color.b} 1",
				textColor: "0 0 0 0",
				text: string.Empty, 0,
				xMin: 0, yMin: size * (scale - 1f), xMax: 1f - (size * (scale - 1f)), yMax: 1f,
				OxMin: xOffset, OyMin: yOffset, OxMax: xOffset, OyMax: yOffset,
				fadeIn: fade,
				command: PanelId + $".pickcolor {mode} {ColorUtility.ToHtmlStringRGBA(color)} {color.r} {color.g} {color.b}");

			if (mode == "brightness" && index == ap.GetStorage<int>(ap.SelectedTab, BrightnessIndicator, 8))
			{
				cui.CreatePanel(container, id, null, "0.75 0.75 0.2 0.8", yMin: 0.85f, yMax: 1, OyMin: 4, OyMax: 8.5f);
			}
		}
		internal static void DrawCursorLocker(BasePlayer player)
		{
			using var cui = new CUI(Singleton.Handler);

			var container = cui.CreateContainer(PanelCursorLockId,
				color: "0 0 0 0",
				xMin: 0, xMax: 0, yMin: 0, yMax: 0,
				fadeIn: 0.005f,
				needsCursor: true);

			cui.Send(container, player);
		}
	}

	public void OpenColorPicker(BasePlayer player, Action<string, string> onColorPicked)
	{
		ColorPicker.Draw(player, onColorPicked);
	}

	#region Custom Commands

	[ProtectedCommand(ColorPicker.PanelId + ".close")]
	private void CloseColorPickerUI(Arg args)
	{
		ColorPicker.Close(args.Player());
	}

	[ProtectedCommand(ColorPicker.PanelId + ".pickcolor")]
	private void PickColorPickerUI(Arg args)
	{
		var player = args.Player();
		var ap = GetPlayerSession(player);
		var mode = args.Args[0];
		var hex = args.Args[1];
		var rawColor = args.Args.Skip(2).ToArray().ToString(" ", " ");
		ColorUtility.TryParseHtmlString($"#{hex}", out var color);

		var brightness = ap.GetStorage<float>(ap.SelectedTab, ColorPicker.Brightness, 1f);
		var brightnessIndicator = ap.GetStorage<int>(ap.SelectedTab, ColorPicker.BrightnessIndicator, 8);
		var onColorPicked = ap.GetStorage<Action<string, string>>(ap.SelectedTab, ColorPicker.OnColorPicked);

		switch (mode)
		{
			case "brightness":
				ap.SetStorage(ap.SelectedTab, ColorPicker.Brightness, color.r.Scale(0f, 1f, 0f, 2.5f));
				ap.SetStorage(ap.SelectedTab, ColorPicker.BrightnessIndicator, (int)color.r.Scale(0f, 1f, 0f, 20.5f));
				ap.SetStorage(ap.SelectedTab, ColorPicker.FirstOpen, true);
				ColorPicker.Draw(player, onColorPicked);
				return;
		}

		onColorPicked?.Invoke(hex, rawColor);
		ColorPicker.Close(args.Player());
	}

	[ProtectedCommand(ColorPicker.PanelId + ".pickhexcolor")]
	private void PickHexColorPickerUI(Arg args)
	{
		var player = args.Player();
		var ap = GetPlayerSession(player);
		var hex = args.Args[0];

		if (args.Args.Length == 0 || string.IsNullOrEmpty(hex) || hex == "#")
		{
			return;
		}

		var onColorPicked = ap.GetStorage<Action<string, string>>(ap.SelectedTab, ColorPicker.OnColorPicked);

		if (!hex.StartsWith("#")) hex = $"#{hex}";
		var rawColor = HexToRustColor(hex, includeAlpha: false);
		onColorPicked?.Invoke(hex, rawColor);
		ColorPicker.Close(args.Player());
	}

	#endregion

	#endregion

	#region Modal

	public class Modal
	{
		public string Title;
		public Dictionary<string, Field> Fields;
		public Action OnCancel;
		public Action<BasePlayer, Modal> OnConfirm;
		public int Page;
		public string BackgroundColor = "0 0 0 0.99";
		public float PositionXMin = 0.3f;
		public float PositionXMax = 0.7f;
		public float PositionYMin = 0.275f;
		public float PositionYMax = 0.825f;

		internal Handler Handler;
		internal const string PanelId = "carbonmodalui";
		internal BasePlayer Player;
		internal Action<Modal, string, Field, object, object> OnFieldChanged;

		public static Modal Open(BasePlayer player,
			string title,
			Dictionary<string, Field> fields,
			Action<BasePlayer, Modal> onConfirm = null,
			Action onCancel = null,
			Action<Modal, string, Field, object, object> onFieldChanged = null)
		{
			var tab = new Modal()
			{
				Title = title,
				Fields = fields,
				OnCancel = onCancel,
				OnConfirm = onConfirm,
				Player = player,
				OnFieldChanged = onFieldChanged,
				Handler = new()
			};

			Singleton.NextFrame(() => tab.Draw(player));

			return tab;
		}
		public static void Close(BasePlayer player)
		{
			using var cui = new CUI(Singleton.Handler);
			cui.Destroy(PanelId, player);
		}

		public bool IsValid()
		{
			foreach (var field in Fields)
			{
				if (field.Value.IsInvalid()) return false;
			}

			return true;
		}
		public int Pages => Fields.Count > 4 ? (Fields.Count - 1) / 4 : 0;
		public string[] InvalidMessages => Fields.Where(x => x.Value.Value != null && x.Value.CustomIsInvalid != null).Select(x =>
		{
			try
			{
				var value = x.Value?.CustomIsInvalid?.Invoke(x.Value);
				return $"    <color=red>{value?.ToUpper()?.SpacedString(1)}</color>";
			}
			catch { }

			return string.Empty;
		}).ToArray();

		public void Draw(BasePlayer player)
		{
			var ap = Singleton.GetPlayerSession(player);

			using var cui = new CUI(Handler);
			var container = cui.CreateContainer(PanelId,
				color: BackgroundColor,
				xMin: 0, xMax: 1, yMin: 0, yMax: 1,
				needsCursor: true, destroyUi: PanelId);

			var color = cui.CreatePanel(container, parent: PanelId, id: PanelId + ".color",
				color: "0 0 0 0.6",
				xMin: PositionXMin, xMax: PositionXMax, yMin: PositionYMin, yMax: PositionYMax);
			var main = cui.CreatePanel(container, parent: PanelId + ".color", id: PanelId + ".main",
				color: "0 0 0 0.5", blur: true);

			_drawInternal(cui, container, main);

			ap.SetStorage(null, "modal", this);
			cui.Send(container, player);
		}
		public void _drawInternal(CUI cui, CuiElementContainer container, string panel)
		{
			var subText = $"<b><color=red>*</color></b>  {"Assign all required field values.".ToUpper().SpacedString(1)}" +
				$"\n    {(IsValid() ? $"<b><color=green>{"The modal is valid.".ToUpper().SpacedString(1)}</color></b>" : $"<b><color=red>{"The modal has invalid fields.".ToUpper().SpacedString(1)}</color></b>")}" +
				$"{(InvalidMessages != null ? $"\n{InvalidMessages.ToString("\n")}" : "")}";
			cui.CreateText(container, panel, null, "1 1 1 1", subText.Trim(), 9, align: TextAnchor.LowerLeft, xMin: 0.05f, yMin: 0.05f);

			var main = cui.CreatePanel(container, panel, null, "0 0 0 0", xMin: 0.1f, xMax: 0.9f, yMin: 0.2f, yMax: 0.9f);
			cui.CreateText(container, main, null, "1 1 1 0.7", Title, 20, xMin: 0.025f, yMax: 0.985f, align: TextAnchor.UpperLeft);

			var offset = 0f;
			var spacing = 60f;

			var content = cui.CreatePanel(container, main, null, "0 0 0 0", xMin: 0, yMin: 0, OyMin: -35, OyMax: -35);

			var pageContent = Fields.Skip(Page * 4).Take(4);
			foreach (var field in pageContent)
			{
				var fieldPanel = cui.CreatePanel(container, content, null, "0 0 0 0.5", yMin: 0.8f, OyMin: offset, OyMax: offset);
				cui.CreateText(container, fieldPanel, null, "1 1 1 1", $"<b>{field.Value.DisplayName.ToUpper().SpacedString(1)}</b>{(field.Value.IsRequired ? "  <b><color=red>*</color></b>" : "")}", 11, xMin: 0.03f, yMax: 0.85f, align: TextAnchor.UpperLeft);

				var option = cui.CreatePanel(container, fieldPanel, null, $"0.1 0.1 0.1 {(field.Value.IsReadOnly ? "0.45" : "0.75")}", yMax: 0.55f);
				var textColor = field.Value.IsReadOnly ? "1 1 1 0.15" : "1 1 1 1";

				if (field.Value.IsInvalid())
				{
					cui.CreatePanel(container, option, null, HexToRustColor("#b8302e", 0.5f));
				}

				switch (field.Value.Type)
				{
					case Field.FieldTypes.String:
					case Field.FieldTypes.Float:
					case Field.FieldTypes.Integer:
						var value = field.Value.Value?.ToString();
						cui.CreateProtectedInputField(container, option, null, textColor, value, 15, 256, false, xMin: 0.025f, align: TextAnchor.MiddleLeft, command: $"modal.action {field.Key}", needsKeyboard: Singleton.HandleEnableNeedsKeyboard(Player));
						break;

					case Field.FieldTypes.Boolean:
						cui.CreateProtectedButton(container, option, null, "0 0 0 0", "0 0 0 0", string.Empty, 0, command: $"modal.action {field.Key} {field.Value.Value}");
						var toggle = cui.CreateProtectedButton(container, option, null, "0.1 0.1 0.1 0.8", "0 0 0 0", string.Empty, 0, xMin: 0.025f, xMax: 0.085f, yMin: 0.1f, yMax: 0.9f, command: $"modal.action {field.Key} {field.Value.Value}");
						if (field.Value.Value is bool booleanValue && booleanValue)
						{
							cui.CreateImage(container, toggle, null, "checkmark", textColor, 0.2f, xMax: 0.8f, yMin: 0.2f, yMax: 0.8f);
						}
						break;

					case Field.FieldTypes.RustColor:
					case Field.FieldTypes.HexColor:
						var originalColor = field.Value.Value == null || (string.IsNullOrEmpty(field.Value.Value.ToString())) ? (field.Value.Type == Field.FieldTypes.RustColor ? "1 1 1" : "#ffffff") : field.Value.Value.ToString();
						var hexColor = field.Value.Type == Field.FieldTypes.RustColor ? RustToHexColor(originalColor, includeAlpha: false) : originalColor;
						var rustColor = field.Value.Type == Field.FieldTypes.HexColor ? HexToRustColor(originalColor, includeAlpha: false) : originalColor;
						var rustColorSplit = rustColor.Split(' ');
						rustColor = $"R:{rustColorSplit[0].ToFloat() * 255:0}   G:{rustColorSplit[1].ToFloat() * 255:0}   B:{rustColorSplit[2].ToFloat() * 255:0}";
						Array.Clear(rustColorSplit, 0, rustColorSplit.Length);

						cui.CreateText(container, option, null, textColor, $"<b>{"HEX".SpacedString(1)}:</b>  {hexColor}", 12, xMin: 0.7f, align: TextAnchor.MiddleLeft);
						cui.CreateText(container, option, null, textColor, $"<b>{"RUST".SpacedString(1)}:</b>  {rustColor}", 12, xMin: 0.115f, align: TextAnchor.MiddleLeft);
						var color = cui.CreateProtectedButton(container, option, null, hexColor, "0 0 0 0", string.Empty, 0, xMin: 0.025f, xMax: 0.085f, yMin: 0.1f, yMax: 0.9f, command: $"modal.action {field.Key}");
						break;

					case Field.FieldTypes.Enum:
						var @enum = field.Value as EnumField;
						cui.CreateText(container, option, null, textColor, @enum.Options[@enum.Value == null ? 0 : @enum.Value.ToString().ToInt()], 12, align: TextAnchor.MiddleCenter);

						cui.CreateProtectedButton(container, option, null, "0.1 0.1 0.1 0.75", "1 1 1 0.7", "<", 10, xMin: 0f, xMax: 0.5f, command: $"modal.action {field.Key} -");
						cui.CreateProtectedButton(container, option, null, "0.1 0.1 0.1 0.75", "1 1 1 0.7", ">", 10, xMin: 0.5f, xMax: 1f, command: $"modal.action {field.Key} +");
						break;

					case Field.FieldTypes.Button:
						var button = field.Value as ButtonField;
						cui.CreateProtectedButton(container, option, null, "0.1 0.1 0.1 0.85", "1 1 1 0.7", button.ButtonName?.ToUpper().SpacedString(1), 10, command: $"modal.action {field.Key}");
						break;
				}

				if (field.Value.IsReadOnly)
				{
					cui.CreatePanel(container, option, null, "0 0 0 0");
				}

				offset -= spacing;
			}

			var buttons = cui.CreatePanel(container, panel, null, "0 0 0 0", xMin: 0.075f, xMax: 0.925f, yMin: 0.025f, yMax: 0.1f);
			cui.CreateProtectedButton(container, buttons, null, "0.1 0.1 0.1 0.85", "1 1 1 0.7", "CANCEL".SpacedString(1), 10, xMin: 0.7f, xMax: 0.84f, command: "modal.cancel");
			cui.CreateProtectedButton(container, buttons, null, IsValid() ? HexToRustColor("#7ebf37", 0.6f) : "0.1 0.1 0.1 0.85", "1 1 1 0.7", "CONFIRM".SpacedString(1), 10, xMin: 0.85f, command: "modal.confirm");

			if (Pages > 0)
			{
				var pages = cui.CreatePanel(container, panel, null, "0 0 0 0", xMin: 0.1f, xMax: 0.9f, yMin: 0.15f, yMax: 0.2f);
				cui.CreateText(container, pages, null, "1 1 1 0.7", $"{Page + 1:n0} / {Pages + 1:n0}", 10, xMin: 0.82f, xMax: 0.92f);
				cui.CreateProtectedButton(container, pages, null, Page == 0 ? "0.2 0.2 0.2 0.6" : HexToRustColor("#7ebf37", 0.6f), "1 1 1 0.7", "<", 10, xMin: 0.82f, xMax: 0.90f, OxMin: -30, OxMax: -30, command: "modal.page -");
				cui.CreateProtectedButton(container, pages, null, Page == Pages ? "0.2 0.2 0.2 0.6" : HexToRustColor("#7ebf37", 0.6f), "1 1 1 0.7", ">", 10, xMin: 0.92f, command: "modal.page +");
			}
		}

		public T Get<T>(string key)
		{
			if (Fields.TryGetValue(key, out var field)) return (T)field.Value;
			return default;
		}

		public class Field : IDisposable
		{
			public string DisplayName { get; set; }
			public object Value { get; set; }
			public FieldTypes Type { get; set; }
			public bool IsRequired { get; set; }
			public bool IsReadOnly { get; set; }

			public T Get<T>()
			{
				return (T)Value;
			}

			public Func<Field, string> CustomIsInvalid { get; set; }

			public bool IsInvalid()
			{
				return IsRequired && (Value == null || string.IsNullOrEmpty(Value.ToString())) || !string.IsNullOrEmpty(CustomIsInvalid?.Invoke(this));
			}

			public enum FieldTypes
			{
				String,
				Integer,
				Float,
				Boolean,
				Enum,
				RustColor,
				HexColor,
				Button
			}

			public static Field Make(string displayName, FieldTypes type, bool required = false, object @default = null, bool isReadOnly = false, Func<Field, string> customIsInvalid = null)
			{
				return new Field
				{
					DisplayName = displayName,
					Type = type,
					IsRequired = required,
					Value = @default,
					IsReadOnly = isReadOnly,
					CustomIsInvalid = customIsInvalid
				};
			}

			public void Dispose()
			{
				DisplayName = null;
				Value = null;
			}
		}
		public class EnumField : Field
		{
			public string[] Options { get; set; }

			public static EnumField MakeEnum(string displayName, string[] options, bool required = false, object @default = null, bool isReadOnly = false, Func<Field, string> customIsInvalid = null)
			{
				return new EnumField
				{
					DisplayName = displayName,
					Type = FieldTypes.Enum,
					IsRequired = required,
					Value = @default,
					Options = options,
					IsReadOnly = isReadOnly,
					CustomIsInvalid = customIsInvalid
				};
			}
		}
		public class ButtonField : Field
		{
			public string ButtonName { get; set; }
			public Action<Modal> Callback { get; set; }

			public static ButtonField MakeButton(string displayName, string buttonName, Action<Modal> callback, bool isReadOnly = false)
			{
				return new ButtonField
				{
					DisplayName = displayName,
					ButtonName = buttonName,
					Type = FieldTypes.Button,
					IsRequired = false,
					Callback = callback,
					IsReadOnly = isReadOnly,
					CustomIsInvalid = null
				};
			}
		}
	}

	#region Custom Commands

	[ProtectedCommand("modal.action")]
	private void ModalAction(ConsoleSystem.Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());
		var modal = ap.GetStorage<Modal>(null, "modal");

		var fieldName = arg.Args[0];
		var field = modal.Fields[fieldName];
		var oldValue = field.Value;

		if (!field.IsReadOnly)
		{
			var value = arg.Args.Skip(1).ToArray().ToString(" ");

			switch (field.Type)
			{
				case Modal.Field.FieldTypes.String:
					field.Value = value;
					modal.OnFieldChanged?.Invoke(modal, fieldName, field, oldValue, value);
					break;

				case Modal.Field.FieldTypes.Integer:
					field.Value = value.ToInt();
					modal.OnFieldChanged?.Invoke(modal, fieldName, field, oldValue, value);
					break;

				case Modal.Field.FieldTypes.Float:
					field.Value = value.ToFloat();
					modal.OnFieldChanged?.Invoke(modal, fieldName, field, oldValue, value);
					break;

				case Modal.Field.FieldTypes.Boolean:
					field.Value = !value.ToBool(false);
					modal.OnFieldChanged?.Invoke(modal, fieldName, field, oldValue, value);
					break;

				case Modal.Field.FieldTypes.RustColor:
				case Modal.Field.FieldTypes.HexColor:
					Community.Runtime.CorePlugin.NextFrame(() =>
					{
						OpenColorPicker(ap.Player, (hexColor, rustColor) =>
						{
							if (field.Type == Modal.Field.FieldTypes.RustColor) field.Value = rustColor;
							else field.Value = $"#{hexColor}";

							modal.OnFieldChanged?.Invoke(modal, fieldName, field, oldValue, value);
							modal.Draw(ap.Player);
						});
					});
					break;

				case Modal.Field.FieldTypes.Enum:
					var @enum = field as Modal.EnumField;
					var enumValue = field.Value == null ? 0 : field.Value.ToString().ToInt();
					switch (value)
					{
						case "+":
							enumValue++;
							break;

						case "-":
							enumValue--;
							field.Value = enumValue - 1;
							break;
					}

					if (enumValue > @enum.Options.Length - 1) enumValue = 0;
					else if (enumValue < 0) enumValue = @enum.Options.Length - 1;

					field.Value = enumValue;
					modal.OnFieldChanged?.Invoke(modal, fieldName, field, oldValue, field.Value);
					break;

				case Modal.Field.FieldTypes.Button:
					var button = field as Modal.ButtonField;
					button.Callback?.Invoke(modal);
					break;
			}
		}

		modal.Draw(ap.Player);
	}

	[ProtectedCommand("modal.confirm")]
	private void ModalConfirm(ConsoleSystem.Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());
		var modal = ap.GetStorage<Modal>(null, "modal");

		if (!modal.IsValid())
		{
			modal.Draw(ap.Player);
			return;
		}

		modal.OnConfirm?.Invoke(ap.Player, modal);
		Modal.Close(ap.Player);
	}

	[ProtectedCommand("modal.cancel")]
	private void ModalCancel(ConsoleSystem.Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());
		var modal = ap.GetStorage<Modal>(null, "modal");
		modal?.OnCancel?.Invoke();
		Modal.Close(ap.Player);
	}

	[ProtectedCommand("modal.page")]
	private void ModalPage(ConsoleSystem.Arg arg)
	{
		var ap = GetPlayerSession(arg.Player());
		var modal = ap.GetStorage<Modal>(null, "modal");
		switch (arg.Args[0])
		{
			case "+":
				modal.Page++;
				break;

			case "-":
				modal.Page--;
				break;
		}

		if (modal.Page > modal.Pages) modal.Page = 0;
		else if (modal.Page < 0) modal.Page = modal.Pages;

		modal.Draw(ap.Player);
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
