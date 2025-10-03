using System.Runtime.CompilerServices;
using Network;
using Oxide.Game.Rust.Cui;
using UnityEngine.UI;

namespace Carbon.Components;

public class LUI : IDisposable
{
	public readonly List<LuiContainer> elements = new();

	private readonly CUI _parent;

	private ImageDatabaseModule imgDb { get; }

	/// <summary>
	/// Boolean that changes default generation of element names.
	/// With this option disabled, you cannot create UI hierarchy without manual name input.
	/// </summary>
	public bool generateNames = true;

	public LUI(CUI cui)
	{
		_parent = cui;
		imgDb = BaseModule.GetModule<ImageDatabaseModule>();
	}

	/// <summary>
	/// Name of last created container. Used for easy new element creation to last parent without creating variable for that.
	/// </summary>
	public string lastName = string.Empty;

	#region Core Panel

	public LuiContainer CreateParent(CUI.ClientPanels parent, LuiPosition position, string name = "") => CreateParent(_parent.GetClientPanel(parent), position, name);

	public LuiContainer CreateParent(LuiContainer container, LuiPosition position, string name = "") => CreateParent(container.name, position, name);
	public LuiContainer CreateParent(string parent, LuiPosition position, string name = "")
	{
		LuiContainer cont = LuiPool.GetContainer();
		cont.parent = parent;
		if (name != string.Empty)
			cont.name = name;
		else if (generateNames)
			cont.name = RandomEx.GetRandomString(4);
		cont.SetAnchors(position);
		elements.Add(cont);
		return cont;
	}

	#endregion

	#region Updates

	public LuiContainer UpdatePosition(string name, LuiPosition pos)
	{
		LuiContainer cont = LuiPool.GetContainer();
		cont.name = name;
		cont.update = true;
		cont.SetAnchors(pos);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer UpdatePosition(string name, LuiOffset off)
	{
		LuiContainer cont = LuiPool.GetContainer();
		cont.name = name;
		cont.update = true;
		cont.SetOffset(off);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer UpdatePosition(string name, LuiPosition pos, LuiOffset off)
	{
		LuiContainer cont = LuiPool.GetContainer();
		cont.name = name;
		cont.update = true;
		cont.SetAnchorAndOffset(pos, off);
		elements.Add(cont);
		return cont;
	}

	/// <summary>
	/// Creates update container without any fields assigned.
	/// </summary>
	public LuiContainer Update(string name)
	{
		LuiContainer cont = LuiPool.GetContainer();
		cont.name = name;
		cont.update = true;
		elements.Add(cont);
		return cont;
	}

	public LuiContainer UpdateColor(string name, string color)
	{
		LuiContainer cont = LuiPool.GetContainer();
		cont.name = name;
		cont.update = true;
		cont.SetColor(color);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer UpdateText(string name, string text, int fontSize = 0, string color = null)
	{
		LuiContainer cont = LuiPool.GetContainer();
		cont.name = name;
		cont.update = true;
		cont.SetText(text, fontSize, color, update: true);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer UpdateButtonCommand(string name, string command, bool isProtected = true)
	{
		LuiContainer cont = LuiPool.GetContainer();
		cont.name = name;
		cont.update = true;
		cont.SetButton(isProtected ? Community.Protect(command) : command);
		elements.Add(cont);
		return cont;
	}

	#endregion

	#region Panel Creation

	public LuiContainer CreateEmptyContainer(LuiContainer container, string name = "", bool add = false) => CreateEmptyContainer(container.name, name, add);

	/// <summary>
	/// Creates empty container without anything. Shouldn't be used outside LUI library, but in rare cases might be useful.
	/// </summary>
	public LuiContainer CreateEmptyContainer(string parent, string name = "", bool add = false)
	{
		LuiContainer cont = LuiPool.GetContainer();
		cont.parent = parent;
		if (name != string.Empty)
			cont.name = name;
		else if (generateNames)
		{
			string newName = _parent.Manager.AppendId();
			lastName = newName;
			cont.name = newName;
		}
		if (add)
			elements.Add(cont);
		return cont;
	}

	public LuiContainer CreatePanel(LuiContainer container, LuiPosition position, LuiOffset offset, string color, string name = "") => CreatePanel(container.name, position, offset, color, name);
	public LuiContainer CreatePanel(LuiContainer container, LuiOffset offset, string color, string name = "") => CreatePanel(container.name, LuiPosition.None, offset, color, name);
	public LuiContainer CreatePanel(string parent, LuiOffset offset, string color, string name = "") => CreatePanel(parent, LuiPosition.None, offset, color, name);

	public LuiContainer CreatePanel(string parent, LuiPosition position, LuiOffset offset, string color, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetColor(color);
		elements.Add(cont);
		return cont;
	}
	public LuiContainer CreateText(LuiContainer container, LuiPosition position, LuiOffset offset, int fontSize, string color, string text, TextAnchor alignment = TextAnchor.UpperLeft, string name = "") => CreateText(container.name, position, offset, fontSize, color, text, alignment, name);
	public LuiContainer CreateText(LuiContainer container, LuiOffset offset, int fontSize, string color, string text, TextAnchor alignment = TextAnchor.UpperLeft, string name = "") => CreateText(container.name, LuiPosition.None, offset, fontSize, color, text, alignment, name);
	public LuiContainer CreateText(string parent, LuiOffset offset, int fontSize, string color, string text, TextAnchor alignment = TextAnchor.UpperLeft, string name = "") => CreateText(parent, LuiPosition.None, offset, fontSize, color, text, alignment, name);

	public LuiContainer CreateText(string parent, LuiPosition position, LuiOffset offset, int fontSize, string color, string text, TextAnchor alignment = TextAnchor.MiddleCenter, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetText(text, fontSize, color, alignment);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer CreateSprite(LuiContainer container, LuiPosition position, LuiOffset offset, string sprite, string color = LuiColors.White, string name = "") => CreateSprite(container.name, position, offset, sprite, color, name);
	public LuiContainer CreateSprite(LuiContainer container, LuiOffset offset, string sprite, string color = LuiColors.White, string name = "") => CreateSprite(container.name, LuiPosition.None, offset, sprite, color, name);
	public LuiContainer CreateSprite(string parent, LuiOffset offset, string sprite, string color = LuiColors.White, string name = "") => CreateSprite(parent, LuiPosition.None, offset, sprite, color, name);

	public LuiContainer CreateSprite(string parent, LuiPosition position, LuiOffset offset, string sprite, string color = LuiColors.White, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetSprite(sprite, color);
		elements.Add(cont);

		return cont;
	}

	public LuiContainer CreateImage(LuiContainer container, LuiPosition position, LuiOffset offset, string png, string color = LuiColors.White, string name = "") => CreateImage(container.name, position, offset, png, color, name);
	public LuiContainer CreateImage(LuiContainer container, LuiOffset offset, string png, string color = LuiColors.White, string name = "") => CreateImage(container.name, LuiPosition.None, offset, png, color, name);
	public LuiContainer CreateImage(string parent, LuiOffset offset, string png, string color = LuiColors.White, string name = "") => CreateImage(parent, LuiPosition.None, offset, png, color, name);

	public LuiContainer CreateImage(string parent, LuiPosition position, LuiOffset offset, string png, string color = LuiColors.White, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetImage(png, color);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer CreateImageFromDb(LuiContainer container, LuiPosition position, LuiOffset offset, string dbName, string color = LuiColors.White, string name = "") => CreateImageFromDb(container.name, position, offset, dbName, color, name);
	public LuiContainer CreateImageFromDb(LuiContainer container, LuiOffset offset, string dbName, string color = LuiColors.White, string name = "") => CreateImageFromDb(container.name, LuiPosition.None, offset, dbName, color, name);
	public LuiContainer CreateImageFromDb(string parent, LuiOffset offset, string dbName, string color = LuiColors.White, string name = "") => CreateImageFromDb(parent, LuiPosition.None, offset, dbName, color, name);

	public LuiContainer CreateImageFromDb(string parent, LuiPosition position, LuiOffset offset, string dbName, string color = LuiColors.White, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		if (imgDb.HasImage(dbName))
		{
			cont.SetAnchorAndOffset(position, offset);
			cont.SetImage(imgDb.GetImageString(dbName), color);
		}
		else
		{
			Logger.Warn($"[LUI] You're trying to load an image from ImageDatabase '{dbName}' which doesn't exist. Ignoring.");
			return null;
		}
		elements.Add(cont);
		return cont;
	}

	public LuiContainer CreateRawImageFromDb(LuiContainer container, LuiPosition position, LuiOffset offset, string dbName, string color = LuiColors.White, string name = "") => CreateRawImageFromDb(container.name, position, offset, dbName, color, name);
	public LuiContainer CreateRawImageFromDb(LuiContainer container, LuiOffset offset, string dbName, string color = LuiColors.White, string name = "") => CreateRawImageFromDb(container.name, LuiPosition.None, offset, dbName, color, name);
	public LuiContainer CreateRawImageFromDb(string parent, LuiOffset offset, string dbName, string color = LuiColors.White, string name = "") => CreateRawImageFromDb(parent, LuiPosition.None, offset, dbName, color, name);

	public LuiContainer CreateRawImageFromDb(string parent, LuiPosition position, LuiOffset offset, string dbName, string color = LuiColors.White, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		if (imgDb.HasImage(dbName))
		{
			cont.SetAnchorAndOffset(position, offset);
			cont.SetRawImage(imgDb.GetImageString(dbName), color);
		}
		else
		{
			Logger.Warn($"[LUI] You're trying to load an image from ImageDatabase '{dbName}' which doesn't exist. Ignoring.");
			return null;
		}
		elements.Add(cont);
		return cont;
	}


	public LuiContainer CreateUrlImage(LuiContainer container, LuiPosition position, LuiOffset offset, string url, string color = LuiColors.White, string name = "") => CreateUrlImage(container.name, position, offset, url, color, name);
	public LuiContainer CreateUrlImage(LuiContainer container, LuiOffset offset, string url, string color = LuiColors.White, string name = "") => CreateUrlImage(container.name, LuiPosition.None, offset, url, color, name);
	public LuiContainer CreateUrlImage(string parent, LuiOffset offset, string url, string color = LuiColors.White, string name = "") => CreateUrlImage(parent, LuiPosition.None, offset, url, color, name);

	public LuiContainer CreateUrlImage(string parent, LuiPosition position, LuiOffset offset, string url, string color = LuiColors.White, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetUrlImage(url, color);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer CreateItemIcon(LuiContainer container, LuiPosition position, LuiOffset offset, string shortname, ulong skinId = 0, string color = "", string name = "") => CreateItemIcon(container.name, position, offset, shortname, skinId, color, name);
	public LuiContainer CreateItemIcon(LuiContainer container, LuiOffset offset, string shortname, ulong skinId = 0, string color = "", string name = "") => CreateItemIcon(container.name, LuiPosition.None, offset, shortname, skinId, color, name);
	public LuiContainer CreateItemIcon(string parent, LuiOffset offset, string shortname, ulong skinId = 0, string color = "", string name = "") => CreateItemIcon(parent, LuiPosition.None, offset, shortname, skinId, color, name);

	public LuiContainer CreateItemIcon(string parent, LuiPosition position, LuiOffset offset, string shortname, ulong skinId = 0, string color = "", string name = "")
	{
		ItemDefinition def = ItemManager.FindItemDefinition(shortname);
		if (def)
			return CreateItemIcon(parent, position, offset, def.itemid, skinId, color, name);
		Logger.Warn($"[LUI] We couldn't find '{shortname}' as valid item shortname. Ignoring.");
		return null;
	}

	public LuiContainer CreateItemIcon(LuiContainer container, LuiPosition position, LuiOffset offset, int itemId, ulong skinId = 0, string color = "", string name = "") => CreateItemIcon(container.name, position, offset, itemId, skinId, color, name);
	public LuiContainer CreateItemIcon(LuiContainer container, LuiOffset offset, int itemId, ulong skinId = 0, string color = "", string name = "") => CreateItemIcon(container.name, LuiPosition.None, offset, itemId, skinId, color, name);
	public LuiContainer CreateItemIcon(string parent, LuiOffset offset, int itemId, ulong skinId = 0, string color = "", string name = "") => CreateItemIcon(parent, LuiPosition.None, offset, itemId, skinId, color, name);

	public LuiContainer CreateItemIcon(string parent, LuiPosition position, LuiOffset offset, int itemId, ulong skinId = 0, string color = "", string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetItemIcon(itemId, skinId);
		if (color != string.Empty)
			cont.SetColor(color);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer CreateButton(LuiContainer container, LuiPosition position, LuiOffset offset, string command, string color, bool isProtected = true, string name = "") => CreateButton(container.name, position, offset, command, color, isProtected, name);
	public LuiContainer CreateButton(LuiContainer container, LuiOffset offset, string command, string color, bool isProtected = true, string name = "") => CreateButton(container.name, LuiPosition.None, offset, command, color, isProtected, name);
	public LuiContainer CreateButton(string parent, LuiOffset offset, string command, string color, bool isProtected = true, string name = "") => CreateButton(parent, LuiPosition.None, offset, command, color, isProtected, name);

	public LuiContainer CreateButton(string parent, LuiPosition position, LuiOffset offset, string command, string color, bool isProtected = true, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetButton(isProtected ? Community.Protect(command) : command, color);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer CreateInput(LuiContainer container, LuiPosition position, LuiOffset offset, string color, string text, int fontSize, string command, int charLimit = 0, bool isProtected = true, CUI.Handler.FontTypes font = CUI.Handler.FontTypes.RobotoCondensedBold, TextAnchor alignment = TextAnchor.UpperLeft, string name = "") => CreateInput(container.name, position, offset, color, text, fontSize, command, charLimit, isProtected, font, alignment, name);
	public LuiContainer CreateInput(LuiContainer container, LuiOffset offset, string color, string text, int fontSize, string command, int charLimit = 0, bool isProtected = true, CUI.Handler.FontTypes font = CUI.Handler.FontTypes.RobotoCondensedBold, TextAnchor alignment = TextAnchor.UpperLeft, string name = "") => CreateInput(container.name, LuiPosition.None, offset, color, text, fontSize, command, charLimit, isProtected, font, alignment, name);
	public LuiContainer CreateInput(string parent, LuiOffset offset, string color, string text, int fontSize, string command, int charLimit = 0, bool isProtected = true, CUI.Handler.FontTypes font = CUI.Handler.FontTypes.RobotoCondensedBold, TextAnchor alignment = TextAnchor.UpperLeft, string name = "") => CreateInput(parent, LuiPosition.None, offset, color, text, fontSize, command, charLimit, isProtected, font, alignment, name);

	public LuiContainer CreateInput(string parent, LuiPosition position, LuiOffset offset, string color, string text, int fontSize, string command, int charLimit = 0, bool isProtected = true, CUI.Handler.FontTypes font = CUI.Handler.FontTypes.RobotoCondensedBold, TextAnchor alignment = TextAnchor.MiddleCenter, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetInput(color, text, fontSize, isProtected ? Community.Protect(command) : command, charLimit, font, alignment);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer CreateCountdown(LuiContainer container, LuiPosition position, LuiOffset offset, int fontSize, string color, string text, TextAnchor alignment, float startTime, float endTime, float step = 1, float interval = 1, string command = null, bool isProtected = true, string name = "") => CreateCountdown(container.name, position, offset, fontSize, color, text, alignment, startTime, endTime, step, interval, command, isProtected, name);
	public LuiContainer CreateCountdown(LuiContainer container, LuiOffset offset, int fontSize, string color, string text, TextAnchor alignment, float startTime, float endTime, float step = 1, float interval = 1, string command = null, bool isProtected = true, string name = "") => CreateCountdown(container.name, LuiPosition.None, offset, fontSize, color, text, alignment, startTime, endTime, step, interval, command, isProtected, name);
	public LuiContainer CreateCountdown(string parent, LuiOffset offset, int fontSize, string color, string text, TextAnchor alignment, float startTime, float endTime, float step = 1, float interval = 1, string command = null, bool isProtected = true, string name = "") => CreateCountdown(parent, LuiPosition.None, offset, fontSize, color, text, alignment, startTime, endTime, step, interval, command, isProtected, name);

	public LuiContainer CreateCountdown(string parent, LuiPosition position, LuiOffset offset, int fontSize, string color, string text, TextAnchor alignment, float startTime, float endTime, float step = 1, float interval = 1, string command = null, bool isProtected = true, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetText(text, fontSize, color, alignment);
		cont.SetCountdown(startTime, endTime, step, interval, isProtected ? Community.Protect(command) : command);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer CreateHorizontalLayoutGroup(LuiContainer container, LuiPosition position, LuiOffset offset, float spacing = 0, string name = "") => CreateHorizontalLayoutGroup(container.name, position, offset, spacing, name);
	public LuiContainer CreateHorizontalLayoutGroup(LuiContainer container, LuiOffset offset, float spacing = 0, string name = "") => CreateHorizontalLayoutGroup(container.name, LuiPosition.None, offset, spacing, name);
	public LuiContainer CreateHorizontalLayoutGroup(string parent, LuiOffset offset, float spacing = 0, string name = "") => CreateHorizontalLayoutGroup(parent, LuiPosition.None, offset, spacing, name);

	public LuiContainer CreateHorizontalLayoutGroup(string parent, LuiPosition position, LuiOffset offset, float spacing = 0, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetHorizontalLayoutSpacing(spacing);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer CreateVerticalLayoutGroup(LuiContainer container, LuiPosition position, LuiOffset offset, float spacing = 0, string name = "") => CreateVerticalLayoutGroup(container.name, position, offset, spacing, name);
	public LuiContainer CreateVerticalLayoutGroup(LuiContainer container, LuiOffset offset, float spacing = 0, string name = "") => CreateVerticalLayoutGroup(container.name, LuiPosition.None, offset, spacing, name);
	public LuiContainer CreateVerticalLayoutGroup(string parent, LuiOffset offset, float spacing = 0, string name = "") => CreateVerticalLayoutGroup(parent, LuiPosition.None, offset, spacing, name);

	public LuiContainer CreateVerticalLayoutGroup(string parent, LuiPosition position, LuiOffset offset, float spacing = 0, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetVerticalLayoutSpacing(spacing);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer CreateGridLayoutGroup(LuiContainer container, LuiPosition position, LuiOffset offset, Vector2 cellSize, string name = "") => CreateGridLayoutGroup(container.name, position, offset, cellSize, name);
	public LuiContainer CreateGridLayoutGroup(LuiContainer container, LuiOffset offset, Vector2 cellSize, string name = "") => CreateGridLayoutGroup(container.name, LuiPosition.None, offset, cellSize, name);
	public LuiContainer CreateGridLayoutGroup(string parent, LuiOffset offset, Vector2 cellSize, string name = "") => CreateGridLayoutGroup(parent, LuiPosition.None, offset, cellSize, name);

	public LuiContainer CreateGridLayoutGroup(string parent, LuiPosition position, LuiOffset offset, Vector2 cellSize, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetCellSize(cellSize);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer CreateContentFitter(LuiContainer container, LuiPosition position, LuiOffset offset, ContentSizeFitter.FitMode horizontal, ContentSizeFitter.FitMode vertical, string name = "") => CreateContentFitter(container.name, position, offset, horizontal, vertical, name);
	public LuiContainer CreateContentFitter(LuiContainer container, LuiOffset offset, ContentSizeFitter.FitMode horizontal, ContentSizeFitter.FitMode vertical, string name = "") => CreateContentFitter(container.name, LuiPosition.None, offset, horizontal, vertical, name);
	public LuiContainer CreateContentFitter(string parent, LuiOffset offset, ContentSizeFitter.FitMode horizontal, ContentSizeFitter.FitMode vertical, string name = "") => CreateContentFitter(parent, LuiPosition.None, offset, horizontal, vertical, name);

	public LuiContainer CreateContentFitter(string parent, LuiPosition position, LuiOffset offset, ContentSizeFitter.FitMode horizontal, ContentSizeFitter.FitMode vertical, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetFitMode(horizontal, vertical);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer CreateLayoutElement(LuiContainer container, LuiPosition position, LuiOffset offset, float minWidth, float minHeight, string name = "") => CreateLayoutElement(container.name, position, offset, minWidth, minHeight, name);
	public LuiContainer CreateLayoutElement(LuiContainer container, LuiOffset offset, float minWidth, float minHeight, string name = "") => CreateLayoutElement(container.name, LuiPosition.None, offset, minWidth, minHeight, name);
	public LuiContainer CreateLayoutElement(string parent, LuiOffset offset, float minWidth, float minHeight, string name = "") => CreateLayoutElement(parent, LuiPosition.None, offset, minWidth, minHeight, name);

	public LuiContainer CreateLayoutElement(string parent, LuiPosition position, LuiOffset offset, float minWidth, float minHeight, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetMinimalSize(minWidth, minHeight);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer CreateDraggable(LuiContainer container, LuiPosition position, LuiOffset offset, string color, string filter = null, bool dropAnywhere = true, bool keepOnTop = false, bool limitToParent = false, float maxDistance = -1f, bool allowSwapping = false, string name = "") => CreateDraggable(container.name, position, offset, color, filter, dropAnywhere, keepOnTop, limitToParent, maxDistance, allowSwapping, name);
	public LuiContainer CreateDraggable(LuiContainer container, LuiOffset offset, string color, string filter = null, bool dropAnywhere = true, bool keepOnTop = false, bool limitToParent = false, float maxDistance = -1f, bool allowSwapping = false, string name = "") => CreateDraggable(container.name, LuiPosition.None, offset, color, filter, dropAnywhere, keepOnTop, limitToParent, maxDistance, allowSwapping, name);
	public LuiContainer CreateDraggable(string parent, LuiOffset offset, string color, string filter = null, bool dropAnywhere = true, bool keepOnTop = false, bool limitToParent = false, float maxDistance = -1f, bool allowSwapping = false, string name = "") => CreateDraggable(parent, LuiPosition.None, offset, color, filter, dropAnywhere, keepOnTop, limitToParent, maxDistance, allowSwapping, name);

	public LuiContainer CreateDraggable(string parent, LuiPosition position, LuiOffset offset, string color, string filter = null, bool dropAnywhere = true, bool keepOnTop = false, bool limitToParent = false, float maxDistance = -1f, bool allowSwapping = false, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetColor(color);
		cont.SetDraggable(filter, dropAnywhere, keepOnTop, limitToParent, maxDistance, allowSwapping);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer CreateSlot(LuiContainer container, LuiPosition position, LuiOffset offset, string filter = null, string name = "") => CreateSlot(container.name, position, offset, filter, name);
	public LuiContainer CreateSlot(LuiContainer container, LuiOffset offset, string filter = null, string name = "") => CreateSlot(container.name, LuiPosition.None, offset, filter, name);
	public LuiContainer CreateSlot(string parent, LuiOffset offset, string filter = null, string name = "") => CreateSlot(parent, LuiPosition.None, offset, filter, name);

	public LuiContainer CreateSlot(string parent, LuiPosition position, LuiOffset offset, string filter = null, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetSlot(filter);
		elements.Add(cont);
		return cont;
	}

	public LuiContainer CreateScrollView(LuiContainer container, LuiPosition position, LuiOffset offset, bool vertical, bool horizontal, ScrollRect.MovementType movementType = ScrollRect.MovementType.Clamped, float elasticity = 0, bool inertia = false, float decelerationRate = 0, float scrollSensitivity = 0, LuiScrollbar verticalScrollOptions = default, LuiScrollbar horizontalScrollOptions = default, string name = "") => CreateScrollView(container.name, position, offset, vertical, horizontal, movementType, elasticity, inertia, decelerationRate, scrollSensitivity, verticalScrollOptions, horizontalScrollOptions, name);
	public LuiContainer CreateScrollView(LuiContainer container, LuiOffset offset, bool vertical, bool horizontal, ScrollRect.MovementType movementType = ScrollRect.MovementType.Clamped, float elasticity = 0, bool inertia = false, float decelerationRate = 0, float scrollSensitivity = 0, LuiScrollbar verticalScrollOptions = default, LuiScrollbar horizontalScrollOptions = default, string name = "") => CreateScrollView(container.name, LuiPosition.None, offset, vertical, horizontal, movementType, elasticity, inertia, decelerationRate, scrollSensitivity, verticalScrollOptions, horizontalScrollOptions, name);
	public LuiContainer CreateScrollView(string parent, LuiOffset offset, bool vertical, bool horizontal, ScrollRect.MovementType movementType = ScrollRect.MovementType.Clamped, float elasticity = 0, bool inertia = false, float decelerationRate = 0, float scrollSensitivity = 0, LuiScrollbar verticalScrollOptions = default, LuiScrollbar horizontalScrollOptions = default, string name = "") => CreateScrollView(parent, LuiPosition.None, offset, vertical, horizontal, movementType, elasticity, inertia, decelerationRate, scrollSensitivity, verticalScrollOptions, horizontalScrollOptions, name);

	public LuiContainer CreateScrollView(string parent, LuiPosition position, LuiOffset offset, bool vertical, bool horizontal, ScrollRect.MovementType movementType = ScrollRect.MovementType.Clamped, float elasticity = 0, bool inertia = false, float decelerationRate = 0, float scrollSensitivity = 0, LuiScrollbar verticalScrollOptions = default, LuiScrollbar horizontalScrollOptions = default, string name = "")
	{
		LuiContainer cont = CreateEmptyContainer(parent, name);
		cont.SetAnchorAndOffset(position, offset);
		cont.SetScrollView(vertical, horizontal, movementType, elasticity, inertia, decelerationRate, scrollSensitivity, verticalScrollOptions, horizontalScrollOptions);
		elements.Add(cont);
		return cont;
	}

	#endregion

	/// <summary>
	/// Gets the built UI in bytes. Useful when redrawing whole UI, and we want to minimalize delay between destroy and draw.
	/// </summary>
	public byte[] GetUiBytes()
	{
		using LuiBuilderInstance cbi = new LuiBuilderInstance(this);
		return cbi.GetMergedBytes();
	}

	/// <summary>
	/// Builds and sends UI to player.
	/// </summary>
	public void SendUi(BasePlayer player) => Send(new SendInfo(player.Connection));

	/// <summary>
	/// Builds and sends UI to player. Preferred SendUi(BasePlayer) over this method.
	/// </summary>
	public void SendUiJson(BasePlayer player) => SendJson(new SendInfo(player.Connection));

	/// <summary>
	/// Sends already buit UI to player. Need to run GetUiBytes() in order to get the bytes before.
	/// </summary>
	public void SendUiBytes(BasePlayer player, byte[] bytes) => SendBytes(new SendInfo(player.Connection), bytes);

	/// <summary>
	/// Returns string JSON of currently built UI.
	/// </summary>
	public string ToJson()
	{
		using LuiBuilderInstance cbi = new LuiBuilderInstance(this);
		return cbi.GetJsonString();
	}

	private void Send(SendInfo send)
	{
		using LuiBuilderInstance cbi = new LuiBuilderInstance(this);
		NetWrite write = Net.sv.StartWrite();
		write.PacketID(Message.Type.RPCMessage);
		write.EntityID(CommunityEntity.ServerInstance.net.ID);
		write.UInt32(StringPool.Get("AddUI"));
		write.BytesWithSize(cbi.GetMergedBytes());
		write.Send(send);
	}

	private void SendBytes(SendInfo send, byte[] bytes)
	{
		NetWrite write = Net.sv.StartWrite();
		write.PacketID(Message.Type.RPCMessage);
		write.EntityID(CommunityEntity.ServerInstance.net.ID);
		write.UInt32(StringPool.Get("AddUI"));
		write.BytesWithSize(bytes);
		write.Send(send);
	}

	private void SendJson(SendInfo send)
	{
		using LuiBuilderInstance cbi = new LuiBuilderInstance(this);
		NetWrite write = Net.sv.StartWrite();
		write.PacketID(Message.Type.RPCMessage);
		write.EntityID(CommunityEntity.ServerInstance.net.ID);
		write.UInt32(StringPool.Get("AddUI"));
		write.String(cbi.GetJsonString());
		write.Send(send);
	}

	public void Dispose()
	{
		for (var i = 0; i < elements.Count; i++)
		{
			LuiContainer element = elements[i];
			foreach (LuiCompBase component in element.luiComponents)
				LuiPool.ReturnComp(component);
			LuiPool.ReturnContainer(element);
		}
		elements.Clear();
	}

	#region Quicker Strings

	public static string GetFont(CUI.Handler.FontTypes type)
	{
		return type switch
		{
			CUI.Handler.FontTypes.RobotoCondensedBold => "robotocondensed-bold.ttf",
			CUI.Handler.FontTypes.RobotoCondensedRegular => "robotocondensed-regular.ttf",
			CUI.Handler.FontTypes.PermanentMarker => "permanentmarker.ttf",
			CUI.Handler.FontTypes.DroidSansMono => "droidsansmono.ttf",
			CUI.Handler.FontTypes.NotoSansArabicBold => "_nonenglish/notosanscjksc-bold.otf",
			CUI.Handler.FontTypes.Poxel => "poxel.otf",
			CUI.Handler.FontTypes.LCD => "lcd.ttf",
			CUI.Handler.FontTypes.NoToEmoji => "_nonenglish/notoemoji-regular.ttf",
			CUI.Handler.FontTypes.PressStart => "pressstart2p-regular.ttf",
			_ => "robotocondensed-regular.ttf"
		};
	}

	public static string GetAlign(TextAnchor anchor)
	{
		return anchor switch
		{
			TextAnchor.UpperLeft => nameof(TextAnchor.UpperLeft),
			TextAnchor.UpperCenter => nameof(TextAnchor.UpperCenter),
			TextAnchor.UpperRight => nameof(TextAnchor.UpperRight),
			TextAnchor.MiddleLeft => nameof(TextAnchor.MiddleLeft),
			TextAnchor.MiddleCenter => nameof(TextAnchor.MiddleCenter),
			TextAnchor.MiddleRight => nameof(TextAnchor.MiddleRight),
			TextAnchor.LowerLeft => nameof(TextAnchor.LowerLeft),
			TextAnchor.LowerCenter => nameof(TextAnchor.LowerCenter),
			TextAnchor.LowerRight => nameof(TextAnchor.LowerRight),
			_ => nameof(TextAnchor.UpperLeft)
		};
	}

	public static string GetImageType(UnityEngine.UI.Image.Type imgType)
	{
		return imgType switch
		{
			Image.Type.Simple => nameof(Image.Type.Simple),
			Image.Type.Sliced => nameof(Image.Type.Sliced),
			Image.Type.Tiled => nameof(Image.Type.Tiled),
			Image.Type.Filled => nameof(Image.Type.Filled),
			_ => nameof(Image.Type.Simple)
		};
	}

	public static string GetWrapMode(VerticalWrapMode mode)
	{
		return mode switch
		{
			VerticalWrapMode.Truncate => nameof(VerticalWrapMode.Truncate),
			VerticalWrapMode.Overflow => nameof(VerticalWrapMode.Overflow),
			_ => nameof(VerticalWrapMode.Truncate)
		};
	}

	public static string GetLineType(InputField.LineType lineType)
	{
		return lineType switch
		{
			InputField.LineType.SingleLine => nameof(InputField.LineType.SingleLine),
			InputField.LineType.MultiLineSubmit => nameof(InputField.LineType.MultiLineSubmit),
			InputField.LineType.MultiLineNewline => nameof(InputField.LineType.MultiLineNewline),
			_ => nameof(InputField.LineType.SingleLine)
		};
	}

	public static string GetMovementType(ScrollRect.MovementType movementType)
	{
		return movementType switch
		{
			ScrollRect.MovementType.Unrestricted => nameof(ScrollRect.MovementType.Unrestricted),
			ScrollRect.MovementType.Elastic => nameof(ScrollRect.MovementType.Elastic),
			ScrollRect.MovementType.Clamped => nameof(ScrollRect.MovementType.Clamped),
			_ => nameof(ScrollRect.MovementType.Unrestricted)
		};
	}

	public static string GetTimerFormat(TimerFormat format)
	{
		return format switch
		{
			TimerFormat.None => nameof(TimerFormat.None),
			TimerFormat.SecondsHundreth => nameof(TimerFormat.SecondsHundreth),
			TimerFormat.MinutesSeconds => nameof(TimerFormat.MinutesSeconds),
			TimerFormat.MinutesSecondsHundreth => nameof(TimerFormat.MinutesSecondsHundreth),
			TimerFormat.HoursMinutes => nameof(TimerFormat.HoursMinutes),
			TimerFormat.HoursMinutesSeconds => nameof(TimerFormat.HoursMinutesSeconds),
			TimerFormat.HoursMinutesSecondsMilliseconds => nameof(TimerFormat.HoursMinutesSecondsMilliseconds),
			TimerFormat.HoursMinutesSecondsTenths => nameof(TimerFormat.HoursMinutesSecondsTenths),
			TimerFormat.DaysHoursMinutes => nameof(TimerFormat.DaysHoursMinutes),
			TimerFormat.DaysHoursMinutesSeconds => nameof(TimerFormat.DaysHoursMinutesSeconds),
			TimerFormat.Custom => nameof(TimerFormat.Custom),
			_ => nameof(TimerFormat.None)
		};
	}

	public static string GetCorner(GridLayoutGroup.Corner corner)
	{
		return corner switch
		{
			GridLayoutGroup.Corner.UpperLeft => nameof(GridLayoutGroup.Corner.UpperLeft),
			GridLayoutGroup.Corner.UpperRight => nameof(GridLayoutGroup.Corner.UpperRight),
			GridLayoutGroup.Corner.LowerLeft => nameof(GridLayoutGroup.Corner.LowerLeft),
			GridLayoutGroup.Corner.LowerRight => nameof(GridLayoutGroup.Corner.LowerRight),
			_ => nameof(GridLayoutGroup.Corner.UpperLeft)
		};
	}

	public static string GetAxis(GridLayoutGroup.Axis axis)
	{
		return axis switch
		{
			GridLayoutGroup.Axis.Horizontal => nameof(GridLayoutGroup.Axis.Horizontal),
			GridLayoutGroup.Axis.Vertical => nameof(GridLayoutGroup.Axis.Vertical),
			_ => nameof(GridLayoutGroup.Axis.Vertical)
		};
	}

	public static string GetConstraint(GridLayoutGroup.Constraint constraint)
	{
		return constraint switch
		{
			GridLayoutGroup.Constraint.Flexible => nameof(GridLayoutGroup.Constraint.Flexible),
			GridLayoutGroup.Constraint.FixedColumnCount => nameof(GridLayoutGroup.Constraint.FixedColumnCount),
			GridLayoutGroup.Constraint.FixedRowCount => nameof(GridLayoutGroup.Constraint.FixedRowCount),
			_ => nameof(GridLayoutGroup.Constraint.FixedRowCount)
		};
	}

	public static string GetFitMode(ContentSizeFitter.FitMode mode)
	{
		return mode switch
		{
			ContentSizeFitter.FitMode.Unconstrained => nameof(ContentSizeFitter.FitMode.Unconstrained),
			ContentSizeFitter.FitMode.MinSize => nameof(ContentSizeFitter.FitMode.MinSize),
			ContentSizeFitter.FitMode.PreferredSize => nameof(ContentSizeFitter.FitMode.PreferredSize),
			_ => nameof(ContentSizeFitter.FitMode.Unconstrained)
		};
	}

	public static string GetSendType(CommunityEntity.DraggablePositionSendType type)
	{
		return type switch
		{
			CommunityEntity.DraggablePositionSendType.NormalizedScreen => nameof(CommunityEntity.DraggablePositionSendType.NormalizedScreen),
			CommunityEntity.DraggablePositionSendType.NormalizedParent => nameof(CommunityEntity.DraggablePositionSendType.NormalizedParent),
			CommunityEntity.DraggablePositionSendType.Relative => nameof(CommunityEntity.DraggablePositionSendType.Relative),
			CommunityEntity.DraggablePositionSendType.RelativeAnchor => nameof(CommunityEntity.DraggablePositionSendType.RelativeAnchor),
			_ => nameof(CommunityEntity.DraggablePositionSendType.NormalizedScreen)
		};
	}

	#endregion

	public class LuiContainer
	{
		public string name;
		public string parent;
		public LuiComponentDictionary luiComponents;
		public string destroyUi;
		public float fadeOut;
		public bool update;
		public bool activeSelf = true;

		#region Container Methods - Global

		public LuiContainer SetDestroy(string name)
		{
			destroyUi = name;
			return this;
		}

		public LuiContainer SetFadeOut(float time)
		{
			fadeOut = time;
			return this;
		}

		public LuiContainer SetName(string newName)
		{
			name = newName;
			return this;
		}

		public LuiContainer SetActiveSelf(bool active)
		{
			activeSelf = active;
			return this;
		}

	    /// <summary>
	    /// Updates or creates new component in current element.
		/// Recommended to use only when there is no built-in method for that.
	    /// </summary>
		public T UpdateComp<T>() where T : LuiCompBase
		{
			//if (!update)
			//	Logger.Warn($"[LUI] You're trying to create update in element '{name}' (of parent '{parent}') which doesn't allow updates. Ignoring.");
			if (luiComponents.TryGetValue<T>(LuiPool.GetLuiCompType(typeof(T)), out var component))
			{
				return component;
			}
			component = LuiPool.GetLuiCompFromPool<T>(typeof(T));
			luiComponents.Add(component.type, component);
			return component;
		}

		public void SetEnabled<T>(bool enabled = true) where T : LuiCompBase
		{
			//if (!update)
			//{
			//	Logger.Warn($"[LUI] You're trying to create update in element '{name}' (of parent '{parent}') which doesn't allow updates. Ignoring.");
			//	return;
			//}
			if (luiComponents.TryGetValue<T>(LuiPool.GetLuiCompType(typeof(T)), out var component))
			{
				component.enabled = enabled;
			}
			else
			{
				Logger.Warn($"[LUI] You're trying to switch state of component '{typeof(T)}' but it isn't present. Ignoring.");
			}
		}

		public void SetFadeIn<T>(float fadeIn) where T : LuiCompBase
		{
			if (luiComponents.TryGetValue<T>(LuiPool.GetLuiCompType(typeof(T)), out var component))
			{
				component.fadeIn = fadeIn;
			}
			else
			{
				Logger.Warn($"[LUI] You're trying to switch fadeIn of component '{typeof(T)}' but it isn't present. Ignoring.");
			}
		}

		public void SetPlaceholderParentId<T>(string placeholderParentId) where T : LuiCompBase
		{
			if (luiComponents.TryGetValue<T>(LuiPool.GetLuiCompType(typeof(T)), out var component))
			{
				component.placeholderParentId = placeholderParentId;
			}
			else
			{
				Logger.Warn($"[LUI] You're trying to switch fadeIn of component '{typeof(T)}' but it isn't present. Ignoring.");
			}
		}

		#endregion

		#region Container Methods - LuiTextComp

		public LuiContainer SetText(string input, int fontSize = 0, string color = null, TextAnchor alignment = TextAnchor.MiddleCenter, bool update = false)
		{
			if (luiComponents.TryGetValue<LuiTextComp>(LuiCompType.Text, out var text))
			{
				text.text = input;
				if (fontSize > 0)
					text.fontSize = fontSize;
				if (color != null)
					text.color = color;
				if (!update)
					text.align = GetAlign(alignment);
			}
			else
			{
				text = LuiPool.GetText();
				text.text = input;
				if (fontSize > 0)
					text.fontSize = fontSize;
				if (color != null)
					text.color = color;
				if (!update)
					text.align = GetAlign(alignment);
				luiComponents.Add(text.type, text);
			}
			return this;
		}

		public LuiContainer SetTextColor(string color)
		{
			if (luiComponents.TryGetValue<LuiTextComp>(LuiCompType.Text, out var text))
			{
				text.color = color;
			}
			else
			{
				text = LuiPool.GetText();
				text.color = color;
				luiComponents.Add(text.type, text);
			}
			return this;
		}

		public LuiContainer SetTextFont(CUI.Handler.FontTypes font)
		{
			if (luiComponents.TryGetValue<LuiTextComp>(LuiCompType.Text, out var text))
			{
				text.font = GetFont(font);
			}
			else
			{
				text = LuiPool.GetText();
				text.font = GetFont(font);
				luiComponents.Add(text.type, text);
			}
			return this;
		}

		public LuiContainer SetTextAlign(TextAnchor align)
		{
			if (luiComponents.TryGetValue<LuiTextComp>(LuiCompType.Text, out var text))
			{
				text.align = GetAlign(align);
			}
			else
			{
				text = LuiPool.GetText();
				text.align = GetAlign(align);
				luiComponents.Add(text.type, text);
			}
			return this;
		}

		public LuiContainer SetTextOverflow(VerticalWrapMode verticalOverflow)
		{
			if (luiComponents.TryGetValue<LuiTextComp>(LuiCompType.Text, out var text))
			{
				text.verticalOverflow = GetWrapMode(verticalOverflow);
			}
			else
			{
				text = LuiPool.GetText();
				text.verticalOverflow = GetWrapMode(verticalOverflow);
				luiComponents.Add(text.type, text);
			}
			return this;
		}

		#endregion

		#region Container Methods - LuiImageComp

		public LuiContainer SetColor(string color)
		{
			if (luiComponents.TryGetValue<LuiImageComp>(LuiCompType.Image, out var img))
			{
				img.color = color;
			}
			else
			{
				img = LuiPool.GetImage();
				img.color = color;
				luiComponents.Add(img.type, img);
			}
			return this;
		}

		public LuiContainer SetMaterial(string material)
		{
			if (luiComponents.TryGetValue<LuiImageComp>(LuiCompType.Image, out var img))
			{
				img.material = material;
			}
			else
			{
				img = LuiPool.GetImage();
				img.material = material;
				luiComponents.Add(img.type, img);
			}
			return this;
		}

		public LuiContainer SetImageType(UnityEngine.UI.Image.Type imageType)
		{
			if (luiComponents.TryGetValue<LuiImageComp>(LuiCompType.Image, out var img))
			{
				img.imageType = GetImageType(imageType);
			}
			else
			{
				img = LuiPool.GetImage();
				img.imageType = GetImageType(imageType);
				luiComponents.Add(img.type, img);
			}
			return this;
		}

		public LuiContainer SetSprite(string sprite = null, string color = null, UnityEngine.UI.Image.Type imageType = Image.Type.Simple)
		{
			if (luiComponents.TryGetValue<LuiImageComp>(LuiCompType.Image, out var img))
			{
				if (sprite != null)
				{
					img.sprite = sprite;
					img.imageType = GetImageType(imageType);
				}
				if (color != null)
					img.color = color;
			}
			else
			{
				img = LuiPool.GetImage();
				if (sprite != null)
				{
					img.sprite = sprite;
					img.imageType = GetImageType(imageType);
				}
				if (color != null)
					img.color = color;
				luiComponents.Add(img.type, img);
			}
			return this;
		}

		public LuiContainer SetImage(string png = null, string color = null)
		{
			if (luiComponents.TryGetValue<LuiImageComp>(LuiCompType.Image, out var img))
			{
				if (png != null)
					img.png = png;
				if (color != null)
					img.color = color;
			}
			else
			{
				img = LuiPool.GetImage();
				if (png != null)
					img.png = png;
				if (color != null)
					img.color = color;
				luiComponents.Add(img.type, img);
			}
			return this;
		}

		public LuiContainer SetFillCenter(bool fill)
		{
			if (luiComponents.TryGetValue<LuiImageComp>(LuiCompType.Image, out var img))
			{
				img.fillCenter = fill;
			}
			else
			{
				img = LuiPool.GetImage();
				img.fillCenter = fill;
				luiComponents.Add(img.type, img);
			}
			return this;
		}

		public LuiContainer SetImageSlice(string sliceValue)
		{
			if (luiComponents.TryGetValue<LuiImageComp>(LuiCompType.Image, out var img))
			{
				img.slice = sliceValue;
			}
			else
			{
				img = LuiPool.GetImage();
				img.slice = sliceValue;
				luiComponents.Add(img.type, img);
			}
			return this;
		}

		public LuiContainer SetItemIcon(int itemid, ulong skinid)
		{
			if (luiComponents.TryGetValue<LuiImageComp>(LuiCompType.Image, out var img))
			{
				img.itemid = itemid;
				img.skinid = skinid;
			}
			else
			{
				img = LuiPool.GetImage();
				img.itemid = itemid;
				img.skinid = skinid;
				luiComponents.Add(img.type, img);
			}
			return this;
		}

		#endregion

		#region Container Methods - LuiRawImageComp

		public LuiContainer SetUrlImage(string url = null, string color = null)
		{
			if (luiComponents.TryGetValue<LuiRawImageComp>(LuiCompType.RawImage, out var img))
			{
				if (url != null)
					img.url = url;
				if (color != null)
					img.color = color;
			}
			else
			{
				img = LuiPool.GetRawImage();
				if (url != null)
					img.url = url;
				if (color != null)
					img.color = color;
				luiComponents.Add(img.type, img);
			}
			return this;
		}

		public LuiContainer SetRawImage(string png = null, string color = null)
		{
			if (luiComponents.TryGetValue<LuiRawImageComp>(LuiCompType.RawImage, out var img))
			{
				if (png != null)
					img.png = png;
				if (color != null)
					img.color = color;
			}
			else
			{
				img = LuiPool.GetRawImage();
				if (png != null)
					img.png = png;
				if (color != null)
					img.color = color;
				luiComponents.Add(img.type, img);
			}
			return this;
		}

		public LuiContainer SetSteamIcon(string steamid, string color = null)
		{
			if (luiComponents.TryGetValue<LuiRawImageComp>(LuiCompType.RawImage, out var img))
			{
				img.steamid = steamid;
				if (color != null)
					img.color = color;
			}
			else
			{
				img = LuiPool.GetRawImage();
				img.steamid = steamid;
				if (color != null)
					img.color = color;
				luiComponents.Add(img.type, img);
			}
			return this;
		}

		public LuiContainer SetRawSprite(string sprite, string color = null)
		{
			if (luiComponents.TryGetValue<LuiRawImageComp>(LuiCompType.RawImage, out var img))
			{
				img.sprite = sprite;
				if (color != null)
					img.color = color;
			}
			else
			{
				img = LuiPool.GetRawImage();
				img.sprite = sprite;
				if (color != null)
					img.color = color;
				luiComponents.Add(img.type, img);
			}
			return this;
		}

		public LuiContainer SetRawMaterial(string material, string color = null)
		{
			if (luiComponents.TryGetValue<LuiRawImageComp>(LuiCompType.RawImage, out var img))
			{
				img.material = material;
				if (color != null)
					img.color = color;
			}
			else
			{
				img = LuiPool.GetRawImage();
				img.material = material;
				if (color != null)
					img.color = color;
				luiComponents.Add(img.type, img);
			}
			return this;
		}

		#endregion

		#region Container Methods - LuiButtonComp

		public LuiContainer SetButton(string command = null, string color = null)
		{
			if (luiComponents.TryGetValue<LuiButtonComp>(LuiCompType.Button, out var button))
			{
				if (command != null)
					button.command = command;
				if (color != null)
					button.color = color;
			}
			else
			{
				button = LuiPool.GetButton();
				if (command != null)
					button.command = command;
				if (color != null)
					button.color = color;
				luiComponents.Add(button.type, button);
			}
			return this;
		}

		public LuiContainer SetButtonColors(string color = null, string normalColor = null, string highlightedColor = null, string pressedColor = null, string selectedColor = null, string disabledColor = null, float colorMultiplier = -1, float fadeDuration = -1)
		{
			if (luiComponents.TryGetValue<LuiButtonComp>(LuiCompType.Button, out var button))
			{
				if (color != null)
					button.color = color;
				if (normalColor != null)
					button.normalColor = normalColor;
				if (highlightedColor != null)
					button.highlightedColor = highlightedColor;
				if (pressedColor != null)
					button.pressedColor = pressedColor;
				if (selectedColor != null)
					button.selectedColor = selectedColor;
				if (disabledColor != null)
					button.disabledColor = disabledColor;
				if (colorMultiplier != -1)
					button.colorMultiplier = colorMultiplier;
				if (fadeDuration != -1)
					button.fadeDuration = fadeDuration;
			}
			else
			{
				button = LuiPool.GetButton();
				if (color != null)
					button.color = color;
				if (normalColor != null)
					button.normalColor = normalColor;
				if (highlightedColor != null)
					button.highlightedColor = highlightedColor;
				if (pressedColor != null)
					button.pressedColor = pressedColor;
				if (selectedColor != null)
					button.selectedColor = selectedColor;
				if (disabledColor != null)
					button.disabledColor = disabledColor;
				if (colorMultiplier != -1)
					button.colorMultiplier = colorMultiplier;
				if (fadeDuration != -1)
					button.fadeDuration = fadeDuration;
				luiComponents.Add(button.type, button);
			}
			return this;
		}

		public LuiContainer SetButtonMaterial(string material)
		{
			if (luiComponents.TryGetValue<LuiButtonComp>(LuiCompType.Button, out var button))
			{
				button.material = material;
			}
			else
			{
				button = LuiPool.GetButton();
				button.material = material;
				luiComponents.Add(button.type, button);
			}
			return this;
		}

		public LuiContainer SetButtonSprite(string sprite,  UnityEngine.UI.Image.Type imageType = Image.Type.Simple)
		{
			if (luiComponents.TryGetValue<LuiButtonComp>(LuiCompType.Button, out var button))
			{
				button.sprite = sprite;
				button.imageType = GetImageType(imageType);
			}
			else
			{
				button = LuiPool.GetButton();
				button.sprite = sprite;
				button.imageType = GetImageType(imageType);
				luiComponents.Add(button.type, button);
			}
			return this;
		}

		public LuiContainer SetButtonClose(string close)
		{
			if (luiComponents.TryGetValue<LuiButtonComp>(LuiCompType.Button, out var button))
			{
				button.close = close;
			}
			else
			{
				button = LuiPool.GetButton();
				button.close = close;
				luiComponents.Add(button.type, button);
			}
			return this;
		}

		#endregion

		#region Container Methods - LuiOutlineComp

		public LuiContainer SetOutline(string color, Vector2 distance, bool useGraphicAlpha = false)
		{
			if (luiComponents.TryGetValue<LuiOutlineComp>(LuiCompType.Outline, out var outline))
			{
				outline.color = color;
				outline.distance = distance;
				outline.useGraphicAlpha = useGraphicAlpha;
			}
			else
			{
				outline = LuiPool.GetOutline();
				outline.color = color;
				outline.distance = distance;
				outline.useGraphicAlpha = useGraphicAlpha;
				luiComponents.Add(outline.type, outline);
			}
			return this;
		}

		#endregion

		#region Container Methods - LuiInputComp

		public LuiContainer SetInput(string color = null, string text = null, int fontSize = 0, string command = null, int charLimit = 0, CUI.Handler.FontTypes font = CUI.Handler.FontTypes.RobotoCondensedBold, TextAnchor alignment = TextAnchor.MiddleCenter, bool update = false)
		{
			if (luiComponents.TryGetValue<LuiInputComp>(LuiCompType.InputField, out var input))
			{
				if (color != null)
					input.color = color;
				if (text != null)
					input.text = text;
				if (fontSize > 0)
					input.fontSize = fontSize;
				if (command != null)
					input.command = command;
				if (charLimit > 0)
					input.characterLimit = charLimit;
				if (!update)
				{
					input.align = GetAlign(alignment);
					input.font = GetFont(font);
				}
			}
			else
			{
				input = LuiPool.GetInput();
				if (color != null)
					input.color = color;
				if (text != null)
					input.text = text;
				if (fontSize > 0)
					input.fontSize = fontSize;
				if (command != null)
					input.command = command;
				if (charLimit > 0)
					input.characterLimit = charLimit;
				if (!update)
				{
					input.align = GetAlign(alignment);
					input.font = GetFont(font);
				}
				luiComponents.Add(input.type, input);
			}
			return this;
		}

		public LuiContainer SetInputReadOnly(bool readOnly)
		{
			if (luiComponents.TryGetValue<LuiInputComp>(LuiCompType.InputField, out var input))
			{
				input.readOnly = readOnly;
			}
			else
			{
				input = LuiPool.GetInput();
				input.readOnly = readOnly;
				luiComponents.Add(input.type, input);
			}
			return this;
		}

		public LuiContainer SetInputPassword(bool password)
		{
			if (luiComponents.TryGetValue<LuiInputComp>(LuiCompType.InputField, out var input))
			{
				input.password = password;
			}
			else
			{
				input = LuiPool.GetInput();
				input.password = password;
				luiComponents.Add(input.type, input);
			}
			return this;
		}

		public LuiContainer SetInputAutoFocus(bool autofocus)
		{
			if (luiComponents.TryGetValue<LuiInputComp>(LuiCompType.InputField, out var input))
			{
				input.autofocus = autofocus;
			}
			else
			{
				input = LuiPool.GetInput();
				input.autofocus = autofocus;
				luiComponents.Add(input.type, input);
			}
			return this;
		}

		public LuiContainer SetInputKeyboard(bool needsKeyboard = false, bool hudMenuInput = false)
		{
			if (luiComponents.TryGetValue<LuiInputComp>(LuiCompType.InputField, out var input))
			{
				input.needsKeyboard = needsKeyboard;
				input.hudMenuInput = hudMenuInput;
			}
			else
			{
				input = LuiPool.GetInput();
				input.needsKeyboard = needsKeyboard;
				input.hudMenuInput = hudMenuInput;
				luiComponents.Add(input.type, input);
			}
			return this;
		}

		public LuiContainer SetInputLineType(InputField.LineType lineType)
		{
			if (luiComponents.TryGetValue<LuiInputComp>(LuiCompType.InputField, out var input))
			{
				input.lineType = GetLineType(lineType);
			}
			else
			{
				input = LuiPool.GetInput();
				input.lineType = GetLineType(lineType);
				luiComponents.Add(input.type, input);
			}
			return this;
		}

		public LuiContainer SetInputPlaceholder(string placeholderId)
		{
			if (luiComponents.TryGetValue<LuiInputComp>(LuiCompType.InputField, out var input))
			{
				input.placeholderId = placeholderId;
			}
			else
			{
				input = LuiPool.GetInput();
				input.placeholderId = placeholderId;
				luiComponents.Add(input.type, input);
			}
			return this;
		}

		#endregion

		#region Container Methods - LuiCursorComp

		public LuiContainer AddCursor()
		{
			if (!luiComponents.TryGetValue<LuiCursorComp>(LuiCompType.Button, out var cursor))
			{
				cursor = LuiPool.GetCursor();
				luiComponents.Add(cursor.type, cursor);
			}
			return this;
		}

		#endregion

		#region Container Methods - LuiRectTransformComp

		public LuiContainer SetAnchors(LuiPosition pos)
		{
			if (luiComponents.TryGetValue<LuiRectTransformComp>(LuiCompType.RectTransform, out var rect))
			{
				rect.anchor = pos;
			}
			else
			{
				rect = LuiPool.GetRect();
				rect.anchor = pos;
				luiComponents.Add(rect.type, rect);
			}
			return this;
		}

		public LuiContainer SetOffset(LuiOffset off)
		{
			if (luiComponents.TryGetValue<LuiRectTransformComp>(LuiCompType.RectTransform, out var rect))
			{
				rect.offset = off;
			}
			else
			{
				rect = LuiPool.GetRect();
				rect.offset = off;
				luiComponents.Add(rect.type, rect);
			}
			return this;
		}

		public LuiContainer SetRotation(float rotation)
		{
			if (luiComponents.TryGetValue<LuiRectTransformComp>(LuiCompType.RectTransform, out var rect))
			{
				rect.rotation = rotation;
			}
			else
			{
				rect = LuiPool.GetRect();
				rect.rotation = rotation;
				luiComponents.Add(rect.type, rect);
			}
			return this;
		}

		public LuiContainer SetAnchorAndOffset(LuiPosition pos, LuiOffset off)
		{
			if (luiComponents.TryGetValue<LuiRectTransformComp>(LuiCompType.RectTransform, out var rect))
			{
				rect.anchor = pos;
				rect.offset = off;
			}
			else
			{
				rect = LuiPool.GetRect();
				rect.anchor = pos;
				rect.offset = off;
				luiComponents.Add(rect.type, rect);
			}
			return this;
		}

		public LuiContainer SetRectParent(string setParent)
		{
			if (luiComponents.TryGetValue<LuiRectTransformComp>(LuiCompType.RectTransform, out var rect))
			{
				rect.setParent = setParent;
			}
			else
			{
				rect = LuiPool.GetRect();
				rect.setParent = setParent;
				luiComponents.Add(rect.type, rect);
			}
			return this;
		}

		public LuiContainer SetRectIndex(int setTransformIndex)
		{
			if (luiComponents.TryGetValue<LuiRectTransformComp>(LuiCompType.RectTransform, out var rect))
			{
				rect.setTransformIndex = setTransformIndex;
			}
			else
			{
				rect = LuiPool.GetRect();
				rect.setTransformIndex = setTransformIndex;
				luiComponents.Add(rect.type, rect);
			}
			return this;
		}

		#endregion

		#region Container Methods - LuiCountdownComp

		public LuiContainer SetCountdown(float startTime, float endTime, float step = 1, float interval = 1, string command = null, string numberFormat = null)
		{
			if (luiComponents.TryGetValue<LuiCountdownComp>(LuiCompType.Countdown, out var countdown))
			{
				countdown.startTime = startTime;
				countdown.endTime = endTime;
				if (step != 1)
					countdown.step = step;
				if (interval != 1)
					countdown.interval = interval;
				if (command != null)
					countdown.command = command;
				if (numberFormat != null)
					countdown.numberFormat = numberFormat;
			}
			else
			{
				countdown = LuiPool.GetCountdown();
				countdown.startTime = startTime;
				countdown.endTime = endTime;
				if (step != 1)
					countdown.step = step;
				if (interval != 1)
					countdown.interval = interval;
				if (command != null)
					countdown.command = command;
				if (numberFormat != null)
					countdown.numberFormat = numberFormat;
				luiComponents.Add(countdown.type, countdown);
			}
			return this;
		}

		public LuiContainer SetCountdownDestroy(bool destroy)
		{
			if (luiComponents.TryGetValue<LuiCountdownComp>(LuiCompType.Countdown, out var countdown))
			{
				countdown.destroyIfDone = destroy;
			}
			else
			{
				countdown = LuiPool.GetCountdown();
				countdown.destroyIfDone = destroy;
				luiComponents.Add(countdown.type, countdown);
			}
			return this;
		}

		public LuiContainer SetCountdownTimerFormat(TimerFormat format)
		{
			if (luiComponents.TryGetValue<LuiCountdownComp>(LuiCompType.Countdown, out var countdown))
			{
				countdown.timerFormat = GetTimerFormat(format);
			}
			else
			{
				countdown = LuiPool.GetCountdown();
				countdown.timerFormat = GetTimerFormat(format);
				luiComponents.Add(countdown.type, countdown);
			}
			return this;
		}

		#endregion

		#region Container Methods - LuiHorizontalLayoutGroupComp

		public LuiContainer SetHorizontalLayoutSpacing(float spacing)
		{
			if (luiComponents.TryGetValue<LuiHorizontalLayoutGroupComp>(LuiCompType.HorizontalLayoutGroup, out var layoutGroup))
			{
				layoutGroup.spacing = spacing;
			}
			else
			{
				layoutGroup = LuiPool.GetHorizontalLayoutGroup();
				layoutGroup.spacing = spacing;
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetHorizontalLayoutAlignment(TextAnchor anchor)
		{
			if (luiComponents.TryGetValue<LuiHorizontalLayoutGroupComp>(LuiCompType.HorizontalLayoutGroup, out var layoutGroup))
			{
				layoutGroup.childAlignment = GetAlign(anchor);
			}
			else
			{
				layoutGroup = LuiPool.GetHorizontalLayoutGroup();
				layoutGroup.childAlignment = GetAlign(anchor);
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetHorizontalLayoutForceExpand(bool width, bool height)
		{
			if (luiComponents.TryGetValue<LuiHorizontalLayoutGroupComp>(LuiCompType.HorizontalLayoutGroup, out var layoutGroup))
			{
				layoutGroup.childForceExpandWidth = width;
				layoutGroup.childForceExpandHeight = height;
			}
			else
			{
				layoutGroup = LuiPool.GetHorizontalLayoutGroup();
				layoutGroup.childForceExpandWidth = width;
				layoutGroup.childForceExpandHeight = height;
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetHorizontalLayoutControl(bool width, bool height)
		{
			if (luiComponents.TryGetValue<LuiHorizontalLayoutGroupComp>(LuiCompType.HorizontalLayoutGroup, out var layoutGroup))
			{
				layoutGroup.childControlWidth = width;
				layoutGroup.childControlHeight = height;
			}
			else
			{
				layoutGroup = LuiPool.GetHorizontalLayoutGroup();
				layoutGroup.childControlWidth = width;
				layoutGroup.childControlHeight = height;
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetHorizontalLayoutScale(bool width, bool height)
		{
			if (luiComponents.TryGetValue<LuiHorizontalLayoutGroupComp>(LuiCompType.HorizontalLayoutGroup, out var layoutGroup))
			{
				layoutGroup.childScaleWidth = width;
				layoutGroup.childScaleHeight = height;
			}
			else
			{
				layoutGroup = LuiPool.GetHorizontalLayoutGroup();
				layoutGroup.childScaleWidth = width;
				layoutGroup.childScaleHeight = height;
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetHorizontalLayoutPadding(string padding)
		{
			if (luiComponents.TryGetValue<LuiHorizontalLayoutGroupComp>(LuiCompType.HorizontalLayoutGroup, out var layoutGroup))
			{
				layoutGroup.padding = padding;
			}
			else
			{
				layoutGroup = LuiPool.GetHorizontalLayoutGroup();
				layoutGroup.padding = padding;
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		#endregion


		#region Container Methods - LuiVerticalLayoutGroupComp

		public LuiContainer SetVerticalLayoutSpacing(float spacing)
		{
			if (luiComponents.TryGetValue<LuiVerticalLayoutGroupComp>(LuiCompType.VerticalLayoutGroup, out var layoutGroup))
			{
				layoutGroup.spacing = spacing;
			}
			else
			{
				layoutGroup = LuiPool.GetVerticalLayoutGroup();
				layoutGroup.spacing = spacing;
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetVerticalLayoutAlignment(TextAnchor anchor)
		{
			if (luiComponents.TryGetValue<LuiVerticalLayoutGroupComp>(LuiCompType.VerticalLayoutGroup, out var layoutGroup))
			{
				layoutGroup.childAlignment = GetAlign(anchor);
			}
			else
			{
				layoutGroup = LuiPool.GetVerticalLayoutGroup();
				layoutGroup.childAlignment = GetAlign(anchor);
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetVerticalLayoutForceExpand(bool width, bool height)
		{
			if (luiComponents.TryGetValue<LuiVerticalLayoutGroupComp>(LuiCompType.VerticalLayoutGroup, out var layoutGroup))
			{
				layoutGroup.childForceExpandWidth = width;
				layoutGroup.childForceExpandHeight = height;
			}
			else
			{
				layoutGroup = LuiPool.GetVerticalLayoutGroup();
				layoutGroup.childForceExpandWidth = width;
				layoutGroup.childForceExpandHeight = height;
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetVerticalLayoutControl(bool width, bool height)
		{
			if (luiComponents.TryGetValue<LuiVerticalLayoutGroupComp>(LuiCompType.VerticalLayoutGroup, out var layoutGroup))
			{
				layoutGroup.childControlWidth = width;
				layoutGroup.childControlHeight = height;
			}
			else
			{
				layoutGroup = LuiPool.GetVerticalLayoutGroup();
				layoutGroup.childControlWidth = width;
				layoutGroup.childControlHeight = height;
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetVerticalLayoutScale(bool width, bool height)
		{
			if (luiComponents.TryGetValue<LuiVerticalLayoutGroupComp>(LuiCompType.VerticalLayoutGroup, out var layoutGroup))
			{
				layoutGroup.childScaleWidth = width;
				layoutGroup.childScaleHeight = height;
			}
			else
			{
				layoutGroup = LuiPool.GetVerticalLayoutGroup();
				layoutGroup.childScaleWidth = width;
				layoutGroup.childScaleHeight = height;
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetVerticalLayoutPadding(string padding)
		{
			if (luiComponents.TryGetValue<LuiVerticalLayoutGroupComp>(LuiCompType.VerticalLayoutGroup, out var layoutGroup))
			{
				layoutGroup.padding = padding;
			}
			else
			{
				layoutGroup = LuiPool.GetVerticalLayoutGroup();
				layoutGroup.padding = padding;
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		#endregion

		#region Container Methods - LuiGridLayoutGroupComp

		public LuiContainer SetCellSize(Vector2 size)
		{
			if (luiComponents.TryGetValue<LuiGridLayoutGroupComp>(LuiCompType.GridLayoutGroup, out var layoutGroup))
			{
				layoutGroup.cellSize = size;
			}
			else
			{
				layoutGroup = LuiPool.GetGridLayoutGroup();
				layoutGroup.cellSize = size;
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetCellSpacing(Vector2 spacing)
		{
			if (luiComponents.TryGetValue<LuiGridLayoutGroupComp>(LuiCompType.GridLayoutGroup, out var layoutGroup))
			{
				layoutGroup.spacing = spacing;
			}
			else
			{
				layoutGroup = LuiPool.GetGridLayoutGroup();
				layoutGroup.spacing = spacing;
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetStartCorner(GridLayoutGroup.Corner corner)
		{
			if (luiComponents.TryGetValue<LuiGridLayoutGroupComp>(LuiCompType.GridLayoutGroup, out var layoutGroup))
			{
				layoutGroup.startCorner = GetCorner(corner);
			}
			else
			{
				layoutGroup = LuiPool.GetGridLayoutGroup();
				layoutGroup.startCorner = GetCorner(corner);
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetStartAxis(GridLayoutGroup.Axis axis)
		{
			if (luiComponents.TryGetValue<LuiGridLayoutGroupComp>(LuiCompType.GridLayoutGroup, out var layoutGroup))
			{
				layoutGroup.startAxis = GetAxis(axis);
			}
			else
			{
				layoutGroup = LuiPool.GetGridLayoutGroup();
				layoutGroup.startAxis = GetAxis(axis);
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetChildAlign(TextAnchor align)
		{
			if (luiComponents.TryGetValue<LuiGridLayoutGroupComp>(LuiCompType.GridLayoutGroup, out var layoutGroup))
			{
				layoutGroup.childAlignment = GetAlign(align);
			}
			else
			{
				layoutGroup = LuiPool.GetGridLayoutGroup();
				layoutGroup.childAlignment = GetAlign(align);
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetContraint(GridLayoutGroup.Constraint constraint)
		{
			if (luiComponents.TryGetValue<LuiGridLayoutGroupComp>(LuiCompType.GridLayoutGroup, out var layoutGroup))
			{
				layoutGroup.constraint = GetConstraint(constraint);
			}
			else
			{
				layoutGroup = LuiPool.GetGridLayoutGroup();
				layoutGroup.constraint = GetConstraint(constraint);
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetContraintCount(int count)
		{
			if (luiComponents.TryGetValue<LuiGridLayoutGroupComp>(LuiCompType.GridLayoutGroup, out var layoutGroup))
			{
				layoutGroup.constraintCount = count;
			}
			else
			{
				layoutGroup = LuiPool.GetGridLayoutGroup();
				layoutGroup.constraintCount = count;
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		public LuiContainer SetGridLayoutPadding(string padding)
		{
			if (luiComponents.TryGetValue<LuiGridLayoutGroupComp>(LuiCompType.GridLayoutGroup, out var layoutGroup))
			{
				layoutGroup.padding = padding;
			}
			else
			{
				layoutGroup = LuiPool.GetGridLayoutGroup();
				layoutGroup.padding = padding;
				luiComponents.Add(layoutGroup.type, layoutGroup);
			}
			return this;
		}

		#endregion

		#region Container Methods - LuiContentSizeFitterComp

		public LuiContainer SetFitMode(ContentSizeFitter.FitMode horizontalFit, ContentSizeFitter.FitMode verticalFit)
		{
			if (luiComponents.TryGetValue<LuiContentSizeFitterComp>(LuiCompType.ContentSizeFitter, out var fitterComp))
			{
				fitterComp.horizontalFit = GetFitMode(horizontalFit);
				fitterComp.verticalFit = GetFitMode(verticalFit);
			}
			else
			{
				fitterComp = LuiPool.GetContentSizeFitter();
				fitterComp.horizontalFit = GetFitMode(horizontalFit);
				fitterComp.verticalFit = GetFitMode(verticalFit);
				luiComponents.Add(fitterComp.type, fitterComp);
			}
			return this;
		}

		#endregion

		#region Container Methods - LuiLayoutElementComp

		public LuiContainer SetPrefferedSize(float width = -1f, float height = -1f)
		{
			if (luiComponents.TryGetValue<LuiLayoutElementComp>(LuiCompType.LayoutElement, out var layoutComp))
			{
				if (width != -1f)
					layoutComp.preferredWidth = width;
				if (height != -1f)
					layoutComp.preferredHeight = height;
			}
			else
			{
				layoutComp = LuiPool.GetLayoutElement();
				if (width != -1f)
					layoutComp.preferredWidth = width;
				if (height != -1f)
					layoutComp.preferredHeight = height;
				luiComponents.Add(layoutComp.type, layoutComp);
			}
			return this;
		}

		public LuiContainer SetMinimalSize(float width = -1f, float height = -1f)
		{
			if (luiComponents.TryGetValue<LuiLayoutElementComp>(LuiCompType.LayoutElement, out var layoutComp))
			{
				if (width != -1f)
					layoutComp.minWidth = width;
				if (height != -1f)
					layoutComp.minHeight = height;
			}
			else
			{
				layoutComp = LuiPool.GetLayoutElement();
				if (width != -1f)
					layoutComp.minWidth = width;
				if (height != -1f)
					layoutComp.minHeight = height;
				luiComponents.Add(layoutComp.type, layoutComp);
			}
			return this;
		}

		public LuiContainer SetFlexible(float width = -1f, float height = -1f)
		{
			if (luiComponents.TryGetValue<LuiLayoutElementComp>(LuiCompType.LayoutElement, out var layoutComp))
			{
				if (width != -1f)
					layoutComp.flexibleWidth = width;
				if (height != -1f)
					layoutComp.flexibleHeight = height;
			}
			else
			{
				layoutComp = LuiPool.GetLayoutElement();
				if (width != -1f)
					layoutComp.flexibleWidth = width;
				if (height != -1f)
					layoutComp.flexibleHeight = height;
				luiComponents.Add(layoutComp.type, layoutComp);
			}
			return this;
		}

		public LuiContainer SetIgnoreLayout(bool ignore)
		{
			if (luiComponents.TryGetValue<LuiLayoutElementComp>(LuiCompType.LayoutElement, out var layoutComp))
			{
				layoutComp.ignoreLayout = ignore;
			}
			else
			{
				layoutComp = LuiPool.GetLayoutElement();
				layoutComp.ignoreLayout = ignore;
				luiComponents.Add(layoutComp.type, layoutComp);
			}
			return this;
		}

		#endregion

		#region Container Methods - LuiDraggableComp

		public LuiContainer SetDraggable(string filter = null, bool dropAnywhere = true, bool keepOnTop = false, bool limitToParent = false, float maxDistance = -1f, bool allowSwapping = false)
		{
			if (luiComponents.TryGetValue<LuiDraggableComp>(LuiCompType.Draggable, out var drag))
			{
				if (filter != null)
					drag.filter = filter;
				if (drag.maxDistance != -1)
					drag.maxDistance = maxDistance;
				drag.dropAnywhere = dropAnywhere;
				drag.keepOnTop = keepOnTop;
				drag.limitToParent = limitToParent;
				drag.allowSwapping = allowSwapping;
			}
			else
			{
				drag = LuiPool.GetDraggable();
				if (filter != null)
					drag.filter = filter;
				if (drag.maxDistance != -1)
					drag.maxDistance = maxDistance;
				drag.dropAnywhere = dropAnywhere;
				drag.keepOnTop = keepOnTop;
				drag.limitToParent = limitToParent;
				drag.allowSwapping = allowSwapping;
				luiComponents.Add(drag.type, drag);
			}
			return this;
		}

		public LuiContainer SetDragAlpha(float alpha)
		{
			if (luiComponents.TryGetValue<LuiDraggableComp>(LuiCompType.Draggable, out var drag))
			{
				drag.dragAlpha = alpha;
			}
			else
			{
				drag = LuiPool.GetDraggable();
				drag.dragAlpha = alpha;
				luiComponents.Add(drag.type, drag);
			}
			return this;
		}

		public LuiContainer SetParentLimitIndex(int index)
		{
			if (luiComponents.TryGetValue<LuiDraggableComp>(LuiCompType.Draggable, out var drag))
			{
				drag.parentLimitIndex = index;
			}
			else
			{
				drag = LuiPool.GetDraggable();
				drag.parentLimitIndex = index;
				luiComponents.Add(drag.type, drag);
			}
			return this;
		}

		public LuiContainer SetDraggableParentPadding(Vector2 padding)
		{
			if (luiComponents.TryGetValue<LuiDraggableComp>(LuiCompType.Draggable, out var drag))
			{
				drag.parentPadding = padding;
			}
			else
			{
				drag = LuiPool.GetDraggable();
				drag.parentPadding = padding;
				luiComponents.Add(drag.type, drag);
			}
			return this;
		}

		public LuiContainer SetDraggableAnchorOffset(Vector2 offset)
		{
			if (luiComponents.TryGetValue<LuiDraggableComp>(LuiCompType.Draggable, out var drag))
			{
				drag.anchorOffset = offset;
			}
			else
			{
				drag = LuiPool.GetDraggable();
				drag.anchorOffset = offset;
				luiComponents.Add(drag.type, drag);
			}
			return this;
		}

		public LuiContainer SetDraggableRPC(CommunityEntity.DraggablePositionSendType posSendType)
		{
			if (luiComponents.TryGetValue<LuiDraggableComp>(LuiCompType.Draggable, out var drag))
			{
				drag.positionRPC = GetSendType(posSendType);
			}
			else
			{
				drag = LuiPool.GetDraggable();
				drag.positionRPC = GetSendType(posSendType);
				luiComponents.Add(drag.type, drag);
			}
			return this;
		}

		#endregion

		#region Container Methods - LuiSlotComp

		public LuiContainer SetSlot(string filter = null)
		{
			if (luiComponents.TryGetValue<LuiSlotComp>(LuiCompType.Slot, out var slot))
			{
				if (filter != null)
					slot.filter = filter;
			}
			else
			{
				slot = LuiPool.GetSlot();
				if (filter != null)
					slot.filter = filter;
				luiComponents.Add(slot.type, slot);
			}
			return this;
		}

		#endregion

		#region Container Methods - LuiKeyboardComp

		public LuiContainer AddKeyboard()
		{
			if (!luiComponents.TryGetValue<LuiKeyboardComp>(LuiCompType.Button, out var keyboard))
			{
				keyboard = LuiPool.GetKeyboard();
				luiComponents.Add(keyboard.type, keyboard);
			}
			return this;
		}

		#endregion

		#region Container Methods - LuiScrollComp

		public LuiContainer SetScrollView(bool vertical, bool horizontal, ScrollRect.MovementType movementType = ScrollRect.MovementType.Clamped, float elasticity = 0, bool inertia = false, float decelerationRate = 0, float scrollSensitivity = 0, LuiScrollbar verticalScrollOptions = default, LuiScrollbar horizontalScrollOptions = default, bool update = false)
		{
			if (luiComponents.TryGetValue<LuiScrollComp>(LuiCompType.ScrollView, out var scroll))
			{
				if (!update)
				{
					scroll.vertical = vertical;
					scroll.horizontal = horizontal;
					scroll.movementType = GetMovementType(movementType);
					scroll.inertia = inertia;
				}
				if (elasticity != 0)
					scroll.elasticity = elasticity;
				if (decelerationRate != 0)
					scroll.decelerationRate = decelerationRate;
				if (scrollSensitivity != 0)
					scroll.scrollSensitivity = scrollSensitivity;
				scroll.verticalScrollbar = verticalScrollOptions;
				scroll.horizontalScrollbar = horizontalScrollOptions;
			}
			else
			{
				scroll = LuiPool.GetScroll();
				if (!update)
				{
					scroll.vertical = vertical;
					scroll.horizontal = horizontal;
					scroll.movementType = GetMovementType(movementType);
					scroll.inertia = inertia;
				}
				if (elasticity != 0)
					scroll.elasticity = elasticity;
				if (decelerationRate != 0)
					scroll.decelerationRate = decelerationRate;
				if (scrollSensitivity != 0)
					scroll.scrollSensitivity = scrollSensitivity;
				scroll.verticalScrollbar = verticalScrollOptions;
				scroll.horizontalScrollbar = horizontalScrollOptions;
				luiComponents.Add(scroll.type, scroll);
			}
			return this;
		}

		public LuiContainer SetScrollContent(LuiPosition pos, LuiOffset offset)
		{
			if (luiComponents.TryGetValue<LuiScrollComp>(LuiCompType.ScrollView, out var scroll))
			{
				scroll.anchor = pos;
				scroll.offset = offset;
			}
			else
			{
				scroll = LuiPool.GetScroll();
				scroll.anchor = pos;
				scroll.offset = offset;
				luiComponents.Add(scroll.type, scroll);
			}
			return this;
		}

		public LuiContainer SetScrollPivot(Vector2 pivot)
		{
			if (luiComponents.TryGetValue<LuiScrollComp>(LuiCompType.ScrollView, out var scroll))
			{
				scroll.pivot = pivot;
			}
			else
			{
				scroll = LuiPool.GetScroll();
				scroll.pivot = pivot;
				luiComponents.Add(scroll.type, scroll);
			}
			return this;
		}

		public LuiContainer SetScrollbarPosition(float horizontal = 0, float vertical = 0)
		{
			if (luiComponents.TryGetValue<LuiScrollComp>(LuiCompType.ScrollView, out var scroll))
			{
				if (horizontal != 0)
					scroll.horizontalNormalizedPosition = horizontal;
				if (vertical != 0)
					scroll.verticalNormalizedPosition = vertical;
			}
			else
			{
				scroll = LuiPool.GetScroll();
				if (horizontal != 0)
					scroll.horizontalNormalizedPosition = horizontal;
				if (vertical != 0)
					scroll.verticalNormalizedPosition = vertical;
				luiComponents.Add(scroll.type, scroll);
			}
			return this;
		}

		#endregion

	}
}

public static class LuiColors
{
	public const string Transparent = "0.0 0.0 0.0 0.0";
	public const string White = "1.0 1.0 1.0 1.0";
	public const string Gray = "0.5 0.5 0.5 1.0";
	public const string Black = "0.0 0.0 0.0 1.0";
	public const string Red = "1.0 0.0 0.0 1.0";
	public const string Green = "0.0 1.0 0.0 1.0";
	public const string Blue = "0.0 0.0 1.0 1.0";
}

public class LuiComponentDictionary : IEnumerable
{
	private readonly LuiCompBase[] _values;
	private int _count;
	private const int DictionarySize = 10; //Dictionary based on most possible types of components in one element, at the date od 24.02.2025 it's 8 (including draggables), so adding 2 more for safety.

	public LuiComponentDictionary()
	{
		_values = new LuiCompBase[DictionarySize];
		_count = 0;
	}

	public int Count => _count;

	public void Add<T>(LuiCompType key, T value) where T : LuiCompBase
	{
		if (_count >= _values.Length)
			throw new InvalidOperationException("Dictionary is full");

		_values[_count] = value;
		_count++;
	}

	public void Clear()
	{
		_count = 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool TryGetValue<T>(LuiCompType key, out T value) where T : LuiCompBase
	{
		for (int i = 0; i < _count; i++)
		{
			if (_values[i].type == key && _values[i] is T typedValue)
			{
				value = typedValue;
				return true;
			}
		}
		value = null;
		return false;
	}

	public IEnumerator GetEnumerator()
	{
		for (int i = 0; i < _count; i++)
		{
			yield return _values[i];
		}
	}
}

public readonly struct LuiOffset
{
	public static readonly LuiOffset None = new(0, 0, 0, 0);

	public readonly Vector2 offsetMin;
	public readonly Vector2 offsetMax;

	public LuiOffset(float xMin, float yMin, float xMax, float yMax)
	{
		offsetMin = new Vector2(xMin, yMin);
		offsetMax = new Vector2(xMax, yMax);
	}

	public static bool operator ==(LuiOffset a, LuiOffset b)
	{
		return a.offsetMax == b.offsetMax && a.offsetMin == b.offsetMin;
	}

	public static bool operator !=(LuiOffset a, LuiOffset b)
	{
		return a.offsetMax != b.offsetMax || a.offsetMin != b.offsetMin;
	}

	public override bool Equals(object obj)
	{
		return obj is LuiOffset other && Equals(other);
	}

	private bool Equals(LuiOffset other)
	{
		return offsetMin.Equals(other.offsetMin) && offsetMax.Equals(other.offsetMax);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;
			hash = hash * 31 + offsetMin.GetHashCode();
			hash = hash * 31 + offsetMax.GetHashCode();
			return hash;
		}
	}
}

public readonly struct LuiPosition
{
	public static readonly LuiPosition None = new(0, 0, 0, 0);
	public static readonly LuiPosition Full = new(0, 0, 1, 1);
	public static readonly LuiPosition UpperLeft = new(0, 1, 0, 1);
	public static readonly LuiPosition UpperCenter = new(.5f, 1, .5f, 1);
	public static readonly LuiPosition UpperRight = new(1, 1, 1, 1);
	public static readonly LuiPosition MiddleLeft = new(0, .5f, 0, .5f);
	public static readonly LuiPosition MiddleCenter = new(.5f, .5f, .5f, .5f);
	public static readonly LuiPosition MiddleRight = new(1, .5f, 1, .5f);
	public static readonly LuiPosition LowerLeft = new(0, 0, 0, 0);
	public static readonly LuiPosition LowerCenter = new(.5f, 0, .5f, 0);
	public static readonly LuiPosition LowerRight = new(1, 0, 1, 0);

	public readonly Vector2 anchorMin;
	public readonly Vector2 anchorMax;

	public LuiPosition(float xMin, float yMin, float xMax, float yMax)
	{
		anchorMin = new Vector2(xMin, yMin);
		anchorMax = new Vector2(xMax, yMax);
	}

	public static bool operator ==(LuiPosition a, LuiPosition b)
	{
		return a.anchorMax == b.anchorMax && a.anchorMin == b.anchorMin;
	}

	public static bool operator !=(LuiPosition a, LuiPosition b)
	{
		return a.anchorMax != b.anchorMax || a.anchorMin != b.anchorMin;
	}

	public override bool Equals(object obj)
	{
		return obj is LuiPosition other && Equals(other);
	}

	private bool Equals(LuiPosition other)
	{
		return anchorMax.Equals(other.anchorMax) && anchorMin.Equals(other.anchorMin);
	}

	public override int GetHashCode()
	{
		unchecked
		{
			int hash = 17;
			hash = hash * 31 + anchorMax.GetHashCode();
			hash = hash * 31 + anchorMin.GetHashCode();
			return hash;
		}
	}
}

public enum LuiCompType
{
	Text,
	Image,
	RawImage,
	Button,
	Outline,
	InputField,
	NeedsCursor,
	RectTransform,
	Countdown,
	HorizontalLayoutGroup,
	VerticalLayoutGroup,
	GridLayoutGroup,
	ContentSizeFitter,
	LayoutElement,
	Draggable,
	Slot,
	NeedsKeyboard,
	ScrollView,
}

public class LuiCompBase
{
	public LuiCompType type;
	public bool enabled = true;
	public float fadeIn; //Present in like 80% of elements but to reduce method list, adding it here.
	public string placeholderParentId; //Present in like 80% of elements but to reduce method list, adding it here.
}

public class LuiTextComp : LuiCompBase
{
	public string text;
	public int fontSize;
	public string font;
	public string align;
	public string color;
	public string verticalOverflow;

	public LuiTextComp()
	{
		type = LuiCompType.Text;
	}
}

public class LuiImageComp : LuiCompBase
{
	public string sprite;
	public string material;
	public string color;
	public string imageType;
	public bool fillCenter;
	public string png;
	public string slice;
	public int itemid;
	public ulong skinid;

	public LuiImageComp()
	{
		type = LuiCompType.Image;
	}
}

public class LuiRawImageComp : LuiCompBase
{
	public string sprite;
	public string color;
	public string material;
	public string url;
	public string png;
	public string steamid;

	public LuiRawImageComp()
	{
		type = LuiCompType.RawImage;
	}
}

public class LuiButtonComp : LuiCompBase
{
	public string command;
	public string close;
	public string sprite;
	public string material;
	public string color;
	public string imageType;
	public string normalColor;
	public string highlightedColor;
	public string pressedColor;
	public string selectedColor;
	public string disabledColor;
	public float colorMultiplier = -1;
	public float fadeDuration = -1;

	public LuiButtonComp()
	{
		type = LuiCompType.Button;
	}
}

public class LuiOutlineComp : LuiCompBase
{
	public string color;
	public Vector2 distance;
	public bool useGraphicAlpha;

	public LuiOutlineComp()
	{
		type = LuiCompType.Outline;
	}
}

public class LuiInputComp : LuiCompBase
{
	public int fontSize;
	public string font;
	public string align;
	public string color;
	public int characterLimit;
	public string command;
	public string lineType;
	public string text;
	public bool readOnly;
	public string placeholderId;
	public bool password;
	public bool needsKeyboard;
	public bool hudMenuInput;
	public bool autofocus;

	public LuiInputComp()
	{
		type = LuiCompType.InputField;
	}
}

public class LuiCursorComp : LuiCompBase
{
	public LuiCursorComp()
	{
		type = LuiCompType.NeedsCursor;
	}
}

public class LuiRectTransformComp : LuiCompBase
{
	public LuiPosition anchor = LuiPosition.Full;
	public LuiOffset offset = LuiOffset.None;
	public float rotation;
	public string setParent;
	public int setTransformIndex = -1;

	public LuiRectTransformComp()
	{
		type = LuiCompType.RectTransform;
	}
}

public class LuiCountdownComp : LuiCompBase
{
	public float endTime = -1;
	public float startTime = -1;
	public float step;
	public float interval;
	public string timerFormat;
	public string numberFormat;
	public bool destroyIfDone = true;
	public string command;

	public LuiCountdownComp()
	{
		type = LuiCompType.Countdown;
	}
}

public class LuiHorizontalLayoutGroupComp : LuiCompBase
{
	public float spacing;
	public string childAlignment;
	public bool childForceExpandWidth = true;
	public bool childForceExpandHeight = true;
	public bool childControlWidth;
	public bool childControlHeight;
	public bool childScaleWidth;
	public bool childScaleHeight;
	public string padding;

	public LuiHorizontalLayoutGroupComp()
	{
		type = LuiCompType.HorizontalLayoutGroup;
	}
}

public class LuiVerticalLayoutGroupComp : LuiCompBase
{
	public float spacing;
	public string childAlignment;
	public bool childForceExpandWidth = true;
	public bool childForceExpandHeight = true;
	public bool childControlWidth;
	public bool childControlHeight;
	public bool childScaleWidth;
	public bool childScaleHeight;
	public string padding;

	public LuiVerticalLayoutGroupComp()
	{
		type = LuiCompType.VerticalLayoutGroup;
	}
}

public class LuiGridLayoutGroupComp : LuiCompBase
{
	public Vector2 cellSize = new Vector2(100, 100);
	public Vector2 spacing;
	public string startCorner;
	public string startAxis;
	public string childAlignment;
	public string constraint;
	public int constraintCount;
	public string padding;

	public LuiGridLayoutGroupComp()
	{
		type = LuiCompType.GridLayoutGroup;
	}
}

public class LuiContentSizeFitterComp : LuiCompBase
{
	public string horizontalFit;
	public string verticalFit;

	public LuiContentSizeFitterComp()
	{
		type = LuiCompType.ContentSizeFitter;
	}
}

public class LuiLayoutElementComp : LuiCompBase
{
	public float preferredWidth = -1f;
	public float preferredHeight = -1f;
	public float minWidth;
	public float minHeight;
	public float flexibleWidth;
	public float flexibleHeight;
	public bool ignoreLayout;

	public LuiLayoutElementComp()
	{
		type = LuiCompType.LayoutElement;
	}
}

public class LuiDraggableComp : LuiCompBase
{
	public bool limitToParent;
	public float maxDistance;
	public bool allowSwapping;
	public bool dropAnywhere = true;
	public float dragAlpha = -1;
	public int parentLimitIndex = -1;
	public string filter;
	public Vector2 parentPadding;
	public Vector2 anchorOffset;
	public bool keepOnTop;
	public string positionRPC;
	public bool moveToAnchor;
	public bool rebuildAnchor;

	public LuiDraggableComp()
	{
		type = LuiCompType.Draggable;
	}
}

public class LuiSlotComp : LuiCompBase
{
	public string filter;

	public LuiSlotComp()
	{
		type = LuiCompType.Slot;
	}
}

public class LuiKeyboardComp : LuiCompBase
{
	public LuiKeyboardComp()
	{
		type = LuiCompType.NeedsKeyboard;
	}
}

public class LuiScrollComp : LuiCompBase
{
	public LuiPosition anchor = LuiPosition.Full;
	public LuiOffset offset = LuiOffset.None;
	public Vector2 pivot = new Vector2(0.5f, 0.5f);
	public bool horizontal;
	public bool vertical;
	public string movementType;
	public float elasticity = -1;
	public bool inertia;
	public float decelerationRate = -1;
	public float scrollSensitivity = -1;
	public LuiScrollbar horizontalScrollbar;
	public LuiScrollbar verticalScrollbar;
	public float horizontalNormalizedPosition;
	public float verticalNormalizedPosition;


	public LuiScrollComp()
	{
		type = LuiCompType.ScrollView;
	}
}

public struct LuiScrollbar
{
	public bool disabled; //reverse of enabled
	public bool invert;
	public bool autoHide;
	public string handleSprite;
	public float size;
	public string handleColor;
	public string highlightColor;
	public string pressedColor;
	public string trackSprite;
	public string trackColor;
}

//Uncomment when draggables will be out and there won't be anything like that in CUI.
/*public enum DraggablePositionSendType
{
	NormalizedScreen = 0,
	NormalizedParent = 1,
	Relative = 2,
	RelativeAnchor = 3,
}*/

//Currently it relies on Oxide CUI enum, uncomment if it shouldn't.
/*public enum TimerFormat
{
	None,
	SecondsHundreth,
	MinutesSeconds,
	MinutesSecondsHundreth,
	HoursMinutes,
	HoursMinutesSeconds,
	HoursMinutesSecondsMilliseconds,
	HoursMinutesSecondsTenths,
	DaysHoursMinutes,
	DaysHoursMinutesSeconds,
	Custom
}*/

public static class LuiPool
{
	private static readonly Stack<LUI.LuiContainer> _containers = new();
	private static readonly Stack<LuiCompBase> _texts  = new();
	private static readonly Stack<LuiCompBase> _images  = new();
	private static readonly Stack<LuiCompBase> _rawImages  = new();
	private static readonly Stack<LuiCompBase> _buttons  = new();
	private static readonly Stack<LuiCompBase> _outlines = new();
	private static readonly Stack<LuiCompBase> _inputs  = new();
	private static readonly Stack<LuiCompBase> _cursors  = new();
	private static readonly Stack<LuiCompBase> _rects = new();
	private static readonly Stack<LuiCompBase> _countdowns = new();
	private static readonly Stack<LuiCompBase> _horizontals = new();
	private static readonly Stack<LuiCompBase> _verticals = new();
	private static readonly Stack<LuiCompBase> _grids = new();
	private static readonly Stack<LuiCompBase> _fitters = new();
	private static readonly Stack<LuiCompBase> _layouts = new();
	private static readonly Stack<LuiCompBase> _draggables = new();
	private static readonly Stack<LuiCompBase> _slots = new();
	private static readonly Stack<LuiCompBase> _keyboards = new();
	private static readonly Stack<LuiCompBase> _scrolls = new();

	public static int poolElements => _containers.Count + _texts.Count + _images.Count + _rawImages.Count + _buttons.Count + _outlines.Count + _inputs.Count + _cursors.Count + _rects.Count + _countdowns.Count + _draggables.Count + _slots.Count + _keyboards.Count + _scrolls.Count;

	public static void ReturnComp<T>(T component) where T : LuiCompBase
	{
		if (component == null) return;

		switch (component.type)
		{
			case LuiCompType.Text: _texts.Push(component); break;
			case LuiCompType.Image: _images.Push(component); break;
			case LuiCompType.RawImage: _rawImages.Push(component); break;
			case LuiCompType.Button: _buttons.Push(component); break;
			case LuiCompType.Outline: _outlines.Push(component); break;
			case LuiCompType.InputField: _inputs.Push(component); break;
			case LuiCompType.NeedsCursor: _cursors.Push(component); break;
			case LuiCompType.RectTransform: _rects.Push(component); break;
			case LuiCompType.Countdown: _countdowns.Push(component); break;
			case LuiCompType.Draggable: _draggables.Push(component); break;
			case LuiCompType.Slot: _slots.Push(component); break;
			case LuiCompType.NeedsKeyboard: _keyboards.Push(component); break;
			case LuiCompType.ScrollView: _scrolls.Push(component); break;
		}
	}

	public static LuiTextComp GetText()
	{
		if (_texts.Count == 0)
			return new();

		LuiTextComp comp = _texts.Pop() as LuiTextComp;
		comp.enabled = true;
		comp.text = null;
		comp.fontSize = 0;
		comp.font = null;
		comp.align = null;
		comp.color = null;
		comp.verticalOverflow = null;
		comp.fadeIn = 0;
		comp.placeholderParentId = null;
		return comp;
	}

	public static LuiImageComp GetImage()
	{
		if (_images.Count == 0)
			return new();

		LuiImageComp comp = _images.Pop() as LuiImageComp;
		comp.enabled = true;
		comp.sprite = null;
		comp.material = null;
		comp.color = null;
		comp.imageType = null;
		comp.fillCenter = false;
		comp.slice = null;
		comp.png = null;
		comp.itemid = 0;
		comp.skinid = 0;
		comp.fadeIn = 0;
		comp.placeholderParentId = null;
		return comp;
	}

	public static LuiRawImageComp GetRawImage()
	{
		if (_rawImages.Count == 0)
			return new();

		LuiRawImageComp comp = _rawImages.Pop() as LuiRawImageComp;
		comp.enabled = true;
		comp.sprite = null;
		comp.color = null;
		comp.material = null;
		comp.url = null;
		comp.png = null;
		comp.steamid = null;
		comp.fadeIn = 0;
		comp.placeholderParentId = null;
		return comp;
	}

	public static LuiButtonComp GetButton()
	{
		if (_buttons.Count == 0)
			return new();

		LuiButtonComp comp = _buttons.Pop() as LuiButtonComp;
		comp.enabled = true;
		comp.command = null;
		comp.close = null;
		comp.sprite = null;
		comp.material = null;
		comp.color = null;
		comp.imageType = null;
		comp.normalColor = null;
		comp.highlightedColor = null;
		comp.pressedColor = null;
		comp.selectedColor = null;
		comp.disabledColor = null;
		comp.colorMultiplier = -1;
		comp.fadeDuration = -1;
		comp.fadeIn = 0;
		comp.placeholderParentId = null;
		return comp;
	}

	public static LuiOutlineComp GetOutline()
	{
		if (_outlines.Count == 0)
			return new();

		LuiOutlineComp comp = _outlines.Pop() as LuiOutlineComp;
		comp.enabled = true;
		comp.color = null;
		comp.distance = default;
		comp.useGraphicAlpha = false;

		return comp;
	}

	public static LuiInputComp GetInput()
	{
		if (_inputs.Count == 0)
			return new();

		LuiInputComp comp = _inputs.Pop() as LuiInputComp;
		comp.enabled = true;
		comp.fontSize = 0;
		comp.font = null;
		comp.align = null;
		comp.color = null;
		comp.characterLimit = 0;
		comp.command = null;
		comp.text= null;
		comp.readOnly = false;
		comp.placeholderId = null;
		comp.lineType = null;
		comp.password = false;
		comp.needsKeyboard = false;
		comp.hudMenuInput = false;
		comp.autofocus = false;
		comp.fadeIn = 0;
		comp.placeholderParentId = null;
		return comp;
	}

	public static LuiCursorComp GetCursor()
	{
		if (_cursors.Count == 0)
			return new();

		LuiCursorComp comp = _cursors.Pop() as LuiCursorComp;
		comp.enabled = true;

		return comp;
	}

	public static LuiRectTransformComp GetRect()
	{
		if (_rects.Count == 0)
			return new();

		LuiRectTransformComp comp = _rects.Pop() as LuiRectTransformComp;
		comp.anchor = LuiPosition.Full;
		comp.offset = LuiOffset.None;
		comp.rotation = 0;
		comp.setParent = null;
		comp.setTransformIndex = -1;
		return comp;
	}

	public static LuiCountdownComp GetCountdown()
	{
		if (_countdowns.Count == 0)
			return new();

		LuiCountdownComp comp = _countdowns.Pop() as LuiCountdownComp;
		comp.enabled = true;
		comp.endTime = -1;
		comp.startTime = -1;
		comp.step = 0;
		comp.interval = 0;
		comp.timerFormat = null;
		comp.numberFormat = null;
		comp.destroyIfDone = true;
		comp.command = null;
		return comp;
	}

	public static LuiHorizontalLayoutGroupComp GetHorizontalLayoutGroup()
	{
		if (_horizontals.Count == 0)
			return new();

		LuiHorizontalLayoutGroupComp comp = _horizontals.Pop() as LuiHorizontalLayoutGroupComp;
		comp.spacing = 0;
		comp.childAlignment = null;
		comp.childForceExpandWidth = true;
		comp.childForceExpandWidth = true;
		comp.childControlWidth = false;
		comp.childControlHeight = false;
		comp.childScaleWidth = false;
		comp.childScaleHeight = false;
		comp.padding = null;
		return comp;
	}

	public static LuiVerticalLayoutGroupComp GetVerticalLayoutGroup()
	{
		if (_verticals.Count == 0)
			return new();

		LuiVerticalLayoutGroupComp comp = _verticals.Pop() as LuiVerticalLayoutGroupComp;
		comp.spacing = 0;
		comp.childAlignment = null;
		comp.childForceExpandWidth = true;
		comp.childForceExpandWidth = true;
		comp.childControlWidth = false;
		comp.childControlHeight = false;
		comp.childScaleWidth = false;
		comp.childScaleHeight = false;
		comp.padding = null;
		return comp;
	}

	public static LuiGridLayoutGroupComp GetGridLayoutGroup()
	{
		if (_grids.Count == 0)
			return new();

		LuiGridLayoutGroupComp comp = _grids.Pop() as LuiGridLayoutGroupComp;
		comp.cellSize = new Vector2(100, 100);
		comp.spacing = default;
		comp.startCorner = null;
		comp.startAxis = null;
		comp.childAlignment = null;
		comp.constraint = null;
		comp.constraintCount = 0;
		comp.padding = null;
		return comp;
	}

	public static LuiContentSizeFitterComp GetContentSizeFitter()
	{
		if (_fitters.Count == 0)
			return new();

		LuiContentSizeFitterComp comp = _fitters.Pop() as LuiContentSizeFitterComp;
		comp.horizontalFit = null;
		comp.verticalFit = null;
		return comp;
	}

	public static LuiLayoutElementComp GetLayoutElement()
	{
		if (_layouts.Count == 0)
			return new();

		LuiLayoutElementComp comp = _layouts.Pop() as LuiLayoutElementComp;
		comp.preferredWidth = -1f;
		comp.preferredHeight = -1f;
		comp.minWidth = 0;
		comp.minHeight = 0;
		comp.flexibleWidth = 0;
		comp.flexibleHeight = 0;
		comp.ignoreLayout = false;
		return comp;
	}

	public static LuiDraggableComp GetDraggable()
	{
		if (_draggables.Count == 0)
			return new();

		LuiDraggableComp comp = _draggables.Pop() as LuiDraggableComp;
		comp.enabled = true;
		comp.limitToParent = false;
		comp.maxDistance = 0;
		comp.allowSwapping = false;
		comp.dropAnywhere = true;
		comp.dragAlpha = -1;
		comp.parentLimitIndex = -1;
		comp.filter = null;
		comp.parentPadding = default;
		comp.anchorOffset = default;
		comp.keepOnTop = false;
		comp.positionRPC = null;
		comp.moveToAnchor = false;
		comp.rebuildAnchor = false;
		return comp;
	}

	public static LuiSlotComp GetSlot()
	{
		if (_slots.Count == 0)
			return new();

		LuiSlotComp comp = _slots.Pop() as LuiSlotComp;
		comp.enabled = true;
		comp.filter = null;
		return comp;
	}

	public static LuiKeyboardComp GetKeyboard()
	{
		if (_keyboards.Count == 0)
			return new();

		LuiKeyboardComp comp = _keyboards.Pop() as LuiKeyboardComp;
		comp.enabled = true;
		return comp;
	}

	public static LuiScrollComp GetScroll()
	{
		if (_scrolls.Count == 0)
			return new();

		LuiScrollComp comp = _scrolls.Pop() as LuiScrollComp;
		comp.enabled = true;
		comp.anchor = LuiPosition.Full;
		comp.offset = LuiOffset.None;
		comp.pivot = new Vector2(0.5f, 0.5f);
		comp.horizontal = false;
		comp.vertical = false;
		comp.movementType = null;
		comp.elasticity = -1;
		comp.inertia = false;
		comp.decelerationRate = -1;
		comp.scrollSensitivity = -1;
		comp.horizontalScrollbar = default;
		comp.verticalScrollbar = default;
		comp.horizontalNormalizedPosition = 0;
		comp.verticalNormalizedPosition = 0;
		return comp;
	}

	public static LuiCompType GetLuiCompType(Type type)
	{
		return type switch
		{
			not null when type == typeof(LuiTextComp) => LuiCompType.Text,
			not null when type == typeof(LuiImageComp) => LuiCompType.Image,
			not null when type == typeof(LuiRawImageComp) => LuiCompType.RawImage,
			not null when type == typeof(LuiButtonComp) => LuiCompType.Button,
			not null when type == typeof(LuiOutlineComp) => LuiCompType.Outline,
			not null when type == typeof(LuiInputComp) => LuiCompType.InputField,
			not null when type == typeof(LuiCursorComp) => LuiCompType.NeedsCursor,
			not null when type == typeof(LuiRectTransformComp) => LuiCompType.RectTransform,
			not null when type == typeof(LuiCountdownComp) => LuiCompType.Countdown,
			not null when type == typeof(LuiDraggableComp) => LuiCompType.Draggable,
			not null when type == typeof(LuiSlotComp) => LuiCompType.Slot,
			not null when type == typeof(LuiKeyboardComp) => LuiCompType.NeedsKeyboard,
			not null when type == typeof(LuiScrollComp) => LuiCompType.ScrollView,
			_ => LuiCompType.Image
		};
	}

	public static T GetLuiCompFromPool<T>(Type type) where T : LuiCompBase
	{
		return type switch
		{
			not null when type == typeof(LuiTextComp) => LuiPool.GetText() as T,
			not null when type == typeof(LuiImageComp) => LuiPool.GetImage() as T,
			not null when type == typeof(LuiRawImageComp) => LuiPool.GetRawImage() as T,
			not null when type == typeof(LuiButtonComp) => LuiPool.GetButton() as T,
			not null when type == typeof(LuiOutlineComp) => LuiPool.GetOutline() as T,
			not null when type == typeof(LuiInputComp) => LuiPool.GetInput() as T,
			not null when type == typeof(LuiCursorComp) => LuiPool.GetCursor() as T,
			not null when type == typeof(LuiRectTransformComp) => LuiPool.GetRect() as T,
			not null when type == typeof(LuiCountdownComp) => LuiPool.GetCountdown() as T,
			not null when type == typeof(LuiDraggableComp) => LuiPool.GetDraggable() as T,
			not null when type == typeof(LuiSlotComp) => LuiPool.GetSlot() as T,
			not null when type == typeof(LuiKeyboardComp) => LuiPool.GetKeyboard() as T,
			not null when type == typeof(LuiScrollComp) => LuiPool.GetScroll() as T,
			_ => LuiPool.GetImage() as T,
		};
	}

	public static LUI.LuiContainer GetContainer()
	{
		if (_containers.Count == 0)
		{
			LUI.LuiContainer cont = new LUI.LuiContainer();
			cont.name = null;
			cont.parent = null;
			cont.luiComponents = new();
			cont.destroyUi = null;
			cont.fadeOut = 0;
			cont.update = false;
			cont.activeSelf = true;
			return cont;
		}
		else
		{
			LUI.LuiContainer cont = _containers.Pop();
			cont.name = null;
			cont.parent = null;
			cont.luiComponents.Clear();
			cont.destroyUi = null;
			cont.fadeOut = 0;
			cont.update = false;
			return cont;
		}
	}

	public static void ReturnContainer(LUI.LuiContainer container) => _containers.Push(container);
}
