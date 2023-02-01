using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Carbon.Base;
using Carbon.Extensions;
using Facepunch;
using Oxide.Core.Libraries;
using Oxide.Game.Rust.Cui;
using Oxide.Plugins;
using Steamworks.Data;
using UnityEngine;
using UnityEngine.UI;
using static ConsoleSystem;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Modules;

public class AdminModule : CarbonModule<AdminConfig, AdminData>
{
	public override string Name => "Admin";
	public override Type Type => typeof(AdminModule);
	public override bool EnabledByDefault => true;

	internal List<Tab> Tabs = new();
	internal Dictionary<BasePlayer, AdminPlayer> AdminPlayers = new();
	internal CUI.Handler Handler { get; } = new();

	const string PanelId = "carbonmodularui";

	private void OnServerInitialized()
	{
		Community.Runtime.CorePlugin.cmd.AddChatCommand(Config.OpenCommand, this, (player, cmd, args) =>
		{
			if (!CanAccess(player)) return;

			var ap = GetOrCreateAdminPlayer(player);
			ap.TabIndex = 0;

			var tab = GetTab(player);
			tab?.OnChange?.Invoke(ap, tab);

			ap.Clear();

			Draw(player);
		});

		RegisterTab(PermissionsTab.Get(Community.Runtime.CorePlugin.permission));
		RegisterTab(PlayersTab.Get());
	}

	private bool CanAccess(BasePlayer player)
	{
		var level = Config.MinimumAuthLevel;

		switch (level)
		{
			case 0:
				return true;

			case 1:
				return ServerUsers.Is(player.userID, ServerUsers.UserGroup.Moderator);

			case 2:
				return ServerUsers.Is(player.userID, ServerUsers.UserGroup.Owner);
		}

		return false;
	}

	#region Option Elements

	internal void TabButton(CUI cui, CuiElementContainer container, string parent, string text, string command, float width, float offset, bool highlight = false)
	{
		cui.CreateProtectedButton(container, parent: parent, id: $"{parent}btn",
			color: highlight ? "0.4 0.7 0.2 0.7" : "0.3 0.3 0.3 0.1",
			textColor: "1 1 1 0.5",
			text: text, 11,
			xMin: offset, xMax: offset + width, yMin: 0, yMax: 1,
			command: command,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		if (highlight)
		{
			cui.CreatePanel(container, $"{parent}btn", null,
				color: "1 1 1 0.4",
				xMin: 0, xMax: 1f, yMin: 0f, yMax: 0.03f);
		}
	}

	public void TabColumnPagination(CUI cui, CuiElementContainer container, string parent, int column, AdminPlayer.Page page, float height, float offset)
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
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedInputField(container, parent: id, id: null,
			color: "1 1 1 1",
			text: $"{page.CurrentPage + 1}", 9,
			xMin: 0f, xMax: 0.495f, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleRight,
			command: PanelId + $".changecolumnpage {column} 4 ",
			characterLimit: 0,
			readOnly: false,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		#region Left

		cui.CreateProtectedButton(container, parent: id, id: null,
			color: page.CurrentPage > 0 ? "0.8 0.7 0.2 0.7" : "0.3 0.3 0.3 0.1",
			textColor: "1 1 1 0.5",
			text: "<<", 8,
			xMin: 0, xMax: 0.1f, yMin: 0f, yMax: 1f,
			command: page.CurrentPage > 0 ? PanelId + $".changecolumnpage {column} 2" : "",
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedButton(container, parent: id, id: null,
			color: "0.4 0.7 0.2 0.7",
			textColor: "1 1 1 0.5",
			text: "<", 8,
			xMin: 0.1f, xMax: 0.2f, yMin: 0f, yMax: 1f,
			command: PanelId + $".changecolumnpage {column} 0",
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		#endregion

		#region Right

		cui.CreateProtectedButton(container, parent: id, id: null,
			color: page.CurrentPage < page.TotalPages ? "0.8 0.7 0.2 0.7" : "0.3 0.3 0.3 0.1",
			textColor: "1 1 1 0.5",
			text: ">>", 8,
			xMin: 0.9f, xMax: 1f, yMin: 0f, yMax: 1f,
			command: page.CurrentPage < page.TotalPages ? PanelId + $".changecolumnpage {column} 3" : "",
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedButton(container, parent: id, id: null,
			color: "0.4 0.7 0.2 0.7",
			textColor: "1 1 1 0.5",
			text: ">", 8,
			xMin: 0.8f, xMax: 0.9f, yMin: 0f, yMax: 1f,
			command: PanelId + $".changecolumnpage {column} 1",
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		#endregion
	}
	public void TabPanelName(CUI cui, CuiElementContainer container, string parent, string text, float height, float offset, TextAnchor align)
	{
		cui.CreateText(container, parent: parent, id: $"{parent}text",
			color: "1 1 1 0.7",
			text: text.ToUpper(), 12,
			xMin: 0.025f, xMax: 0.98f, yMin: offset, yMax: offset + height,
			align: align,
			font: CUI.Handler.FontTypes.RobotoCondensedBold);

		cui.CreatePanel(container, $"{parent}text", null,
			color: "1 1 1 0.7",
			xMin: 0, xMax: 1, yMin: 0f, yMax: 0.025f);
	}
	public void TabPanelText(CUI cui, CuiElementContainer container, string parent, string text, int size, string color, float height, float offset, TextAnchor align, CUI.Handler.FontTypes font)
	{
		cui.CreateText(container, parent: parent, id: $"{parent}text",
			color: color,
			text: text, size,
			xMin: 0.025f, xMax: 0.98f, yMin: offset, yMax: offset + height,
			align: align,
			font: font);
	}
	public void TabPanelButton(CUI cui, CuiElementContainer container, string parent, string text, string command, float height, float offset, Tab.OptionButton.Types type = Tab.OptionButton.Types.None, TextAnchor align = TextAnchor.MiddleCenter)
	{
		var color = "0 0 0 0";

		switch (type)
		{
			case Tab.OptionButton.Types.Selected:
				color = "0.4 0.7 0.2 0.7";
				break;

			case Tab.OptionButton.Types.Warned:
				color = "0.8 0.7 0.2 0.7";
				break;

			case Tab.OptionButton.Types.Important:
				color = "0.97 0.2 0.1 0.7";
				break;

			default:
				color = "0.2 0.2 0.2 0.5";
				break;
		}

		cui.CreateProtectedButton(container, parent: parent, id: $"{parent}btn",
			color: color,
			textColor: "1 1 1 0.5",
			text: text, 11,
			xMin: 0.015f, xMax: 0.985f, yMin: offset, yMax: offset + height,
			command: command,
			align: align,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);
	}
	public void TabPanelToggle(CUI cui, CuiElementContainer container, string parent, string text, string command, float height, float offset, bool isOn)
	{
		var toggleButtonScale = 0.93f;

		cui.CreatePanel(container, parent, $"{parent}panel",
			color: "0.2 0.2 0.2 0",
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		cui.CreateText(container, parent: $"{parent}panel", id: $"{parent}text",
			color: "1 1 1 0.7",
			text: $"{text}:", 12,
			xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleLeft,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		cui.CreatePanel(container, $"{parent}panel", null,
			color: "0.2 0.2 0.2 0.5",
			xMin: 0, xMax: toggleButtonScale, yMin: 0, yMax: 0.015f);

		cui.CreateProtectedButton(container, parent: parent, id: $"{parent}btn",
			color: "0.2 0.2 0.2 0.5",
			textColor: "1 1 1 0.5",
			text: string.Empty, 11,
			xMin: toggleButtonScale, xMax: 0.985f, yMin: offset, yMax: offset + height,
			command: command,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		if (isOn)
		{
			cui.CreatePanel(container, $"{parent}btn", null,
				color: "0.4 0.7 0.2 0.7",
				xMin: 0.2f, xMax: 0.8f, yMin: 0.2f, yMax: 0.8f);
		}
	}
	public void TabPanelInput(CUI cui, CuiElementContainer container, string parent, string text, string placeholder, string command, int characterLimit, bool readOnly, float height, float offset, Tab.OptionButton.Types type = Tab.OptionButton.Types.None)
	{
		var color = "0 0 0 0";

		switch (type)
		{
			case Tab.OptionButton.Types.Selected:
				color = "0.4 0.7 0.2 0.7";
				break;

			case Tab.OptionButton.Types.Warned:
				color = "0.8 0.7 0.2 0.7";
				break;

			case Tab.OptionButton.Types.Important:
				color = "0.97 0.2 0.1 0.7";
				break;

			default:
				color = "0.2 0.2 0.2 0.5";
				break;
		}

		cui.CreatePanel(container, parent, $"{parent}panel",
			color: "0.2 0.2 0.2 0",
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		cui.CreateText(container, parent: $"{parent}panel", id: $"{parent}text",
			color: "1 1 1 0.7",
			text: $"{text}:", 12,
			xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleLeft,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		cui.CreatePanel(container, $"{parent}panel", $"{parent}inppanel",
			color: color,
			xMin: Config.OptionWidth, xMax: 0.985f, yMin: 0, yMax: 1);

		cui.CreatePanel(container, $"{parent}panel", null,
			color: color,
			xMin: 0, xMax: Config.OptionWidth, yMin: 0, yMax: 0.015f);

		cui.CreateProtectedInputField(container, parent: $"{parent}inppanel", id: null,
			color: "1 1 1 1",
			text: placeholder, 11,
			xMin: 0.03f, xMax: 1, yMin: 0, yMax: 1,
			command: command,
			align: TextAnchor.MiddleLeft,
			characterLimit: characterLimit,
			readOnly: readOnly,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);
	}
	public void TabPanelEnum(CUI cui, CuiElementContainer container, string parent, string text, string value, string command, float height, float offset, Tab.OptionButton.Types type = Tab.OptionButton.Types.Selected)
	{
		var color = "0 0 0 0";

		switch (type)
		{
			case Tab.OptionButton.Types.Selected:
				color = "0.4 0.7 0.2 0.7";
				break;

			case Tab.OptionButton.Types.Warned:
				color = "0.8 0.7 0.2 0.7";
				break;

			case Tab.OptionButton.Types.Important:
				color = "0.97 0.2 0.1 0.7";
				break;

			default:
				color = "0.2 0.2 0.2 0.5";
				break;
		}

		cui.CreatePanel(container, parent, $"{parent}panel",
			color: "0.2 0.2 0.2 0",
			xMin: 0, xMax: 1f, yMin: offset, yMax: offset + height);

		cui.CreateText(container, parent: $"{parent}panel", id: $"{parent}text",
			color: "1 1 1 0.7",
			text: text, 12,
			xMin: 0.025f, xMax: 0.98f, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleLeft,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		cui.CreatePanel(container, $"{parent}panel", $"{parent}inppanel",
			color: "0.2 0.2 0.2 0.5",
			xMin: Config.OptionWidth, xMax: 0.985f, yMin: 0, yMax: 1);

		cui.CreateText(container, parent: $"{parent}inppanel", id: null,
			color: "1 1 1 0.7",
			text: value, 11,
			xMin: 0, xMax: 1, yMin: 0, yMax: 1,
			align: TextAnchor.MiddleCenter,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedButton(container, parent: $"{parent}inppanel", id: null,
			color: color,
			textColor: "1 1 1 0.7",
			text: "<", 10,
			xMin: 0f, xMax: 0.15f, yMin: 0, yMax: 1,
			command: $"{command} true",
			align: TextAnchor.MiddleCenter,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);

		cui.CreateProtectedButton(container, parent: $"{parent}inppanel", id: null,
			color: color,
			textColor: "1 1 1 0.7",
			text: ">", 10,
			xMin: 0.85f, xMax: 1f, yMin: 0, yMax: 1,
			command: $"{command} false",
			align: TextAnchor.MiddleCenter,
			font: CUI.Handler.FontTypes.RobotoCondensedRegular);
	}

	#endregion

	#region UI Commands

	[UiCommand(PanelId + ".changetab")]
	private void ChangeTab(Arg args)
	{
		var player = args.Player();
		var ap = GetOrCreateAdminPlayer(player);
		var previous = ap.TabIndex;

		var tab = GetTab(player);
		tab?.OnChange?.Invoke(ap, tab);

		ap.Clear();

		if (int.TryParse(args.Args[0], out int index))
		{
			ap.TabIndex = index;
		}
		else
		{
			ap.TabIndex += args.Args[0] == "up" ? 1 : -1;

			if (ap.TabIndex > Tabs.Count - 1) ap.TabIndex = 0;
			else if (ap.TabIndex < 0) ap.TabIndex = Tabs.Count - 1;
		}

		if (ap.TabIndex != previous) Draw(player);
	}

	[UiCommand(PanelId + ".callaction")]
	private void CallAction(Arg args)
	{
		var player = args.Player();

		if (CallColumnRow(player, args.Args[0].ToInt(), args.Args[1].ToInt(), args.Args.Skip(2).ToArray()))
			Draw(player);
	}

	[UiCommand(PanelId + ".changecolumnpage")]
	private void ChangeColumnPage(Arg args)
	{
		var player = args.Player();
		var instance = GetOrCreateAdminPlayer(player);
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

	[UiCommand(PanelId + ".closetest")]
	private void CloseUI(Arg args)
	{
		Handler.Destroy(PanelId, args.Player());
	}

	#endregion

	#region Methods

	public void Draw(BasePlayer player)
	{
		try
		{
			var ap = GetOrCreateAdminPlayer(player);
			var tab = GetTab(player);

			using var cui = new CUI(Handler);

			cui.Destroy(PanelId, player);

			var container = cui.CreateContainer(PanelId,
				color: "0 0 0 0.75",
				xMin: 0, xMax: 1, yMin: 0, yMax: 1,
				needsCursor: true,
				needsKeyboard: true);

			cui.CreatePanel(container, parent: PanelId, id: "color",
				color: "0 0 0 0.6",
				xMin: 0.15f, xMax: 0.85f, yMin: 0.1f, yMax: 0.9f);
			cui.CreatePanel(container, "color", "main",
				color: "0 0 0 0.5",
				blur: true);

			#region Title

			cui.CreateText(container, parent: "main", id: null,
				color: "1 1 1 0.8",
				text: "<b>Admin Settings</b>", 18,
				xMin: 0.0175f, yMin: 0.8f, xMax: 1f, yMax: 0.98f,
				align: TextAnchor.UpperLeft,
				font: CUI.Handler.FontTypes.RobotoCondensedBold);
			cui.CreateText(container, parent: "main", id: null,
				color: "1 1 1 0.5",
				text: $"Carbon {Community.InformationalVersion}", 11,
				xMin: 0.0175f, yMin: 0.8f, xMax: 1f, yMax: 0.95f,
				align: TextAnchor.UpperLeft,
				font: CUI.Handler.FontTypes.RobotoCondensedRegular);

			#endregion

			#region Exit

			cui.CreateProtectedButton(container, parent: "main", id: null,
				color: "0.6 0.2 0.2 0.9",
				textColor: "1 0.5 0.5 1",
				text: "X", 13,
				xMin: 0.96f, xMax: 0.99f, yMin: 0.95f, yMax: 0.99f,
				command: PanelId + ".closetest",
				font: CUI.Handler.FontTypes.DroidSansMono);

			#endregion

			#region Tabs

			cui.CreatePanel(container, parent: "main", id: "tab_buttons",
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
				TabButton(cui, container, "tab_buttons", $"{(ap.TabIndex == i ? $"<b>{_tab.Name}</b>" : _tab.Name)}<size=8>\n{_tab.Plugin?.Name} ({_tab.Plugin?.Version}) by {_tab.Plugin?.Author}</size>", PanelId + $".changetab {i}", tabWidth, tabIndex, ap.TabIndex == i);
				tabIndex += tabWidth;
			}

			#endregion

			#region Panels

			cui.CreatePanel(container, "main", "panels",
				color: "0 0 0 0",
				xMin: 0.01f, xMax: 0.99f, yMin: 0.02f, yMax: 0.86f);

			if (tab != null)
			{
				#region Override

				tab.Override?.Invoke(tab, container, "panels");

				#endregion

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
					var rowIndex = 1f - rowSpacing - rowHeight;
					var rowPage = rows.Skip(contentsPerPage * columnPage.CurrentPage).Take(contentsPerPage);
					columnPage.TotalPages = (int)Math.Ceiling((double)rows.Count / contentsPerPage - 1);
					columnPage.Check();

					for (int r = 0; r < rowPage.Count(); r++)
					{
						var actualI = r + (columnPage.CurrentPage * contentsPerPage);
						var row = rows.ElementAt(actualI);

						switch (row)
						{
							case Tab.OptionName name:
								TabPanelName(cui, container, panel, name.Name, rowHeight, rowIndex, name.Align);
								break;

							case Tab.OptionButton button:
								TabPanelButton(cui, container, panel, button.Name, PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex, button.Type == null ? Tab.OptionButton.Types.None : button.Type.Invoke(ap));
								break;

							case Tab.OptionText text:
								TabPanelText(cui, container, panel, text.Name, text.Size, text.Color, rowHeight, rowIndex, text.Align, text.Font);
								break;

							case Tab.OptionInput input:
								TabPanelInput(cui, container, panel, input.Name, input.Placeholder, PanelId + $".callaction {i} {actualI}", input.CharacterLimit, input.ReadOnly, rowHeight, rowIndex);
								break;

							case Tab.OptionEnum @enum:
								TabPanelEnum(cui, container, panel, @enum.Name, @enum.Text?.Invoke(), PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex);
								break;

							case Tab.OptionToggle toggle:
								TabPanelToggle(cui, container, panel, toggle.Name, PanelId + $".callaction {i} {actualI}", rowHeight, rowIndex, toggle.IsOn != null ? toggle.IsOn.Invoke(ap) : false);
								break;

							default:
								break;
						}

						rowIndex -= rowHeight + rowSpacing;
					}

					if (columnPage.TotalPages > 0)
					{
						TabColumnPagination(cui, container, panel, i, columnPage, rowHeight, rowIndex);
						TabColumnPagination(cui, container, panel, i, columnPage, rowHeight, rowIndex);
					}

					#endregion

					panelIndex += panelWidth + spacing;
				}

				#endregion
			}

			#endregion

			cui.Send(container, player);
		}
		catch (Exception ex)
		{
			PutsError($"Draw(player) failed.", ex);
		}
	}

	public void RegisterTab(Tab tab, int? insert = null)
	{
		var existentTab = Tabs.FirstOrDefault(x => x.Id == tab.Id);
		if (existentTab != null)
		{
			var index = Tabs.IndexOf(existentTab);
			Tabs.RemoveAt(index);
			Pool.Free(ref existentTab);

			Tabs.Insert(insert ?? index, tab);
		}
		else
		{
			if (insert != null) Tabs.Insert(insert.Value, tab);
			else Tabs.Add(tab);
		}

		AdminPlayers.Clear();
	}
	public void UnregisterTab(string id)
	{
		AdminPlayers.Clear();

		var tab = Tabs.FirstOrDefault(x => x.Id == id);

		if (tab != null)
		{
			tab.Dispose();
		}

		Tabs.RemoveAll(x => x.Id == id);
	}

	public AdminPlayer GetOrCreateAdminPlayer(BasePlayer player)
	{
		if (AdminPlayers.TryGetValue(player, out AdminPlayer adminPlayer)) return adminPlayer;

		adminPlayer = new AdminPlayer(player);
		AdminPlayers.Add(player, adminPlayer);
		return adminPlayer;
	}
	public void SetTab(BasePlayer player, string id)
	{
		var ap = GetOrCreateAdminPlayer(player);
		var previous = ap.TabIndex;

		var tab = Tabs.FirstOrDefault(x => x.Id == id);
		if (tab != null)
		{
			ap.TabIndex = Tabs.IndexOf(tab);
			tab?.OnChange?.Invoke(ap, tab);
		}

		if (ap.TabIndex != previous) Draw(player);
	}
	public Tab GetTab(BasePlayer player)
	{
		if (Tabs.Count == 0) return null;

		var adminPlayer = GetOrCreateAdminPlayer(player);
		if (adminPlayer.TabIndex > Tabs.Count - 1 || adminPlayer.TabIndex < 0) return null;

		return Tabs[adminPlayer.TabIndex];
	}
	public Tab FindTab(string id)
	{
		return Tabs.FirstOrDefault(x => x.Id == id);
	}
	public bool CallColumnRow(BasePlayer player, int column, int row, string[] args)
	{
		var ap = GetOrCreateAdminPlayer(player);
		var tab = GetTab(player);

		ap.LastPressedColumn = column;
		ap.LastPressedRow = row;

		switch (tab.Columns[column][row])
		{
			case Tab.OptionButton button:
				button.Callback?.Invoke(ap);
				return button.Callback != null;

			case Tab.OptionInput input:
				input.Callback?.Invoke(ap, args);
				return input.Callback != null;

			case Tab.OptionEnum @enum:
				@enum.Callback?.Invoke(args[0].ToBool());
				return @enum.Callback != null;

			case Tab.OptionToggle toggle:
				toggle.Callback?.Invoke(ap);
				return toggle.Callback != null;
		}

		return false;
	}

	#endregion

	public class AdminPlayer : IDisposable
	{
		public static AdminPlayer Blank { get; } = new AdminPlayer(null);

		public BasePlayer Player;
		public Dictionary<int, Page> ColumnPages = new Dictionary<int, Page>();
		public Dictionary<string, object> LocalStorage = new Dictionary<string, object>();

		public int TabIndex;
		public int TabSkip;
		public int LastPressedColumn;
		public int LastPressedRow;

		public AdminPlayer(BasePlayer player)
		{
			Player = player;
		}

		public T GetStorage<T>(string id)
		{
			if (LocalStorage.TryGetValue(id, out var storage)) return (T)storage;

			return default;
		}
		public object GetStorage(string id)
		{
			if (LocalStorage.TryGetValue(id, out var storage)) return storage;

			return default;
		}
		public void SetStorage<T>(string id, T value)
		{
			LocalStorage[id] = value;
		}
		public void ClearStorage(string id)
		{
			LocalStorage[id] = null;
		}
		public void Clear()
		{
			foreach (var page in ColumnPages)
			{
				var value = ColumnPages[page.Key];
				Pool.Free(ref value);
			}

			ColumnPages.Clear();
			LocalStorage.Clear();
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
		public RustPlugin Plugin;
		public Action<Tab, CuiElementContainer, string> Override;
		public Dictionary<int, List<Option>> Columns = new Dictionary<int, List<Option>>();
		public Action<AdminPlayer, Tab> OnChange;

		public Tab(string id, string name, RustPlugin plugin, Action<AdminPlayer, Tab> onChange = null)
		{
			Id = id;
			Name = name;
			Plugin = plugin;
			OnChange = onChange;
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
		public void ClearAfter(int index)
		{
			for (int i = 0; i < Columns.Count; i++)
			{
				if (i >= index) ClearColumn(i);
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
		public Tab AddName(int column, string name, TextAnchor align)
		{
			AddRow(column, new OptionName(name, align));
			return this;
		}
		public Tab AddButton(int column, string name, TextAnchor align, Action<AdminPlayer> callback, Func<AdminPlayer, OptionButton.Types> type = null)
		{
			AddRow(column, new OptionButton(name, align, callback, type));
			return this;
		}
		public Tab AddToggle(int column, string name, Action<AdminPlayer> callback, Func<AdminPlayer, bool> isOn = null)
		{
			AddRow(column, new OptionToggle(name, callback, isOn));
			return this;
		}
		public Tab AddText(int column, string name, int size, string color, TextAnchor align, CUI.Handler.FontTypes font)
		{
			AddRow(column, new OptionText(name, size, color, align, font));
			return this;
		}
		public Tab AddInput(int column, string name, string placeholder, int characterLimit, bool readOnly, Action<AdminPlayer, string[]> callback)
		{
			AddRow(column, new OptionInput(name, placeholder, characterLimit, readOnly, callback));
			return this;
		}
		public Tab AddInput(int column, string name, string placeholder, Action<AdminPlayer, string[]> callback)
		{
			return AddInput(column, name, placeholder, 0, false, callback);
		}
		public Tab AddEnum(int column, string name, Action<bool> callback, Func<string> text)
		{
			AddRow(column, new OptionEnum(name, callback, text));
			return this;
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

		public class Option
		{
			public string Name;

			public Option(string name)
			{
				Name = name;
			}
		}
		public class OptionName : Option
		{
			public TextAnchor Align;

			public OptionName(string name, TextAnchor align) : base(name) { Align = align; }
		}
		public class OptionText : Option
		{
			public int Size;
			public string Color;
			public TextAnchor Align;
			public CUI.Handler.FontTypes Font;

			public OptionText(string name, int size, string color, TextAnchor align, CUI.Handler.FontTypes font) : base(name) { Align = align; Size = size; Color = color; Font = font; }
		}
		public class OptionInput : Option
		{
			public string Placeholder;
			public int CharacterLimit;
			public bool ReadOnly;
			public Action<AdminPlayer, string[]> Callback;

			public OptionInput(string name, string placeholder, int characterLimit, bool readOnly, Action<AdminPlayer, string[]> args) : base(name)
			{
				Placeholder = placeholder;
				Callback = args;
				CharacterLimit = characterLimit;
				ReadOnly = readOnly;
			}
		}
		public class OptionButton : Option
		{
			public Func<AdminPlayer, Types> Type;
			public Action<AdminPlayer> Callback;
			public TextAnchor Align = TextAnchor.MiddleCenter;

			public enum Types
			{
				None,
				Selected,
				Warned,
				Important
			}

			public OptionButton(string name, TextAnchor align, Action<AdminPlayer> callback, Func<AdminPlayer, Types> type = null) : base(name) { Align = align; Callback = callback; Type = type; }
			public OptionButton(string name, Action<AdminPlayer> callback, Func<AdminPlayer, Types> type = null) : base(name) { Callback = callback; Type = type; }
		}
		public class OptionToggle : Option
		{
			public Func<AdminPlayer, bool> IsOn;
			public Action<AdminPlayer> Callback;

			public OptionToggle(string name, Action<AdminPlayer> callback, Func<AdminPlayer, bool> isOn = null) : base(name) { Callback = callback; IsOn = isOn; }
		}
		public class OptionEnum : Option
		{
			public Func<string> Text;
			public Action<bool> Callback;

			public OptionEnum(string name, Action<bool> callback, Func<string> text) : base(name) { Callback = callback; Text = text; }
		}
		public class OptionRange : Option
		{
			public float Value;
			public Action<float> Callback;

			public OptionRange(string name, float value, Action<float> callback, Func<float> text) : base(name) { Callback = callback; Value = value; }
		}
	}

	#region Core Tabs

	public class PermissionsTab
	{
		public static Tab Get(Permission permission)
		{
			var perms = new Tab("permissions", "Permissions", Community.Runtime.CorePlugin, (instance, tab) =>
			{
				tab.ClearColumn(1);
				tab.ClearColumn(2);
				tab.ClearColumn(3);
				GeneratePlayers(tab, permission, instance);
			});
			{
				perms.AddName(0, "Options", TextAnchor.MiddleLeft);

				perms.AddRow(0, new Tab.OptionButton($"Players", instance =>
				{
					perms.ClearColumn(1);
					perms.ClearColumn(2);
					perms.ClearColumn(3);
					instance.Clear();

					instance.SetStorage("option", 0);

					GeneratePlayers(perms, permission, instance);
				}, type: (instance) => instance.GetStorage<int>("option") == 0 ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));

				GeneratePlayers(perms, permission, AdminPlayer.Blank);

				perms.AddRow(0, new Tab.OptionButton($"Groups", instance =>
				{
					perms.ClearColumn(1);
					perms.ClearColumn(2);
					perms.ClearColumn(3);
					instance.Clear();

					instance.ClearStorage("player");
					instance.ClearStorage("plugin");

					instance.SetStorage("option", 1);

					perms.AddName(1, "Groups", TextAnchor.MiddleLeft);
					{
						foreach (var group in permission.GetGroups())
						{
							perms.AddRow(1, new Tab.OptionButton($"{group}", instance2 =>
							{
								instance.SetStorage("group", group);

								instance.ClearStorage("plugin");

								perms.ClearColumn(2);
								perms.ClearColumn(3);

								perms.AddName(2, "Plugins", TextAnchor.MiddleLeft);
								{
									foreach (var plugin in Community.Runtime.Plugins.Plugins.Where(x => x.permission.GetPermissions().Any(y => y.StartsWith(x.Name.ToLower()))))
									{
										perms.AddRow(2, new Tab.OptionButton($"{plugin.Name} ({plugin.Version})", instance3 =>
										{
											instance.SetStorage("plugin", plugin);

											perms.ClearColumn(3);

											perms.AddName(3, "Permissions", TextAnchor.MiddleLeft);
											foreach (var perm in permission.GetPermissions().Where(x => x.StartsWith(plugin.Name.ToLower())))
											{
												perms.AddRow(3, new Tab.OptionButton($"{perm}", instance5 =>
												{
													if (permission.GroupHasPermission(group, perm))
														permission.RevokeGroupPermission(group, perm);
													else permission.GrantGroupPermission(group, perm, plugin);
												}, type: (_instance) => permission.GroupHasPermission(group, perm) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
											}
										}, type: (_instance) => instance.GetStorage("plugin") == plugin ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
									}
								}
							}, type: (_instance) => instance.GetStorage<string>("group") == group ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
						}
					}
					perms.AddRow(1, new Tab.OptionButton("Add Group", null, (_instance) => Tab.OptionButton.Types.Warned));
				}, type: (instance) => instance.GetStorage<int>("option") == 1 ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
				perms.AddColumn(1);
				perms.AddColumn(2);
				perms.AddColumn(3);
			}

			return perms;
		}

		public static void GeneratePlayers(Tab perms, Permission permission, AdminPlayer instance)
		{
			perms.ClearColumn(1);
			perms.AddName(1, "Players", TextAnchor.MiddleLeft);
			{
				foreach (var player in BasePlayer.allPlayerList)
				{
					perms.AddRow(1, new Tab.OptionButton($"{player.displayName} ({player.userID})", instance2 =>
					{
						instance.SetStorage("player", player);

						instance.ClearStorage("plugin");

						perms.ClearColumn(3);

						GeneratePlugins(perms, instance, permission, player);
					}, type: (_instance) => instance.GetStorage<BasePlayer>("player") == player ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
				}
			}
		}

		public static void GeneratePlugins(Tab perms, AdminPlayer instance, Permission permission, BasePlayer player)
		{
			perms.ClearColumn(2);
			perms.AddName(2, "Plugins", TextAnchor.MiddleLeft);
			{
				foreach (var plugin in Community.Runtime.Plugins.Plugins.Where(x => x.permission.GetPermissions().Any(y => y.StartsWith(x.Name.ToLower()))))
				{
					perms.AddRow(2, new Tab.OptionButton($"{plugin.Name} ({plugin.Version})", instance3 =>
					{
						instance.SetStorage("plugin", plugin);
						instance.SetStorage("pluginr", instance3.LastPressedRow);
						instance.SetStorage("pluginc", instance3.LastPressedColumn);

						GeneratePermissions(perms, permission, plugin, player);
					}, type: (_instance) => instance.GetStorage("plugin") == plugin ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));
				}
			}
		}

		public static void GeneratePermissions(Tab perms, Permission permission, RustPlugin plugin, BasePlayer player)
		{
			perms.ClearColumn(3);
			perms.AddName(3, "Permissions", TextAnchor.MiddleLeft);
			foreach (var perm in permission.GetPermissions().Where(x => x.StartsWith(plugin.Name.ToLower())))
			{
				var isInherited = false;
				var list = "";

				foreach (var group in permission.GetUserGroups(player.UserIDString))
					if (permission.GroupHasPermission(group, perm))
					{
						isInherited = true;
						list += $"<b>{group}</b>, ";
					}

				perms.AddRow(3, new Tab.OptionButton($"{perm}", instance5 =>
				{
					if (permission.UserHasPermission(player.UserIDString, perm))
						permission.RevokeUserPermission(player.UserIDString, perm);
					else permission.GrantUserPermission(player.UserIDString, perm, plugin);
				}, type: (_instance) => isInherited ? Tab.OptionButton.Types.Important : permission.UserHasPermission(player.UserIDString, perm) ? Tab.OptionButton.Types.Selected : Tab.OptionButton.Types.None));

				if (isInherited)
				{
					perms.AddText(3, $"Inherited by the following groups: {list.TrimEnd(',', ' ')}", 8, "1 1 1 0.6", TextAnchor.UpperLeft, CUI.Handler.FontTypes.RobotoCondensedRegular);
				}
			}

		}
	}
	public class PlayersTab
	{
		internal static AdminModule Admin => BaseModule.GetModule<AdminModule>();

		public static Tab Get()
		{
			var players = new Tab("players", "Players", Community.Runtime.CorePlugin, (instance, tab) =>
			{
				tab.ClearColumn(1);
				RefreshPlayers(tab, instance);
			});
			{
				AddInitial(players, null);
				RefreshPlayers(players, null);
			}
			players.AddColumn(1);

			return players;
		}

		public static void AddInitial(Tab tab, AdminPlayer ap)
		{
			tab.AddInput(0, "Search", ap?.GetStorage<string>("playerfilter"), (ap2, args) => { ap2.SetStorage("playerfilter", args.ToString(" ")); RefreshPlayers(tab, ap2); });
			tab.AddName(0, "Player List", TextAnchor.MiddleLeft);
		}
		public static void RefreshPlayers(Tab tab, AdminPlayer ap)
		{
			tab.ClearColumn(0);

			AddInitial(tab, ap);

			foreach (var player in BasePlayer.allPlayerList)
			{
				AddPlayer(tab, ap, player);
			}
		}
		public static void AddPlayer(Tab tab, AdminPlayer ap, BasePlayer player)
		{
			if (ap != null)
			{
				var filter = ap.GetStorage<string>("playerfilter");

				if (!string.IsNullOrEmpty(filter) && !(player.displayName.ToLower().Contains(filter.ToLower()) || player.UserIDString.Contains(filter))) return;
			}

			tab.AddRow(0, new Tab.OptionButton($"{player.displayName}", aap =>
			{
				ap.SetStorage("playerfilterpl", player);
				ShowInfo(tab, ap, player);
			}, aap => aap == null || !(aap.GetStorage<BasePlayer>("playerfilterpl") == player) ? Tab.OptionButton.Types.None : Tab.OptionButton.Types.Selected));
		}
		public static void ShowInfo(Tab tab, AdminPlayer aap, BasePlayer player)
		{
			tab.ClearColumn(1);

			tab.AddName(1, $"Player Information", TextAnchor.MiddleLeft);
			tab.AddInput(1, "Name", player.displayName, null);
			tab.AddInput(1, "Steam ID", player.UserIDString, null);
			tab.AddInput(1, "Net ID", $"{player.net?.ID}", null);
			try
			{
				var position = player.transform.position;
				tab.AddInput(1, "Position", $"{position.x:0.0}x {position.y:0.0}y {position.z:0.0}z", null);
			}
			catch { }

			tab.AddName(1, $"Permissions", TextAnchor.MiddleLeft);
			{
				tab.AddRow(1, new Tab.OptionButton("View Permissions", ap =>
				{
					var perms = Admin.FindTab("permissions");
					var permission = Community.Runtime.CorePlugin.permission;
					Admin.SetTab(ap.Player, "permissions");

					ap.SetStorage("player", player);
					PermissionsTab.GeneratePlayers(perms, permission, ap);
					PermissionsTab.GeneratePlugins(perms, ap, permission, ap.Player);
				}, (ap) => Tab.OptionButton.Types.Important));
			}

			if (aap == null || aap.Player != player)
			{
				tab.AddName(1, $"Actions", TextAnchor.MiddleLeft);

				tab.AddRow(1, new Tab.OptionButton("Teleport", ap => { ap.Player.Teleport(player); }, (ap) => Tab.OptionButton.Types.Warned));
				tab.AddRow(1, new Tab.OptionButton("Teleport to me", ap => { player.Teleport(ap.Player); }, (ap) => Tab.OptionButton.Types.Warned));
			}
			else
			{
				tab.AddText(1, "This is you.", 8, "1 1 1 0.5", TextAnchor.MiddleCenter, CUI.Handler.FontTypes.RobotoCondensedBold);
			}
		}
	}

	#endregion
}

public class AdminConfig
{
	public string OpenCommand = "cadmin";
	public int MinimumAuthLevel = 2;
	public float OptionWidth = 0.55f;
}
public class AdminData
{

}
