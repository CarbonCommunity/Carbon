using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UnityEngine.UI;

namespace Oxide.Game.Rust.Cui;

public class ComponentConverter : JsonConverter
{
	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { }

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		var jObject = JObject.Load(reader);
		var typeName = jObject["type"].ToString();
		var type = (Type)null;

		switch (typeName)
		{
			case "UnityEngine.UI.Text":
				type = typeof(CuiTextComponent);
				break;

			case "UnityEngine.UI.Image":
				type = typeof(CuiImageComponent);
				break;

			case "UnityEngine.UI.RawImage":
				type = typeof(CuiRawImageComponent);
				break;

			case "UnityEngine.UI.Button":
				type = typeof(CuiButtonComponent);
				break;

			case "UnityEngine.UI.Outline":
				type = typeof(CuiOutlineComponent);
				break;

			case "UnityEngine.UI.InputField":
				type = typeof(CuiInputFieldComponent);
				break;

			case "Countdown":
				type = typeof(CuiCountdownComponent);
				break;

			case "NeedsCursor":
				type = typeof(CuiNeedsCursorComponent);
				break;

			case "NeedsKeyboard":
				type = typeof(CuiNeedsKeyboardComponent);
				break;

			case "RectTransform":
				type = typeof(CuiRectTransformComponent);
				break;

			case "UnityEngine.UI.ScrollView":
				type = typeof(CuiScrollViewComponent);
				break;

			case "UnityEngine.UI.HorizontalLayoutGroup":
				type = typeof(CuiHorizontalLayoutGroupComponent);
				break;

			case "UnityEngine.UI.VerticalLayoutGroup":
				type = typeof(CuiVerticalLayoutGroupComponent);
				break;

			case "UnityEngine.UI.GridLayoutGroup":
				type = typeof(CuiGridLayoutGroupComponent);
				break;

			case "UnityEngine.UI.ContentSizeFitter":
				type = typeof(CuiContentSizeFitterComponent);
				break;

			case "UnityEngine.UI.LayoutElement":
				type = typeof(CuiLayoutElementComponent);
				break;

			case "Draggable":
				type = typeof(CuiDraggableComponent);
				break;

			case "Slot":
				type = typeof(CuiSlotComponent);
				break;

			default:
				return null;
		}

		var target = Activator.CreateInstance(type);
		serializer.Populate(jObject.CreateReader(), target);
		return target;
	}

	public override bool CanConvert(Type objectType) => objectType == typeof(ICuiComponent);

	public override bool CanWrite => false;
}

#region Components

public class CuiButtonComponent : ICuiComponent, ICuiColor, ICuiEnableable, ICuiGraphic
{
	public string Type => "UnityEngine.UI.Button";

	[JsonProperty("command")]
	public string Command { get; set; }

	[JsonProperty("close")]
	public string Close { get; set; }

	[JsonProperty("sprite")]
	public string Sprite { get; set; }

	[JsonProperty("material")]
	public string Material { get; set; }

	public string Color { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("imagetype")]
	public Image.Type ImageType { get; set; }

	[JsonProperty("normalColor")]
	public string NormalColor { get; set; }

	[JsonProperty("highlightedColor")]
	public string HighlightedColor { get; set; }

	[JsonProperty("pressedColor")]
	public string PressedColor { get; set; }

	[JsonProperty("selectedColor")]
	public string SelectedColor { get; set; }

	[JsonProperty("disabledColor")]
	public string DisabledColor { get; set; }

	[JsonProperty("colorMultiplier")]
	public float ColorMultiplier { get; set; }

	[JsonProperty("fadeDuration")]
	public float FadeDuration { get; set; }

	[JsonProperty("fadeIn")]
	public float FadeIn { get; set; }

	[JsonProperty("placeholderParentId")]
	public string PlaceholderParentId { get; set; }

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }
}
public class CuiElementContainer : List<CuiElement>
{
	public string Name { get; set; }

	public string Add(CuiButton button, string parent = "Hud", string name = null, string destroyUi = null)
	{
		if (string.IsNullOrEmpty(name))
		{
			name = CuiHelper.GetGuid();
		}
		Add(new CuiElement
		{
			Name = name,
			Parent = parent,
			FadeOut = button.FadeOut,
			Components =
			{
				button.Button,
				button.RectTransform
			},
			DestroyUi = destroyUi
		});
		if (!string.IsNullOrEmpty(button.Text.Text))
		{
			Add(new CuiElement
			{
				Parent = name,
				FadeOut = button.FadeOut,
				Components =
				{
					button.Text,
					new CuiRectTransformComponent()
				}
			});
		}
		return name;
	}

	public string Add(CuiLabel label, string parent = "Hud", string name = null, string destroyUi = null)
	{
		if (string.IsNullOrEmpty(name))
		{
			name = CuiHelper.GetGuid();
		}

		Add(new CuiElement
		{
			Name = name,
			Parent = parent,
			FadeOut = label.FadeOut,
			Components =
			{
				label.Text,
				label.RectTransform
			},
			DestroyUi = destroyUi
		});
		return name;
	}

	public string Add(CuiPanel panel, string parent = "Hud", string name = null, string destroyUi = null)
	{
		if (string.IsNullOrEmpty(name))
		{
			name = CuiHelper.GetGuid();
		}

		var cuiElement = new CuiElement
		{
			Name = name,
			Parent = parent,
			FadeOut = panel.FadeOut,
			DestroyUi = destroyUi
		};
		if (panel.Image != null)
		{
			cuiElement.Components.Add(panel.Image);
		}
		if (panel.RawImage != null)
		{
			cuiElement.Components.Add(panel.RawImage);
		}

		cuiElement.Components.Add(panel.RectTransform);

		if (panel.CursorEnabled)
		{
			cuiElement.Components.Add(new CuiNeedsCursorComponent());
		}

		if (panel.KeyboardEnabled)
		{
			cuiElement.Components.Add(new CuiNeedsKeyboardComponent());
		}

		Add(cuiElement);
		return name;
	}

	public string ToJson()
	{
		return this.ToString();
	}

	public override string ToString()
	{
		return CuiHelper.ToJson(this, false);
	}
}
public class CuiImageComponent : ICuiComponent, ICuiColor, ICuiEnableable, ICuiGraphic
{
	public string Type => "UnityEngine.UI.Image";

	[JsonProperty("sprite")]
	public string Sprite { get; set; }

	[JsonProperty("material")]
	public string Material { get; set; }

	public string Color { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("imagetype")]
	public Image.Type ImageType { get; set; }

	[JsonProperty("fillCenter")]
	public bool? FillCenter { get; set; }

	[JsonProperty("png")]
	public string Png { get; set; }

	[JsonProperty("slice")]
	public string Slice { get; set; }

	[JsonProperty("fadeIn")]
	public float FadeIn { get; set; }

	[JsonProperty("itemid")]
	public int ItemId { get; set; }

	[JsonProperty("skinid")]
	public ulong SkinId { get; set; }

	[JsonProperty("placeholderParentId")]
	public string PlaceholderParentId { get; set; }

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }

}
public class CuiInputFieldComponent : ICuiComponent, ICuiColor, ICuiEnableable, ICuiGraphic
{
	public string Type => "UnityEngine.UI.InputField";

	[JsonProperty("text")]
	public string Text { get; set; } = string.Empty;

	[JsonProperty("fontSize")]
	public int FontSize { get; set; }

	[JsonProperty("font")]
	public string Font { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("align")]
	public TextAnchor Align { get; set; }

	public string Color { get; set; }

	[JsonProperty("characterLimit")]
	public int CharsLimit { get; set; }

	[JsonProperty("command")]
	public string Command { get; set; }

	[JsonProperty("password", DefaultValueHandling = DefaultValueHandling.Include)]
	public bool IsPassword { get; set; }

	[JsonProperty("readOnly", DefaultValueHandling = DefaultValueHandling.Include)]
	public bool ReadOnly { get; set; }

	[JsonProperty("placeholderId")]
	public string PlaceholderId { get; set; }

	[JsonProperty("needsKeyboard", DefaultValueHandling = DefaultValueHandling.Include)]
	public bool NeedsKeyboard { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("lineType")]
	public InputField.LineType LineType { get; set; }

	[JsonProperty("autofocus")]
	public bool Autofocus { get; set; }

	[JsonProperty("hudMenuInput", DefaultValueHandling = DefaultValueHandling.Include)]
	public bool HudMenuInput { get; set; }

	[JsonProperty("fadeIn")]
	public float FadeIn { get; set; }

	[JsonProperty("placeholderParentId")]
	public string PlaceholderParentId { get; set; }

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }

}
public class CuiNeedsCursorComponent : ICuiComponent, ICuiEnableable
{
	public string Type => "NeedsCursor";

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }
}
public class CuiNeedsKeyboardComponent : ICuiComponent, ICuiEnableable
{
	public string Type => "NeedsKeyboard";

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }
}
public class CuiOutlineComponent : ICuiComponent, ICuiColor, ICuiEnableable
{
	public string Type => "UnityEngine.UI.Outline";

	public string Color { get; set; }

	[JsonProperty("distance")]
	public string Distance { get; set; }

	[JsonProperty("useGraphicAlpha")]
	public bool UseGraphicAlpha { get; set; }

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }

}
public class CuiRawImageComponent : ICuiComponent, ICuiColor, ICuiEnableable, ICuiGraphic
{
	public string Type => "UnityEngine.UI.RawImage";

	[JsonProperty("sprite")]
	public string Sprite { get; set; }

	public string Color { get; set; }

	[JsonProperty("material")]
	public string Material { get; set; }

	[JsonProperty("url")]
	public string Url { get; set; }

	[JsonProperty("png")]
	public string Png { get; set; }

	[JsonProperty("fadeIn")]
	public float FadeIn { get; set; }

	[JsonProperty("steamid")]
	public string SteamId { get; set; }

	[JsonProperty("placeholderParentId")]
	public string PlaceholderParentId { get; set; }

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }
}
public class CuiRectTransformComponent : CuiRectTransform, ICuiComponent
{
	public string Type => "RectTransform";

}
public class CuiRectTransform
{
	[JsonProperty("anchormin")]
	public string AnchorMin { get; set; }

	[JsonProperty("anchormax")]
	public string AnchorMax { get; set; }

	[JsonProperty("offsetmin")]
	public string OffsetMin { get; set; }

	[JsonProperty("offsetmax")]
	public string OffsetMax { get; set; }

	[JsonProperty("rotation")]
	public float Rotation { get; set; }

	[JsonProperty("pivot")]
	public string Pivot { get; set; }

	[JsonProperty("setParent")]
	public string SetParent { get; set; }

	[JsonProperty("setTransformIndex")]
	[DefaultValue(-1)]
	public int SetTransformIndex { get; set; } = -1;
}
public class CuiCountdownComponent : ICuiComponent, ICuiEnableable
{
	public string Type => "Countdown";

	[JsonProperty("endTime")]
	public float EndTime { get; set; }

	[JsonProperty("startTime")]
	public float StartTime { get; set; }

	[JsonProperty("step")]
	public float Step { get; set; }

	[JsonProperty("interval")]
	public float Interval { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("timerFormat")]
	public TimerFormat TimerFormat { get; set; }

	[JsonProperty("numberFormat")]
	public string NumberFormat { get; set; }

	[JsonProperty("destroyIfDone", DefaultValueHandling = DefaultValueHandling.Include)]
	public bool DestroyIfDone { get; set; }

	[JsonProperty("command")]
	public string Command { get; set; }

	[JsonProperty("fadeIn")]
	public float FadeIn { get; set; }

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }
}

public enum TimerFormat
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
}

public class CuiTextComponent : ICuiComponent, ICuiColor, ICuiEnableable, ICuiGraphic
{
	public string Type => "UnityEngine.UI.Text";

	[JsonProperty("text")]
	public string Text { get; set; }

	[JsonProperty("fontSize")]
	public int FontSize { get; set; }

	[JsonProperty("font")]
	public string Font { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("align")]
	public TextAnchor Align { get; set; }

	public string Color { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("verticalOverflow")]
	public VerticalWrapMode VerticalOverflow { get; set; }

	[JsonProperty("fadeIn")]
	public float FadeIn { get; set; }

	[JsonProperty("placeholderParentId")]
	public string PlaceholderParentId { get; set; }

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }
}
public class CuiScrollViewComponent : ICuiComponent, ICuiEnableable
{
	public string Type => "UnityEngine.UI.ScrollView";

	[JsonProperty("vertical", DefaultValueHandling = DefaultValueHandling.Include)]
	public bool Vertical { get; set; }

	[JsonProperty("horizontal", DefaultValueHandling = DefaultValueHandling.Include)]
	public bool Horizontal { get; set; }

	[JsonProperty("movementType")]
	[JsonConverter(typeof(StringEnumConverter))]
	public ScrollRect.MovementType MovementType { get; set; }

	[JsonProperty("elasticity")]
	public float Elasticity { get; set; }

	[JsonProperty("inertia", DefaultValueHandling = DefaultValueHandling.Include)]
	public bool Inertia { get; set; }

	[JsonProperty("decelerationRate")]
	public float DecelerationRate { get; set; }

	[JsonProperty("scrollSensitivity")]
	public float ScrollSensitivity { get; set; }

	[JsonProperty("contentTransform")]
	public CuiRectTransform ContentTransform { get; set; }

	[JsonProperty("horizontalScrollbar")]
	public CuiScrollbar HorizontalScrollbar { get; set; }

	[JsonProperty("verticalScrollbar")]
	public CuiScrollbar VerticalScrollbar { get; set; }

	[JsonProperty("horizontalNormalizedPosition")]
	public float? HorizontalNormalizedPosition { get; set; }

	[JsonProperty("verticalNormalizedPosition")]
	public float? VerticalNormalizedPosition { get; set; }

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }
}
public class CuiScrollbar : ICuiComponent, ICuiEnableable
{
	public string Type => "UnityEngine.UI.Scrollbar";

	[JsonProperty("invert")]
	public bool Invert { get; set; }

	[JsonProperty("autoHide")]
	public bool AutoHide { get; set; }

	[JsonProperty("handleSprite")]
	public string HandleSprite { get; set; }

	[JsonProperty("size")]
	public float Size { get; set; }

	[JsonProperty("handleColor")]
	public string HandleColor { get; set; }

	[JsonProperty("highlightColor")]
	public string HighlightColor { get; set; }

	[JsonProperty("pressedColor")]
	public string PressedColor { get; set; }

	[JsonProperty("trackSprite")]
	public string TrackSprite { get; set; }

	[JsonProperty("trackColor")]
	public string TrackColor { get; set; }

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }
}
public abstract class CuiLayoutGroupComponent : ICuiComponent, ICuiEnableable
{
	public abstract string Type { get; }

	[JsonProperty("spacing")]
	public float Spacing { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("childAlignment")]
	public TextAnchor ChildAlignment { get; set; }

	[JsonProperty("childForceExpandWidth")]
	public bool? ChildForceExpandWidth { get; set; }

	[JsonProperty("childForceExpandHeight")]
	public bool? ChildForceExpandHeight { get; set; }

	[JsonProperty("childControlWidth")]
	public bool? ChildControlWidth { get; set; }

	[JsonProperty("childControlHeight")]
	public bool? ChildControlHeight { get; set; }

	[JsonProperty("childScaleWidth")]
	public bool? ChildScaleWidth { get; set; }

	[JsonProperty("childScaleHeight")]
	public bool? ChildScaleHeight { get; set; }

	[JsonProperty("padding")]
	public string Padding { get; set; }

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }
}
public class CuiHorizontalLayoutGroupComponent : CuiLayoutGroupComponent
{
	public override string Type => "UnityEngine.UI.HorizontalLayoutGroup";
}
public class CuiVerticalLayoutGroupComponent : CuiLayoutGroupComponent
{
	public override string Type => "UnityEngine.UI.VerticalLayoutGroup";
}
public class CuiGridLayoutGroupComponent : ICuiComponent, ICuiEnableable
{
	public string Type => "UnityEngine.UI.GridLayoutGroup";

	[JsonProperty("cellSize")]
	public string CellSize { get; set; }

	[JsonProperty("spacing")]
	public string Spacing { get; set; }

	[JsonProperty("startCorner")]
	[JsonConverter(typeof(StringEnumConverter))]
	public GridLayoutGroup.Corner StartCorner { get; set; }

	[JsonProperty("startAxis")]
	[JsonConverter(typeof(StringEnumConverter))]
	public GridLayoutGroup.Axis StartAxis { get; set; }

	[JsonProperty("childAlignment")]
	[JsonConverter(typeof(StringEnumConverter))]
	public TextAnchor ChildAlignment { get; set; }

	[JsonProperty("constraint")]
	[JsonConverter(typeof(StringEnumConverter))]
	public GridLayoutGroup.Constraint Constraint { get; set; }

	[JsonProperty("constraintCount")]
	public int ConstraintCount { get; set; }

	[JsonProperty("padding")]
	public string Padding { get; set; }

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }
}
public class CuiContentSizeFitterComponent : ICuiComponent, ICuiEnableable
{
	public string Type => "UnityEngine.UI.ContentSizeFitter";

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("horizontalFit")]
	public ContentSizeFitter.FitMode HorizontalFit { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]
	[JsonProperty("verticalFit")]
	public ContentSizeFitter.FitMode VerticalFit { get; set; }

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }
}
public class CuiLayoutElementComponent : ICuiComponent, ICuiEnableable
{
	public string Type => "UnityEngine.UI.LayoutElement";

	[JsonProperty("preferredWidth")]
	public float PreferredWidth { get; set; }

	[JsonProperty("preferredHeight")]
	public float PreferredHeight { get; set; }

	[JsonProperty("minWidth")]
	public float MinWidth { get; set; }

	[JsonProperty("minHeight")]
	public float MinHeight { get; set; }

	[JsonProperty("flexibleWidth")]
	public float FlexibleWidth { get; set; }

	[JsonProperty("flexibleHeight")]
	public float FlexibleHeight { get; set; }

	[JsonProperty("ignoreLayout")]
	public bool? IgnoreLayout { get; set; }

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }
}
public class CuiDraggableComponent : ICuiComponent, ICuiEnableable
{
	public string Type => "Draggable";

	[JsonProperty("limitToParent")]
	public bool? LimitToParent { get; set; }

	[JsonProperty("maxDistance")]
	public float MaxDistance { get; set; }

	[JsonProperty("allowSwapping")]
	public bool? AllowSwapping { get; set; }

	[JsonProperty("dropAnywhere")]
	public bool? DropAnywhere { get; set; }

	[JsonProperty("dragAlpha")]
	public float DragAlpha { get; set; }

	[JsonProperty("parentLimitIndex")]
	public int ParentLimitIndex { get; set; }

	[JsonProperty("filter")]
	public string Filter { get; set; }

	[JsonProperty("parentPadding")]
	public string ParentPadding { get; set; }

	[JsonProperty("anchorOffset")]
	public string AnchorOffset { get; set; }

	[JsonProperty("keepOnTop")]
	public bool? KeepOnTop { get; set; }

	[JsonConverter(typeof(StringEnumConverter))]

	[JsonProperty("positionRPC")]
	public CommunityEntity.DraggablePositionSendType PositionRPC { get; set; }

	[JsonProperty("moveToAnchor")]
	public bool MoveToAnchor { get; set; }

	[JsonProperty("rebuildAnchor")]
	public bool RebuildAnchor { get; set; }

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }
}
public class CuiSlotComponent : ICuiComponent, ICuiEnableable
{
	public string Type => "Slot";

	[JsonProperty("filter")]
	public string Filter { get; set; }

	[JsonProperty("enabled")]
	public bool? Enabled { get; set; }
}
#endregion

public class CuiButton
{
	public CuiButtonComponent Button { get; } = new();
	public CuiRectTransformComponent RectTransform { get; } = new();
	public CuiTextComponent Text { get; } = new();
	public float FadeOut { get; set; }
}
public class CuiElement
{
	[JsonProperty("name")]
	public string Name { get; set; }

	[JsonProperty("parent")]
	public string Parent { get; set; }

	[JsonProperty("components")]
	public List<ICuiComponent> Components { get; } = new();

	[JsonProperty("destroyUi")]
	public string DestroyUi { get; set; }

	[JsonProperty("fadeOut")]
	public float FadeOut { get; set; }

	[JsonProperty("update")]
	public bool Update { get; set; }

	[JsonProperty("activeSelf")]
	public bool? ActiveSelf { get; set; }
}

public interface ICuiGraphic
{
	[JsonProperty("fadeIn")]
	float FadeIn { get; set; }

	[JsonProperty("placeholderParentId")]
	string PlaceholderParentId { get; set; }
}
public interface ICuiEnableable
{
	[JsonProperty("enabled")]
	bool? Enabled { get; set; }
}
public class CuiLabel
{
	public CuiTextComponent Text { get; } = new();
	public CuiRectTransformComponent RectTransform { get; } = new();
	public float FadeOut { get; set; }
}
public class CuiPanel
{
	public CuiImageComponent Image { get; set; } = new CuiImageComponent();
	public CuiRawImageComponent RawImage { get; set; }
	public CuiRectTransformComponent RectTransform { get; } = new CuiRectTransformComponent();
	public bool KeyboardEnabled { get; set; }
	public bool CursorEnabled { get; set; }
	public float FadeOut { get; set; }
}
