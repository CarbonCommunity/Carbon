using Network;
using Oxide.Game.Rust.Cui;
using UnityEngine.UI;
using static Carbon.Components.CUI;
using Net = Network.Net;

namespace Carbon.Components;

/// <summary>
/// Carbon's built-in pooled CUI cache-based structure.
/// </summary>
public readonly struct CUI : IDisposable
{
	public Handler Manager { get; }

	public LUI v2 { get; }

	public ImageDatabaseModule ImageDatabase { get; }
	public Handler.Cache CacheInstance => Manager.CacheInstance;

	/// <summary>
	/// All currently available client-side UI panels.
	/// </summary>
	public enum ClientPanels
	{
		Overall,
		Overlay,
		OverlayNonScaled,
		Hud,
		HudMenu,
		Under,
		UnderNonScaled,
		Inventory,
		TechTree,
		Crafting,
		Contacts,
		Clans,
		Map

	}

	/// <summary>
	/// Gets the Rust-identifiable client-side panel name.
	/// </summary>
	public string GetClientPanel(ClientPanels panel)
	{
		return panel switch
		{
			ClientPanels.Overall => "Overall",
			ClientPanels.OverlayNonScaled => "OverlayNonScaled",
			ClientPanels.Hud => "Hud",
			ClientPanels.HudMenu => "Hud.Menu",
			ClientPanels.Under => "Under",
			ClientPanels.UnderNonScaled => "UnderNonScaled",
			ClientPanels.Inventory => "Inventory",
			ClientPanels.TechTree => "TechTree",
			ClientPanels.Crafting => "Crafting",
			ClientPanels.Contacts => "Contacts",
			ClientPanels.Clans => "Clans",
			ClientPanels.Map => "Map",
			_ => "Overlay",
		};
	}

	public CUI(Handler manager)
	{
		Manager = manager;
		v2 = new LUI(this);
		ImageDatabase = BaseModule.GetModule<ImageDatabaseModule>();
	}

	#region Update

	/// <summary>
	/// Gets a disposable Update pool used to update existent Carbon elements already drawn onto the client.
	/// </summary>
	public Handler.UpdatePool UpdatePool()
	{
		return new Handler.UpdatePool();
	}

    #endregion

    #region Methods
    public CuiElementContainer CreateContainer(string panel, string color = "0 0 0 0", float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, ClientPanels parent = ClientPanels.Overlay, string destroyUi = null, bool activeSelf = true)
    {
        return CreateContainerParent(panel, color, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, GetClientPanel(parent), destroyUi, activeSelf);
    }
    public CuiElementContainer CreateContainerParent(string panel, string color = "0 0 0 0", float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string parentName = "Overlay", string destroyUi = null, bool activeSelf = true)
	{
		var container = Manager.TakeFromPoolContainer();
		container.Name = panel;

		var element = Manager.TakeFromPool(panel, parentName);
		element.FadeOut = fadeOut;
		element.DestroyUi = destroyUi;
		element.ActiveSelf = activeSelf;

		if (!string.IsNullOrEmpty(color) && color != "0 0 0 0")
		{
			var image = Manager.TakeFromPoolImage();
			image.Color = color;
			image.FadeIn = fadeIn;
			element.Components.Add(image);
		}

		var rect = Manager.TakeFromPoolRect();
		rect.AnchorMin = LUIBuilder.GetStringFloat(xMin, yMin);
		rect.AnchorMax = LUIBuilder.GetStringFloat(xMax, yMax);
		rect.OffsetMin = LUIBuilder.GetStringFloat(OxMin, OyMin);
		rect.OffsetMax = LUIBuilder.GetStringFloat(OxMax, OyMax);
		element.Components.Add(rect);

		if (needsCursor) element.Components.Add(Manager.TakeFromPoolNeedsCursor());
		if (needsKeyboard) element.Components.Add(Manager.TakeFromPoolNeedsKeyboard());

		container.Add(element);
		container.Add(Manager.TakeFromPool(Manager.AppendId(), parentName));
		return container;
	}
	public Pair<string, CuiElement> CreatePanel(CuiElementContainer container, string parent, string color, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, bool blur = false, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		return Manager.Panel(container, parent, color, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, blur, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, update, activeSelf);
	}
	public Pair<string, CuiElement> CreateText(CuiElementContainer container, string parent, string color, string text, int size, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, TextAnchor align = TextAnchor.MiddleCenter, Handler.FontTypes font = Handler.FontTypes.RobotoCondensedRegular, VerticalWrapMode verticalOverflow = VerticalWrapMode.Overflow, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		return Manager.Text(container, parent, color, text, size, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, align, font, verticalOverflow, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, update, activeSelf);
	}
	public Pair<string, CuiElement, CuiElement> CreateButton(CuiElementContainer container, string parent, string color, string textColor, string text, int size, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, string command = null, TextAnchor align = TextAnchor.MiddleCenter, Handler.FontTypes font = Handler.FontTypes.RobotoCondensedRegular, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		return Manager.Button(container, parent, color, textColor, text, size, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, command, align, font, false, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, update, activeSelf);
	}
	public Pair<string, CuiElement, CuiElement> CreateProtectedButton(CuiElementContainer container, string parent, string color, string textColor, string text, int size, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, string command = null, TextAnchor align = TextAnchor.MiddleCenter, Handler.FontTypes font = Handler.FontTypes.RobotoCondensedRegular, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		return Manager.Button(container, parent, color, textColor, text, size, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, command, align, font, true, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, update, activeSelf);
	}
	public Pair<string, CuiElement> CreateInputField(CuiElementContainer container, string parent, string color, string text, int size, int characterLimit, bool readOnly, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, string command = null, TextAnchor align = TextAnchor.MiddleCenter, Handler.FontTypes font = Handler.FontTypes.RobotoCondensedRegular, bool autoFocus = false, bool hudMenuInput = false, InputField.LineType lineType = InputField.LineType.SingleLine, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		return Manager.InputField(container, parent, color, text, size, characterLimit, readOnly, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, command, align, font, false, autoFocus, hudMenuInput, lineType, fadeIn, fadeOut, needsCursor, needsKeyboard, id, destroyUi, update, activeSelf);
	}
	public Pair<string, CuiElement> CreateProtectedInputField(CuiElementContainer container, string parent, string color, string text, int size, int characterLimit, bool readOnly, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, string command = null, TextAnchor align = TextAnchor.MiddleCenter, Handler.FontTypes font = Handler.FontTypes.RobotoCondensedRegular, bool autoFocus = false, bool hudMenuInput = false, InputField.LineType lineType = InputField.LineType.SingleLine, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		return Manager.InputField(container, parent, color, text, size, characterLimit, readOnly, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, command, align, font, true, autoFocus, hudMenuInput, lineType, fadeIn, fadeOut, needsCursor, needsKeyboard, id, destroyUi, update, activeSelf);
	}
	public Pair<string, CuiElement> CreateImage(CuiElementContainer container, string parent, uint png, string color, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		return Manager.Image(container, parent, png.ToString(), null, null, color, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, update, activeSelf);
	}
	public Pair<string, CuiElement> CreateImage(CuiElementContainer container, string parent, string url, string color, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		if (!HasImage(url))
		{
			return Manager.Panel(container, parent, color, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax,
				false, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance,
				outlineUseGraphicAlpha, id, destroyUi, update, activeSelf);
		}

		return Manager.Image(container, parent, GetImage(url), null, null, color, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, update, activeSelf);
	}
	public Pair<string, CuiElement> CreatePlayerImage(CuiElementContainer container, string parent, string steamId, string color, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		return Manager.Image(container, parent, null, null, steamId, color, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, update, activeSelf);
	}
	public Pair<string, CuiElement> CreateSimpleImage(CuiElementContainer container, string parent, string png, string sprite, string color, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		return Manager.SimpleImage(container, parent, png, sprite, color, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, update, activeSelf);
	}
	public Pair<string, CuiElement> CreateSprite(CuiElementContainer container, string parent, string sprite, string color, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		return Manager.Sprite(container, parent, sprite, color, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, update, activeSelf);
	}
	public Pair<string, CuiElement> CreateItemImage(CuiElementContainer container, string parent, int itemID, ulong skinID, string color, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		return Manager.ItemImage(container, parent, itemID, skinID, color, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, update, activeSelf);
	}
	public Pair<string, CuiElement> CreateQRCodeImage(CuiElementContainer container, string parent, string text, string brandUrl, string brandColor, string brandBgColor, int pixels, bool transparent, bool quietZones, string color, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		try
		{
			var codeImage = ImageDatabase.GetQRCode(text, pixels, transparent, quietZones, true);
			var qr = CreateImage(container, parent, codeImage, color, null, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin,
				OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance,
				outlineUseGraphicAlpha, id, destroyUi, update, activeSelf);
			if (!string.IsNullOrEmpty(brandUrl))
			{
				var panel = CreatePanel(container, qr, brandBgColor,
					xMin: 0.4f, xMax: 0.6f, yMin: 0.4f, yMax: 0.6f);

				CreateImage(container, panel, url: brandUrl, color: brandColor,
					material: null, xMin: 0.15f, 0.85f, yMin: 0.15f, yMax: 0.85f);
			}
			return qr;
		}
		catch (Exception)
		{
			var qr = CreatePanel(container, parent, Cache.CUI.WhiteColor, color, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin,
				OyMax, false, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance,
				outlineUseGraphicAlpha, id, destroyUi, update, activeSelf);
			CreateText(container, qr, "0 0 0 0.5", "GDI+ is not installed!", 10);
			return qr;
		}
	}
	public Pair<string, CuiElement> CreateClientImage(CuiElementContainer container, string parent, string url, string color, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		return Manager.Image(container, parent, null, url, null, color, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, update, activeSelf);
	}
	public Pair<string, CuiElement> CreateCountdown(CuiElementContainer container, string parent, int startTime, int endTime, int step, string command, float fadeIn = 0f, float fadeOut = 0f, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		return Manager.Countdown(container, parent, startTime, endTime, step, command, fadeIn, fadeOut, id, destroyUi, update, activeSelf);
	}
	public Pair<string, CuiElement> CreateScrollView(CuiElementContainer container, string parent,bool vertical, bool horizontal, ScrollRect.MovementType movementType, float elasticity, bool inertia, float decelerationRate, float scrollSensitivity, out CuiRectTransform contentTransformComponent, out CuiScrollbar horizontalScrollBar, out CuiScrollbar verticalScrollBar, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		return Manager.ScrollView(container, parent, vertical, horizontal, movementType, elasticity, inertia, decelerationRate, scrollSensitivity, out contentTransformComponent, out horizontalScrollBar, out verticalScrollBar, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, id, destroyUi, update, activeSelf);
	}

	public static string HexToRustColor(string hexColor, float? alpha = null)
	{
		if (!ColorUtility.TryParseHtmlString(hexColor, out var color))
		{
			return LUIBuilder.GetStringFloat(1, 1, 1, alpha.GetValueOrDefault(1));
		}
		return LUIBuilder.GetStringFloat(color.r, color.g, color.b, alpha ?? color.a);
	}
	public static string RustToHexColor(string rustColor, float? alpha = null, bool includeAlpha = true)
	{
		var colors = rustColor.Split(' ');
		var color = new Color(colors[0].ToFloat(), colors[1].ToFloat(), colors[2].ToFloat(), includeAlpha ? alpha ?? (colors.Length > 2 ? colors[3].ToFloat() : 1f) : 1);
		var result = includeAlpha ? ColorUtility.ToHtmlStringRGBA(color) : ColorUtility.ToHtmlStringRGB(color);
		Array.Clear(colors, 0, colors.Length);
		return $"#{result}";
	}

	#endregion

	#region ImageDatabase

	/// <summary>
	/// Gets an existent image based on a mapped key or full length URL from Carbon's ImageDatabase module.
	/// </summary>
	public string GetImage(string keyOrUrl)
	{
		return ImageDatabase.GetImageString(keyOrUrl);
	}

	/// <summary>
	/// Checks if an image is existent in Carbon's ImageDatabase module, using a key identifier or a full length URL.
	/// </summary>
	/// <param name="url"></param>
	/// <returns></returns>
	public bool HasImage(string url)
	{
		return ImageDatabase.HasImage(url);
	}

	/// <summary>
	/// Queues up various URLs to be downloaded and stored within Carbon's ImageDatabase module.
	/// </summary>
	public void QueueImages(IEnumerable<string> urls)
	{
		ImageDatabase.QueueBatch(false, urls);
	}

	/// <summary>
	/// Removes all stored images (if they exist) in the provided urls enumerable.
	/// </summary>
	public void ClearImages(IEnumerable<string> urls)
	{
		foreach (var url in urls)
		{
			ImageDatabase.DeleteImage(url);
		}
	}

	#endregion

	#region Send

	/// <summary>
	/// Serializes and networks the Carbon managed pooled container to a specified player.
	/// </summary>
	public void Send(CuiElementContainer container, BasePlayer player)
	{
		Manager.Send(container, player);
	}

	/// <summary>
	/// Destroys a container using its panel identifier from a specific player.
	/// </summary>
	public void Destroy(string name, BasePlayer player)
	{
		Manager.Destroy(name, player);
	}

	#endregion

	/// <summary>
	/// Pair of one sub-element.
	/// </summary>
	/// <typeparam name="T1">Key identifier.</typeparam>
	public struct Pair<T1, T2>
	{
		public T1 Id;
		public T2 Element;

		public Pair(T1 id, T2 element)
		{
			Id = id;
			Element = element;
		}

		public static implicit operator string(Pair<T1, T2> value)
		{
			return value.Id.ToString();
		}
	}

	/// <summary>
	/// Pair of two sub-elements.
	/// </summary>
	/// <typeparam name="T1">Key identifier.</typeparam>
	public struct Pair<T1, T2, T3>
	{
		public T1 Id;
		public T2 Element1;
		public T3 Element2;

		public Pair(T1 id, T2 element1, T3 element2)
		{
			Id = id;
			Element1 = element1;
			Element2 = element2;
		}

		public static implicit operator string(Pair<T1, T2, T3> value)
		{
			return value.Id.ToString();
		}
	}

	/// <summary>
	/// Disposes and sends all currently used CUI elements to pool, for further reuse.
	/// </summary>
	public void Dispose()
	{
		v2.Dispose();
		Manager.SendToPool();
	}

	public class Handler
	{
		internal string Identifier { get; set; } = RandomEx.GetRandomString(4);

		public Cache CacheInstance = new();
		public int Pooled => _containerPool.Count + _elements.Count + _images.Count + _rawImages.Count + _texts.Count + _buttons.Count + _inputFields.Count + _rects.Count + _needsCursors.Count + _needsKeyboards.Count;
		public int Used => _queue.Count;

		#region Properties

		private int _currentId { get; set; }

		private List<object> _queue = new();
		private List<CuiElementContainer> _containerPool = new();

		private List<CuiElement> _elements = new();
		private List<ICuiComponent> _images = new();
		private List<ICuiComponent> _rawImages = new();
		private List<ICuiComponent> _texts = new();
		private List<ICuiComponent> _buttons = new();
		private List<ICuiComponent> _inputFields = new();
		private List<ICuiComponent> _rects = new();
		private List<ICuiComponent> _needsCursors = new();
		private List<ICuiComponent> _needsKeyboards = new();
		private List<ICuiComponent> _countdowns = new();
		private List<ICuiComponent> _outlines = new();
		private List<ICuiComponent> _scrollViews = new();
		private List<ICuiComponent> _scrollbars = new();

		#endregion

		#region Default Instances

		private CuiImageComponent _defaultImage = new();
		private CuiRawImageComponent _defaultRawImage = new();
		private CuiRectTransformComponent DefaultRectTransformComponent = new()
		{
			OffsetMax = "0 0"
		};
		private CuiTextComponent _defaultText = new();
		private CuiButtonComponent _defaultButton = new();
		private CuiInputFieldComponent _defaultInputField = new();
		private CuiCountdownComponent _defaultCountdown = new();
		private CuiOutlineComponent _defaultOutline = new();
		private CuiScrollViewComponent _defaultScrollView = new();
		private CuiScrollbar _defaultScrollBar = new();

		#endregion

		#region Pooling

		public string AppendId()
		{
			_currentId++;
			return LUIBuilder.BuildElementId(Identifier, _currentId);
		}
		public void SendToPool<T>(T element) where T : ICuiComponent
		{
			if (element == null) return;

			switch (element)
			{
				case CuiImageComponent: _images.Add(element); break;
				case CuiRawImageComponent: _rawImages.Add(element); break;
				case CuiTextComponent: _texts.Add(element); break;
				case CuiButtonComponent: _buttons.Add(element); break;
				case CuiRectTransformComponent: _rects.Add(element); break;
				case CuiInputFieldComponent: _inputFields.Add(element); break;
				case CuiNeedsCursorComponent: _needsCursors.Add(element); break;
				case CuiNeedsKeyboardComponent: _needsKeyboards.Add(element); break;
				case CuiCountdownComponent: _countdowns.Add(element); break;
				case CuiOutlineComponent: _outlines.Add(element); break;
				case CuiScrollViewComponent scrollView:
				{
					SendToPool(scrollView.HorizontalScrollbar);
					SendToPool(scrollView.VerticalScrollbar);

					_scrollViews.Add(element);
					break;
				}
				case CuiScrollbar: _scrollbars.Add(element); break;
			}
		}
		public void SendToPool()
		{
			foreach (var entry in _queue)
			{
				if (entry is CuiElement element)
				{
					_elements.Add(element);
				}
				else if (entry is CuiElementContainer elementContainer)
				{
					_containerPool.Add(elementContainer);
				}
				else
				{
					SendToPool(entry as ICuiComponent);
				}
			}

			_queue.Clear();
			Identifier = RandomEx.GetRandomString(4);
			_currentId = 0;
		}

		#endregion

		#region Pooled Elements

		public CuiElement TakeFromPool(string name = null, string parent = "Hud", float fadeOut = 0f, string destroyUi = null, bool update = false, bool activeSelf = true)
		{
			var element = (CuiElement)null;

			if (_elements.Count == 0)
			{
				element = new CuiElement();
			}
			else
			{
				element = _elements[0];
				_elements.RemoveAt(0);
			}

			element.Name = name;
			element.Update = update;
			element.Parent = parent;
			element.Components.Clear();
			element.DestroyUi = destroyUi;
			element.FadeOut = fadeOut;
			element.ActiveSelf = activeSelf;

			_queue.Add(element);
			return element;
		}
		public CuiElementContainer TakeFromPoolContainer()
		{
			var element = (CuiElementContainer)null;

			if (_containerPool.Count == 0)
			{
				element = new CuiElementContainer();
			}
			else
			{
				element = _containerPool[0];
				element.Clear();

				_containerPool.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		public CuiImageComponent TakeFromPoolImage()
		{
			var element = (CuiImageComponent)null;

			if (_images.Count == 0)
			{
				element = new CuiImageComponent();
			}
			else
			{
				element = _images[0] as CuiImageComponent;
				element.Sprite = _defaultImage.Sprite;
				element.Material = _defaultImage.Material;
				element.Color = _defaultImage.Color;
				element.SkinId = _defaultImage.SkinId;
				element.ImageType = _defaultImage.ImageType;
				element.Png = _defaultImage.Png;
				element.FadeIn = _defaultImage.FadeIn;
				element.ItemId = _defaultImage.ItemId;
				element.SkinId = _defaultImage.SkinId;
				_images.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		public CuiRawImageComponent TakeFromPoolRawImage()
		{
			var element = (CuiRawImageComponent)null;

			if (_rawImages.Count == 0)
			{
				element = new CuiRawImageComponent();
			}
			else
			{
				element = _rawImages[0] as CuiRawImageComponent;
				element.Sprite = _defaultRawImage.Sprite;
				element.Color = _defaultRawImage.Color;
				element.Material = _defaultRawImage.Material;
				element.Url = _defaultRawImage.Url;
				element.Png = _defaultRawImage.Png;
				element.FadeIn = _defaultRawImage.FadeIn;
				_rawImages.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		public CuiRectTransformComponent TakeFromPoolRect()
		{
			var element = (CuiRectTransformComponent)null;

			if (_rects.Count == 0)
			{
				element = new CuiRectTransformComponent();
			}
			else
			{
				element = _rects[0] as CuiRectTransformComponent;
				element.AnchorMin = DefaultRectTransformComponent.AnchorMin;
				element.AnchorMax = DefaultRectTransformComponent.AnchorMax;
				element.OffsetMin = DefaultRectTransformComponent.OffsetMin;
				element.OffsetMax = DefaultRectTransformComponent.OffsetMax;
				_rects.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		public CuiTextComponent TakeFromPoolText()
		{
			var element = (CuiTextComponent)null;

			if (_texts.Count == 0)
			{
				element = new CuiTextComponent();
			}
			else
			{
				element = _texts[0] as CuiTextComponent;
				element.Text = _defaultText.Text;
				element.FontSize = _defaultText.FontSize;
				element.Font = _defaultText.Font;
				element.Align = _defaultText.Align;
				element.Color = _defaultText.Color;
				element.FadeIn = _defaultText.FadeIn;
				element.VerticalOverflow = _defaultText.VerticalOverflow;
				_texts.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		public CuiButtonComponent TakeFromPoolButton()
		{
			var element = (CuiButtonComponent)null;

			if (_buttons.Count == 0)
			{
				element = new CuiButtonComponent();
			}
			else
			{
				element = _buttons[0] as CuiButtonComponent;
				element.Command = _defaultButton.Command;
				element.Close = _defaultButton.Close;
				element.Sprite = _defaultButton.Sprite;
				element.Material = _defaultButton.Material;
				element.Color = _defaultButton.Color;
				element.ImageType = _defaultButton.ImageType;
				element.FadeIn = _defaultButton.FadeIn;
				_buttons.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		public CuiInputFieldComponent TakeFromPoolInputField()
		{
			var element = (CuiInputFieldComponent)null;

			if (_inputFields.Count == 0)
			{
				element = new CuiInputFieldComponent();
			}
			else
			{
				element = _inputFields[0] as CuiInputFieldComponent;
				element.Text = _defaultInputField.Text;
				element.FontSize = _defaultInputField.FontSize;
				element.Font = _defaultInputField.Font;
				element.Align = _defaultInputField.Align;
				element.Color = _defaultInputField.Color;
				element.CharsLimit = _defaultInputField.CharsLimit;
				element.Command = _defaultInputField.Command;
				element.IsPassword = _defaultInputField.IsPassword;
				element.ReadOnly = _defaultInputField.ReadOnly;
				element.NeedsKeyboard = _defaultInputField.NeedsKeyboard;
				element.LineType = _defaultInputField.LineType;
				element.Autofocus = _defaultInputField.Autofocus;
				element.HudMenuInput = _defaultInputField.HudMenuInput;
				_inputFields.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		public CuiNeedsCursorComponent TakeFromPoolNeedsCursor()
		{
			var element = (CuiNeedsCursorComponent)null;

			if (_needsCursors.Count == 0)
			{
				element = new CuiNeedsCursorComponent();
			}
			else
			{
				element = _needsCursors[0] as CuiNeedsCursorComponent;
				_needsCursors.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		public CuiNeedsKeyboardComponent TakeFromPoolNeedsKeyboard()
		{
			var element = (CuiNeedsKeyboardComponent)null;

			if (_needsKeyboards.Count == 0)
			{
				element = new CuiNeedsKeyboardComponent();
			}
			else
			{
				element = _needsKeyboards[0] as CuiNeedsKeyboardComponent;
				_needsKeyboards.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		public CuiCountdownComponent TakeFromPoolCountdown()
		{
			var element = (CuiCountdownComponent)null;

			if (_countdowns.Count == 0)
			{
				element = new CuiCountdownComponent();
			}
			else
			{
				element = _countdowns[0] as CuiCountdownComponent;
				element.EndTime = _defaultCountdown.EndTime;
				element.StartTime = _defaultCountdown.StartTime;
				element.Step = _defaultCountdown.Step;
				element.Command = _defaultCountdown.Command;
				element.FadeIn = _defaultCountdown.FadeIn;
				_countdowns.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		public CuiOutlineComponent TakeFromPoolOutline()
		{
			var element = (CuiOutlineComponent)null;

			if (_outlines.Count == 0)
			{
				element = new CuiOutlineComponent();
			}
			else
			{
				element = _outlines[0] as CuiOutlineComponent;
				element.Color = _defaultOutline.Color;
				element.Distance = _defaultOutline.Distance;
				element.UseGraphicAlpha = _defaultOutline.UseGraphicAlpha;
				_outlines.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		public CuiScrollViewComponent TakeFromPoolScrollView()
		{
			var element = (CuiScrollViewComponent)null;

			if (_scrollViews.Count == 0)
			{
				element = new CuiScrollViewComponent();
				element.ContentTransform ??= new();
				element.HorizontalScrollbar ??= TakeFromPoolScrollbar();
				element.VerticalScrollbar ??= TakeFromPoolScrollbar();
			}
			else
			{
				element = _scrollViews[0] as CuiScrollViewComponent;
				element.Vertical = _defaultScrollView.Vertical;
				element.Horizontal = _defaultScrollView.Horizontal;
				element.MovementType = _defaultScrollView.MovementType;
				element.Elasticity = _defaultScrollView.Elasticity;
				element.Inertia = _defaultScrollView.Inertia;
				element.DecelerationRate = _defaultScrollView.DecelerationRate;
				element.ScrollSensitivity = _defaultScrollView.ScrollSensitivity;
				element.ContentTransform ??= new();
				element.ContentTransform.AnchorMin = "0 0";
				element.ContentTransform.AnchorMax = "1 1";
				element.ContentTransform.OffsetMin = "0 0";
				element.ContentTransform.OffsetMax = "0 0";
				element.HorizontalScrollbar = TakeFromPoolScrollbar();
				element.VerticalScrollbar = TakeFromPoolScrollbar();

				_scrollViews.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		public CuiScrollbar TakeFromPoolScrollbar()
		{
			var element = (CuiScrollbar)null;

			if (_scrollbars.Count == 0)
			{
				element = new CuiScrollbar();
			}
			else
			{
				element = _scrollbars[0] as CuiScrollbar;
				element.Invert = _defaultScrollBar.Invert;
				element.AutoHide = _defaultScrollBar.AutoHide;
				element.HandleSprite = _defaultScrollBar.HandleSprite;
				element.Size = _defaultScrollBar.Size;
				element.HandleColor = _defaultScrollBar.HandleColor;
				element.HighlightColor = _defaultScrollBar.HighlightColor;
				element.PressedColor = _defaultScrollBar.PressedColor;
				element.TrackSprite = _defaultScrollBar.TrackSprite;
				element.TrackColor = _defaultScrollBar.TrackColor;

				_scrollbars.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}

		#endregion

		#region Classes

		public enum FontTypes
		{
			RobotoCondensedBold, RobotoCondensedRegular,
			PermanentMarker, DroidSansMono, NotoSansArabicBold
		}

		public string GetFont(FontTypes type)
		{
			return type switch
			{
				FontTypes.RobotoCondensedBold => "robotocondensed-bold.ttf",
				FontTypes.RobotoCondensedRegular => "robotocondensed-regular.ttf",
				FontTypes.PermanentMarker => "permanentmarker.ttf",
				FontTypes.DroidSansMono => "droidsansmono.ttf",
				FontTypes.NotoSansArabicBold => "NotoSansArabic-Bold.ttf",
				_ => "robotocondensed-regular.ttf"
			};
		}

		#endregion

		#region Network

		public void Send(CuiElementContainer container, BasePlayer player)
		{
			container.Send(player);
		}
		public void SendUpdate(Pair<string, CuiElement> pair, BasePlayer player)
		{
			pair.SendUpdate(player);
		}
		public void Destroy(CuiElementContainer container, BasePlayer player)
		{
			container.Destroy(player);
		}
		public void Destroy(string name, BasePlayer player)
		{
			CUIStatics.Destroy(name, player);
		}

		#endregion

		public class UpdatePool : CuiElementContainer, IDisposable
		{
			private bool _disposed;

			public void Add(Pair<string, CuiElement> pair)
			{
				if (pair.Element != null)
				{
					if (!pair.Element.Update)
					{
						Logger.Warn($"You're trying to update element '{pair.Element.Name}' (of parent '{pair.Element.Parent}') which doesn't allow updates. Ignoring.");
						return;
					}

					Add(pair.Element);
				}
			}
			public void Add(Pair<string, CuiElement, CuiElement> pair)
			{
				if (pair.Element1 != null)
				{
					if (!pair.Element1.Update)
					{
						Logger.Warn($"You're trying to update element '{pair.Element1.Name}' (of parent '{pair.Element1.Parent}') which doesn't allow updates. Ignoring.");
						return;
					}

					Add(pair.Element1);
				}

				if (pair.Element2 != null)
				{
					if (!pair.Element2.Update)
					{
						// Logger.Warn($"You're trying to update element '{pair.Element2.Name}' (of parent '{pair.Element2.Parent}') which doesn't allow updates. Ignoring.");
						return;
					}

					Add(pair.Element2);
				}
			}

			public void Send(BasePlayer player)
			{
				CUIStatics.Send(this, player);
			}

			public void Dispose()
			{
				if (_disposed) return;

				Clear();

				_disposed = true;
			}
		}

		public class Cache
		{
			internal Dictionary<string, byte[]> _cuiData = new();

			public bool TryStore(string id, CuiElementContainer container)
			{
				if (TryTake(id, out _))
				{
					return false;
				}

				_cuiData.Add(id, container.GetData());
				return true;
			}
			public bool TryTake(string id, out byte[] data)
			{
				data = default;

				if (_cuiData.TryGetValue(id, out var content))
				{
					data = content;
					return true;
				}

				return false;
			}
			public bool TrySend(string id, BasePlayer player)
			{
				if (!TryTake(id, out var data))
				{
					return false;
				}

				CUIStatics.SendData(data, player);
				return true;
			}
		}
	}
}

public static class CUIStatics
{
	internal static string ProcessColor(string color)
	{
		if (color.StartsWith("#")) return CUI.HexToRustColor(color);

		return color;
	}

	public static Pair<string, CuiElement> UpdatePanel(this CUI cui, string id, string color, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, bool blur = false, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string destroyUi = null, bool activeSelf = true)
	{
		return cui.CreatePanel(null, null, color, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, blur, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, true, activeSelf);
	}
	public static Pair<string, CuiElement> UpdateText(this CUI cui, string id, string color, string text, int size, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, TextAnchor align = TextAnchor.MiddleCenter, Handler.FontTypes font = Handler.FontTypes.RobotoCondensedRegular, VerticalWrapMode verticalOverflow = VerticalWrapMode.Overflow, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string destroyUi = null, bool activeSelf = true)
	{
		return cui.CreateText(null, null, color, text, size, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, align, font, verticalOverflow, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, true, activeSelf);
	}
	public static Pair<string, CuiElement, CuiElement> UpdateButton(this CUI cui, string id, string color, string textColor, string text, int size, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, string command = null, TextAnchor align = TextAnchor.MiddleCenter, Handler.FontTypes font = Handler.FontTypes.RobotoCondensedRegular, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string destroyUi = null, bool activeSelf = true)
	{
		return cui.CreateButton(null, null, color, textColor, text, size, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, command, align, font, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, true, activeSelf);
	}
	public static Pair<string, CuiElement, CuiElement> UpdateProtectedButton(this CUI cui, string id, string color, string textColor, string text, int size, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, string command = null, TextAnchor align = TextAnchor.MiddleCenter, Handler.FontTypes font = Handler.FontTypes.RobotoCondensedRegular, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string destroyUi = null, bool activeSelf = true)
	{
		return cui.CreateProtectedButton(null, null, color, textColor, text, size, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, command, align, font, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, true, activeSelf);
	}
	public static Pair<string, CuiElement> UpdateInputField(this CUI cui, string id, string color, string text, int size, int characterLimit, bool readOnly, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, string command = null, TextAnchor align = TextAnchor.MiddleCenter, Handler.FontTypes font = Handler.FontTypes.RobotoCondensedRegular, bool autoFocus = false, bool hudMenuInput = false, InputField.LineType lineType = UnityEngine.UI.InputField.LineType.SingleLine, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string destroyUi = null, bool activeSelf = true)
	{
		return cui.CreateInputField(null, null, color, text, size, characterLimit, readOnly, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, command, align, font, autoFocus, hudMenuInput, lineType, fadeIn, fadeOut, needsCursor, needsKeyboard, id, destroyUi, true, activeSelf);
	}
	public static Pair<string, CuiElement> UpdateProtectedInputField(this CUI cui, string id, string color, string text, int size, int characterLimit, bool readOnly, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, string command = null, TextAnchor align = TextAnchor.MiddleCenter, Handler.FontTypes font = Handler.FontTypes.RobotoCondensedRegular, bool autoFocus = false, bool hudMenuInput = false, InputField.LineType lineType = UnityEngine.UI.InputField.LineType.SingleLine, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string destroyUi = null, bool activeSelf = true)
	{
		return cui.CreateProtectedInputField(null, null, color, text, size, characterLimit, readOnly, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, command, align, font, autoFocus, hudMenuInput, lineType, fadeIn, fadeOut, needsCursor, needsKeyboard, id, destroyUi, true, activeSelf);
	}
	public static Pair<string, CuiElement> UpdateImage(this CUI cui, string id, uint png, string color, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string destroyUi = null, bool activeSelf = true)
	{
		return cui.CreateImage(null, null, png, color, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, true, activeSelf);
	}
	public static Pair<string, CuiElement> UpdateImage(this CUI cui, string id, string url, string color, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string destroyUi = null, bool activeSelf = true)
	{
		return cui.CreateImage(null, null, url, color, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, true, activeSelf);
	}
	public static Pair<string, CuiElement> UpdateSimpleImage(this CUI cui, string id, string png, string sprite, string color, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string destroyUi = null, bool activeSelf = true)
	{
		return cui.CreateSimpleImage(null, null, png, sprite, color, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, true, activeSelf);
	}
	public static Pair<string, CuiElement> UpdateSprite(this CUI cui, string id, string sprite, string color, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string destroyUi = null, bool activeSelf = true)
	{
		return cui.CreateSprite(null, null, sprite, color, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, true, activeSelf);
	}
	public static Pair<string, CuiElement> UpdateItemImage(this CUI cui, string id, int itemID, ulong skinID, string color, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string destroyUi = null, bool activeSelf = true)
	{
		return cui.CreateItemImage(null, null, itemID, skinID, color, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, true, activeSelf);
	}
	public static Pair<string, CuiElement> UpdateQRCodeImage(this CUI cui, string id, string text, string brandUrl, string brandColor, string brandBgColor, int pixels, bool transparent, bool quietZones, string color, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string destroyUi = null, bool activeSelf = true)
	{
		return cui.CreateQRCodeImage(null, null, text, brandUrl, brandColor, brandBgColor, pixels, transparent, quietZones, color, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, true, activeSelf);
	}
	public static Pair<string, CuiElement> UpdateClientImage(this CUI cui, string id, string url, string color, string material = null, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string destroyUi = null, bool activeSelf = true)
	{
		return cui.CreateClientImage(null, null, url, color, material, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, outlineColor, outlineDistance, outlineUseGraphicAlpha, id, destroyUi, true, activeSelf);
	}
	public static Pair<string, CuiElement> UpdateCountdown(this CUI cui, string id, int startTime, int endTime, int step, string command, float fadeIn = 0f, float fadeOut = 0f, string destroyUi = null, bool activeSelf = true)
	{
		return cui.CreateCountdown(null, null, startTime, endTime, step, command, fadeIn, fadeOut, id, destroyUi, true, activeSelf);
	}
	public static Pair<string, CuiElement> UpdateScrollView(this CUI cui, string id, bool vertical, bool horizontal, ScrollRect.MovementType movementType, float elasticity, bool inertia, float decelerationRate, float scrollSensitivity, out CuiRectTransform contentTransformComponent, out CuiScrollbar horizontalScrollBar, out CuiScrollbar verticalScrollBar, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float OxMin = 0f, float OxMax = 0f, float OyMin = 0f, float OyMax = 0f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string destroyUi = null, bool activeSelf = true)
	{
		return cui.CreateScrollView(null, null, vertical, horizontal, movementType, elasticity, inertia, decelerationRate, scrollSensitivity, out contentTransformComponent, out horizontalScrollBar, out verticalScrollBar, xMin, xMax, yMin, yMax, OxMin, OxMax, OyMin, OyMax, fadeIn, fadeOut, needsCursor, needsKeyboard, id, destroyUi, true, activeSelf);
	}

	public static Pair<string, CuiElement> Panel(this Handler cui, CuiElementContainer container, string parent, string color, string material, float xMin, float xMax, float yMin, float yMax, float OxMin, float OxMax, float OyMin, float OyMax, bool blur = false, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		if (id == null) id = cui.AppendId();
		var element = cui.TakeFromPool(id, parent, fadeOut, destroyUi, update, activeSelf);

		var image = cui.TakeFromPoolImage();
		image.Color = ProcessColor(color);
		if (blur) image.Material = "assets/content/ui/uibackgroundblur.mat";
		else if (material != null) image.Material = material;
		image.FadeIn = fadeIn;
		element.Components.Add(image);

		if (!update || (update && (xMin != 0 || xMax != 1 || yMin != 0 || yMax != 1)))
		{
			var rect = cui.TakeFromPoolRect();
			rect.AnchorMin = LUIBuilder.GetStringFloat(xMin, yMin);
			rect.AnchorMax = LUIBuilder.GetStringFloat(xMax, yMax);
			rect.OffsetMin = LUIBuilder.GetStringFloat(OxMin, OyMin);
			rect.OffsetMax = LUIBuilder.GetStringFloat(OxMax, OyMax);
			element.Components.Add(rect);
		}

		if (needsCursor) element.Components.Add(cui.TakeFromPoolNeedsCursor());
		if (needsKeyboard) element.Components.Add(cui.TakeFromPoolNeedsKeyboard());

		if (outlineColor != null)
		{
			var outline = cui.TakeFromPoolOutline();
			outline.Color = ProcessColor(outlineColor);
			outline.Distance = outlineDistance;
			outline.UseGraphicAlpha = outlineUseGraphicAlpha;
			element.Components.Add(outline);
		}

		if (!update) container?.Add(element);
		return new Pair<string, CuiElement>(id, element);
	}
	public static Pair<string, CuiElement> Text(this Handler cui, CuiElementContainer container, string parent, string color, string text, int size, float xMin, float xMax, float yMin, float yMax, float OxMin, float OxMax, float OyMin, float OyMax, TextAnchor align, Handler.FontTypes font, VerticalWrapMode verticalOverflow, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		if (id == null) id = cui.AppendId();
		var element = cui.TakeFromPool(id, parent, fadeOut, destroyUi, update, activeSelf);

		var label = cui.TakeFromPoolText();
		label.Text = string.IsNullOrEmpty(text) ? string.Empty : text;
		label.FontSize = size;
		label.Align = align;
		label.Font = cui.GetFont(font);
		label.Color = ProcessColor(color);
		label.FadeIn = fadeIn;
		label.VerticalOverflow = verticalOverflow;
		element.Components.Add(label);

		if (!update || (update && (xMin != 0 || xMax != 1 || yMin != 0 || yMax != 1)))
		{
			var rect = cui.TakeFromPoolRect();
			rect.AnchorMin = LUIBuilder.GetStringFloat(xMin, yMin);
			rect.AnchorMax = LUIBuilder.GetStringFloat(xMax, yMax);
			rect.OffsetMin = LUIBuilder.GetStringFloat(OxMin, OyMin);
			rect.OffsetMax = LUIBuilder.GetStringFloat(OxMax, OyMax);
			element.Components.Add(rect);
		}

		if (needsCursor) element.Components.Add(cui.TakeFromPoolNeedsCursor());
		if (needsKeyboard) element.Components.Add(cui.TakeFromPoolNeedsKeyboard());

		if (outlineColor != null)
		{
			var outline = cui.TakeFromPoolOutline();
			outline.Color = ProcessColor(outlineColor);
			outline.Distance = outlineDistance;
			outline.UseGraphicAlpha = outlineUseGraphicAlpha;
			element.Components.Add(outline);
		}

		if (!update) container?.Add(element);
		return new Pair<string, CuiElement>(id, element);
	}
	public static Pair<string, CuiElement, CuiElement> Button(this Handler cui, CuiElementContainer container, string parent, string color, string textColor, string text, int size, string material, float xMin, float xMax, float yMin, float yMax, float OxMin, float OxMax, float OyMin, float OyMax, string command, TextAnchor align, Handler.FontTypes font, bool @protected, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		if (id == null) id = cui.AppendId();
		var element = cui.TakeFromPool(id, parent, fadeOut, destroyUi, update, activeSelf);

		var button = cui.TakeFromPoolButton();
		button.FadeIn = fadeIn;
		button.Color = ProcessColor(color);
		button.Command = @protected ? Community.Protect(command) : command;
		if (material != null) button.Material = material;
		element.Components.Add(button);

		if (!update || (update && (xMin != 0 || xMax != 1 || yMin != 0 || yMax != 1)))
		{
			var rect = cui.TakeFromPoolRect();
			rect.AnchorMin = LUIBuilder.GetStringFloat(xMin, yMin);
			rect.AnchorMax = LUIBuilder.GetStringFloat(xMax, yMax);
			rect.OffsetMin = LUIBuilder.GetStringFloat(OxMin, OyMin);
			rect.OffsetMax = LUIBuilder.GetStringFloat(OxMax, OyMax);
			element.Components.Add(rect);
		}

		if (needsCursor) element.Components.Add(cui.TakeFromPoolNeedsCursor());
		if (needsKeyboard) element.Components.Add(cui.TakeFromPoolNeedsKeyboard());

		if (!update) container?.Add(element);

		var textElement = (CuiElement)null;

		if (!string.IsNullOrEmpty(text))
		{
			textElement = cui.TakeFromPool(cui.AppendId(), element.Name);

			var ptext = cui.TakeFromPoolText();
			ptext.Text = text;
			ptext.FontSize = size;
			ptext.Align = align;
			ptext.Color = ProcessColor(textColor);
			ptext.Font = cui.GetFont(font);
			textElement.Components.Add(ptext);

			var prect = cui.TakeFromPoolRect();
			prect.AnchorMin = "0.02 0";
			prect.AnchorMax = "0.98 1";
			textElement.Components.Add(prect);

			if (!update) container?.Add(textElement);
		}

		if (outlineColor != null)
		{
			var outline = cui.TakeFromPoolOutline();
			outline.Color = ProcessColor(outlineColor);
			outline.Distance = outlineDistance;
			outline.UseGraphicAlpha = outlineUseGraphicAlpha;
			element.Components.Add(outline);
		}

		return new Pair<string, CuiElement, CuiElement>(id, element, textElement);
	}
	public static Pair<string, CuiElement> InputField(this Handler cui, CuiElementContainer container, string parent, string color, string text, int size, int characterLimit, bool readOnly, float xMin, float xMax, float yMin, float yMax, float OxMin, float OxMax, float OyMin, float OyMax, string command, TextAnchor align, Handler.FontTypes font, bool @protected, bool autoFocus = false, bool hudMenuInput = false, InputField.LineType lineType = UnityEngine.UI.InputField.LineType.SingleLine, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		if (id == null) id = cui.AppendId();
		var element = cui.TakeFromPool(id, parent, fadeOut, destroyUi, update, activeSelf);

		var inputField = cui.TakeFromPoolInputField();
		inputField.Color = ProcessColor(color);
		inputField.Text = string.IsNullOrEmpty(text) ? string.Empty : text;
		inputField.FontSize = size;
		inputField.Font = cui.GetFont(font);
		inputField.Align = align;
		inputField.CharsLimit = characterLimit;
		inputField.ReadOnly = readOnly;
		inputField.Command = @protected ? Community.Protect(command) : command;
		inputField.LineType = lineType;
		inputField.Autofocus = autoFocus;
		inputField.HudMenuInput = hudMenuInput;
		element.Components.Add(inputField);

		if (needsCursor) element.Components.Add(cui.TakeFromPoolNeedsCursor());
		if (needsKeyboard && !inputField.ReadOnly) element.Components.Add(cui.TakeFromPoolNeedsKeyboard());

		if (!update || (update && (xMin != 0 || xMax != 1 || yMin != 0 || yMax != 1)))
		{
			var rect = cui.TakeFromPoolRect();
			rect.AnchorMin = LUIBuilder.GetStringFloat(xMin, yMin);
			rect.AnchorMax = LUIBuilder.GetStringFloat(xMax, yMax);
			rect.OffsetMin = LUIBuilder.GetStringFloat(OxMin, OyMin);
			rect.OffsetMax = LUIBuilder.GetStringFloat(OxMax, OyMax);
			element.Components.Add(rect);
		}

		if (!update) container?.Add(element);
		return new Pair<string, CuiElement>(id, element);
	}
	public static Pair<string, CuiElement> Image(this Handler cui, CuiElementContainer container, string parent, string png, string url, string steamId, string color, string material, float xMin, float xMax, float yMin, float yMax, float OxMin, float OxMax, float OyMin, float OyMax, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		if (id == null) id = cui.AppendId();
		var element = cui.TakeFromPool(id, parent, fadeOut, destroyUi, update, activeSelf);

		var rawImage = cui.TakeFromPoolRawImage();
		rawImage.Png = png;
		rawImage.Url = url;
		rawImage.SteamId = steamId;
		rawImage.FadeIn = fadeIn;
		rawImage.Color = ProcessColor(color);
		if (material != null) rawImage.Material = material;
		element.Components.Add(rawImage);

		if (!update || (update && (xMin != 0 || xMax != 1 || yMin != 0 || yMax != 1)))
		{
			var rect = cui.TakeFromPoolRect();
			rect.AnchorMin = LUIBuilder.GetStringFloat(xMin, yMin);
			rect.AnchorMax = LUIBuilder.GetStringFloat(xMax, yMax);
			rect.OffsetMin = LUIBuilder.GetStringFloat(OxMin, OyMin);
			rect.OffsetMax = LUIBuilder.GetStringFloat(OxMax, OyMax);
			element.Components.Add(rect);
		}

		if (needsCursor) element.Components.Add(cui.TakeFromPoolNeedsCursor());
		if (needsKeyboard) element.Components.Add(cui.TakeFromPoolNeedsKeyboard());

		if (outlineColor != null)
		{
			var outline = cui.TakeFromPoolOutline();
			outline.Color = ProcessColor(outlineColor);
			outline.Distance = outlineDistance;
			outline.UseGraphicAlpha = outlineUseGraphicAlpha;
			element.Components.Add(outline);
		}

		if (!update) container?.Add(element);
		return new Pair<string, CuiElement>(id, element);
	}
	public static Pair<string, CuiElement> SimpleImage(this Handler cui, CuiElementContainer container, string parent, string png, string sprite, string color, string material, float xMin, float xMax, float yMin, float yMax, float OxMin, float OxMax, float OyMin, float OyMax, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		if (id == null) id = cui.AppendId();
		var element = cui.TakeFromPool(id, parent, fadeOut, destroyUi, update, activeSelf);

		var simpleImage = cui.TakeFromPoolImage();
		simpleImage.Png = png;
		simpleImage.Sprite = sprite;
		simpleImage.FadeIn = fadeIn;
		simpleImage.Color = ProcessColor(color);
		if (material != null) simpleImage.Material = material;
		element.Components.Add(simpleImage);

		if (!update || (update && (xMin != 0 || xMax != 1 || yMin != 0 || yMax != 1)))
		{
			var rect = cui.TakeFromPoolRect();
			rect.AnchorMin = LUIBuilder.GetStringFloat(xMin, yMin);
			rect.AnchorMax = LUIBuilder.GetStringFloat(xMax, yMax);
			rect.OffsetMin = LUIBuilder.GetStringFloat(OxMin, OyMin);
			rect.OffsetMax = LUIBuilder.GetStringFloat(OxMax, OyMax);
			element.Components.Add(rect);
		}

		if (needsCursor) element.Components.Add(cui.TakeFromPoolNeedsCursor());
		if (needsKeyboard) element.Components.Add(cui.TakeFromPoolNeedsKeyboard());

		if (outlineColor != null)
		{
			var outline = cui.TakeFromPoolOutline();
			outline.Color = ProcessColor(outlineColor);
			outline.Distance = outlineDistance;
			outline.UseGraphicAlpha = outlineUseGraphicAlpha;
			element.Components.Add(outline);
		}

		if (!update) container?.Add(element);
		return new Pair<string, CuiElement>(id, element);
	}
	public static Pair<string, CuiElement> Sprite(this Handler cui, CuiElementContainer container, string parent, string sprite, string color, string material, float xMin, float xMax, float yMin, float yMax, float OxMin, float OxMax, float OyMin, float OyMax, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		if (id == null) id = cui.AppendId();
		var element = cui.TakeFromPool(id, parent, fadeOut, destroyUi, update, activeSelf);

		var rawImage = cui.TakeFromPoolRawImage();
		rawImage.Sprite = sprite;
		rawImage.FadeIn = fadeIn;
		rawImage.Color = ProcessColor(color);
		if (material != null) rawImage.Material = material;
		element.Components.Add(rawImage);

		if (!update || (update && (xMin != 0 || xMax != 1 || yMin != 0 || yMax != 1)))
		{
			var rect = cui.TakeFromPoolRect();
			rect.AnchorMin = LUIBuilder.GetStringFloat(xMin, yMin);
			rect.AnchorMax = LUIBuilder.GetStringFloat(xMax, yMax);
			rect.OffsetMin = LUIBuilder.GetStringFloat(OxMin, OyMin);
			rect.OffsetMax = LUIBuilder.GetStringFloat(OxMax, OyMax);
			element.Components.Add(rect);
		}

		if (needsCursor) element.Components.Add(cui.TakeFromPoolNeedsCursor());
		if (needsKeyboard) element.Components.Add(cui.TakeFromPoolNeedsKeyboard());

		if (outlineColor != null)
		{
			var outline = cui.TakeFromPoolOutline();
			outline.Color = ProcessColor(outlineColor);
			outline.Distance = outlineDistance;
			outline.UseGraphicAlpha = outlineUseGraphicAlpha;
			element.Components.Add(outline);
		}

		if (!update) container?.Add(element);
		return new Pair<string, CuiElement>(id, element);
	}
	public static Pair<string, CuiElement> ItemImage(this Handler cui, CuiElementContainer container, string parent, int itemID, ulong skinID, string color, string material, float xMin, float xMax, float yMin, float yMax, float OxMin, float OxMax, float OyMin, float OyMax, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string outlineColor = null, string outlineDistance = null, bool outlineUseGraphicAlpha = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		if (id == null) id = cui.AppendId();
		var element = cui.TakeFromPool(id, parent, fadeOut, destroyUi, update, activeSelf);

		var rawImage = cui.TakeFromPoolImage();
		rawImage.ItemId = itemID;
		rawImage.SkinId = skinID;
		rawImage.FadeIn = fadeIn;
		rawImage.Color = ProcessColor(color);
		if (material != null) rawImage.Material = material;
		element.Components.Add(rawImage);

		if (!update || (update && (xMin != 0 || xMax != 1 || yMin != 0 || yMax != 1)))
		{
			var rect = cui.TakeFromPoolRect();
			rect.AnchorMin = LUIBuilder.GetStringFloat(xMin, yMin);
			rect.AnchorMax = LUIBuilder.GetStringFloat(xMax, yMax);
			rect.OffsetMin = LUIBuilder.GetStringFloat(OxMin, OyMin);
			rect.OffsetMax = LUIBuilder.GetStringFloat(OxMax, OyMax);
			element.Components.Add(rect);
		}

		if (needsCursor) element.Components.Add(cui.TakeFromPoolNeedsCursor());
		if (needsKeyboard) element.Components.Add(cui.TakeFromPoolNeedsKeyboard());

		if (outlineColor != null)
		{
			var outline = cui.TakeFromPoolOutline();
			outline.Color = ProcessColor(outlineColor);
			outline.Distance = outlineDistance;
			outline.UseGraphicAlpha = outlineUseGraphicAlpha;
			element.Components.Add(outline);
		}

		if (!update) container?.Add(element);
		return new Pair<string, CuiElement>(id, element);
	}
	public static Pair<string, CuiElement> Countdown(this Handler cui, CuiElementContainer container, string parent, int startTime, int endTime, int step, string command, float fadeIn = 0f, float fadeOut = 0f, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		if (id == null) id = cui.AppendId();
		var element = cui.TakeFromPool(id, parent, fadeOut, destroyUi, update, activeSelf);

		var countdown = cui.TakeFromPoolCountdown();
		countdown.StartTime = startTime;
		countdown.EndTime = endTime;
		countdown.Step = step;
		countdown.Command = command;
		countdown.FadeIn = fadeIn;
		element.Components.Add(countdown);

		if (!update) container?.Add(element);
		return new Pair<string, CuiElement>(id, element);
	}
	public static Pair<string, CuiElement> ScrollView(this Handler cui, CuiElementContainer container, string parent, bool vertical, bool horizontal, ScrollRect.MovementType movementType, float elasticity, bool inertia, float decelerationRate, float scrollSensitivity, out CuiRectTransform contentTransformComponent, out CuiScrollbar horizontalScrollBar, out CuiScrollbar verticalScrollBar, float xMin, float xMax, float yMin, float yMax, float OxMin, float OxMax, float OyMin, float OyMax, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false, string id = null, string destroyUi = null, bool update = false, bool activeSelf = true)
	{
		if (id == null) id = cui.AppendId();
		var element = cui.TakeFromPool(id, parent, fadeOut, destroyUi, update, activeSelf);

		var scrollview = cui.TakeFromPoolScrollView();
		scrollview.Vertical = vertical;
		scrollview.Horizontal = horizontal;
		scrollview.MovementType = movementType;
		scrollview.Elasticity = elasticity;
		scrollview.Inertia = inertia;
		scrollview.DecelerationRate = decelerationRate;
		scrollview.ScrollSensitivity = scrollSensitivity;
		contentTransformComponent = scrollview.ContentTransform;
		horizontalScrollBar = scrollview.HorizontalScrollbar;
		verticalScrollBar = scrollview.VerticalScrollbar;

		element.Components.Add(scrollview);

		if (!update || (update && (xMin != 0 || xMax != 1 || yMin != 0 || yMax != 1)))
		{
			var rect = cui.TakeFromPoolRect();
			rect.AnchorMin = LUIBuilder.GetStringFloat(xMin, yMin);
			rect.AnchorMax = LUIBuilder.GetStringFloat(xMax, yMax);
			rect.OffsetMin = LUIBuilder.GetStringFloat(OxMin, OyMin);
			rect.OffsetMax = LUIBuilder.GetStringFloat(OxMax, OyMax);
			element.Components.Add(rect);
		}

		if (needsCursor) element.Components.Add(cui.TakeFromPoolNeedsCursor());
		if (needsKeyboard) element.Components.Add(cui.TakeFromPoolNeedsKeyboard());

		if (!update) container?.Add(element);
		return new Pair<string, CuiElement>(id, element);
	}

	public static readonly uint AddUiString = StringPool.Get("AddUi");

	public static void Send(this CuiElementContainer container, BasePlayer player)
	{
		CuiHelper.AddUi(player, container);
	}
	public static byte[] GetData(this CuiElementContainer container)
	{
		var write = Net.sv.StartWrite();
		write.PacketID(Message.Type.RPCMessage);
		write.EntityID(CommunityEntity.ServerInstance.net.ID);
		write.UInt32(AddUiString);
		write.UInt64(0UL);
		write.String(container.ToJson());

		var bytes = new byte[write.Length];
		Array.Copy(write.stream._buffer, bytes, write.Length);

		Facepunch.Pool.Free(ref write);

		return bytes;
	}
	public static void SendData(byte[] data, BasePlayer player)
	{
		var write = Net.sv.StartWrite();
		write.PacketID(Message.Type.RPCMessage);
		Array.Copy(data, 0, write.stream._buffer,write.Length , data.Length);
		write.stream._length += data.Length;
		write.Send(new SendInfo(player.Connection));
	}
	public static void SendUpdate(this Pair<string, CuiElement> pair, BasePlayer player)
	{
		var elements = Facepunch.Pool.Get<List<CuiElement>>();
		elements.Add(pair.Element);

		CuiHelper.AddUi(player, elements);

		Facepunch.Pool.FreeUnmanaged(ref elements);
	}
	public static void Destroy(this CuiElementContainer container, BasePlayer player)
	{
		CuiHelper.DestroyUi(player, container.Name);
	}
	public static void Destroy(string name, BasePlayer player)
	{
		CuiHelper.DestroyUi(player, name);
	}
}
