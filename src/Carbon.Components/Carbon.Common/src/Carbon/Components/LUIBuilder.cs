using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace Carbon.Components;

public static class LUIBuilder
{
	#region Old CUI Improvements

	private const int floatStringBuffer = 32;

	public static string GetStringFloat(float value)
	{
		Span<char> buffer = stackalloc char[floatStringBuffer];
		int written = WriteFloatDigits(value, buffer);
		return MakeString(buffer, written);
	}

	public static string GetStringFloat(float x, float y)
	{
		Span<char> buffer = stackalloc char[floatStringBuffer];
		int index = WriteFloatDigits(x, buffer);
		buffer[index++] = whitespace;
		index += WriteFloatDigits(y, buffer[index..]);
		return MakeString(buffer, index);
	}

	public static string GetStringFloat(float x, float y, float z)
	{
		Span<char> buffer = stackalloc char[floatStringBuffer];
		int index = WriteFloatDigits(x, buffer);
		buffer[index++] = whitespace;
		index += WriteFloatDigits(y, buffer[index..]);
		buffer[index++] = whitespace;
		index += WriteFloatDigits(z, buffer[index..]);
		return MakeString(buffer, index);
	}

	public static string GetStringFloat(float x, float y, float z, float w)
	{
		Span<char> buffer = stackalloc char[floatStringBuffer];
		int index = WriteFloatDigits(x, buffer);
		buffer[index++] = whitespace;
		index += WriteFloatDigits(y, buffer[index..]);
		buffer[index++] = whitespace;
		index += WriteFloatDigits(z, buffer[index..]);
		buffer[index++] = whitespace;
		index += WriteFloatDigits(w, buffer[index..]);
		return MakeString(buffer, index);
	}

	public static string GetStringFloat(params float[] values)
	{
		Span<char> buffer = values.Length * 9 <= floatStringBuffer
			? stackalloc char[floatStringBuffer]
			: new char[values.Length * 9];
		int index = 0;
		for (int i = 0; i < values.Length; i++)
		{
			index += WriteFloatDigits(values[i], buffer[index..]);
			if (i < values.Length - 1)
				buffer[index++] = whitespace;
		}
		return MakeString(buffer, index);
	}

	public static string BuildElementId(string elementId, int id)
	{
		Span<char> buffer = stackalloc char[elementId.Length + 1 + maxIntChars];
		elementId.AsSpan().CopyTo(buffer);
		buffer[elementId.Length] = under;
		int written = elementId.Length + 1 + WriteIntDigits(id, buffer[(elementId.Length + 1)..]);
		return MakeString(buffer, written);
	}

	private static unsafe string MakeString(Span<char> buffer, int length)
	{
		fixed (char* ptr = buffer)
			return new string(ptr, 0, length);
	}

	#endregion

	#region Field Builders

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteStartObject(this ref LuiBuilderInstance inst)
	{
		inst.Reserve(1);
		inst.Put(startObject);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteEndObject(this ref LuiBuilderInstance inst)
	{
		inst.Reserve(1);
		inst.Put(endObject);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteStartArray(this ref LuiBuilderInstance inst)
	{
		inst.Reserve(1);
		inst.Put(startArray);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteEndArray(this ref LuiBuilderInstance inst)
	{
		inst.Reserve(1);
		inst.Put(endArray);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteComma(this ref LuiBuilderInstance inst)
	{
		inst.Reserve(1);
		inst.Put(coma);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteMark(this ref LuiBuilderInstance inst)
	{
		inst.Reserve(1);
		inst.Put(mark);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteDots(this ref LuiBuilderInstance inst)
	{
		inst.Reserve(1);
		inst.Put(dots);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteSpace(this ref LuiBuilderInstance inst)
	{
		inst.Reserve(1);
		inst.Put(whitespace);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteStartObject(this ref LuiBuilderInstance inst, string key)
	{
		inst.Reserve(key.Length + 4);
		inst.PutKey(key);
		inst.Put(startObject);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteStartArray(this ref LuiBuilderInstance inst, string key)
	{
		inst.Reserve(key.Length + 4);
		inst.PutKey(key);
		inst.Put(startArray);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteField(this ref LuiBuilderInstance inst, string key, int value)
	{
		inst.Reserve(key.Length + 3 + maxIntChars);
		inst.PutKey(key);
		inst.PutInt(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteField(this ref LuiBuilderInstance inst, string key, Vector2 value)
	{
		inst.Reserve(key.Length + 6 + maxFloatChars * 2);
		inst.PutKey(key);
		inst.Put(mark);
		inst.PutFloat(value.x);
		inst.Put(whitespace);
		inst.PutFloat(value.y);
		inst.Put(mark);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteField(this ref LuiBuilderInstance inst, string key, float value)
	{
		inst.Reserve(key.Length + 3 + maxFloatChars);
		inst.PutKey(key);
		inst.PutFloat(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteField(this ref LuiBuilderInstance inst, string key, ulong value)
	{
		inst.Reserve(key.Length + 3 + maxUlongChars);
		inst.PutKey(key);
		inst.PutUlong(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteField(this ref LuiBuilderInstance inst, string key, bool value)
	{
		inst.Reserve(key.Length + 8);
		inst.PutKey(key);
		inst.Put(value ? trueValue : falseValue);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteField(this ref LuiBuilderInstance inst, string key, string value)
	{
		inst.Reserve(key.Length + value.Length + 5);
		inst.PutKey(key);
		inst.Put(mark);
		inst.Put(value);
		inst.Put(mark);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteFieldId(this ref LuiBuilderInstance inst, string key, string identifier, int id)
	{
		inst.Reserve(key.Length + identifier.Length + 6 + maxIntChars);
		inst.PutKey(key);
		inst.Put(mark);
		inst.Put(identifier);
		inst.Put(under);
		inst.PutInt(id);
		inst.Put(mark);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteFieldIdComma(this ref LuiBuilderInstance inst, string key, string identifier, int id)
	{
		inst.Reserve(key.Length + identifier.Length + 7 + maxIntChars);
		inst.Put(coma);
		inst.PutKey(key);
		inst.Put(mark);
		inst.Put(identifier);
		inst.Put(under);
		inst.PutInt(id);
		inst.Put(mark);
	}


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteFieldComma(this ref LuiBuilderInstance inst, string key, int value)
	{
		inst.Reserve(key.Length + 4 + maxIntChars);
		inst.Put(coma);
		inst.PutKey(key);
		inst.PutInt(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteFieldComma(this ref LuiBuilderInstance inst, string key, Vector2 value)
	{
		inst.Reserve(key.Length + 7 + maxFloatChars * 2);
		inst.Put(coma);
		inst.PutKey(key);
		inst.Put(mark);
		inst.PutFloat(value.x);
		inst.Put(whitespace);
		inst.PutFloat(value.y);
		inst.Put(mark);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteFieldComma(this ref LuiBuilderInstance inst, string key, float value)
	{
		inst.Reserve(key.Length + 4 + maxFloatChars);
		inst.Put(coma);
		inst.PutKey(key);
		inst.PutFloat(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteFieldComma(this ref LuiBuilderInstance inst, string key, ulong value)
	{
		inst.Reserve(key.Length + 4 + maxUlongChars);
		inst.Put(coma);
		inst.PutKey(key);
		inst.PutUlong(value);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteFieldComma(this ref LuiBuilderInstance inst, string key, bool value)
	{
		inst.Reserve(key.Length + 9);
		inst.Put(coma);
		inst.PutKey(key);
		inst.Put(value ? trueValue : falseValue);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteFieldComma(this ref LuiBuilderInstance inst, string key, string value)
	{
		inst.Reserve(key.Length + value.Length + 6);
		inst.Put(coma);
		inst.PutKey(key);
		inst.Put(mark);
		inst.Put(value);
		inst.Put(mark);
	}

	#endregion

	#region Builders

	private const char startObject = '{';
	private const char endObject = '}';
	private const char startArray = '[';
	private const char endArray = ']';
	private const char coma = ',';
	private const char mark = '\"';
	private const char dots = ':';
	private const char under = '_';
	private const char minus = '-';
	private const char zero = '0';
	private const char dot = '.';
	private const char whitespace = ' ';
	private const float zeroFloat = 0;
	private const string trueValue = "true";
	private const string falseValue = "false";

	private const int maxIntChars = 11;
	private const int maxUlongChars = 20;
	private const int maxFloatChars = 24;

	private static readonly long[] _pow10 = {
		1L, 10L, 100L, 1_000L, 10_000L, 100_000L,
		1_000_000L, 10_000_000L, 100_000_000L, 1_000_000_000L
	};

	/// <summary>
	/// Makes sure at least <paramref name="required"/> more chars fit. Growing is the cold path and
	/// stays out of line so the common case is a single compare.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static void Reserve(this ref LuiBuilderInstance inst, int required)
	{
		if (inst._charIndex + required > inst._charBuffer.Length)
			inst.Grow(required);
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	private static void Grow(this ref LuiBuilderInstance inst, int required)
	{
		int wanted = Math.Max(inst._charBuffer.Length * 2, inst._charIndex + required);
		char[] next = ArrayPool<char>.Shared.Rent(wanted);
		inst._charBuffer.AsSpan(0, inst._charIndex).CopyTo(next);
		ArrayPool<char>.Shared.Return(inst._charBuffer);
		inst._charBuffer = next;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Put(this ref LuiBuilderInstance inst, char character)
	{
		inst._charBuffer[inst._charIndex++] = character;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void Put(this ref LuiBuilderInstance inst, string text)
	{
		text.AsSpan().CopyTo(inst._charBuffer.AsSpan(inst._charIndex));
		inst._charIndex += text.Length;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void PutKey(this ref LuiBuilderInstance inst, string key)
	{
		Span<char> span = inst._charBuffer.AsSpan(inst._charIndex);
		span[0] = mark;
		key.AsSpan().CopyTo(span[1..]);
		int index = key.Length + 1;
		span[index++] = mark;
		span[index++] = dots;
		inst._charIndex += index;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void PutInt(this ref LuiBuilderInstance inst, int value)
	{
		inst._charIndex += WriteIntDigits(value, inst._charBuffer.AsSpan(inst._charIndex));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void PutFloat(this ref LuiBuilderInstance inst, float value)
	{
		inst._charIndex += WriteFloatDigits(value, inst._charBuffer.AsSpan(inst._charIndex));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static void PutUlong(this ref LuiBuilderInstance inst, ulong value)
	{
		inst._charIndex += WriteUlongDigits(value, inst._charBuffer.AsSpan(inst._charIndex));
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int WriteFloatDigits(float value, Span<char> buffer, int startIndex = 0, int precision = 3)
	{
		int startsAt = startIndex;
		int index = startIndex;
		if (value == zeroFloat)
		{
			buffer[index++] = zero;
			return index - startsAt;
		}
		if (value < 0)
		{
			buffer[index++] = minus;
			value = -value;
		}
		int intPart = (int)value;
		float fractional = value - intPart;
		long factor = _pow10[precision];
		long fracAsInt = (long)(fractional * factor + 0.5f);
		if (fracAsInt == factor)
		{
			intPart++;
			fracAsInt = 0;
		}
		index += WriteIntDigits(intPart, buffer[index..]);
		if (fracAsInt == 0)
			return index - startsAt;
		buffer[index++] = dot;
		int fracStart = index;
		for (int i = 0; i < precision; i++)
		{
			buffer[index + precision - i - 1] = (char)(zero + (fracAsInt % 10));
			fracAsInt /= 10;
		}
		index += precision;
		int lastNonZero = index - 1;
		while (lastNonZero >= fracStart && buffer[lastNonZero] == zero)
			lastNonZero--;
		if (lastNonZero < fracStart)
			index = fracStart - 1;
		else
			index = lastNonZero + 1;
		return index - startsAt;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int WriteUlongDigits(ulong value, Span<char> buffer)
	{
		if (value == 0)
		{
			buffer[0] = zero;
			return 1;
		}
		int index = 0;
		while (value > 0)
		{
			buffer[index++] = (char)(zero + (value % 10));
			value /= 10;
		}
		for (int i = 0, j = index - 1; i < j; i++, j--)
			(buffer[i], buffer[j]) = (buffer[j], buffer[i]);
		return index;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int WriteIntDigits(int value, Span<char> buffer)
	{
		if (value == 0)
		{
			buffer[0] = zero;
			return 1;
		}
		int index = 0;
		bool isBelow = value < 0;
		if (isBelow)
			value = -value;
		while (value > 0)
		{
			buffer[index++] = (char)(zero + (value % 10));
			value /= 10;
		}
		if (isBelow)
			buffer[index++] = minus;
		for (int i = 0, j = index - 1; i < j; i++, j--)
			(buffer[i], buffer[j]) = (buffer[j], buffer[i]);
		return index;
	}

	#endregion
}

public struct LuiBuilderInstance : IDisposable
{
	/// <summary>
	/// Big enough that a typical UI never grows, and rented rather than allocated so a build costs
	/// nothing on the GC. Grows by doubling if a UI does turn out to be larger.
	/// </summary>
	private const int initialCapacity = 8192;

	internal char[] _charBuffer = ArrayPool<char>.Shared.Rent(initialCapacity);

	public int _charIndex = 0;

	private static string GetFieldName(LuiCompType type)
	{
		return type switch
		{
			LuiCompType.Text => "UnityEngine.UI.Text",
			LuiCompType.Image => "UnityEngine.UI.Image",
			LuiCompType.RawImage => "UnityEngine.UI.RawImage",
			LuiCompType.Button => "UnityEngine.UI.Button",
			LuiCompType.Outline => "UnityEngine.UI.Outline",
			LuiCompType.InputField => "UnityEngine.UI.InputField",
			LuiCompType.NeedsCursor => "NeedsCursor",
			LuiCompType.RectTransform => "RectTransform",
			LuiCompType.Countdown => "Countdown",
			LuiCompType.HorizontalLayoutGroup => "UnityEngine.UI.HorizontalLayoutGroup",
			LuiCompType.VerticalLayoutGroup => "UnityEngine.UI.VerticalLayoutGroup",
			LuiCompType.GridLayoutGroup => "UnityEngine.UI.GridLayoutGroup",
			LuiCompType.ContentSizeFitter => "UnityEngine.UI.ContentSizeFitter",
			LuiCompType.LayoutElement => "UnityEngine.UI.LayoutElement",
			LuiCompType.Draggable => "Draggable",
			LuiCompType.Slot  => "Slot",
			LuiCompType.NeedsKeyboard => "NeedsKeyboard",
			LuiCompType.ScrollView => "UnityEngine.UI.ScrollView",
			LuiCompType.CanvasGroup => "UnityEngine.UI.CanvasGroup",
			LuiCompType.Mask => "UnityEngine.UI.Mask",
			LuiCompType.Tooltip => "Tooltip",
			_ => "UnityEngine.UI.Image"
		};
	}

	/// <summary>
	/// Writes an element's name, using the raw identifier plus counter when it was auto-generated so
	/// no string has to be built for it.
	/// </summary>
	private static void WriteName(ref LuiBuilderInstance inst, LUI.LuiContainer element, bool comma)
	{
		if (element.nameId != 0)
		{
			if (comma) inst.WriteFieldIdComma("name", element.nameIdentifier, element.nameId);
			else inst.WriteFieldId("name", element.nameIdentifier, element.nameId);
			return;
		}

		if (comma) inst.WriteFieldComma("name", element._name);
		else inst.WriteField("name", element._name);
	}

	private static void WriteParent(ref LuiBuilderInstance inst, LUI.LuiContainer element, bool comma)
	{
		if (element.parentId != 0)
		{
			if (comma) inst.WriteFieldIdComma("parent", element.parentIdentifier, element.parentId);
			else inst.WriteFieldId("parent", element.parentIdentifier, element.parentId);
			return;
		}

		if (comma) inst.WriteFieldComma("parent", element._parent);
		else inst.WriteField("parent", element._parent);
	}

	public LuiBuilderInstance(LUI cui)
    {
        this.WriteStartArray();
        List<LUI.LuiContainer> elements = cui.elements;
        int elementCount = elements.Count;
        for (int elementIndex = 0; elementIndex < elementCount; elementIndex++)
        {
            LUI.LuiContainer element = elements[elementIndex];
            this.WriteStartObject();
            if (element.update)
            {
	            WriteName(ref this, element, false);
	            if (element.HasParent)
		            WriteParent(ref this, element, true);
            }
            else
            {
	            WriteParent(ref this, element, false);
	            if (element.HasName)
		            WriteName(ref this, element, true);
            }
            if (element.destroyUi != null)
	            this.WriteFieldComma("destroyUi", element.destroyUi);
            if (element.fadeOut > 0)
	            this.WriteFieldComma("fadeOut", element.fadeOut);
            if (element.update)
	            this.WriteFieldComma("update", true);
            if (element.activeSelfSet || !element.activeSelf)
	            this.WriteFieldComma("activeSelf", element.activeSelf);
            if (element.luiComponents.Count > 0)
            {
	            this.WriteComma();
	            this.WriteStartArray("components");
                LuiComponentDictionary components = element.luiComponents;
                int componentCount = components.Count;
                for (int componentIndex = 0; componentIndex < componentCount; componentIndex++)
                {
	                LuiCompBase component = components[componentIndex];
	                this.WriteStartObject();
	                this.WriteField("type", GetFieldName(component.type));
	                if (!component.enabled)
		                this.WriteFieldComma("enabled", false);
	                if (component.fadeIn > 0)
		                this.WriteFieldComma("fadeIn", component.fadeIn);
	                if (!component.blocksRaycast)
		                this.WriteFieldComma("blocksRaycast", false);
	                if (component.placeholderParentId != null)
		                this.WriteFieldComma("placeholderParentId", component.placeholderParentId);
                    switch (component.type)
                    {
	                    case LuiCompType.Text:
		                    LuiTextComp text = component as LuiTextComp;
		                    if (text.text != null)
			                    this.WriteFieldComma("text", text.text);
		                    if (text.fontSize > 0)
			                    this.WriteFieldComma("fontSize", text.fontSize);
		                    if (text.font != null)
			                    this.WriteFieldComma("font", text.font);
		                    if (text.align != null)
			                    this.WriteFieldComma("align", text.align);
		                    if (text.color != null)
			                    this.WriteFieldComma("color", text.color);
		                    if (text.verticalOverflow != null)
			                    this.WriteFieldComma("verticalOverflow", text.verticalOverflow);
		                    break;
	                    case LuiCompType.Image:
		                    LuiImageComp image = component as LuiImageComp;
		                    if (image.sprite != null)
			                    this.WriteFieldComma("sprite", image.sprite);
		                    if (image.material != null)
			                    this.WriteFieldComma("material", image.material);
		                    if (image.color != null)
			                    this.WriteFieldComma("color", image.color);
		                    if (image.imageType != null)
			                    this.WriteFieldComma("imagetype", image.imageType);
		                    if (image.fillCenter)
			                    this.WriteFieldComma("fillCenter", true);
		                    if (image.ppuMultiplier != -1)
			                    this.WriteFieldComma("ppuMultiplier", image.ppuMultiplier);
		                    if (image.png != null)
			                    this.WriteFieldComma("png", image.png);
		                    if (image.slice != null)
			                    this.WriteFieldComma("slice", image.slice);
		                    if (image.itemid != 0)
			                    this.WriteFieldComma("itemid", image.itemid);
		                    if (image.skinid != 0)
			                    this.WriteFieldComma("skinid", image.skinid);
		                    break;
	                    case LuiCompType.RawImage:
		                    LuiRawImageComp rawImage = component as LuiRawImageComp;
		                    if (rawImage.sprite != null)
			                    this.WriteFieldComma("sprite", rawImage.sprite);
		                    if (rawImage.color != null)
			                    this.WriteFieldComma("color", rawImage.color);
		                    if (rawImage.material != null)
			                    this.WriteFieldComma("material", rawImage.material);
		                    if (rawImage.url != null)
			                    this.WriteFieldComma("url", rawImage.url);
		                    if (rawImage.png != null)
			                    this.WriteFieldComma("png", rawImage.png);
		                    if (rawImage.steamid != null)
			                    this.WriteFieldComma("steamid", rawImage.steamid);
		                    break;
	                    case LuiCompType.Button:
		                    LuiButtonComp button = component as LuiButtonComp;
		                    if (!button.interactable)
			                    this.WriteFieldComma("interactable", false);
		                    if (button.command != null)
			                    this.WriteFieldComma("command", button.command);
		                    if (button.close != null)
			                    this.WriteFieldComma("close", button.close);
		                    if (button.sprite != null)
			                    this.WriteFieldComma("sprite", button.sprite);
		                    if (button.material != null)
			                    this.WriteFieldComma("material", button.material);
		                    if (button.color != null)
			                    this.WriteFieldComma("color", button.color);
		                    if (button.imageType != null)
			                    this.WriteFieldComma("imagetype", button.imageType);
		                    if (button.normalColor != null)
			                    this.WriteFieldComma("normalColor", button.normalColor);
		                    if (button.highlightedColor != null)
			                    this.WriteFieldComma("highlightedColor", button.highlightedColor);
		                    if (button.pressedColor != null)
			                    this.WriteFieldComma("pressedColor", button.pressedColor);
		                    if (button.selectedColor != null)
			                    this.WriteFieldComma("selectedColor", button.selectedColor);
		                    if (button.disabledColor != null)
			                    this.WriteFieldComma("disabledColor", button.disabledColor);
		                    if (button.colorMultiplier != -1)
			                    this.WriteFieldComma("colorMultiplier", button.colorMultiplier);
		                    if (button.fadeDuration != -1)
			                    this.WriteFieldComma("fadeDuration", button.fadeDuration);
		                    break;
	                    case LuiCompType.Outline:
		                    LuiOutlineComp outline = component as LuiOutlineComp;
		                    if (outline.color != null)
			                    this.WriteFieldComma("color", outline.color);
		                    if (outline.distance != default)
			                    this.WriteFieldComma("distance", outline.distance);
		                    if (outline.useGraphicAlpha)
			                    this.WriteFieldComma("useGraphicAlpha", true);
		                    break;
	                    case LuiCompType.InputField:
		                    LuiInputComp input = component as LuiInputComp;
		                    if (input.fontSize > 0)
			                    this.WriteFieldComma("fontSize", input.fontSize);
		                    if (input.font != null)
			                    this.WriteFieldComma("font", input.font);
		                    if (input.align != null)
			                    this.WriteFieldComma("align", input.align);
		                    if (input.color != null)
			                    this.WriteFieldComma("color", input.color);
		                    if (input.characterLimit > 0)
			                    this.WriteFieldComma("characterLimit", input.characterLimit);
		                    if (input.command != null)
			                    this.WriteFieldComma("command", input.command);
		                    if (input.lineType != null)
			                    this.WriteFieldComma("lineType", input.lineType);
		                    if (input.text != null)
			                    this.WriteFieldComma("text", input.text);
		                    if (input.readOnly)
			                    this.WriteFieldComma("readOnly", true);
		                    if (input.placeholderId != null)
			                    this.WriteFieldComma("placeholderId", input.placeholderId);
		                    if (input.password)
			                    this.WriteFieldComma("password", true);
		                    if (input.needsKeyboard)
			                    this.WriteFieldComma("needsKeyboard", true);
		                    if (input.hudMenuInput)
			                    this.WriteFieldComma("hudMenuInput", true);
		                    if (input.autofocus)
			                    this.WriteFieldComma("autofocus", true);
		                    if (!input.interactable)
			                    this.WriteFieldComma("interactable", false);
		                    break;
	                    case LuiCompType.NeedsCursor:
		                    break;
                        case LuiCompType.RectTransform:
	                        LuiRectTransformComp rect = component as LuiRectTransformComp;
	                        if (rect.anchor != LuiPosition.Full)
	                        {
		                        this.WriteFieldComma("anchormin", rect.anchor.anchorMin);
		                        this.WriteFieldComma("anchormax", rect.anchor.anchorMax);
	                        }
	                        this.WriteComma(); //Always adding offset, as RUST UI have weird one pixel offset by default, idk who came to this idea lol.
                            this.WriteField("offsetmin", rect.offset.offsetMin);
	                        this.WriteFieldComma("offsetmax", rect.offset.offsetMax);
	                        if (rect.rotation != 0)
		                        this.WriteFieldComma("rotation", rect.rotation);
	                        if (rect.setParent != null)
		                        this.WriteFieldComma("setParent", rect.setParent);
	                        if (rect.setTransformIndex != -1)
		                        this.WriteFieldComma("setTransformIndex", rect.setTransformIndex);
                            break;
	                    case LuiCompType.Countdown:
		                    LuiCountdownComp countdown = component as LuiCountdownComp;
		                    if (countdown.endTime != -1)
			                    this.WriteFieldComma("endTime", countdown.endTime);
		                    if (countdown.startTime != -1)
			                    this.WriteFieldComma("startTime", countdown.startTime);
		                    if (countdown.step > 0)
			                    this.WriteFieldComma("step", countdown.step);
		                    if (countdown.interval > 0)
			                    this.WriteFieldComma("interval", countdown.interval);
		                    if (countdown.timerFormat != null)
			                    this.WriteFieldComma("timerFormat", countdown.timerFormat);
		                    if (countdown.numberFormat != null)
			                    this.WriteFieldComma("numberFormat", countdown.numberFormat);
		                    if (!countdown.destroyIfDone)
			                    this.WriteFieldComma("destroyIfDone", false);
		                    if (countdown.command != null)
			                    this.WriteFieldComma("command", countdown.command);
		                    break;
	                    case LuiCompType.HorizontalLayoutGroup:
		                    LuiHorizontalLayoutGroupComp horizontalLayoutGroup = component as LuiHorizontalLayoutGroupComp;
		                    if (horizontalLayoutGroup.spacing != 0)
			                    this.WriteFieldComma("spacing", horizontalLayoutGroup.spacing);
		                    if (horizontalLayoutGroup.childAlignment != null)
			                    this.WriteFieldComma("childAlignment", horizontalLayoutGroup.childAlignment);
		                    if (!horizontalLayoutGroup.childForceExpandWidth)
			                    this.WriteFieldComma("childForceExpandWidth", false);
		                    if (!horizontalLayoutGroup.childForceExpandHeight)
			                    this.WriteFieldComma("childForceExpandHeight", false);
		                    if (horizontalLayoutGroup.childControlWidth)
			                    this.WriteFieldComma("childControlWidth", true);
		                    if (horizontalLayoutGroup.childControlHeight)
			                    this.WriteFieldComma("childControlHeight", true);
		                    if (horizontalLayoutGroup.childScaleWidth)
			                    this.WriteFieldComma("childScaleWidth", true);
		                    if (horizontalLayoutGroup.childScaleHeight)
			                    this.WriteFieldComma("childScaleHeight", true);
		                    if (horizontalLayoutGroup.padding != null)
			                    this.WriteFieldComma("padding", horizontalLayoutGroup.padding);
		                    break;
	                    case LuiCompType.VerticalLayoutGroup:
		                    LuiVerticalLayoutGroupComp verticalLayoutGroup = component as LuiVerticalLayoutGroupComp;
		                    if (verticalLayoutGroup.spacing != 0)
			                    this.WriteFieldComma("spacing", verticalLayoutGroup.spacing);
		                    if (verticalLayoutGroup.childAlignment != null)
			                    this.WriteFieldComma("childAlignment", verticalLayoutGroup.childAlignment);
		                    if (!verticalLayoutGroup.childForceExpandWidth)
			                    this.WriteFieldComma("childForceExpandWidth", false);
		                    if (!verticalLayoutGroup.childForceExpandHeight)
			                    this.WriteFieldComma("childForceExpandHeight", false);
		                    if (verticalLayoutGroup.childControlWidth)
			                    this.WriteFieldComma("childControlWidth", true);
		                    if (verticalLayoutGroup.childControlHeight)
			                    this.WriteFieldComma("childControlHeight", true);
		                    if (verticalLayoutGroup.childScaleWidth)
			                    this.WriteFieldComma("childScaleWidth", true);
		                    if (verticalLayoutGroup.childScaleHeight)
			                    this.WriteFieldComma("childScaleHeight", true);
		                    if (verticalLayoutGroup.padding != null)
			                    this.WriteFieldComma("padding", verticalLayoutGroup.padding);
		                    break;
	                    case LuiCompType.GridLayoutGroup:
		                    LuiGridLayoutGroupComp gridLayoutGroup = component as LuiGridLayoutGroupComp;
		                    if (gridLayoutGroup.cellSize != LUI.defaultCellSize)
			                    this.WriteFieldComma("cellSize", gridLayoutGroup.cellSize);
		                    if (gridLayoutGroup.spacing != default)
			                    this.WriteFieldComma("spacing", gridLayoutGroup.spacing);
		                    if (gridLayoutGroup.startCorner != null)
			                    this.WriteFieldComma("startCorner", gridLayoutGroup.startCorner);
		                    if (gridLayoutGroup.startAxis != null)
			                    this.WriteFieldComma("startAxis", gridLayoutGroup.startAxis);
		                    if (gridLayoutGroup.childAlignment != null)
			                    this.WriteFieldComma("childAlignment", gridLayoutGroup.childAlignment);
		                    if (gridLayoutGroup.constraint != null)
			                    this.WriteFieldComma("constraint", gridLayoutGroup.constraint);
		                    if (gridLayoutGroup.constraintCount != 0)
			                    this.WriteFieldComma("constraintCount", gridLayoutGroup.constraintCount);
		                    if (gridLayoutGroup.padding != null)
			                    this.WriteFieldComma("padding", gridLayoutGroup.padding);
		                    break;
	                    case LuiCompType.ContentSizeFitter:
		                    LuiContentSizeFitterComp contentSizeFitter = component as LuiContentSizeFitterComp;
		                    if (contentSizeFitter.horizontalFit != null)
			                    this.WriteFieldComma("horizontalFit", contentSizeFitter.horizontalFit);
		                    if (contentSizeFitter.verticalFit != null)
			                    this.WriteFieldComma("verticalFit", contentSizeFitter.verticalFit);
		                    break;
	                    case LuiCompType.LayoutElement:
		                    LuiLayoutElementComp layoutElement = component as LuiLayoutElementComp;
		                    if (layoutElement.preferredWidth != -1)
			                    this.WriteFieldComma("preferredWidth", layoutElement.preferredWidth);
		                    if (layoutElement.preferredHeight != -1)
			                    this.WriteFieldComma("preferredHeight", layoutElement.preferredHeight);
		                    if (layoutElement.minWidth != 0)
			                    this.WriteFieldComma("minWidth", layoutElement.minWidth);
		                    if (layoutElement.minHeight != 0)
			                    this.WriteFieldComma("minHeight", layoutElement.minHeight);
		                    if (layoutElement.flexibleWidth != 0)
			                    this.WriteFieldComma("flexibleWidth", layoutElement.flexibleWidth);
		                    if (layoutElement.flexibleHeight != 0)
			                    this.WriteFieldComma("flexibleHeight", layoutElement.flexibleHeight);
		                    if (layoutElement.ignoreLayout)
			                    this.WriteFieldComma("ignoreLayout", true);
		                    break;
	                    case LuiCompType.Draggable:
		                    LuiDraggableComp draggable = component as LuiDraggableComp;
		                    if (draggable.limitToParent)
			                    this.WriteFieldComma("limitToParent", true);
		                    if (draggable.maxDistance > 0)
			                    this.WriteFieldComma("maxDistance", draggable.maxDistance);
		                    if (draggable.allowSwapping)
			                    this.WriteFieldComma("allowSwapping", true);
		                    if (!draggable.dropAnywhere)
			                    this.WriteFieldComma("dropAnywhere", false);
		                    if (draggable.dragAlpha != -1)
			                    this.WriteFieldComma("dragAlpha", draggable.dragAlpha);
		                    if (draggable.parentLimitIndex != -1)
			                    this.WriteFieldComma("parentLimitIndex", draggable.parentLimitIndex);
		                    if (draggable.filter != null)
			                    this.WriteFieldComma("filter", draggable.filter);
		                    if (draggable.parentPadding != default)
			                    this.WriteFieldComma("parentPadding", draggable.parentPadding);
		                    if (draggable.anchorOffset != default)
			                    this.WriteFieldComma("anchorOffset", draggable.anchorOffset);
		                    if (draggable.keepOnTop)
			                    this.WriteFieldComma("keepOnTop", draggable.keepOnTop);
		                    if (draggable.positionRPC != null)
			                    this.WriteFieldComma("positionRPC", draggable.positionRPC);
		                    if (draggable.moveToAnchor)
			                    this.WriteFieldComma("moveToAnchor", draggable.moveToAnchor);
		                    if (draggable.rebuildAnchor)
			                    this.WriteFieldComma("rebuildAnchor", draggable.rebuildAnchor);
		                    break;
	                    case LuiCompType.Slot:
		                    LuiSlotComp slot = component as LuiSlotComp;
		                    if (slot.filter != null)
			                    this.WriteFieldComma("filter", slot.filter);
		                    break;
	                    case LuiCompType.NeedsKeyboard:
		                    break;
	                    case LuiCompType.ScrollView:
		                    LuiScrollComp scroll = component as LuiScrollComp;
		                    bool changeAnchor = scroll.anchor != LuiPosition.Full;
		                    bool changeOffset = scroll.offset != LuiOffset.None;
		                    if (changeAnchor || changeOffset || scroll.pivot != LUI.defaultPivot)
		                    {
			                    this.WriteComma();
			                    this.WriteStartObject("contentTransform");
			                    if (changeAnchor)
			                    {
				                    this.WriteField("anchormin", scroll.anchor.anchorMin);
				                    this.WriteFieldComma("anchormax", scroll.anchor.anchorMax);
			                    }
			                    if (changeOffset)
			                    {
				                    if (changeAnchor)
					                    this.WriteFieldComma("offsetmin", scroll.offset.offsetMin);
				                    else
					                    this.WriteField("offsetmin", scroll.offset.offsetMin);
				                    this.WriteFieldComma("offsetmax", scroll.offset.offsetMax);
			                    }
			                    if (scroll.pivot != LUI.defaultPivot)
			                    {
				                    if (changeAnchor || changeOffset)
										this.WriteFieldComma("pivot", scroll.pivot);
									else
										this.WriteField("pivot", scroll.pivot);
			                    }
			                    this.WriteEndObject();
		                    }
		                    if (scroll.horizontal)
			                    this.WriteFieldComma("horizontal", true);
		                    if (scroll.vertical)
			                    this.WriteFieldComma("vertical", true);
		                    if (scroll.movementType != null)
			                    this.WriteFieldComma("movementType", scroll.movementType);
		                    if (scroll.elasticity != -1)
			                    this.WriteFieldComma("elasticity", scroll.elasticity);
		                    if (scroll.inertia)
			                    this.WriteFieldComma("inertia", true);
		                    if (scroll.decelerationRate != -1)
			                    this.WriteFieldComma("decelerationRate", scroll.decelerationRate);
		                    if (scroll.scrollSensitivity != -1)
			                    this.WriteFieldComma("scrollSensitivity", scroll.scrollSensitivity);
		                    if (scroll.horizontal  && !scroll.horizontalScrollbar.disabled)
		                    {
			                    this.WriteComma();
			                    this.WriteStartObject("horizontalScrollbar");
			                    WriteScrollBar(scroll.horizontalScrollbar);
			                    this.WriteEndObject();

		                    }
		                    if (scroll.vertical && !scroll.verticalScrollbar.disabled)
		                    {
			                    this.WriteComma();
			                    this.WriteStartObject("verticalScrollbar");
			                    WriteScrollBar(scroll.verticalScrollbar);
			                    this.WriteEndObject();
		                    }
		                    if (scroll.horizontalNormalizedPosition != 0)
			                    this.WriteFieldComma("horizontalNormalizedPosition", scroll.horizontalNormalizedPosition);
		                    if (scroll.verticalNormalizedPosition != 0)
			                    this.WriteFieldComma("verticalNormalizedPosition", scroll.verticalNormalizedPosition);
		                    break;
	                    case LuiCompType.CanvasGroup:
		                    LuiCanvasGroupComp canvasGroup = component as LuiCanvasGroupComp;
		                    if (canvasGroup.alpha != -1)
			                    this.WriteFieldComma("alpha", canvasGroup.alpha);
		                    if (!canvasGroup.blocksRaycasts)
			                    this.WriteFieldComma("blocksRaycasts", false);
		                    if (!canvasGroup.interactable)
			                    this.WriteFieldComma("interactable", false);
		                    if (canvasGroup.fade != LUI.defaultFade)
			                    this.WriteFieldComma("fade", canvasGroup.fade);
		                    break;
	                    case LuiCompType.Mask:
		                    LuiMaskComp mask = component as LuiMaskComp;
		                    if (!mask.showMaskGraphic)
			                    this.WriteFieldComma("showMaskGraphic", false);
		                    break;
	                    case LuiCompType.Tooltip:
		                    LuiTooltipComp tooltip = component as LuiTooltipComp;
		                    if (tooltip.tooltipType != null)
			                    this.WriteFieldComma("tooltipType", tooltip.tooltipType);
		                    if (tooltip.offset != null)
			                    this.WriteFieldComma("offset", tooltip.offset);
		                    if (tooltip.useCentre)
			                    this.WriteFieldComma("useCentre", true);
		                    if (tooltip.text != null)
			                    this.WriteFieldComma("text", tooltip.text);
		                    if (tooltip.delay != null)
			                    this.WriteFieldComma("delay", tooltip.delay);
		                    if (tooltip.position != null)
			                    this.WriteFieldComma("position", tooltip.position);
		                    break;
                    }
                    this.WriteEndObject();
                    if (componentIndex < componentCount - 1)
	                    this.WriteComma();
                }
                this.WriteEndArray();
            }
            this.WriteEndObject();
            if (elementIndex < elementCount - 1)
	            this.WriteComma();
        }
        this.WriteEndArray();
    }

	private void WriteScrollBar(LuiScrollbar scroll)
	{
		this.WriteField("enabled", !scroll.disabled); //Adding so I don't need to check for first coma.
		if (scroll.invert)
			this.WriteFieldComma("invert", true);
		if (scroll.autoHide)
			this.WriteFieldComma("autoHide", true);
		if (scroll.handleSprite != null)
			this.WriteFieldComma("handleSprite", scroll.handleSprite);
		if (scroll.size != 0)
			this.WriteFieldComma("size", scroll.size);
		if (scroll.handleColor != null)
			this.WriteFieldComma("handleColor", scroll.handleColor);
		if (scroll.highlightColor != null)
			this.WriteFieldComma("highlightColor", scroll.highlightColor);
		if (scroll.pressedColor != null)
			this.WriteFieldComma("pressedColor", scroll.pressedColor);
		if (scroll.trackSprite != null)
			this.WriteFieldComma("trackSprite", scroll.trackSprite);
		if (scroll.trackColor != null)
			this.WriteFieldComma("trackColor", scroll.trackColor);
	}

    internal int MaxUtf8ByteCount
    {
	    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	    get => _charIndex * 3;
    }

    internal unsafe int WriteUtf8To(Span<byte> destination)
    {
	    if (_charIndex == 0) return 0;
	    fixed (char* source = _charBuffer)
	    fixed (byte* target = destination)
		    return Encoding.UTF8.GetBytes(source, _charIndex, target, destination.Length);
    }

    public string GetJsonString()
    {
	    return new string(_charBuffer, 0, _charIndex);
    }

    public byte[] GetMergedBytes()
    {
	    byte[] buffer = new byte[Encoding.UTF8.GetByteCount(_charBuffer, 0, _charIndex)];
	    Encoding.UTF8.GetBytes(_charBuffer, 0, _charIndex, buffer, 0);
	    return buffer;
    }

    public void Dispose()
    {
	    char[] buffer = _charBuffer;
	    if (buffer == null) return;

	    _charBuffer = null;
	    _charIndex = 0;
	    ArrayPool<char>.Shared.Return(buffer);
    }
}
