using System;
using System.Collections.Generic;
using System.Globalization;
using Oxide.Game.Rust.Cui;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon;

public struct CUI : IDisposable
{
	public Handler Manager { get; private set; }

	public CUI(Handler manager)
	{
		Manager = manager;
	}

	public CuiElementContainer CreateContainer(string panel, string color, float xMin = 0f, float yMin = 0f, float xMax = 1f, float yMax = 1f, bool useCursor = false, string parent = "Overlay")
	{
		var container = Manager.TakeFromPoolContainer();
		container.Name = panel;

		var element = Manager.TakeFromPool(panel, parent);

		var image = Manager.TakeFromPoolImage();
		image.Color = color;
		element.Components.Add(image);

		var rect = Manager.TakeFromPoolRect();
		rect.AnchorMin = $"{xMin} {yMin}";
		rect.AnchorMax = $"{xMax} {yMax}";
		element.Components.Add(rect);

		if (useCursor) element.Components.Add(Manager.TakeFromPoolNeedsCursor());

		container.Add(element);
		container.Add(Manager.TakeFromPool(Manager.AppendId(), parent));
		return container;
	}
	public CuiElementContainer CreatePanel(CuiElementContainer container, string panel, string color, float xMin = 0f, float yMin = 0f, float xMax = 1f, float yMax = 1f, bool cursor = false)
	{
		return CUIStatics.Panel(Manager, container, panel, color, xMin, yMin, xMax, yMax, cursor);
	}
	public CuiElementContainer CreateLabel(CuiElementContainer container, string panel, string text, int size, float xMin = 0f, float yMin = 0f, float xMax = 1f, float yMax = 1f, TextAnchor align = TextAnchor.MiddleCenter, string font = "robotocondensed-bold.ttf")
	{
		return CUIStatics.Label(Manager, container, panel, text, size, xMin, yMin, xMax, yMax, align, font);
	}
	public CuiElementContainer CreateButton(CuiElementContainer container, string panel, string color, string text, int size, float xMin = 0f, float yMin = 0f, float xMax = 1f, float yMax = 1f, string command = null, TextAnchor align = TextAnchor.MiddleCenter, bool @protected = false)
	{
		return CUIStatics.Button(Manager, container, panel, color, text, size, xMin, yMin, xMax, yMax, command, align, @protected);
	}
	public CuiElementContainer CreateProtectedButton(CuiElementContainer container, string panel, string color, string text, int size, float xMin = 0f, float yMin = 0f, float xMax = 1f, float yMax = 1f, string command = null, TextAnchor align = TextAnchor.MiddleCenter)
	{
		return CUIStatics.ProtectedButton(Manager, container, panel, color, text, size, xMin, yMin, xMax, yMax, command, align);
	}
	public CuiElementContainer CreateImage(CuiElementContainer container, string panel, string png, float xMin = 0f, float yMin = 0f, float xMax = 1f, float yMax = 1f)
	{
		return CUIStatics.Image(Manager, container, panel, png, xMin, yMin, xMax, yMax);
	}
	public CuiElementContainer CreateItemImage(CuiElementContainer container, string panel, int itemID, float xMin = 0f, float yMin = 0f, float xMax = 1f, float yMax = 1f)
	{
		return CUIStatics.ItemImage(Manager, container, panel, itemID, xMin, yMin, xMax, yMax);
	}

	public string Color(string hexColor, float alpha)
	{
		if (hexColor.StartsWith("#")) hexColor = hexColor.Substring(1);

		var red = int.Parse(hexColor.Substring(0, 2), NumberStyles.AllowHexSpecifier);
		var green = int.Parse(hexColor.Substring(2, 2), NumberStyles.AllowHexSpecifier);
		var blue = int.Parse(hexColor.Substring(4, 2), NumberStyles.AllowHexSpecifier);

		return $"{(double)red / 255} {(double)green / 255} {(double)blue / 255} {alpha}";
	}

	public void Send(CuiElementContainer container, BasePlayer player)
	{
		Manager.Send(container, player);
	}
	public void Destroy(CuiElementContainer container, BasePlayer player)
	{
		Manager.Destroy(container, player);
	}
	public void Destroy(string name, BasePlayer player)
	{
		Manager.Destroy(name, player);
	}

	public void Dispose()
	{
		Manager.SendToPool();
	}

	public class Handler
	{
		#region Properties

		internal int _currentId { get; set; }

		internal List<object> _queue = new();
		internal List<CuiElementContainer> _containerPool = new();

		internal List<CuiElement> _elements = new();
		internal List<ICuiComponent> _images = new();
		internal List<ICuiComponent> _rawImages = new();
		internal List<ICuiComponent> _texts = new();
		internal List<ICuiComponent> _buttons = new();
		internal List<ICuiComponent> _rects = new();
		internal List<ICuiComponent> _needsCursors = new();
		internal List<ICuiComponent> _needsKeyboards = new();

		#endregion

		#region Default Instances

		internal CuiRectPosition _defaultPosition = new CuiRectPosition(0f, 0f, 1f, 1f);
		internal CuiImageComponent _defaultImage = new CuiImageComponent();
		internal CuiRawImageComponent _defaultRawImage = new CuiRawImageComponent();
		internal CuiRectTransformComponent _defaultRectTransform = new CuiRectTransformComponent();
		internal CuiTextComponent _defaultText = new CuiTextComponent();
		internal CuiButtonComponent _defaultButton = new CuiButtonComponent();

		#endregion

		#region Pooling

		internal string AppendId()
		{
			_currentId++;
			return _currentId.ToString();
		}
		internal void SendToPool<T>(T element)
		{
			if (element == null) return;

			var componentElement = element as ICuiComponent;

			if (element is CuiImageComponent) _images.Add(componentElement);
			else if (element is CuiRawImageComponent) _rawImages.Add(componentElement);
			else if (element is CuiTextComponent) _texts.Add(componentElement);
			else if (element is CuiButtonComponent) _buttons.Add(componentElement);
			else if (element is CuiRectTransformComponent) _rects.Add(componentElement);
			else if (element is CuiNeedsCursorComponent) _needsCursors.Add(componentElement);
			else if (element is CuiNeedsKeyboardComponent) _needsKeyboards.Add(componentElement);
		}
		internal CuiElement TakeFromPool(string name = null, string parent = "Hud")
		{
			var element = (CuiElement)null;

			if (_containerPool.Count == 0)
			{
				element = new CuiElement();
			}
			else
			{
				element = _elements[0];
				_elements.RemoveAt(0);
			}

			element.Name = name;
			element.Parent = parent;
			element.FadeOut = 0.1f;
			element.Components.Clear();

			_queue.Add(element);
			return element;
		}

		public void SendToPool()
		{
			foreach (var entry in _queue)
			{
				SendToPool(entry);
			}

			_queue.Clear();
			_currentId = 0;
		}

		#endregion

		#region Pooled Elements

		internal CuiElementContainer TakeFromPoolContainer()
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
		internal CuiRectPosition TakeFromPoolDimensions()
		{
			var element = new CuiRectPosition(_defaultPosition.xMin, _defaultPosition.yMin, _defaultPosition.xMax, _defaultPosition.yMax);

			return element;
		}
		internal CuiImageComponent TakeFromPoolImage()
		{
			var element = (CuiImageComponent)null;

			if (_images.Count == 0)
			{
				element = new CuiImageComponent();
			}
			else
			{
				element = _images[0] as CuiImageComponent;
				element.SkinId = _defaultImage.SkinId;
				element.Png = _defaultImage.Png;
				element.ItemId = _defaultImage.ItemId;
				element.FadeIn = _defaultImage.FadeIn;
				element.Sprite = _defaultImage.Sprite;
				element.Color = _defaultImage.Color;
				_images.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		internal CuiRawImageComponent TakeFromPoolRawImage()
		{
			var element = (CuiRawImageComponent)null;

			if (_rawImages.Count == 0)
			{
				element = new CuiRawImageComponent();
			}
			else
			{
				element = _rawImages[0] as CuiRawImageComponent;
				element.Url = _defaultRawImage.Url;
				element.Png = _defaultRawImage.Png;
				element.FadeIn = _defaultRawImage.FadeIn;
				element.Sprite = _defaultRawImage.Sprite;
				element.Color = _defaultRawImage.Color;
				_rawImages.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		internal CuiRectTransformComponent TakeFromPoolRect()
		{
			var element = (CuiRectTransformComponent)null;

			if (_rects.Count == 0)
			{
				element = new CuiRectTransformComponent();
			}
			else
			{
				element = _rects[0] as CuiRectTransformComponent;
				element.AnchorMin = _defaultRectTransform.AnchorMin;
				element.AnchorMax = _defaultRectTransform.AnchorMax;
				element.OffsetMin = _defaultRectTransform.OffsetMin;
				element.OffsetMax = _defaultRectTransform.OffsetMax;
				_rects.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		internal CuiTextComponent TakeFromPoolText()
		{
			var element = (CuiTextComponent)null;

			if (_texts.Count == 0)
			{
				element = new CuiTextComponent();
			}
			else
			{
				element = _texts[0] as CuiTextComponent;
				element.Color = _defaultText.Color;
				element.FontSize = _defaultText.FontSize;
				element.Font = _defaultText.Font;
				_texts.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		internal CuiButtonComponent TakeFromPoolButton()
		{
			var element = (CuiButtonComponent)null;

			if (_buttons.Count == 0)
			{
				element = new CuiButtonComponent();
			}
			else
			{
				element = _buttons[0] as CuiButtonComponent;
				element.Color = _defaultButton.Color;
				element.Material = _defaultButton.Material;
				element.Sprite = _defaultButton.Sprite;
				_buttons.RemoveAt(0);
			}

			_queue.Add(element);
			return element;
		}
		internal CuiNeedsCursorComponent TakeFromPoolNeedsCursor()
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
		internal CuiNeedsKeyboardComponent TakeFromPoolNeedsKeyboard()
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

		#endregion

		#region Classes

		public struct CuiRectPosition
		{
			public float xMin { get; set; }
			public float yMin { get; set; }
			public float xMax { get; set; }
			public float yMax { get; set; }

			public CuiRectPosition(float xMin, float yMin, float xMax, float yMax)
			{
				this.xMin = xMin;
				this.yMin = yMin;
				this.xMax = xMax;
				this.yMax = yMax;
			}

			public string GetMin() => $"{xMin} {yMin}";
			public string GetMax() => $"{xMax} {yMax}";
		}

		#endregion

		#region Networking

		public void Send(CuiElementContainer container, BasePlayer player)
		{
			container.Send(player);
		}
		public void Destroy(CuiElementContainer container, BasePlayer player)
		{
			container.Destroy(player);
		}
		public void Destroy(string name, BasePlayer player)
		{
			CuiHelper.DestroyUi(player, name);
		}

		#endregion
	}
}

public static class CUIStatics
{
	public static CuiElementContainer Panel(this CUI.Handler cui, CuiElementContainer container, string panel, string color, float xMin, float yMin, float xMax, float yMax, bool cursor = false)
	{
		var element = cui.TakeFromPool(cui.AppendId(), panel);

		var image = cui.TakeFromPoolImage();
		image.Color = color;
		image.FadeIn = 0.1f;
		element.Components.Add(image);

		var rect = cui.TakeFromPoolRect();
		rect.AnchorMin = $"{xMin} {yMin}";
		rect.AnchorMax = $"{xMax} {yMax}";
		element.Components.Add(rect);

		if (cursor) element.Components.Add(cui.TakeFromPoolNeedsCursor());

		container.Add(element);

		return container;
	}
	public static CuiElementContainer Label(this CUI.Handler cui, CuiElementContainer container, string panel, string text, int size, float xMin, float yMin, float xMax, float yMax, TextAnchor align, string font)
	{
		var element = cui.TakeFromPool(cui.AppendId(), panel);

		var label = cui.TakeFromPoolText();
		label.Text = text;
		label.FontSize = size;
		label.Align = align;
		label.Font = font;
		element.Components.Add(label);

		var rect = cui.TakeFromPoolRect();
		rect.AnchorMin = $"{xMin} {yMin}";
		rect.AnchorMax = $"{xMax} {yMax}";
		element.Components.Add(rect);

		container.Add(element);
		return container;
	}
	public static CuiElementContainer Button(this CUI.Handler cui, CuiElementContainer container, string panel, string color, string text, int size, float xMin, float yMin, float xMax, float yMax, string command, TextAnchor align, bool @protected)
	{
		var buttonElement = cui.TakeFromPool(cui.AppendId(), panel);

		var button = cui.TakeFromPoolButton();
		button.Color = color;
		button.Command = @protected ? UiCommandAttribute.Uniquify(command) : command;
		buttonElement.Components.Add(button);

		var rect = cui.TakeFromPoolRect();
		rect.AnchorMin = $"{xMin} {yMin}";
		rect.AnchorMax = $"{xMax} {yMax}";
		buttonElement.Components.Add(rect);

		container.Add(buttonElement);

		if (!string.IsNullOrEmpty(text))
		{
			var textElement = cui.TakeFromPool(cui.AppendId(), buttonElement.Name);

			var ptext = cui.TakeFromPoolText();
			ptext.Text = text;
			ptext.FontSize = size;
			ptext.Align = align;
			ptext.Color = "1 1 1 1";
			textElement.Components.Add(ptext);

			var prect = cui.TakeFromPoolRect();
			textElement.Components.Add(prect);

			container.Add(textElement);
		}

		return container;
	}
	public static CuiElementContainer ProtectedButton(this CUI.Handler cui, CuiElementContainer container, string panel, string color, string text, int size, float xMin, float yMin, float xMax, float yMax, string command, TextAnchor align)
	{
		return Button(cui, container, panel, color, text, size, xMin, yMin, xMax, yMax, command, align, true);
	}
	public static CuiElementContainer Image(this CUI.Handler cui, CuiElementContainer container, string panel, string png, float xMin, float yMin, float xMax, float yMax)
	{
		var element = cui.TakeFromPool(cui.AppendId(), panel);

		var rawImage = cui.TakeFromPoolRawImage();
		rawImage.Png = png;
		rawImage.FadeIn = 0.1f;
		element.Components.Add(rawImage);

		var rect = cui.TakeFromPoolRect();
		rect.AnchorMin = $"{xMin} {yMin}";
		rect.AnchorMax = $"{xMax} {yMax}";
		element.Components.Add(rect);

		container.Add(element);
		return container;
	}
	public static CuiElementContainer ItemImage(this CUI.Handler cui, CuiElementContainer container, string panel, int itemID, float xMin, float yMin, float xMax, float yMax)
	{
		var element = cui.TakeFromPool(cui.AppendId(), panel);

		var rawImage = cui.TakeFromPoolImage();
		rawImage.ItemId = itemID;
		element.Components.Add(rawImage);

		var rect = cui.TakeFromPoolRect();
		rect.AnchorMin = $"{xMin} {yMin}";
		rect.AnchorMax = $"{xMax} {yMax}";
		element.Components.Add(rect);

		container.Add(element);
		return container;
	}

	public static void Send(this CuiElementContainer container, BasePlayer player)
	{
		CuiHelper.AddUi(player, container);
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
