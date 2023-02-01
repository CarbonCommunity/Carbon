using System;
using System.Collections.Generic;
using System.Globalization;
using Oxide.Game.Rust.Cui;
using UnityEngine;
using static Carbon.CUI.Handler;

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

	public CuiElementContainer CreateContainer(string panel, string color = "0 0 0 0", float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, bool needsCursor = false, bool needsKeyboard = false, string parent = "Overlay")
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

		if (needsCursor) element.Components.Add(Manager.TakeFromPoolNeedsCursor());
		if (needsKeyboard) element.Components.Add(Manager.TakeFromPoolNeedsKeyboard());

		container.Add(element);
		container.Add(Manager.TakeFromPool(Manager.AppendId(), parent));
		return container;
	}
	public string CreatePanel(CuiElementContainer container, string parent, string id, string color, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, bool blur = false, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false)
	{
		return CUIStatics.Panel(Manager, container, parent, id, color, xMin, xMax, yMin, yMax, blur, fadeIn, fadeOut, needsCursor, needsKeyboard);
	}
	public string CreateText(CuiElementContainer container, string parent, string id, string color, string text, int size, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, TextAnchor align = TextAnchor.MiddleCenter, FontTypes font = FontTypes.RobotoCondensedRegular, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false)
	{
		return CUIStatics.Text(Manager, container, parent, id, color, text, size, xMin, xMax, yMin, yMax, align, font, fadeIn, fadeOut, needsCursor, needsKeyboard);
	}
	public string CreateButton(CuiElementContainer container, string parent, string id, string color, string textColor, string text, int size, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, string command = null, TextAnchor align = TextAnchor.MiddleCenter, FontTypes font = FontTypes.RobotoCondensedRegular, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false)
	{
		return CUIStatics.Button(Manager, container, parent, id, color, textColor, text, size, xMin, xMax, yMin, yMax, command, align, font, false, fadeIn, fadeOut, needsCursor, needsKeyboard);
	}
	public string CreateProtectedButton(CuiElementContainer container, string parent, string id, string color, string textColor, string text, int size, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, string command = null, TextAnchor align = TextAnchor.MiddleCenter, FontTypes font = FontTypes.RobotoCondensedRegular, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false)
	{
		return CUIStatics.Button(Manager, container, parent, id, color, textColor, text, size, xMin, xMax, yMin, yMax, command, align, font, true, fadeIn, fadeOut, needsCursor, needsKeyboard);
	}
	public string CreateInputField(CuiElementContainer container, string parent, string id, string color, string text, int size, int characterLimit, bool readOnly, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, string command = null, TextAnchor align = TextAnchor.MiddleCenter, FontTypes font = FontTypes.RobotoCondensedRegular, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false)
	{
		return CUIStatics.InputField(Manager, container, parent, id, color, text, size, characterLimit, readOnly, xMin, xMax, yMin, yMax, command, align, font, false, fadeIn, fadeOut, needsCursor, needsKeyboard);
	}
	public string CreateProtectedInputField(CuiElementContainer container, string parent, string id, string color, string text, int size, int characterLimit, bool readOnly, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, string command = null, TextAnchor align = TextAnchor.MiddleCenter, FontTypes font = FontTypes.RobotoCondensedRegular, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false)
	{
		return CUIStatics.InputField(Manager, container, parent, id, color, text, size, characterLimit, readOnly, xMin, xMax, yMin, yMax, command, align, font, true, fadeIn, fadeOut, needsCursor, needsKeyboard);
	}
	public string CreateImage(CuiElementContainer container, string parent, string id, string png, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false)
	{
		return CUIStatics.Image(Manager, container, parent, id, png, xMin, xMax, yMin, yMax, fadeIn, fadeOut, needsCursor, needsKeyboard);
	}
	public string CreateItemImage(CuiElementContainer container, string parent, string id, int itemID, float xMin = 0f, float xMax = 1f, float yMin = 0f, float yMax = 1f, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false)
	{
		return CUIStatics.ItemImage(Manager, container, parent, id, itemID, xMin, xMax, yMin, yMax, fadeIn, fadeOut, needsCursor, needsKeyboard);
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
		internal List<ICuiComponent> _inputFields = new();
		internal List<ICuiComponent> _rects = new();
		internal List<ICuiComponent> _needsCursors = new();
		internal List<ICuiComponent> _needsKeyboards = new();

		#endregion

		#region Default Instances

		internal CuiRectPosition _defaultPosition = new(0f, 1f, 0f, 1f);
		internal CuiImageComponent _defaultImage = new();
		internal CuiRawImageComponent _defaultRawImage = new();
		internal CuiRectTransformComponent _defaultRectTransform = new();
		internal CuiTextComponent _defaultText = new();
		internal CuiButtonComponent _defaultButton = new();
		internal CuiInputFieldComponent _defaultInputField = new();

		#endregion

		#region Pooling

		internal string AppendId()
		{
			_currentId++;
			return _currentId.ToString();
		}
		internal void SendToPool<T>(T element) where T : ICuiComponent
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
			}
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
				SendToPool(entry as ICuiComponent);
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
				element.Text = _defaultText.Text;
				element.FontSize = _defaultText.FontSize;
				element.Font = _defaultText.Font;
				element.Align = _defaultText.Align;
				element.Color = _defaultText.Color;
				element.FadeIn = _defaultText.FadeIn;
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
		internal CuiInputFieldComponent TakeFromPoolInputField()
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
				element.NeedsCursor = _defaultInputField.NeedsCursor;
				element.NeedsKeyboard = _defaultInputField.NeedsKeyboard;
				element.LineType = _defaultInputField.LineType;
				_inputFields.RemoveAt(0);
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

			public CuiRectPosition(float xMin, float xMax, float yMin, float yMax)
			{
				this.xMin = xMin;
				this.yMin = yMin;
				this.xMax = xMax;
				this.yMax = yMax;
			}

			public string GetMin() => $"{xMin} {yMin}";
			public string GetMax() => $"{xMax} {yMax}";
		}

		public enum FontTypes
		{
			RobotoCondensedBold, RobotoCondensedRegular,
			Daubmark, DroidSansMono
		}

		public string GetFont(FontTypes type)
		{
			switch (type)
			{
				case FontTypes.RobotoCondensedBold:
					return "robotocondensed-bold.ttf";

				case FontTypes.RobotoCondensedRegular:
					return "robotocondensed-regular.ttf";

				case FontTypes.Daubmark:
					return "daubmark.ttf";

				case FontTypes.DroidSansMono:
					return "droidsansmono.ttf";
			}

			return "robotocondensed-regular.ttf";
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
	public static string Panel(this CUI.Handler cui, CuiElementContainer container, string parent, string id, string color, float xMin, float xMax, float yMin, float yMax, bool blur = false, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false)
	{
		if (id == null) id = cui.AppendId();
		var element = cui.TakeFromPool(id, parent);
		element.FadeOut = fadeOut;

		var image = cui.TakeFromPoolImage();
		image.Color = color;
		if (blur) image.Material = "assets/content/ui/uibackgroundblur.mat";
		image.FadeIn = fadeIn;
		element.Components.Add(image);

		var rect = cui.TakeFromPoolRect();
		rect.AnchorMin = $"{xMin} {yMin}";
		rect.AnchorMax = $"{xMax} {yMax}";
		element.Components.Add(rect);

		if (needsCursor) element.Components.Add(cui.TakeFromPoolNeedsCursor());
		if (needsKeyboard) element.Components.Add(cui.TakeFromPoolNeedsKeyboard());

		container.Add(element);

		return id;
	}
	public static string Text(this CUI.Handler cui, CuiElementContainer container, string parent, string id, string color, string text, int size, float xMin, float xMax, float yMin, float yMax, TextAnchor align, FontTypes font, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false)
	{
		if (id == null) id = cui.AppendId();
		var element = cui.TakeFromPool(id, parent);
		element.FadeOut = fadeOut;

		var label = cui.TakeFromPoolText();
		label.Text = text;
		label.FontSize = size;
		label.Align = align;
		label.Font = cui.GetFont(font);
		label.Color = color;
		label.FadeIn = fadeIn;
		element.Components.Add(label);

		var rect = cui.TakeFromPoolRect();
		rect.AnchorMin = $"{xMin} {yMin}";
		rect.AnchorMax = $"{xMax} {yMax}";
		element.Components.Add(rect);

		if (needsCursor) element.Components.Add(cui.TakeFromPoolNeedsCursor());
		if (needsKeyboard) element.Components.Add(cui.TakeFromPoolNeedsKeyboard());

		container.Add(element);
		return id;
	}
	public static string Button(this CUI.Handler cui, CuiElementContainer container, string parent, string id, string color, string textColor, string text, int size, float xMin, float xMax, float yMin, float yMax, string command, TextAnchor align, FontTypes font, bool @protected, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false)
	{
		if (id == null) id = cui.AppendId();
		var buttonElement = cui.TakeFromPool(id, parent);
		buttonElement.FadeOut = fadeOut;

		var button = cui.TakeFromPoolButton();
		button.FadeIn = fadeIn;
		button.Color = color;
		button.Command = @protected ? UiCommandAttribute.Uniquify(command) : command;
		buttonElement.Components.Add(button);

		var rect = cui.TakeFromPoolRect();
		rect.AnchorMin = $"{xMin} {yMin}";
		rect.AnchorMax = $"{xMax} {yMax}";
		buttonElement.Components.Add(rect);

		if (needsCursor) buttonElement.Components.Add(cui.TakeFromPoolNeedsCursor());
		if (needsKeyboard) buttonElement.Components.Add(cui.TakeFromPoolNeedsKeyboard());

		container.Add(buttonElement);

		if (!string.IsNullOrEmpty(text))
		{
			var textElement = cui.TakeFromPool(cui.AppendId(), buttonElement.Name);

			var ptext = cui.TakeFromPoolText();
			ptext.Text = text;
			ptext.FontSize = size;
			ptext.Align = align;
			ptext.Color = textColor;
			ptext.Font = cui.GetFont(font);
			textElement.Components.Add(ptext);

			var prect = cui.TakeFromPoolRect();
			textElement.Components.Add(prect);

			container.Add(textElement);
		}

		return id;
	}
	public static string InputField(this CUI.Handler cui, CuiElementContainer container, string parent, string id, string color, string text, int size, int characterLimit, bool readOnly, float xMin, float xMax, float yMin, float yMax, string command, TextAnchor align, FontTypes font, bool @protected, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false)
	{
		if (id == null) id = cui.AppendId();
		var inputFieldElement = cui.TakeFromPool(id, parent);
		inputFieldElement.FadeOut = fadeOut;

		var inputField = cui.TakeFromPoolInputField();
		inputField.Color = color;
		inputField.Text = text;
		inputField.FontSize = size;
		inputField.Font = cui.GetFont(font);
		inputField.Align = align;
		inputField.CharsLimit = characterLimit;
		inputField.ReadOnly = readOnly;
		inputField.Command = @protected ? UiCommandAttribute.Uniquify(command) : command;
		inputFieldElement.Components.Add(inputField);

		if (needsCursor) inputFieldElement.Components.Add(cui.TakeFromPoolNeedsCursor());
		if (needsKeyboard && !inputField.ReadOnly) inputFieldElement.Components.Add(cui.TakeFromPoolNeedsKeyboard());

		var rect = cui.TakeFromPoolRect();
		rect.AnchorMin = $"{xMin} {yMin}";
		rect.AnchorMax = $"{xMax} {yMax}";
		inputFieldElement.Components.Add(rect);

		container.Add(inputFieldElement);

		return id;
	}
	public static string Image(this CUI.Handler cui, CuiElementContainer container, string parent, string id, string png, float xMin, float xMax, float yMin, float yMax, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false)
	{
		if (id == null) id = cui.AppendId();
		var element = cui.TakeFromPool(id, parent);
		element.FadeOut = fadeOut;

		var rawImage = cui.TakeFromPoolRawImage();
		rawImage.Png = png;
		rawImage.FadeIn = fadeIn;
		element.Components.Add(rawImage);

		var rect = cui.TakeFromPoolRect();
		rect.AnchorMin = $"{xMin} {yMin}";
		rect.AnchorMax = $"{xMax} {yMax}";
		element.Components.Add(rect);

		if (needsCursor) element.Components.Add(cui.TakeFromPoolNeedsCursor());
		if (needsKeyboard) element.Components.Add(cui.TakeFromPoolNeedsKeyboard());

		container.Add(element);
		return id;
	}
	public static string ItemImage(this CUI.Handler cui, CuiElementContainer container, string parent, string id, int itemID, float xMin, float xMax, float yMin, float yMax, float fadeIn = 0f, float fadeOut = 0f, bool needsCursor = false, bool needsKeyboard = false)
	{
		if (id == null) id = cui.AppendId();
		var element = cui.TakeFromPool(id, parent);
		element.FadeOut = fadeOut;

		var rawImage = cui.TakeFromPoolImage();
		rawImage.ItemId = itemID;
		rawImage.FadeIn = fadeIn;
		element.Components.Add(rawImage);

		var rect = cui.TakeFromPoolRect();
		rect.AnchorMin = $"{xMin} {yMin}";
		rect.AnchorMax = $"{xMax} {yMax}";
		element.Components.Add(rect);

		if (needsCursor) element.Components.Add(cui.TakeFromPoolNeedsCursor());
		if (needsKeyboard) element.Components.Add(cui.TakeFromPoolNeedsKeyboard());

		container.Add(element);
		return id;
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
