using ConVar;
using Newtonsoft.Json;
using Oxide.Game.Rust.Cui;
using UnityEngine.UI;
using static Carbon.Components.CUI;
using static ConsoleSystem;
using Color = UnityEngine.Color;
using StringEx = Carbon.Extensions.StringEx;

namespace Carbon.Modules;

#pragma warning disable IDE0051

public partial class AdminModule : CarbonModule<AdminConfig, AdminData>
{
	public static readonly string Title = "<b>Admin Centre</b>";
	public override string Name => "Admin";
	public override VersionNumber Version => new(1, 8, 0);
	public override Type Type => typeof(AdminModule);

#if MINIMAL
	public override bool ForceDisabled => true;
#endif

#if !MINIMAL
	public override bool ForceEnabled => true;
	public override bool EnabledByDefault => true;

	internal static AdminModule Singleton { get; set; }

	public static CorePlugin Core = Community.Runtime.Core;
	public ImageDatabaseModule ImageDatabase;
	public ColorPickerModule ColorPicker;
	public DatePickerModule DatePicker;
	public ModalModule Modal;
	public FileModule File;

	public readonly Handler Handler = new();

	internal const int RangeCuts = 50;
	internal readonly string[] EmptyElement =
	[
		string.Empty
	];

	internal List<Tab> Tabs = new();

	const string PanelId = "carbonmodularui";
	const string CursorPanelId = "carbonmodularuicur";
	const string SpectatePanelId = "carbonmodularuispectate";
	readonly string[] AdminPermissions =
	[
		"greet",
		"config.use",
		"carbon.use",
		"carbon.quickactions",
		"carbon.quickactions.edit",
		"carbon.server_settings",
		"carbon.server_config",
		"carbon.server_info",
		"carbon.server_console",
		"entities.use",
		"entities.kill_entity",
		"entities.tp_entity",
		"entities.loot_entity",
		"entities.loot_players",
		"entities.respawn_players",
		"entities.blind_players",
		"entities.spectate_players",
		"entities.owner_change",
		"environment.use",
		"modules.use",
		"modules.config_edit",
		"permissions.use",
		"players.use",
		"players.inventory_management",
		"players.craft_queue",
		"players.see_ips",
		"plugins.use",
		"plugins.setup",
		"profiler.use",
		"profiler.startstop",
		"profiler.sourceviewer"
	];

	internal bool _logRegistration;

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
	internal static Dictionary<ulong, Vector3> _spectateStartPosition = new();

	public bool HandleEnableNeedsKeyboard(PlayerSession ap)
	{
		return ap.SelectedTab == null || ap.SelectedTab.Dialog == null;
	}
	public bool HandleEnableNeedsKeyboard(BasePlayer player)
	{
		return HandleEnableNeedsKeyboard(GetPlayerSession(player));
	}

	public override void OnServerInit(bool initial)
	{
		base.OnServerInit(initial);

		if (!initial) return;

		ImageDatabase = GetModule<ImageDatabaseModule>();
		ColorPicker = GetModule<ColorPickerModule>();
		DatePicker = GetModule<DatePickerModule>();
		Modal = GetModule<ModalModule>();
		File = GetModule<FileModule>();

		Unsubscribe("OnPluginLoaded");
		Unsubscribe("OnPluginUnloaded");
		Unsubscribe("OnEntityDismounted");
		Unsubscribe("CanDismountEntity");
		Unsubscribe("OnEntityVisibilityCheck");
		Unsubscribe("OnEntityDistanceCheck");
		Unsubscribe("CanAcceptItem");

		if (!_logRegistration)
		{
			Application.logMessageReceived += OnLog;
			_logRegistration = true;
		}

		OnEnabled(true);
	}
	public override void OnPostServerInit(bool initial)
	{
		base.OnPostServerInit(initial);

		GenerateTabs();
	}

	public override void OnEnabled(bool initialized)
	{
		base.OnEnabled(initialized);

		if (!initialized) return;

		foreach (var command in ConfigInstance.OpenCommands)
		{
			var action = new Action<BasePlayer, string, string[]>((player, cmd, args) =>
			{
				if (!CanAccess(player))
				{
					return;
				}

				var ap = GetPlayerSession(player);

				if (ap.IsInMenu)
				{
					Close(player);
					return;
				}

				if (ap.SelectedTab == null)
				{
					ap.SelectedTab = Tabs.FirstOrDefault(x => !DataInstance.IsTabHidden(x.Id) && HasAccess(player, x.Access));
					ap.Clear();
				}
				else if(DataInstance.IsTabHidden(ap.SelectedTab.Id) || !HasAccess(player, ap.SelectedTab.Access))
				{
					ap.SelectedTab = null;
				}

				var tab = GetTab(player);

				try
				{
					tab?.OnChange?.Invoke(ap, tab);
				}
				catch(Exception ex)
				{
					Logger.Error($"Failed OnChange callback for tab '{tab?.Name}[{tab?.Id}], falling back to default tab", ex);

					ap.SelectedTab = Tabs.FirstOrDefault(x => HasAccess(player, x.Access));
					ap.Clear();
				}

				DrawCursorLocker(player);
				Draw(player);
			});

			Community.Runtime.Core.cmd.AddChatCommand(command, this, action, silent: true);
			Community.Runtime.Core.cmd.AddConsoleCommand(command, this, action, silent: true);
		}

		foreach (var perm in AdminPermissions)
		{
			Permissions.RegisterPermission($"adminmodule.{perm}", this);
		}

		ImageDatabase.Queue(true, DataInstance.BackgroundImage);
	}
	public override void OnDisabled(bool initialized)
	{
		if (initialized)
		{
			Community.Runtime.Core.NextTick(() =>
			{
				foreach (var player in BasePlayer.activePlayerList)
				{
					Close(player);
				}
			});
		}

		base.OnDisabled(initialized);
	}

	public override void Shutdown()
	{
		Save();
		base.Shutdown();
	}

	public override void Load()
	{
		base.Load();

		ConfigInstance.MinimumAuthLevel = ConfigInstance.MinimumAuthLevel.Clamp(0, 3);

		if (Community.IsServerInitialized) GenerateTabs();

		if (ModuleConfiguration.HasConfigStructureChanged())
		{
			DataInstance.GreetDisplayed = false;
		}
	}
	public override void Save()
	{
		base.Save();

		PluginsTab.ServerOwner.Save();
	}

	public override void Reload()
	{
		base.Reload();
		OnEnabled(true);
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
				["general"] = "General",
				["watchers"] = "Watchers",
				["scriptwatchers"] = "Script Watchers",
				["scriptwatchers_help"] = "When disabled, you must load/unload plugins manually with 'c.load' or 'c.unload'.",
				["zipscriptwatchers"] = "ZIP Script Watchers",
				["zipscriptwatchers_help"] = "When disabled, you must load/unload plugins manually with 'c.load' or 'c.unload'.",
				["scriptwatchersoption"] = "Script Watchers Option",
				["scriptwatchersoption_help"] = "Indicates wether the script watcher (whenever enabled) listens to the 'carbon/plugins' folder only, or its subfolders.",
				["logging"] = "Logging",
				["logfilemode"] = "Log File Mode",
				["logverbosity"] = "Log Verbosity (Debug)",
				["logseverity"] = "Log Severity",
				["misc"] = "Miscellaneous",
				["serverlang"] = "Server Language",
				["webreqip"] = "WebRequest IP",
				["permmode"] = "Permission Mode",
				["nocontent"] = "There are no options available.\nSelect a sub-tab to populate this area (if available).",
				["consoleinfo"] = "Show Console Info",
				["consoleinfo_help"] = "Show the Windows-only Carbon information at the bottom of the console.",
				["playerdefgroup"] = "Player Group",
				["admindefgroup"] = "Admin Group",
				["moderatordefgroup"] = "Moderator Group",
				["permissions"] = "Permissions",
				["debugging"] = "Debugging",
				["scriptdebugorigin"] = "Script Debugging Origin",
				["scriptdebugorigin_help"] = "Whenever a debugger is attached on server boot, the compiler will replace the debugging origin of the plugin file.",
				["conditionals"] = "Conditionals",
				["quickactions"] = "Quick Actions",
				["quickactions_name"] = "Button Name",
				["quickactions_name_help"] = "The name of the button for the Quick Action.",
				["quickactions_command"] = "Button Command",
				["quickactions_command_help"] = "Command (separated with | for multiple) of the Quick Action button.",
				["quickactions_user"] = "User Mode",
				["quickactions_user_help"] = "When the command gets executed, it'll call it with user permissions.",
				["quickactions_incluserid"] = "Include User ID",
				["quickactions_incluserid_help"] = "When the command gets executed, append the player's Steam ID at the end of the command after a space.",
				["quickactions_confirmdialog"] = "Confirm Dialog",
				["quickactions_confirmdialog_help"] = "Show a dialog which asks you to confirm before executing sensitive command(s).",
				["quickactions_add"] = "Add",
				["quickactions_edit"] = "Edit",
				["quickactions_stopedit"] = "Stop Editing",
				["maxplayers"] = "Maximum Players"
			}
		};
	}

	[Conditional("!MINIMAL")]
	private void OnLog(string condition, string stackTrace, LogType type)
	{
		try
		{
			if (_logQueue.Count >= 6) _logQueue.RemoveAt(0);

			var log = condition.Split('\n');
			var result = log[0];
			Array.Clear(log, 0, log.Length);
			_logQueue.Add(StringEx.Truncate(result, 85));
		}
		catch
		{
		}
	}

	public bool HasAccess(BasePlayer player, string access)
	{
		if ((player != null && player.IsConnected && player.Connection.authLevel == 2))
		{
			return true;
		}

		if (Permissions.UserHasPermission(player.UserIDString, $"adminmodule.{access}"))
		{
			return true;
		}

		return false;
	}
	public void GenerateTabs()
	{
		UnregisterAllTabs();

		RegisterTab(CarbonTab.Get());
		RegisterTab(PlayersTab.Get());
		RegisterTab(EntitiesTab.Get());
		RegisterTab(PermissionsTab.Get());
		RegisterTab(ModulesTab.Get());
		RegisterTab(EnvironmentTab.Get());
		RegisterTab(PluginsTab.Get());
	}

	[Conditional("!MINIMAL")]
	private bool CanAccess(BasePlayer player)
	{
		if (player == null)
		{
			return false;
		}

		// CanAccessAdminModule
		if (HookCaller.CallStaticHook(3266674522, player) is bool result)
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

	[Conditional("!MINIMAL")]
	internal void TabButton(CUI cui, CuiElementContainer container, string parent, string text, string command, float width, float offset, bool highlight = false, bool disabled = false)
	{
		var button = cui.CreateProtectedButton(container, parent: parent,
			color: highlight ? $"{DataInstance.Colors.SelectedTabColor} 0.7" : "0.3 0.3 0.3 0.1",
			textColor: $"1 1 1 {(disabled ? 0.15 : 0.5)}",
			text: text, 11,
			xMin: offset, xMax: offset + width, yMin: 0, yMax: 1,
			command: disabled ? string.Empty : command,
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateImage(container, button, "fade", Cache.CUI.WhiteColor);

		if (highlight)
		{
			cui.CreatePanel(container, button,
				color: "1 1 1 0.4",
				xMin: 0, xMax: 1f, yMin: 0f, yMax: 0.03f,
				OxMax: -0.5f);
		}
	}

	public void TabColumnPagination(CUI cui, CuiElementContainer container, string parent, int column, PlayerSession.Page page, float height, float offset)
	{
		var id = cui.CreatePanel(container, parent,
			color: Cache.CUI.BlankColor,
			xMin: 0.02f, xMax: 0.98f, yMin: offset, yMax: offset + height);

		cui.CreateText(container, parent: id,
			color: "1 1 1 0.5",
			text: $" / {page.TotalPages + 1:n0}", 9,
			xMin: 0.5f, xMax: 1f, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleLeft,
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedInputField(container, parent: id,
			color: "1 1 1 1",
			text: $"{page.CurrentPage + 1}", 9,
			xMin: 0f, xMax: 0.495f, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleRight,
			command: PanelId + $".changecolumnpage {column} 4 ",
			characterLimit: 0,
			readOnly: false,
			font: Handler.FontTypes.RobotoCondensedRegular);

		#region Left

		cui.CreateProtectedButton(container, parent: id,
			color: "0.3 0.3 0.3 0.1",
			textColor: page.CurrentPage > 0 ? "1 1 1 0.5" : "0.5 0.5 0.5 0.5",
			text: "<<", 8,
			xMin: 0, xMax: 0.1f, yMin: 0f, yMax: 1f,
			command: page.CurrentPage > 0 ? PanelId + $".changecolumnpage {column} 2" : "",
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedButton(container, parent: id,
			color: "0.3 0.3 0.3 0.1",
			textColor: "1 1 1 0.5",
			text: "<", 8,
			xMin: 0.1f, xMax: 0.2f, yMin: 0f, yMax: 1f,
			command: PanelId + $".changecolumnpage {column} 0",
			font: Handler.FontTypes.RobotoCondensedRegular);

		#endregion

		#region Right

		cui.CreateProtectedButton(container, parent: id,
			color: "0.3 0.3 0.3 0.1",
			textColor: page.CurrentPage < page.TotalPages ? "1 1 1 0.5" : "0.5 0.5 0.5 0.5",
			text: ">>", 8,
			xMin: 0.9f, xMax: 1f, yMin: 0f, yMax: 1f,
			command: page.CurrentPage < page.TotalPages ? PanelId + $".changecolumnpage {column} 3" : "",
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedButton(container, parent: id,
			color: "0.3 0.3 0.3 0.1",
			textColor: "1 1 1 0.5",
			text: ">", 8,
			xMin: 0.8f, xMax: 0.9f, yMin: 0f, yMax: 1f,
			command: PanelId + $".changecolumnpage {column} 1",
			font: Handler.FontTypes.RobotoCondensedRegular);

		#endregion
	}

	public void TabPanelName(CUI cui, CuiElementContainer container, string parent, string text, float height, float offset, TextAnchor align)
	{
		var cuiText = cui.CreateText(container, parent,
			color: DataInstance.Colors.NameTextColor,
			text: text?.ToUpper(), 12,
			xMin: 0.025f, xMax: 0.98f, yMin: offset, yMax: offset + height,
			align: align,
			font: Handler.FontTypes.RobotoCondensedBold);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreatePanel(container, cuiText,
				color: $"1 1 1 {DataInstance.Colors.TitleUnderlineOpacity}",
				xMin: 0, xMax: 1, yMin: 0f, yMax: 0.015f);
		}
	}
	public void TabPanelText(CUI cui, CuiElementContainer container, string parent, string text, int size, string color, float height, float offset, TextAnchor align, Handler.FontTypes font, bool isInput)
	{
		if (isInput)
		{
			cui.CreateInputField(container, parent: parent,
				color: color,
				text: text, size, characterLimit: 0, readOnly: true,
				xMin: 0.025f, xMax: 0.98f, yMin: offset, yMax: offset + height,
				align: align,
				font: font);
		}
		else
		{
			cui.CreateText(container, parent: parent,
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
			Tab.OptionButton.Types.Selected => DataInstance.Colors.ButtonSelectedColor,
			Tab.OptionButton.Types.Warned => DataInstance.Colors.ButtonWarnedColor,
			Tab.OptionButton.Types.Important => DataInstance.Colors.ButtonImportantColor,
			_ => DataInstance.Colors.OptionColor
		};

		var button = cui.CreateProtectedButton(container, parent: parent,
			color: color,
			textColor: "1 1 1 0.5",
			text: text, 11,
			xMin: 0.015f, xMax: 0.985f, yMin: offset, yMax: offset + height,
			command: command,
			align: align,
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateImage(container, button, "fade", Cache.CUI.WhiteColor);
	}
	public void TabPanelToggle(CUI cui, CuiElementContainer container, string parent, string text, string command, float height, float offset, bool isOn, Tab tab)
	{
		var toggleButtonScale = tab.IsFullscreen ? 0.93f : 0.94f;

		var panel = cui.CreatePanel(container, parent,
			color: Cache.CUI.BlankColor,
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreateText(container, panel,
				color: DataInstance.Colors.OptionNameColor,
				text: $"{text}:", 12,
				xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
				align: TextAnchor.MiddleLeft,
				font: Handler.FontTypes.RobotoCondensedRegular);

			cui.CreatePanel(container, panel,
				color: "0.2 0.2 0.2 0.5",
				xMin: 0, xMax: toggleButtonScale, yMin: 0, yMax: 0.015f);
		}

		var button = cui.CreateProtectedButton(container, parent,
			color: DataInstance.Colors.OptionColor,
			textColor: "1 1 1 0.5",
			text: string.Empty, 11,
			xMin: toggleButtonScale, xMax: 0.985f, yMin: offset, yMax: offset + height,
			command: command,
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateImage(container, button, "fade", Cache.CUI.WhiteColor);

		if (isOn)
		{
			cui.CreateImage(container, button,
				url: "checkmark",
				color: DataInstance.Colors.ButtonSelectedColor,
				xMin: 0.15f, xMax: 0.85f, yMin: 0.15f, yMax: 0.85f);
		}
	}
	public void TabPanelInput(CUI cui, CuiElementContainer container, string parent, string text, string placeholder, string command, int characterLimit, bool readOnly, float height, float offset, PlayerSession session, Tab.OptionButton.Types type = Tab.OptionButton.Types.None, Tab.Option option = null)
	{
		var color = type switch
		{
			Tab.OptionButton.Types.Selected => DataInstance.Colors.ButtonSelectedColor,
			Tab.OptionButton.Types.Warned => DataInstance.Colors.ButtonWarnedColor,
			Tab.OptionButton.Types.Important => DataInstance.Colors.ButtonImportantColor,
			_ => DataInstance.Colors.OptionColor
		};

		var panel = cui.CreatePanel(container, parent,
			color: Cache.CUI.BlankColor,
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreateText(container, panel,
				color: DataInstance.Colors.OptionNameColor,
				text: $"{text}:", 12,
				xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
				align: TextAnchor.MiddleLeft,
				font: Handler.FontTypes.RobotoCondensedRegular);

			cui.CreatePanel(container, panel,
				color: color,
				xMin: 0, xMax: DataInstance.Colors.OptionWidth, yMin: 0, yMax: 0.015f);
		}

		var inPanel = cui.CreatePanel(container, panel,
			color: color,
			xMin: DataInstance.Colors.OptionWidth, xMax: 0.985f, yMin: 0, yMax: 1);

		cui.CreateImage(container, inPanel, "fade", Cache.CUI.WhiteColor);

		cui.CreateProtectedInputField(container, parent: inPanel,
			color: $"1 1 1 {(readOnly ? 0.2f : 1f)}",
			text: placeholder, 11,
			xMin: 0.03f, xMax: 1, yMin: 0, yMax: 1,
			command: command,
			align: TextAnchor.MiddleLeft,
			characterLimit: characterLimit,
			readOnly: readOnly,
			needsKeyboard: session.Input == option,
			autoFocus: session.Input == option && session.Input != session.PreviousInput,
			font: Handler.FontTypes.RobotoCondensedRegular);

		if (session.Input == option)
		{
			session.PreviousInput = session.Input;
		}

		if (!readOnly)
		{
			cui.CreatePanel(container, inPanel,
				color: $"{DataInstance.Colors.EditableInputHighlight} 0.9",
				xMin: 0, xMax: 1, yMin: 0, yMax: 0.05f,
				OxMax: -0.5f);
		}
	}
	public void TabPanelEnum(CUI cui, CuiElementContainer container, string parent, string text, string value, string command, float height, float offset, Tab.OptionButton.Types type = Tab.OptionButton.Types.Selected)
	{
		var color = type switch
		{
			Tab.OptionButton.Types.Selected => DataInstance.Colors.ButtonSelectedColor,
			Tab.OptionButton.Types.Warned => DataInstance.Colors.ButtonWarnedColor,
			Tab.OptionButton.Types.Important => DataInstance.Colors.ButtonImportantColor,
			_ => DataInstance.Colors.OptionColor
		};

		var panel = cui.CreatePanel(container, parent,
			color: Cache.CUI.BlankColor,
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreateText(container, panel,
				color: DataInstance.Colors.OptionNameColor,
				text: $"{text}:", 12,
				xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
				align: TextAnchor.MiddleLeft,
				font: Handler.FontTypes.RobotoCondensedRegular);

			cui.CreatePanel(container, panel,
				color: "0.2 0.2 0.2 0.5",
				xMin: 0, xMax: DataInstance.Colors.OptionWidth, yMin: 0, yMax: 0.015f);
		}

		var inPanel = cui.CreatePanel(container, panel,
			color: DataInstance.Colors.OptionColor,
			xMin: DataInstance.Colors.OptionWidth, xMax: 0.985f, yMin: 0, yMax: 1);

		cui.CreateImage(container, inPanel, "fade", Cache.CUI.WhiteColor);

		cui.CreateText(container, inPanel,
			color: "1 1 1 0.7",
			text: value, 11,
			xMin: 0, xMax: 1, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleCenter,
			font: Handler.FontTypes.RobotoCondensedRegular);

		var left = cui.CreateProtectedButton(container, inPanel,
			color: color,
			textColor: "1 1 1 0.7",
			text: "<", 10,
			xMin: 0f, xMax: 0.15f, yMin: 0, yMax: 1,
			command: $"{command} true",
			align: TextAnchor.MiddleCenter,
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateImage(container, left, "fade", Cache.CUI.WhiteColor);

		var right = cui.CreateProtectedButton(container, inPanel,
			color: color,
			textColor: "1 1 1 0.7",
			text: ">", 10,
			xMin: 0.85f, xMax: 1f, yMin: 0, yMax: 1,
			command: $"{command} false",
			align: TextAnchor.MiddleCenter,
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateImage(container, right, "fade", Cache.CUI.WhiteColor);
	}
	public void TabPanelRadio(CUI cui, CuiElementContainer container, string parent, string text, bool isOn, string command, float height, float offset)
	{
		var toggleButtonScale = 0.93f;

		var panel = cui.CreatePanel(container, parent,
			color: Cache.CUI.BlankColor,
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreateText(container, panel,
				color: DataInstance.Colors.OptionNameColor,
				text: $"{text}:", 12,
				xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
				align: TextAnchor.MiddleLeft,
				font: Handler.FontTypes.RobotoCondensedRegular);

			cui.CreatePanel(container, panel,
				color: "0.2 0.2 0.2 0.5",
				xMin: 0, xMax: toggleButtonScale, yMin: 0, yMax: 0.015f);
		}

		var button = cui.CreateProtectedButton(container, parent,
			color: DataInstance.Colors.OptionColor,
			textColor: "1 1 1 0.5",
			text: string.Empty, 11,
			xMin: toggleButtonScale, xMax: 0.985f, yMin: offset, yMax: offset + height,
			command: command,
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateImage(container, button, "fade", Cache.CUI.WhiteColor);

		if (isOn)
		{
			cui.CreatePanel(container, button,
				color: "0.4 0.7 0.2 0.7",
				xMin: 0.2f, xMax: 0.8f, yMin: 0.2f, yMax: 0.8f);
		}
	}
	public void TabPanelDropdown(CUI cui, PlayerSession.Page page, CuiElementContainer container, string parent, string text, string command, float height, float offset, int index, string[] options, string[] optionsIcons, bool display, Tab.OptionButton.Types type = Tab.OptionButton.Types.Selected)
	{
		var color = type switch
		{
			Tab.OptionButton.Types.Selected => DataInstance.Colors.ButtonSelectedColor,
			Tab.OptionButton.Types.Warned => DataInstance.Colors.ButtonWarnedColor,
			Tab.OptionButton.Types.Important => DataInstance.Colors.ButtonImportantColor,
			_ => DataInstance.Colors.OptionColor2,
		};

		var panel = cui.CreatePanel(container, parent,
			color: Cache.CUI.BlankColor,
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreateText(container, panel,
				color: DataInstance.Colors.OptionNameColor,
				text: $"{text}:", 12,
				xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
				align: TextAnchor.MiddleLeft,
				font: Handler.FontTypes.RobotoCondensedRegular);

			cui.CreatePanel(container, panel,
				color: DataInstance.Colors.OptionColor,
				xMin: 0, xMax: DataInstance.Colors.OptionWidth, yMin: 0, yMax: 0.015f);
		}

		var inPanel = cui.CreatePanel(container, panel,
			color: DataInstance.Colors.OptionColor,
			xMin: DataInstance.Colors.OptionWidth, xMax: 0.985f, yMin: 0, yMax: 1);

		var icon = optionsIcons != null && index <= optionsIcons.Length - 1 ? optionsIcons[index] : null;
		const float iconXmin = 0.015f;
		const float iconXmax = 0.072f;
		const float iconYmin = 0.2f;
		const float iconYmax = 0.8f;

		var button = cui.CreateProtectedButton(container, inPanel,
			color: DataInstance.Colors.OptionColor,
			textColor: Cache.CUI.BlankColor,
			text: string.Empty, 0,
			xMin: 0f, xMax: 1f, yMin: 0, yMax: 1,
			command: $"{command} false",
			align: TextAnchor.MiddleLeft,
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateImage(container, button, "fade", Cache.CUI.WhiteColor);

		cui.CreateText(container, button, "1 1 1 0.7", options[index], 10,
			xMin: string.IsNullOrEmpty(icon) ? 0.035f : 0.09f, xMax: 1f, yMin: 0f, yMax: 1f, align: TextAnchor.MiddleLeft);

		if (!string.IsNullOrEmpty(icon))
		{
			cui.CreateImage(container, button, icon, "1 1 1 0.7",
				xMin: iconXmin, xMax: iconXmax, yMin: iconYmin, yMax: iconYmax);
		}

		if (display)
		{
			const float _spacing = 22;
			var _offset = -_spacing;
			const int contentsPerPage = 10;
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

				var subButton = cui.CreateProtectedButton(container, inPanel,
					color: isSelected ? RustToHexColor(color, 1f) : "0.1 0.1 0.1 1",
					textColor: Cache.CUI.BlankColor,
					text: string.Empty, 0,
					xMin: 0f, xMax: 1f, yMin: 0, yMax: 1,
					OyMin: _offset, OyMax: _offset,
					OxMin: shiftOffset,
					command: $"{command} true call {actualI}",
					align: TextAnchor.MiddleLeft,
					font: Handler.FontTypes.RobotoCondensedRegular);

				cui.CreateImage(container, subButton, "fade", Cache.CUI.WhiteColor);

				cui.CreateText(container, subButton, isSelected ? "1 1 1 0.7" : "1 1 1 0.4", current, 10,
					xMin: string.IsNullOrEmpty(subIcon) ? 0.035f : 0.085f, xMax: 1f, yMin: 0f, yMax: 1f, align: TextAnchor.MiddleLeft);

				if (!string.IsNullOrEmpty(subIcon))
				{
					cui.CreateImage(container, subButton, subIcon, isSelected ? "1 1 1 0.7" : "1 1 1 0.4",
						xMin: iconXmin, xMax: iconXmax, yMin: iconYmin, yMax: iconYmax);
				}

				_offset -= _spacing;
			}

			if (page.TotalPages > 0)
			{
				var controls = cui.CreatePanel(container, inPanel, color: "0.2 0.2 0.2 0.2",
					OyMin: _offset, OyMax: _offset - 2,
					OxMin: shiftOffset);

				var id = cui.CreatePanel(container, controls,
					color: "0.3 0.3 0.3 0.3",
					xMin: 0f, xMax: 1f, yMin: 0, yMax: 1);

				cui.CreateText(container, id,
					color: "1 1 1 0.5",
					text: $"{page.CurrentPage + 1:n0} / {page.TotalPages + 1:n0}", 9,
					xMin: 0.5f, xMax: 1f, yMin: 0, yMax: 1,
					align: TextAnchor.MiddleLeft,
					font: Handler.FontTypes.RobotoCondensedRegular);

				#region Left

				cui.CreateProtectedButton(container, id,
					color: page.CurrentPage > 0 ? "0.8 0.7 0.2 0.7" : "0.3 0.3 0.3 0.1",
					textColor: "1 1 1 0.5",
					text: "<<", 8,
					xMin: 0, xMax: 0.1f, yMin: 0f, yMax: 1f,
					command: $"{command} true --",
					font: Handler.FontTypes.RobotoCondensedRegular);

				cui.CreateProtectedButton(container, id,
					color: "0.4 0.7 0.2 0.7",
					textColor: "1 1 1 0.5",
					text: "<", 8,
					xMin: 0.1f, xMax: 0.2f, yMin: 0f, yMax: 1f,
					command: $"{command} true -1",
					font: Handler.FontTypes.RobotoCondensedRegular);

				#endregion

				#region Right

				cui.CreateProtectedButton(container, id,
					color: page.CurrentPage < page.TotalPages ? "0.8 0.7 0.2 0.7" : "0.3 0.3 0.3 0.1",
					textColor: "1 1 1 0.5",
					text: ">>", 8,
					xMin: 0.9f, xMax: 1f, yMin: 0f, yMax: 1f,
					command: $"{command} true ++",
					font: Handler.FontTypes.RobotoCondensedRegular);

				cui.CreateProtectedButton(container, id,
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
			Tab.OptionButton.Types.Selected => DataInstance.Colors.ButtonSelectedColor,
			Tab.OptionButton.Types.Warned => DataInstance.Colors.ButtonWarnedColor,
			Tab.OptionButton.Types.Important => DataInstance.Colors.ButtonImportantColor,
			_ => DataInstance.Colors.OptionColor
		};

		var panel = cui.CreatePanel(container, parent,
			color: Cache.CUI.BlankColor,
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreateText(container, panel,
				color: DataInstance.Colors.OptionNameColor,
				text: $"{text}:", 12,
				xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
				align: TextAnchor.MiddleLeft,
				font: Handler.FontTypes.RobotoCondensedRegular);

			cui.CreatePanel(container, panel,
				color: color,
				xMin: 0, xMax: DataInstance.Colors.OptionWidth, yMin: 0, yMax: 0.015f);
		}

		var inPanel = cui.CreatePanel(container, panel,
			color: Cache.CUI.BlankColor,
			xMin: DataInstance.Colors.OptionWidth, xMax: 0.985f, yMin: 0, yMax: 1);

		var bar = cui.CreatePanel(container, inPanel,
			color: color, yMin: 0.4f, yMax: 0.6f);

		cui.CreateImage(container, bar, "fade", Cache.CUI.WhiteColor);

		var percentage = value.Scale(min, max, 0f, 1f);

		var barContent = cui.CreatePanel(container, bar,
			color: HexToRustColor("#f54242", 0.8f),
			xMin: 0, xMax: percentage);

		cui.CreateImage(container, barContent, "fade", Cache.CUI.WhiteColor);

		// Indicator
		var indicator = cui.CreatePanel(container, bar,
			color: HexToRustColor("#fc5d5d", 0.8f), xMin: percentage, xMax: percentage, OxMin: -2.5f, OxMax: 2.5f, OyMin: -6f, OyMax: 6f);

		cui.CreateImage(container, indicator, "fade", Cache.CUI.WhiteColor);

		// Text - Align to left ->
		if (percentage <= 0.15f)
		{
			cui.CreateText(container, inPanel, "1 1 1 1", valueText, 8, xMin: percentage + 0.03f, OyMin: -2.5f, align: TextAnchor.LowerLeft);
		}

		// Text - Align to right <-
		else
		{
			cui.CreateText(container, inPanel, "1 1 1 1", valueText, 8, xMax: percentage - 0.03f, OyMin: -2.5f,align: TextAnchor.LowerRight);
		}

		var cuts = max.Clamp(min, RangeCuts);
		var offsetScale = 1f / cuts;
		var currentOffset = 0f;

		for (int i = 0; i < cuts; i++)
		{
			cui.CreateProtectedButton(container, inPanel, Cache.CUI.BlankColor, Cache.CUI.BlankColor, string.Empty, 0,
				xMin: currentOffset, xMax: currentOffset + offsetScale, yMin: 0, yMax: 1,
				command: $"{command} {i}");

			currentOffset += offsetScale;
		}
	}
	public void TabPanelButtonArray(CUI cui, CuiElementContainer container, string parent, string command, float spacing, float height, float offset, PlayerSession session, params Tab.OptionButton[] buttons)
	{
		var panel = cui.CreatePanel(container, parent,
			color: Cache.CUI.BlankColor,
			xMin: 0.015f, xMax: 0.985f, yMin: offset, yMax: offset + height);

		var cuts = (1f / buttons.Length) - spacing;
		var currentOffset = 0f;

		for (int i = 0; i < buttons.Length; i++)
		{
			var button = buttons[i];
			var color = (button.Type == null ? Tab.OptionButton.Types.None : button.Type(session)) switch
			{
				Tab.OptionButton.Types.Selected => DataInstance.Colors.ButtonSelectedColor,
				Tab.OptionButton.Types.Warned => DataInstance.Colors.ButtonWarnedColor,
				Tab.OptionButton.Types.Important => DataInstance.Colors.ButtonImportantColor,
				_ => DataInstance.Colors.OptionColor
			};
			var buttonCui = cui.CreateProtectedButton(container, panel, color, "1 1 1 0.5", button.Name, 11,
				xMin: currentOffset, xMax: currentOffset + cuts, yMin: 0, yMax: 1,
				command: $"{command} {i}");

			cui.CreateImage(container, buttonCui, "fade", Cache.CUI.WhiteColor);

			currentOffset += cuts + spacing;
		}
	}
	public void TabPanelInputButton(CUI cui, CuiElementContainer container, string parent, string text, string command, float buttonPriority, Tab.OptionInput input, Tab.OptionButton button, PlayerSession session, float height, float offset, Tab.Option option = null)
	{
		var color = DataInstance.Colors.OptionColor;
		var buttonColor = (button.Type == null ? Tab.OptionButton.Types.None : button.Type(null)) switch
		{
			Tab.OptionButton.Types.Selected => DataInstance.Colors.ButtonSelectedColor,
			Tab.OptionButton.Types.Warned => DataInstance.Colors.ButtonWarnedColor,
			Tab.OptionButton.Types.Important => DataInstance.Colors.ButtonImportantColor,
			_ => DataInstance.Colors.OptionColor
		};

		var panel = cui.CreatePanel(container, parent,
			color: Cache.CUI.BlankColor,
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreateText(container, panel,
				color: DataInstance.Colors.OptionNameColor,
				text: $"{text}:", 12,
				xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
				align: TextAnchor.MiddleLeft,
				font: Handler.FontTypes.RobotoCondensedRegular);
		}

		var inPanel = cui.CreatePanel(container, panel,
			color: color,
			xMin: DataInstance.Colors.OptionWidth, xMax: 0.985f, yMin: 0, yMax: 1);

		cui.CreatePanel(container, panel,
			color: color,
			xMin: 0, xMax: DataInstance.Colors.OptionWidth, yMin: 0, yMax: 0.015f);

		cui.CreateImage(container, inPanel, "fade", Cache.CUI.WhiteColor, xMin: 0, xMax: 1f - buttonPriority);

		cui.CreateProtectedInputField(container, parent: inPanel,
			color: $"1 1 1 {(input.ReadOnly ? 0.2f : 1f)}",
			text: input.Placeholder?.Invoke(session), 11,
			xMin: 0.03f, xMax: 1f - buttonPriority, yMin: 0, yMax: 1,
			command: $"{command} input",
			align: TextAnchor.MiddleLeft,
			characterLimit: input.CharacterLimit,
			readOnly: input.ReadOnly,
			needsKeyboard: session.Input == option,
			autoFocus: session.Input == option && session.Input != session.PreviousInput,
			font: Handler.FontTypes.RobotoCondensedRegular);

		if (session.Input == option)
		{
			session.PreviousInput = session.Input;
		}

		var buttonCui = cui.CreateProtectedButton(container, parent: inPanel,
			color: buttonColor,
			textColor: "1 1 1 0.5",
			text: button.Name, 11,
			xMin: 1f - buttonPriority, xMax: 1f, yMin: 0f, yMax: 1f,
			command: $"{command} button",
			align: button.Align,
			font: Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateImage(container, buttonCui, "fade", Cache.CUI.WhiteColor);

		if (!input.ReadOnly)
		{
			cui.CreatePanel(container, inPanel,
				color: $"{DataInstance.Colors.EditableInputHighlight} 0.9",
				xMin: 0, xMax: 1f - buttonPriority, yMin: 0, yMax: 0.05f,
				OxMax: -0.5f);
		}
	}
	public void TabPanelColor(CUI cui, CuiElementContainer container, string parent, string text, string color, string command, float height, float offset)
	{
		var toggleButtonScale = 0.825f;

		var panel = cui.CreatePanel(container, parent,
			color: Cache.CUI.BlankColor,
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		if (!string.IsNullOrEmpty(text))
		{
			cui.CreateText(container, panel,
				color: DataInstance.Colors.OptionNameColor,
				text: $"{text}:", 12,
				xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
				align: TextAnchor.MiddleLeft,
				font: Handler.FontTypes.RobotoCondensedRegular);

			cui.CreatePanel(container, panel,
				color: DataInstance.Colors.OptionColor,
				xMin: 0, xMax: toggleButtonScale, yMin: 0, yMax: 0.015f);
		}

		var colorSplit = color.Split(' ');
		cui.CreateProtectedButton(container, parent,
			color: color,
			textColor: "1 1 1 1",
			text: colorSplit.Length > 1 ? $"#{ColorUtility.ToHtmlStringRGB(new Color(colorSplit[0].ToFloat(), colorSplit[1].ToFloat(), colorSplit[2].ToFloat(), 1))}" : string.Empty, 10,
			xMin: toggleButtonScale, xMax: 0.985f, yMin: offset, yMax: offset + height,
			command: command,
			font: Handler.FontTypes.RobotoCondensedRegular);
	}
	public void TabPanelWidget(CUI cui, CuiElementContainer container, string parent, PlayerSession session, Tab.OptionWidget widget, float height, float offset)
	{
		widget.WidgetPanel = cui.CreatePanel(container, parent,
			color: Cache.CUI.BlankColor,
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		widget.Callback?.Invoke(session, cui, container, widget.WidgetPanel);

	}
	public void TabPanelChart(CUI cui, CuiElementContainer container, string parent, PlayerSession session, Tab.OptionChart chart, float height, float offset, float panelSpacing, string layerCommand, string layerShadowCommand, Tab tab, int columnIndex)
	{
		var currentPage = session.GetOrCreatePage(columnIndex);
		var canExpand = !chart.Responsive &&
		                tab.Columns.All(x => session.GetOrCreatePage(x.Key).CurrentPage == currentPage.CurrentPage);

		var width = (chart.Responsive || !canExpand ? 1 : tab.Columns.Count) + (panelSpacing * tab.Columns.Count);
		var panel = cui.CreatePanel(container, parent,
			color: Cache.CUI.BlankColor,
			xMin: 0, xMax: 1f * width, yMin: offset, yMax: offset + height);

		if (!chart.Responsive && !canExpand)
		{
			cui.CreatePanel(container, panel, "0.15 0.15 0.15 0.3", blur: true, OyMax: 25f);
			cui.CreateText(container, panel, "1 1 1 0.5", "To view the chart,\nremain on the same page number as this.", 10);
			return;
		}

		if (chart.IsEmpty())
		{
			cui.CreatePanel(container, panel, "0.15 0.15 0.15 0.3", blur: true, OyMax: 25f);
			cui.CreateText(container, panel, "1 1 1 0.5", "No data available.", 10);
			return;
		}

		if (!chart.Responsive)
		{
			cui.CreatePanel(container, panel, "0.15 0.15 0.15 0.3", blur: true, OyMax: 25f);
		}

		var scroll = cui.CreateScrollView(container, panel, false, true,
			ScrollRect.MovementType.Clamped,
			0.5f, true, 0.3f, 120,
			out var content, out var hScroll, out _,
			xMin: 0);

		content.AnchorMin = "0 0";
		content.AnchorMax = "0 1";
		content.OffsetMin = "0 0";
		content.OffsetMax = "3500 0";

		hScroll.TrackColor = "0 0 0 0";
		hScroll.HandleColor = hScroll.HighlightColor = "0.2 0.2 0.2 1";
		hScroll.Size = 1;
		hScroll.Invert = true;

		var vLabelPanel = cui.CreatePanel(container, panel, Cache.CUI.BlankColor,
			xMin: 0f, xMax: 0.05f, yMin: 0.078f);

		var identifier = chart.GetIdentifier();

		var labelCount = chart.Chart.verticalLabels.Length;

		if (labelCount != 1)
		{
			var labelIndex = 0;

			foreach (var label in chart.Chart.verticalLabels)
			{
				var labelOffset = labelIndex.Scale(0, labelCount - 1, 0, 150);

				cui.CreateText(container, vLabelPanel, "1 1 1 0.9", label, 7,
					xMin: 0f, xMax: 0.9f, yMin: 0f, yMax: 0f, OyMin: labelOffset, OyMax: labelOffset, align: TextAnchor.MiddleRight);

				labelIndex++;
			}
		}

		var layerIndex = -1;
		var xOffset = 0f;
		var xOffsetWidth = 47.5f;
		var xMoving = 50;
		var spacing = -5;

		var loadingOverlay = cui.CreatePanel(container, panel, "0 0 0 0.2", blur: true, id: $"{identifier}_loading");
		var loadingText = cui.CreateText(container, loadingOverlay, "1 1 1 0.5", "Please wait...", 10, id: $"{identifier}_loadingtxt");
		var chartImage = cui.CreateImage(container, scroll, 0, Cache.CUI.WhiteColor, xMin: 0.01f, id: $"{identifier}_chart");

		CreateLayerButton("All", System.Drawing.Color.BlanchedAlmond, chart.Chart.Layers.All(x => x.Disabled), !chart.Chart.Layers.All(x => x.LayerSettings.Shadows == 0));

		foreach (var layer in chart.Chart.Layers)
		{
			CreateLayerButton(layer.Name, layer.LayerSettings.Color, !layer.Disabled, layer.LayerSettings.Shadows > 0);
		}

		void CreateLayerButton(string text, System.Drawing.Color color, bool mainEnabled, bool secondEnabled)
		{
			var textLength = text.Length;
			var pColor = color;
			var sColor = System.Drawing.Color.FromArgb((int)(pColor.R * 1.5f).Clamp(0, 255), (int)(pColor.G * 1.5f).Clamp(0, 255), (int)(pColor.B * 1.5f).Clamp(0, 255));
			var rustSColor = $"{sColor.R / 255f} {sColor.G / 255f} {sColor.B / 255f} 1";

			var mainButton = cui.CreateProtectedButton(container, panel, $"{pColor.R / 255f} {pColor.G / 255f} {pColor.B / 255f} {(!mainEnabled ? 0.15 : 0.5)}", rustSColor, $"    {text}", 8,
				xMin: 0.01f, xMax: 0, yMin: 0.94f, yMax: 1f, OxMin: xMoving + xOffset, OxMax: xMoving + (xOffset += xOffsetWidth + (textLength * 3f)), OyMin: -15, OyMax: -15,
				command: $"{layerCommand} {layerIndex} {identifier} {layerCommand}", id: $"{identifier}_layerbtn_{layerIndex}");

			cui.CreateProtectedButton(container, mainButton, $"{pColor.R / 255f} {pColor.G / 255f} {pColor.B / 255f} {(!secondEnabled ? 0.15 : 0.5)}", rustSColor, "\u29bf", 8,
				xMin: 0, xMax: 0, OxMax: 12.5f, command: $"{layerShadowCommand} {layerIndex} {identifier} {layerShadowCommand}", id: $"{identifier}_layerbtn2_{layerIndex}");

			xOffset += spacing;
			layerIndex++;
		}

		cui.CreateText(container, panel, Cache.CUI.WhiteColor, chart.Name, chart.NameSize, xMin: 0.025f, xMax: 0.95f, yMin: 1, yMax: 1, OyMin: 10, OyMax: 17.5f, align: chart.NameAlign, font: Handler.FontTypes.RobotoCondensedBold);

		Community.Runtime.Core.NextFrame(() =>
		{
			Tab.OptionChart.Cache.GetOrProcessCache(identifier, chart.Chart, chartCache =>
			{
				using var cui = new CUI(Handler);
				using var pool = cui.UpdatePool();

				switch (chartCache.Status)
				{
					case Tab.OptionChart.ChartCache.StatusTypes.Finalized:
					{
						if (!chartCache.HasPlayerReceivedData(session.Player.userID))
						{
							CommunityEntity.ServerInstance.ClientRPC(RpcTarget.Player("CL_ReceiveFilePng", session.Player), chartCache.Crc,
								(uint)chartCache.Data.Length, chartCache.Data, 0, (byte)FileStorage.Type.png);
						}

						pool.Add(cui.UpdatePanel(loadingOverlay, "0 0 0 0", xMax: 0, blur: false));
						pool.Add(cui.UpdateText(loadingText, "0 0 0 0", string.Empty, 0));
						pool.Add(cui.UpdateImage(chartImage, chartCache.Crc, Cache.CUI.WhiteColor));
						pool.Send(session.Player);
						break;
					}

					default:
					case Tab.OptionChart.ChartCache.StatusTypes.Failure:
					{
						pool.Add(cui.UpdateText(loadingText, "0.9 0.1 0.1 0.75", "Failed to load chart!", 10));
						pool.Send(session.Player);
						break;
					}
				}
			});
		});
	}
	public void TabTooltip(CUI cui, CuiElementContainer container, string parent, Tab.Option tooltip, string command, PlayerSession admin, float height, float offset)
	{
		if (admin.Tooltip == tooltip)
		{
			var tip = cui.CreatePanel(container, parent, "#1a6498",
				xMin: 0.05f, xMax: ((float)admin.Tooltip.Tooltip.Length).Scale(1f, 78f, 0.1f, 0.79f), yMin: offset, yMax: offset + height);

			cui.CreateText(container, tip, "#6bc0fc", admin.Tooltip.Tooltip, 10);
		}

		if (!string.IsNullOrEmpty(tooltip.Tooltip))
		{
			cui.CreateProtectedButton(container, parent, Cache.CUI.BlankColor, Cache.CUI.BlankColor, string.Empty, 0,
				xMin: 0, xMax: DataInstance.Colors.OptionWidth, yMin: offset, yMax: offset + height,
				command: $"{command} tooltip");
		}
	}

	#endregion

	#region Methods

	public const float MaximizedScale_XMin = 1.1f;
	public const float MaximizedScale_XMax = 1.1f;
	public const float MaximizedScale_YMin = 1.15f;
	public const float MaximizedScale_YMax = 1.15f;

	public const float OptionHeightOffset = 0.0035f;

	public void Draw(BasePlayer player)
	{
		try
		{
			var ap = GetPlayerSession(player);
			var tab = GetTab(player);
			ap.IsInMenu = true;

			if (CanAccess(player) && !DataInstance.GreetDisplayed && (tab != null && tab.Id != "greet" && tab.Id != "configeditor") && HasAccess(player, "greet"))
			{
				tab = ap.SelectedTab = Greet.Make();
			}

			if(ap.SelectedTab != null && (!string.IsNullOrEmpty(ap.SelectedTab.Access) && !HasAccess(player, ap.SelectedTab.Access) || DataInstance.IsTabHidden(ap.SelectedTab.Id)))
			{
				ap.SelectedTab = null;
			}

			using var cui = new CUI(Handler);

			var container = cui.CreateContainer(PanelId,
				color: $"0 0 0 {DataInstance.BackgroundOpacity}",
				xMin: 0, xMax: 1, yMin: 0, yMax: 1,
				needsCursor: true, destroyUi: PanelId, parent: ClientPanels.HudMenu);

			cui.CreateImage(container, PanelId, "fade", Cache.CUI.WhiteColor);

			var isMaximized = DataInstance.Maximize;

			var shade = cui.CreatePanel(container, parent: PanelId, id: $"{PanelId}color",
				color: "0 0 0 0.6",
				xMin: 0.5f, xMax: 0.5f, yMin: 0.5f, yMax: 0.5f,
				OxMin: -475 * (isMaximized ? MaximizedScale_XMin : 1),
				OxMax: 475 * (isMaximized ? MaximizedScale_XMax : 1),
				OyMin: -300 * (isMaximized ? MaximizedScale_YMin : 1),
				OyMax: 300 * (isMaximized ? MaximizedScale_YMax : 1));
			var main = cui.CreatePanel(container, shade,
				color: "0 0 0 0.5",
				blur: DataInstance.BackgroundBlur);

			cui.CreateImage(container, main, DataInstance.BackgroundImage, "1 1 1 " + DataInstance.BackgroundImageOpacity, yMin: DataInstance.BackgroundImageYAnchor.x, yMax: DataInstance.BackgroundImageYAnchor.y);

			using (TimeMeasure.New($"{Name}.Main"))
			{
				if (tab is not { IsFullscreen: true })
				{
					#region Title

					cui.CreateText(container, parent: main,
						color: "1 1 1 0.8",
						text: Title, 18,
						xMin: 0.0175f, yMin: 0.8f, xMax: 1f, yMax: 0.97f,
						align: TextAnchor.UpperLeft,
						font: Handler.FontTypes.RobotoCondensedBold);

					#endregion

					#region Tabs
					try
					{
						var tabButtons = cui.CreatePanel(container, parent: main, id: null,
							color: "0 0 0 0.6", xMin: 0.01f, xMax: 0.99f, yMin: 0.875f, yMax: 0.92f);

						var availableTabs = Tabs.Where(x => !DataInstance.IsTabHidden(x.Id));
						var tabIndex = 0f;
						var amount = availableTabs.Count();
						var tabWidth = amount == 0 ? 0f : 1f / amount;

						for (int i = ap.TabSkip; i < amount; i++)
						{
							var _tab = availableTabs.ElementAt(ap.TabSkip + i);
							if (DataInstance.IsTabHidden(_tab.Id))
							{
								continue;
							}
							var plugin = _tab.Plugin.IsCorePlugin ? string.Empty : $"<size=8>\nby {_tab.Plugin?.Name}</size>";
							TabButton(cui, container, tabButtons, $"{(availableTabs.IndexOf(ap.SelectedTab) == i ? $"<b>{_tab.Name}</b>" : _tab.Name)}{plugin}", PanelId + $".changetab {_tab.Id}", tabWidth, tabIndex, availableTabs.IndexOf(ap.SelectedTab) == i, !HasAccess(player, _tab.Access));
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
					var panels = cui.CreatePanel(container, main,
						color: Cache.CUI.BlankColor,
						xMin: 0.01f, xMax: 0.99f, yMin: 0.02f, yMax: tab != null && tab.IsFullscreen ? 0.98f : 0.86f);

					cui.CreateImage(container, panels, "fade", Cache.CUI.WhiteColor);

					if (tab != null)
					{
						tab.Under?.Invoke(tab, cui, container, panels, ap);

						if (tab.Override == null)
						{
							#region Columns

							var spacing = 0.005f;
							var panelWidth = (tab.Columns.Count == 0 ? 0f : 1f / tab.Columns.Count) - spacing;
							var panelIndex = (panelWidth + spacing) * (tab.Columns.Count - 1);

							for (int i = tab.Columns.Count; i-- > 0;)
							{
								var rows = tab.Columns[i];
								var panel = cui.CreatePanel(container, panels, color: "0 0 0 " + DataInstance.BackgroundColumnOpacity,
									xMin: panelIndex, xMax: panelIndex + panelWidth - spacing, yMin: 0, yMax: 1, id: $"sub{i}");

								cui.CreateImage(container, panel, "fade", "1 1 1 " + DataInstance.BackgroundColumnOpacity);

								#region Rows

								var columnPage = ap.GetOrCreatePage(i);
								var contentsPerPage = 19 - (rows.pinnedOption == null ? 0 : 1);
								const float rowSpacing = 0.01f;
								var rowHeight = 0.04f;
								var rowPage = rows.Skip(contentsPerPage * columnPage.CurrentPage).Take(contentsPerPage);
								var rowPageCount = rowPage.Count();
								columnPage.TotalPages = (int)Math.Ceiling(((double)rows.Count) / contentsPerPage - 1);
								columnPage.Check();
								var rowIndex = (rowHeight + rowSpacing) * (contentsPerPage - (rowPageCount - (columnPage.TotalPages > 0 ? 0 : 1)));

								if (rowPageCount == 0)
								{
									cui.CreateText(container, panel,
										color: "1 1 1 0.35", text: GetPhrase("nocontent", player.UserIDString), 8, align: TextAnchor.MiddleCenter);
								}

								if (columnPage.TotalPages > 0)
								{
									rowHeight += OptionHeightOffset;

									TabColumnPagination(cui, container, panel, i, columnPage, rowHeight, 0);

									rowHeight -= OptionHeightOffset;

									rowIndex += rowHeight + rowSpacing;
								}

								for (int r = rowPageCount; r-- > 0;)
								{
									var actualI = r + (columnPage.CurrentPage * contentsPerPage);
									var row = rows[actualI];

									rowHeight += OptionHeightOffset;

									DrawRow(cui, container, panel, row, i, actualI, rowHeight, rowIndex, ap, tab);

									rowHeight -= OptionHeightOffset;
									rowIndex += rowHeight + rowSpacing;
								}

								if (rows.pinnedOption != null)
								{
									DrawRow(cui, container, panel, rows.pinnedOption, i, -1, rowHeight, rowIndex, ap, tab);
								}

								static void DrawRow(CUI cui, CuiElementContainer container, string panel, Tab.Option row, int i, int actualI, float rowHeight, float rowIndex, PlayerSession ap, Tab tab)
								{
									switch (row)
									{
										case Tab.OptionName name:
											Singleton.TabPanelName(cui, container, panel, name.Name, rowHeight, rowIndex, name.Align);
											HandleReveal(0f, row, cui, container, panel, rowIndex, rowHeight, i, actualI);
											break;

										case Tab.OptionButton button:
											Singleton.TabPanelButton(cui, container, panel, button.Name, PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex, button.Type == null ? Tab.OptionButton.Types.None : button.Type.Invoke(ap), button.Align);
											HandleReveal(0f, row, cui, container, panel, rowIndex, rowHeight, i, actualI);
											break;

										case Tab.OptionText text:
											Singleton.TabPanelText(cui, container, panel, text.Name, text.Size, text.Color, rowHeight, rowIndex, text.Align, text.Font, text.IsInput);
											HandleReveal(0f, row, cui, container, panel, rowIndex, rowHeight, i, actualI);
											break;

										case Tab.OptionInput input:
											Singleton.TabPanelInput(cui, container, panel, input.Name, input.Placeholder?.Invoke(ap), PanelId + $".callaction {i} {actualI}", input.CharacterLimit, input.ReadOnly, rowHeight, rowIndex, ap, option: input);
											HandleReveal(Singleton.DataInstance.Colors.OptionWidth, row, cui, container, panel, rowIndex, rowHeight, i, actualI);
											HandleInputHighlight(Singleton.DataInstance.Colors.OptionWidth, row, cui, container, panel, ap, rowIndex, rowHeight, i, actualI);
											break;

										case Tab.OptionEnum @enum:
											Singleton.TabPanelEnum(cui, container, panel, @enum.Name, @enum.Text?.Invoke(ap), PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex);
											HandleReveal(Singleton.DataInstance.Colors.OptionWidth, row, cui, container, panel, rowIndex, rowHeight, i, actualI);
											break;

										case Tab.OptionToggle toggle:
											Singleton.TabPanelToggle(cui, container, panel, toggle.Name, PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex, toggle.IsOn != null ? toggle.IsOn.Invoke(ap) : false, tab);
											HandleReveal(Singleton.DataInstance.Colors.OptionWidth, row, cui, container, panel, rowIndex, rowHeight, i, actualI);
											break;

										case Tab.OptionRadio radio:
											Singleton.TabPanelRadio(cui, container, panel, radio.Name, radio.Index == tab.Radios[radio.Id].Selected, PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex);
											HandleReveal(Singleton.DataInstance.Colors.OptionWidth, row, cui, container, panel, rowIndex, rowHeight, i, actualI);
											break;

										case Tab.OptionDropdown dropdown:
											Singleton.TabPanelDropdown(cui, ap._selectedDropdownPage, container, panel, dropdown.Name, PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex, dropdown.Index.Invoke(ap), dropdown.Options, dropdown.OptionsIcons, ap._selectedDropdown == dropdown);
											HandleReveal(Singleton.DataInstance.Colors.OptionWidth, row, cui, container, panel, rowIndex, rowHeight, i, actualI);
											break;

										case Tab.OptionRange range:
											Singleton.TabPanelRange(cui, container, panel, range.Name, PanelId + $".callaction {i} {actualI}", range.Text?.Invoke(ap), range.Min, range.Max, range.Value == null ? 0 : range.Value.Invoke(ap), rowHeight, rowIndex);
											HandleReveal(Singleton.DataInstance.Colors.OptionWidth, row, cui, container, panel, rowIndex, rowHeight, i, actualI);
											break;

										case Tab.OptionButtonArray array:
											Singleton.TabPanelButtonArray(cui, container, panel, PanelId + $".callaction {i} {actualI}", array.Spacing, rowHeight, rowIndex, ap, array.Buttons);
											break;

										case Tab.OptionInputButton inputButton:
											Singleton.TabPanelInputButton(cui, container, panel, inputButton.Name, PanelId + $".callaction {i} {actualI}", inputButton.ButtonPriority, inputButton.Input, inputButton.Button, ap, rowHeight, rowIndex, option: inputButton);
											HandleReveal(Singleton.DataInstance.Colors.OptionWidth, row, cui, container, panel, rowIndex, rowHeight, i, actualI);
											HandleInputHighlight(Singleton.DataInstance.Colors.OptionWidth, row, cui, container, panel, ap, rowIndex, rowHeight, i, actualI, 1f - inputButton.ButtonPriority, "input");
											break;

										case Tab.OptionColor color:
											Singleton.TabPanelColor(cui, container, panel, color.Name, color.Color?.Invoke() ?? "0.1 0.1 0.1 0.5", PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex);
											HandleReveal(Singleton.DataInstance.Colors.OptionWidth, row, cui, container, panel, rowIndex, rowHeight, i, actualI);
											break;

										case Tab.OptionWidget widget:
											Singleton.TabPanelWidget(cui, container, panel, ap, widget, rowHeight * (widget.Height + 1), rowIndex);
											break;

										case Tab.OptionChart chart:
											Singleton.TabPanelChart(cui, container, panel, ap, chart, rowHeight * (Tab.OptionChart.Height + 1), rowIndex, rowSpacing, layerCommand: PanelId + $".callaction {i} {actualI} layer", layerShadowCommand: PanelId + $".callaction {i} {actualI} layershadow", tab, i);
											break;
									}

									#region Reveal

									static void HandleReveal(float xMin, Tab.Option row, CUI cui, CuiElementContainer container, string panel, float rowIndex, float rowHeight, int i, int actualI)
									{
										if (!row.CurrentlyHidden) return;

										var blur = cui.CreatePanel(container, parent: panel,
											color: "0 0 0 0.4",
											xMin: xMin, xMax: 0.985f, yMin: rowIndex, yMax: rowIndex + rowHeight,
											blur: true);

										cui.CreateImage(container, blur, "fade", Cache.CUI.WhiteColor);

										cui.CreateProtectedButton(container, blur,
											color: Cache.CUI.BlankColor, textColor: "1 1 1 0.5", text: "REVEAL".SpacedString(1), 8, command: PanelId + $".callaction {i} {actualI}");
									}

									static void HandleInputHighlight(float xMin, Tab.Option row, CUI cui, CuiElementContainer container, string panel, PlayerSession ap, float rowIndex, float rowHeight, int i, int actualI, float xMax = 0.985f, string command = null)
									{
										if (row == ap.Input) return;

										cui.CreateProtectedButton(container, panel,
											color: Cache.CUI.BlankColor, Cache.CUI.BlankColor, string.Empty, 0,
											xMin: xMin, xMax: xMax, yMin: rowIndex, yMax: rowIndex + rowHeight,
											command: PanelId + $".callaction {i} {actualI} {command}");
									}

									#endregion

									#region Tooltip

									Singleton.TabTooltip(cui, container, panel, row, PanelId + $".callaction {i} {actualI}", ap, rowHeight, rowIndex);

									#endregion
								}

								#endregion

								panelIndex -= panelWidth + spacing;
							}

							#endregion
						}
						else
						{
							tab.Override.Invoke(tab, cui, container, panels, ap);
						}

						tab.Over?.Invoke(tab, cui, container, panels, ap);

						if (tab.Dialog != null)
						{
							var dialog = cui.CreatePanel(container, panels, "0.15 0.15 0.15 0.2", blur: true);
							cui.CreatePanel(container, dialog, "0 0 0 0.9");

							cui.CreateText(container, dialog,
								"1 1 1 1", tab.Dialog.Title, 20, yMin: 0.1f);

							cui.CreateText(container, dialog,
								"1 1 1 0.4", "Confirm action".ToUpper().SpacedString(3), 10, yMin: 0.2f);

							cui.CreateProtectedButton(container, dialog, "0.9 0.4 0.3 0.8", "1 1 1 0.7", "DECLINE".SpacedString(1), 10,
								xMin: 0.4f, xMax: 0.49f, yMin: 0.425f, yMax: 0.475f, command: $"{PanelId}.dialogaction decline");

							cui.CreateProtectedButton(container, dialog, "0.4 0.9 0.3 0.8", "1 1 1 0.7", "CONFIRM".SpacedString(1), 10,
								xMin: 0.51f, xMax: 0.6f, yMin: 0.425f, yMax: 0.475f, command: $"{PanelId}.dialogaction confirm");
						}
					}
					else
					{
						cui.CreateText(container, panels, "1 1 1 0.4", "No tab selected.", 9);
					}
				}
			}
			catch (Exception ex) { PutsError($"Draw({player}).Panels", ex); }
			#endregion

			#region Exit

			using (TimeMeasure.New($"{Name}.Exit"))
			{
				var shift = tab == null || tab.IsFullscreen ? 15 : 0;

				var maximizeButton = cui.CreateProtectedButton(container, main,
					color: "#d1cd56",
					textColor: Cache.CUI.BlankColor,
					text: string.Empty, 0,
					xMin: 0.9675f, xMax: 0.99f, yMin: 0.955f, yMax: 0.99f,
					OxMin: -25 * 3, OxMax: -25 * 3,
					OyMin: shift, OyMax: shift,
					command: PanelId + ".maximize");
				{
					cui.CreateImage(container, maximizeButton, DataInstance.Maximize ? "minimize" : "maximize", "#fffed4",
						xMin: 0.15f, xMax: 0.85f,
						yMin: 0.15f, yMax: 0.85f);

					cui.CreateImage(container, maximizeButton, "fade", Cache.CUI.WhiteColor);
				}

				var canAccessProfiler = HasAccess(ap.Player, "profiler.use");
				var profilerButton = cui.CreateProtectedButton(container, main,
					color: !canAccessProfiler ? "0.3 0.3 0.3 0.7" : "#6651c2",
					textColor: Cache.CUI.BlankColor,
					text: string.Empty, 0,
					xMin: 0.9675f, xMax: 0.99f, yMin: 0.955f, yMax: 0.99f,
					OxMin: -25 * 2, OxMax: -25 * 2,
					OyMin: shift, OyMax: shift,
					command: canAccessProfiler ? PanelId + ".profiler" : string.Empty);
				{
					cui.CreateImage(container, profilerButton, "graph", "#af9ff5",
						xMin: 0.15f, xMax: 0.85f,
						yMin: 0.15f, yMax: 0.85f);

					cui.CreateImage(container, profilerButton, "fade", Cache.CUI.WhiteColor);

					if (ap.SelectedTab != null && ap.SelectedTab.Id == "profiler")
					{
						cui.CreatePanel(container, profilerButton, "1 0 0 1", yMax: 0.1f);
					}
				}

				var canAccessConfig = HasAccess(ap.Player, "config.use");
				var configButton = cui.CreateProtectedButton(container, main,
					color: canAccessConfig ? "0.2 0.6 0.2 0.9" : "0.3 0.3 0.3 0.7",
					textColor: Cache.CUI.BlankColor,
					text: string.Empty, 0,
					xMin: 0.9675f, xMax: 0.99f, yMin: 0.955f, yMax: 0.99f,
					OxMin: -25, OxMax: -25,
					OyMin: shift, OyMax: shift,
					command: canAccessConfig ? PanelId + ".config" : string.Empty);
				{
					cui.CreateImage(container, configButton, "gear", "0.5 1 0.5 1",
						xMin: 0.15f, xMax: 0.85f,
						yMin: 0.15f, yMax: 0.85f);

					cui.CreateImage(container, configButton, "fade", Cache.CUI.WhiteColor);

					if (ap.SelectedTab != null && ap.SelectedTab.Id == "configuration")
					{
						cui.CreatePanel(container, configButton, "1 0 0 1", yMax: 0.1f);
					}
				}

				var closeButton = cui.CreateProtectedButton(container, main,
					color: "0.6 0.2 0.2 0.9",
					textColor: Cache.CUI.BlankColor,
					text: string.Empty, 0,
					xMin: 0.9675f, xMax: 0.99f, yMin: 0.955f, yMax: 0.99f,
					OyMin: shift, OyMax: shift,
					command: PanelId + ".close");
				{
					cui.CreateImage(container, closeButton, "close", "1 0.5 0.5 1",
						xMin: 0.2f, xMax: 0.8f,
						yMin: 0.2f, yMax: 0.8f);

					cui.CreateImage(container, closeButton, "fade", Cache.CUI.WhiteColor);
				}
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
			color: Cache.CUI.BlankColor,
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
		ap.SelectedTab?.ResetHiddens();

		var noneInMenu = true;
		foreach (var admin in PlayerSessions)
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

	public void SetTab(BasePlayer player, string id, bool onChange = true)
	{
		var ap = GetPlayerSession(player);
		var previous = ap.SelectedTab;

		var tab = Tabs.FirstOrDefault(x => !DataInstance.IsTabHidden(x.Id) && HasAccess(player, x.Access) && x.Id == id);
		if (tab != null)
		{
			ap.Tooltip = null;
			if (onChange) try { tab?.OnChange?.Invoke(ap, tab); } catch { }
		}
		ap.SelectedTab = tab;

		if (ap.SelectedTab != previous)
		{
			ap.Input = ap.PreviousInput = null;
			ap.SelectedTab?.ResetHiddens();
			Draw(player);
		}
	}
	public void SetTab(BasePlayer player, Tab tab, bool onChange = true)
	{
		var ap = GetPlayerSession(player);
		var previous = ap.SelectedTab;

		tab = string.IsNullOrEmpty(tab.Access) ? tab : HasAccess(player, tab.Access) ? tab : Tabs.FirstOrDefault(x => !DataInstance.IsTabHidden(x.Id) && HasAccess(player, x.Access));
		if (DataInstance.IsTabHidden(tab.Id))
		{
			tab = null;
		}
		if (tab != null)
		{
			ap.Tooltip = null;
			if (onChange) try { tab?.OnChange?.Invoke(ap, tab); } catch { }
		}
		ap.SelectedTab = tab;

		if (ap.SelectedTab != previous)
		{
			ap.Input = ap.PreviousInput = null;
			ap.SelectedTab?.ResetHiddens();
			Draw(player);
		}
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
	public bool CallColumnRow(BasePlayer player, int column, int row, IEnumerable<string> args)
	{
		var ap = GetPlayerSession(player);
		var tab = GetTab(player);

		ap.LastPressedColumn = column;
		ap.LastPressedRow = row;

		var rows = tab.Columns[column];
		Tab.Option option = row == -1 ? rows.pinnedOption : rows[row];

		if (args.Count() > 0 && args.ElementAt(0) == "tooltip")
		{
			if (ap.Tooltip != option) ap.Tooltip = option;
			else ap.Tooltip = null;
			return true;
		}

		if (option.CurrentlyHidden)
		{
			option.CurrentlyHidden = false;
			return true;
		}

		switch (option)
		{
			case Tab.OptionButton button:
				button.Callback?.Invoke(ap);
				return button.Callback != null;

			case Tab.OptionInput input:
				if (ap.Input != input)
				{
					ap.Input = input;
					return true;
				}
				else
				{
					if (!input.ReadOnly)
					{
						input.Callback?.Invoke(ap, args);
					}

					ap.Input = ap.PreviousInput = null;
					return input.Callback != null;
				}

			case Tab.OptionEnum @enum:
				@enum.Callback?.Invoke(ap, args.ElementAt(0).ToBool());
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
				switch (args.ElementAt(0).ToBool())
				{
					case true:
						switch (args.ElementAt(1))
						{
							case "call":
								ap._selectedDropdown = null;
								dropdown.Callback?.Invoke(ap, args.ElementAt(2).ToInt());
								page.CurrentPage = 0;
								break;

							default:
								switch (args.ElementAt(1))
								{
									case "--":
										page.CurrentPage = 0;
										break;

									case "++":
										page.CurrentPage = page.TotalPages;
										break;

									default:
										page.CurrentPage += args.ElementAt(1).ToInt();
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
				range.Callback?.Invoke(ap, args.ElementAt(0).ToFloat().Scale(0f, range.Max.Clamp(range.Min, RangeCuts) - 1f, range.Min, range.Max));
				return range.Callback != null;

			case Tab.OptionButtonArray array:
				var callback = array.Buttons[args.ElementAt(0).ToInt()].Callback;
				callback?.Invoke(ap);
				return callback != null;

			case Tab.OptionInputButton inputButton:
				switch (args.ElementAt(0))
				{
					case "input":
					{
						if (ap.Input != inputButton)
						{
							ap.Input = inputButton;
							return true;
						}
						else
						{
							ap.Input = ap.PreviousInput = null;

							if (!inputButton.Input.ReadOnly)
							{
								var enumerable = args.Skip(1);
								inputButton.Input.Callback?.Invoke(ap, enumerable.Count() == 0 ? EmptyElement : enumerable);
							}

							return inputButton.Input.Callback != null;
						}
					}
					case "button":
						inputButton.Button.Callback?.Invoke(ap);
						return inputButton.Button.Callback != null;
				}
				break;

			case Tab.OptionColor color:
				if (color.Callback != null)
				{
					ColorPicker.Open(player, (rustColor, hexColor, alpha) => { color.Callback?.Invoke(ap, rustColor, hexColor, alpha); });
					return false;
				}
				break;

			case Tab.OptionChart chart:
			{
				var layerIndex = args.ElementAt(1).ToInt();

				switch (args.ElementAt(0))
				{
					case "layer":
					{
						var oldIdentifier = args.ElementAt(2);
						var newIdentifier = string.Empty;

						using var cui = new CUI(Handler);
						using var pool = cui.UpdatePool();

						var mainEnabled = false;
						var pColor = System.Drawing.Color.BlanchedAlmond;
						var text = $"    All";

						if (layerIndex == -1)
						{
							var allDisabled = chart.Chart.Layers.All(x => x.Disabled);

							foreach (var chartLayer in chart.Chart.Layers)
							{
								chartLayer.Disabled = !allDisabled;
							}

							newIdentifier = chart.GetIdentifier(reset: true);
							return true;
						}
						else
						{
							var layer = chart.Chart.Layers.ElementAt(layerIndex);
							layer.ToggleDisabled();

							newIdentifier = chart.GetIdentifier(reset: true);
							mainEnabled = !layer.Disabled;
							pColor = layer.LayerSettings.Color;
							text = $"    {layer.Name}";
						}

						var sColor = System.Drawing.Color.FromArgb((int)(pColor.R * 1.5f).Clamp(0, 255), (int)(pColor.G * 1.5f).Clamp(0, 255), (int)(pColor.B * 1.5f).Clamp(0, 255));
						var rustSColor = $"{sColor.R / 255f} {sColor.G / 255f} {sColor.B / 255f} 1";

						var mainCommand = args.Skip(3).ToString(" ");

						pool.Add(cui.UpdatePanel($"{oldIdentifier}_loading", "0 0 0 0.2", xMin: 0.01f, xMax: 0.99f, yMin: 0.01f, yMax: 0.99f, blur: true));
						pool.Add(cui.UpdateText($"{oldIdentifier}_loadingtxt", "1 1 1 0.5", "Please wait...", 10, xMin: 0.01f, xMax: 0.99f, yMin: 0.01f, yMax: 0.99f));
						pool.Add(cui.UpdateImage($"{oldIdentifier}_chart", 0, Cache.CUI.WhiteColor, xMin: 0.01f));
						pool.Add(cui.UpdateProtectedButton($"{oldIdentifier}_layerbtn_{layerIndex}", $"{pColor.R / 255f} {pColor.G / 255f} {pColor.B / 255f} {(!mainEnabled ? 0.15 : 0.5)}", rustSColor, text, 8,
							command: $"{mainCommand} {layerIndex} {oldIdentifier} {mainCommand}"));
						pool.Send(ap.Player);

						Tab.OptionChart.Cache.GetOrProcessCache(newIdentifier, chart.Chart, chartCache =>
						{
							using var cui = new CUI(Handler);
							using var pool = cui.UpdatePool();

							switch (chartCache.Status)
							{
								case Tab.OptionChart.ChartCache.StatusTypes.Finalized:
								{
									if (!chartCache.HasPlayerReceivedData(ap.Player.userID))
									{
										CommunityEntity.ServerInstance.ClientRPC(
											RpcTarget.Player("CL_ReceiveFilePng", ap.Player), chartCache.Crc,
											(uint)chartCache.Data.Length, chartCache.Data, 0,
											(byte)FileStorage.Type.png);
									}

									pool.Add(cui.UpdatePanel($"{oldIdentifier}_loading", "0 0 0 0", xMax: 0, blur: false));
									pool.Add(cui.UpdateText($"{oldIdentifier}_loadingtxt", "0 0 0 0", string.Empty, 0));
									pool.Add(cui.UpdateImage($"{oldIdentifier}_chart", chartCache.Crc, Cache.CUI.WhiteColor));
									pool.Send(ap.Player);
									break;
								}

								default:
								case Tab.OptionChart.ChartCache.StatusTypes.Failure:
								{
									pool.Add(cui.UpdateText($"{oldIdentifier}_loadingtxt", "0.9 0.1 0.1 0.75", "Failed to load chart!", 10));
									pool.Send(ap.Player);
									break;
								}
							}
						});

						return false;
					}
					case "layershadow":
					{
						var oldIdentifier = args.ElementAt(2);
						var newIdentifier = string.Empty;

						using var cui = new CUI(Handler);
						using var pool = cui.UpdatePool();

						var secondEnabled = false;
						var pColor = System.Drawing.Color.BlanchedAlmond;

						if (layerIndex == -1)
						{
							var allOff = chart.Chart.Layers.All(x => x.LayerSettings.Shadows == 0);

							foreach (var chartLayer in chart.Chart.Layers)
							{
								if (allOff)
								{
									chartLayer.LayerSettings.Shadows = 1;
								}
								else
								{
									chartLayer.LayerSettings.Shadows = 0;
								}
							}

							newIdentifier = chart.GetIdentifier(reset: true);
							return true;
						}
						else
						{
							var layer = chart.Chart.Layers.ElementAt(layerIndex);

							layer.LayerSettings.Shadows++;

							if (layer.LayerSettings.Shadows > 4)
							{
								layer.LayerSettings.Shadows = 0;
							}

							newIdentifier = chart.GetIdentifier(reset: true);
							pColor = layer.LayerSettings.Color;
							secondEnabled = layer.LayerSettings.Shadows > 0;
						}

						var sColor = System.Drawing.Color.FromArgb((int)(pColor.R * 1.5f).Clamp(0, 255), (int)(pColor.G * 1.5f).Clamp(0, 255), (int)(pColor.B * 1.5f).Clamp(0, 255));
						var rustSColor = $"{sColor.R / 255f} {sColor.G / 255f} {sColor.B / 255f} 1";

						var mainCommand = args.Skip(3).ToString(" ");

						pool.Add(cui.UpdatePanel($"{oldIdentifier}_loading", "0 0 0 0.2", xMin: 0.01f, xMax: 0.99f, yMin: 0.01f, yMax: 0.99f, blur: true));
						pool.Add(cui.UpdateText($"{oldIdentifier}_loadingtxt", "1 1 1 0.5", "Please wait...", 10, xMin: 0.01f, xMax: 0.99f, yMin: 0.01f, yMax: 0.99f));
						pool.Add(cui.UpdateImage($"{oldIdentifier}_chart", 0, Cache.CUI.WhiteColor, xMin: 0.01f));
						pool.Add(cui.UpdateProtectedButton($"{oldIdentifier}_layerbtn2_{layerIndex}", $"{pColor.R / 255f} {pColor.G / 255f} {pColor.B / 255f} {(!secondEnabled ? 0.15 : 0.5)}", rustSColor, "\u29bf", 8,
							command: $"{mainCommand} {layerIndex} {oldIdentifier} {mainCommand}"));
						pool.Send(ap.Player);

						Tab.OptionChart.Cache.GetOrProcessCache(newIdentifier, chart.Chart, chartCache =>
						{
							using var cui = new CUI(Handler);
							using var pool = cui.UpdatePool();

							switch (chartCache.Status)
							{
								case Tab.OptionChart.ChartCache.StatusTypes.Finalized:
								{
									if (!chartCache.HasPlayerReceivedData(ap.Player.userID))
									{
										CommunityEntity.ServerInstance.ClientRPC(
											RpcTarget.Player("CL_ReceiveFilePng", ap.Player), chartCache.Crc,
											(uint)chartCache.Data.Length, chartCache.Data, 0,
											(byte)FileStorage.Type.png);
									}

									pool.Add(cui.UpdatePanel($"{oldIdentifier}_loading", "0 0 0 0", xMax: 0, blur: false));
									pool.Add(cui.UpdateText($"{oldIdentifier}_loadingtxt", "0 0 0 0", string.Empty, 0));
									pool.Add(cui.UpdateImage($"{oldIdentifier}_chart", chartCache.Crc, Cache.CUI.WhiteColor));
									pool.Send(ap.Player);
									break;
								}

								default:
								case Tab.OptionChart.ChartCache.StatusTypes.Failure:
								{
									pool.Add(cui.UpdateText($"{oldIdentifier}_loadingtxt", "0.9 0.1 0.1 0.75", "Failed to load chart!", 10));
									pool.Send(ap.Player);
									break;
								}
							}
						});
						return false;
					}
				}
				break;
			}
		}

		return false;
	}

	#endregion

#endif

#if !MINIMAL

	#region Core Tabs

	#region Administration - Custom Commands

	[Conditional("!MINIMAL")]
	[ProtectedCommand("carbongg.endspectate")]
	private void EndSpectate(Arg arg)
	{
		StopSpectating(arg.Player());
	}

	[Conditional("!MINIMAL")]
	[ProtectedCommand("carbongg.skipspectate")]
	private void SkipSpectate(Arg arg)
	{
		var player = arg.Player();

		if (!player.IsSpectating())
		{
			return;
		}

		var parent = player.GetParentEntity();

		if (parent is not BasePlayer spectatedPlayer)
		{
			return;
		}

		var skip = arg.GetInt(0);
		var players = BasePlayer.allPlayerList.Where(x => x != player);
		var index = players.IndexOf(spectatedPlayer) + skip;

		var lastIndex = players.Count() - 1;

		if (index > lastIndex)
		{
			index = 0;
		}
		else if (index < 0)
		{
			index = lastIndex;
		}

		StopSpectating(player, false);
		StartSpectating(player, players.FindAt(index));
	}

	#endregion

	[Conditional("!MINIMAL")]
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

	[Conditional("!MINIMAL")]
	private void OnPluginUnloaded(RustPlugin plugin)
	{
		Community.Runtime.Core.NextTick(() =>
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

	public static void BlindPlayer(BasePlayer player)
	{
		using CUI cui = new CUI(Singleton.Handler);

		var container = cui.v2.CreateParent(ClientPanels.Overlay, LuiPosition.Full, "blindingpanel").AddCursor()
			.AddKeyboard().SetDestroy("blindingpanel");
		cui.v2.CreateImageFromDb(container, LuiPosition.Full, LuiOffset.None, "bsod","0 0 0 1");

		PlayersTab.BlindedPlayers.Add(player);
		cui.v2.SendUi(player);
		// OnCarbonBlinded
		HookCaller.CallStaticHook(3658185574, player);
	}

	public static void UnblindPlayer(BasePlayer player)
	{
		if (!PlayersTab.BlindedPlayers.Remove(player)) return;
		using CUI cui = new CUI(Singleton.Handler);
		cui.Destroy("blindingpanel", player);
		// OnCarbonUnblinded
		HookCaller.CallStaticHook(3911772319, player);
	}

	public static void EmpowerPlayerStats(BasePlayer player)
	{
		Debugging.RefillPlayerVitals(player, true);

		// OnCarbonEmpowerPlayerStats
		HookCaller.CallStaticHook(837777771, player);
	}

	public static void LockPlayerContainer(BasePlayer player, ItemContainer container, bool wants)
	{
		container.SetLocked(wants);

		// OnCarbonLockPlayerContainer
		HookCaller.CallStaticHook(298795244, player, container, wants);
	}

	public static void PrivateMessagePlayer(BasePlayer player, BasePlayer target, string message)
	{
		if (string.IsNullOrEmpty(message)) return;

		target.ChatMessage($"[{player.displayName}]: {message}");

		if (Singleton.ConfigInstance.PlayPMSound)
		{
			Effect.server.Run(Singleton.ConfigInstance.PMSound, target,2, Vector3.zero, new Vector3(0,2,0));
		}

		// OnCarbonPrivateMessage
		HookCaller.CallStaticHook(468227819, player, target, message);
	}


	public static void StartSpectating(BasePlayer player, BaseEntity target)
	{
		if (!string.IsNullOrEmpty(player.spectateFilter))
		{
			StopSpectating(player);
		}

		if (target == null)
		{
			return;
		}

		_spectateStartPosition[player.userID] = player.transform.position;

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

		// OnCarbonSpectateStart
		HookCaller.CallStaticHook(597991647, player, targetPlayer);

		using var cui = new CUI(Singleton.Handler);
		var container = cui.CreateContainer(SpectatePanelId, color: Cache.CUI.BlankColor, needsCursor: targetPlayer != null && targetPlayer.IsSleeping(), parent: ClientPanels.Overlay, destroyUi: SpectatePanelId);
		var panel = cui.CreatePanel(container, SpectatePanelId, Cache.CUI.BlankColor);

		if (Singleton.ConfigInstance.SpectatingInfoOverlay)
		{
			var item = target.GetItem();
			cui.CreateText(container, panel,
				color: "1 1 1 0.2",
				text: $"YOU'RE SPECTATING ".SpacedString(1, false) +
				      $"<b>{(targetPlayer == null ? item != null ? item.info.displayName.english.ToUpper().SpacedString(1) : target.ShortPrefabName.ToUpper().SpacedString(1) : targetPlayer.displayName.ToUpper().SpacedString(1))}</b>",
				15);
		}

		if (targetPlayer != null)
		{
			cui.CreateProtectedButton(container, panel,
				color: "0.3 0.3 0.3 0.9", textColor: "0.7 0.7 0.7 1",
				text: "<", 10,
				xMin: 0.425f, xMax: 0.445f, yMin: 0.15f, yMax: 0.19f, command: "carbongg.skipspectate -1");

			cui.CreateProtectedButton(container, panel,
				color: "0.3 0.3 0.3 0.9", textColor: "0.7 0.7 0.7 1",
				text: ">", 10,
				xMin: 0.555f, xMax: 0.575f, yMin: 0.15f, yMax: 0.19f, command: "carbongg.skipspectate 1");
		}

		cui.CreateProtectedButton(container, panel,
			color: "0.3 0.3 0.3 0.9", textColor: "0.7 0.7 0.7 1",
			text: "END SPECTATE".SpacedString(1), 10,
			xMin: 0.45f, xMax: 0.55f, yMin: 0.15f, yMax: 0.19f, command: "carbongg.endspectate");

		cui.Send(container, player);

		Community.Runtime.Core.NextTick(() => Singleton.Close(player));
	}
	public static void StopSpectating(BasePlayer player, bool clearUi = true)
	{
		if (clearUi)
		{
			using var cui = new CUI(Singleton.Handler);
			cui.Destroy(SpectatePanelId, player);
		}

		if (!player.IsSpectating() || string.IsNullOrEmpty(player.spectateFilter))
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

		if (Singleton.ConfigInstance.SpectatingEndTeleportBack && _spectateStartPosition.TryGetValue(player.userID, out var position))
		{
			player.Teleport(position);
		}
		else
		{
			player.Teleport(player.transform.position + (Vector3.up * -3f));
		}

		// OnCarbonSpectateEnd
		HookCaller.CallStaticHook(2609635685, player, spectated);

		if (clearUi)
		{
			var tab = Singleton.GetTab(player);
			var ap = Singleton.GetPlayerSession(player);
			EntitiesTab.SelectEntity(tab, ap, spectated);
			EntitiesTab.DrawEntitySettings(tab, 1, ap);
			Singleton.Draw(player);
		}
	}

	public static void OpenPlayerContainer(PlayerSession ap, BasePlayer player, Tab tab)
	{
		Singleton.Subscribe("OnEntityVisibilityCheck");
		Singleton.Subscribe("OnEntityDistanceCheck");
		Singleton.Subscribe("CanAcceptItem");

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

			ap.Player.ClientRPC(RpcTarget.Player("RPC_OpenLootPanel", ap.Player), "player_corpse");
		});
	}
	public static void OpenContainer(PlayerSession ap, ItemContainer container, Tab tab)
	{
		EntitiesTab.LastContainerLooter = null;
		ap.ClearStorage(tab, "lootedent");

		ap.Player.inventory.loot.Clear();

		Core.timer.In(0.5f, () =>
		{
			EntitiesTab.LastContainerLooter = ap;
			ap.SetStorage(tab, "lootedent", ap.Player);

			ap.Player.inventory.loot.PositionChecks = false;
			ap.Player.inventory.loot.entitySource = RelationshipManager.ServerInstance;
			ap.Player.inventory.loot.itemSource = null;
			ap.Player.inventory.loot.AddContainer(container);
			ap.Player.inventory.loot.MarkDirty();
			ap.Player.inventory.loot.SendImmediate();

			ap.Player.ClientRPC(RpcTarget.Player("RPC_OpenLootPanel", ap.Player), "generic");
		});
	}

#endif
}

public class AdminConfig
{
	[JsonProperty("OpenCommands")]
	public string[] OpenCommands = ["cp", "cpanel"];
	public int MinimumAuthLevel = 2;
	public bool SpectatingInfoOverlay = true;
	public bool SpectatingEndTeleportBack = false;
	public List<ActionButton> QuickActions = new();
	public bool HideConsole = false;
	public bool PlayPMSound = true;
	public string PMSound = "assets/prefabs/locks/keypad/effects/lock.code.unlock.prefab";

	public class ActionButton
	{
		public string Name;
		public string Command;
		public bool User;
		public bool IncludeUserId;
		public bool ConfirmDialog;
	}
}
public class AdminData
{
	public bool GreetDisplayed = false;
	public bool HidePluginIcons = false;
	public bool Maximize = false;
	public bool BackgroundBlur = true;
	public float BackgroundOpacity = 0.75f;
	public float BackgroundImageOpacity = 0.75f;
	public string BackgroundImage = "http://cdn.carbonmod.gg/content/carbon-background.png";
	public Vector2 BackgroundImageYAnchor = new(0.15f, 1);
	public float BackgroundColumnOpacity = 0.5f;
	public DataColors Colors = new();
	public Dictionary<string, bool> TabsHiddenStatus = new();

	public bool IsTabHidden(string id)
	{
		return TabsHiddenStatus.TryGetValue(id, out var hidden) && hidden;
	}

	public void MarkTabHidden(string id, bool wants)
	{
		TabsHiddenStatus[id] = wants;
	}

	public class DataColors
	{
		public string SelectedTabColor = "0.4 0.7 0.2";
		public string EditableInputHighlight = "0.259 0.529 0.961";
		public string NameTextColor = "1 1 1 0.7";
		public string ButtonSelectedColor = "0.4 0.7 0.2 0.75";
		public string ButtonWarnedColor = "0.8 0.7 0.2 0.75";
		public string ButtonImportantColor = "0.97 0.2 0.1 0.75";
		public string OptionColor = "0.2 0.2 0.2 0.75";
		public string OptionColor2 = "0.2 0.2 0.2 0.1";
		public string OptionNameColor = $"1 1 1 0.7";
		public float TitleUnderlineOpacity = 0.9f;
		public float OptionWidth = 0.475f;

	}
}
