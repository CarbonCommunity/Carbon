using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace Carbon.Components;

public static class LUIBuilder
{
	#region Old CUI Improvements

	private static readonly char[] charPreset = new char[32]; //Max 4 values with 8 chars as no ui x/y should be bigger than 1280

	public static string GetStringFloat(params float[] values)
	{
		char[] charBuffer = charPreset;
		int charIndex = 0;
		for (int i = 0; i < values.Length; i++)
		{
			int written = WriteFloatDigits(values[i], charBuffer, charIndex);
			charIndex += written;
			if (i < values.Length - 1)
				charBuffer[charIndex++] = whitespace;
		}
		return new string(charBuffer, 0, charIndex);
	}

	public static string BuildElementId(string elementId, int id)
	{
		int value = id;
		int idLength = 0;
		while (value > 0)
		{
			idLength++;
			value /= 10;
		}
		char[] charBuffer = new char[elementId.Length + 1 + idLength];
		int charIndex = 0;
		for (int i = 0; i < elementId.Length; i++)
			charBuffer[charIndex++] = elementId[i];
		charBuffer[charIndex++] = under;
		Span<char> tempBuffer = stackalloc char[11];
		int written = WriteIntDigits(id, tempBuffer);
		tempBuffer.Slice(0, written).CopyTo(charBuffer.AsSpan(charIndex));
		return new string(charBuffer);
	}

	#endregion

	#region Field Builders

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteStartObject(this ref LuiBuilderInstance inst) => inst.Write(startObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteEndObject(this ref LuiBuilderInstance inst) => inst.Write(endObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteStartArray(this ref LuiBuilderInstance inst) => inst.Write(startArray);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteEndArray(this ref LuiBuilderInstance inst) => inst.Write(endArray);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteComma(this ref LuiBuilderInstance inst) => inst.Write(coma);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteMark(this ref LuiBuilderInstance inst) => inst.Write(mark);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteDots(this ref LuiBuilderInstance inst) => inst.Write(dots);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSpace(this ref LuiBuilderInstance inst) => inst.Write(whitespace);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteStartObject(this ref LuiBuilderInstance inst, string key)
    {
	    inst.WriteMark();
	    inst.Write(key);
	    inst.WriteMark();
	    inst.WriteDots();
	    inst.WriteStartObject();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteStartArray(this ref LuiBuilderInstance inst, string key)
    {
	    inst.WriteMark();
	    inst.Write(key);
	    inst.WriteMark();
	    inst.WriteDots();
	    inst.WriteStartArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteField(this ref LuiBuilderInstance inst, string key, int value)
    {
	    inst.WriteMark();
	    inst.Write(key);
	    inst.WriteMark();
	    inst.WriteDots();
	    inst.Write(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteField(this ref LuiBuilderInstance inst, string key, Vector2 value)
    {
	    inst.WriteMark();
	    inst.Write(key);
	    inst.WriteMark();
	    inst.WriteDots();
	    inst.WriteMark();
	    inst.Write(value.x);
	    inst.WriteSpace();
	    inst.Write(value.y);
	    inst.WriteMark();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteField(this ref LuiBuilderInstance inst, string key, float value)
    {
	    inst.WriteMark();
	    inst.Write(key);
	    inst.WriteMark();
	    inst.WriteDots();
	    inst.Write(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteField(this ref LuiBuilderInstance inst, string key, ulong value)
    {
	    inst.WriteMark();
	    inst.Write(key);
	    inst.WriteMark();
	    inst.WriteDots();
	    inst.Write(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteField(this ref LuiBuilderInstance inst, string key, bool value)
    {
	    inst.WriteMark();
	    inst.Write(key);
	    inst.WriteMark();
	    inst.WriteDots();
        if (value)
	        inst.Write(trueValue);
        else
	        inst.Write(falseValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteField(this ref LuiBuilderInstance inst, string key, string value)
    {
	    inst.WriteMark();
	    inst.Write(key);
	    inst.WriteMark();
	    inst.WriteDots();
	    inst.WriteMark();
	    inst.Write(value);
	    inst.WriteMark();
    }

	#endregion

	#region Builders

	public readonly struct WriteArray
	{
		public readonly byte[] values;
		public readonly int size;

		public WriteArray(byte[] values, int size)
		{
			this.values = values;
			this.size = size;
		}
	}

	private const int segmentCheck = 2048;

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

	private static readonly long[] _pow10 = {
		1L, 10L, 100L, 1_000L, 10_000L, 100_000L,
		1_000_000L, 10_000_000L, 100_000_000L, 1_000_000_000L
	};

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Write(this ref LuiBuilderInstance inst, char character)
    {
	    inst._charBuffer[inst._charIndex] = character;
	    inst._charIndex++;
	    if (inst._charIndex >= segmentCheck)
		    inst.Flush();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Write(this ref LuiBuilderInstance inst, int value)
    {
        Span<char> tempBuffer = stackalloc char[11];
        int written = WriteIntDigits(value, tempBuffer);
        tempBuffer.Slice(0, written).CopyTo(inst._charBuffer.AsSpan(inst._charIndex));
        inst._charIndex += written;
        if (inst._charIndex >= segmentCheck)
	        inst.Flush();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Write(this ref LuiBuilderInstance inst, float value)
    {
        Span<char> tempBuffer = stackalloc char[16];
        int written = WriteFloatDigits(value, tempBuffer);
        tempBuffer.Slice(0, written).CopyTo(inst._charBuffer.AsSpan(inst._charIndex));
        inst._charIndex += written;
        if (inst._charIndex >= segmentCheck)
	        inst.Flush();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Write(this ref LuiBuilderInstance inst, ulong value)
    {
	    Span<char> tempBuffer = stackalloc char[20];
	    int written = WriteUlongDigits(value, tempBuffer);
	    tempBuffer.Slice(0, written).CopyTo(inst._charBuffer.AsSpan(inst._charIndex));
	    inst._charIndex += written;
	    if (inst._charIndex >= segmentCheck)
		    inst.Flush();
    }

    private static int stringLengthCheck = Mathf.CeilToInt(segmentCheck * 1.9f);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Write(this ref LuiBuilderInstance inst, string text, int startPos = 0)
    {
	    int length = text.Length;
	    Span<char> buffer = inst._charBuffer.AsSpan();
	    int charIndex = inst._charIndex;
	    int index = 0;
	    bool shouldCheckForSize = length > segmentCheck;
	    for (int i = startPos; i < length; i++)
	    {
		    int lengthSum = charIndex + index;
		    buffer[lengthSum] = text[i];
		    index++;
		    if (shouldCheckForSize && lengthSum > stringLengthCheck)
		    {
			    inst._charIndex += index;
			    inst.Flush();
			    inst.Write(text, index);
			    return;
		    }
	    }
	    inst._charIndex += length - startPos;
	    if (inst._charIndex > segmentCheck)
		    inst.Flush();
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
		if (fracAsInt == _pow10[precision])
			intPart++;
		index += WriteIntDigits(intPart, buffer[index..]);
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
	private static int _segmentLimit = 100;
	private LUIBuilder.WriteArray[] _segments = ArrayPool<LUIBuilder.WriteArray>.Shared.Rent(_segmentLimit);
	private int _segmentCount = 0;

	private const int maxSegmentSize = 4096;

	public readonly char[] _charBuffer = new char[maxSegmentSize];

	public int _charIndex = 0;

	private string GetFieldName(LuiCompType type)
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
			_ => "UnityEngine.UI.Image"
		};
	}

	public LuiBuilderInstance(LUI cui)
    {
        this.WriteStartArray();
        int elementCount = cui.elements.Count;
        int elementCounter = 0;
        foreach (var element in cui.elements)
        {
            elementCounter++;
            this.WriteStartObject();
            if (element.update)
            {
	            this.WriteField("name", element.name);
	            if (!string.IsNullOrEmpty(element.parent))
	            {
		            this.WriteComma();
		            this.WriteField("parent", element.parent);
	            }
            }
            else
            {
	            this.WriteField("parent", element.parent);
	            if (!string.IsNullOrEmpty(element.name))
	            {
		            this.WriteComma();
		            this.WriteField("name", element.name);
	            }
            }
            if (element.destroyUi != null)
            {
	            this.WriteComma();
	            this.WriteField("destroyUi", element.destroyUi);
            }
            if (element.fadeOut > 0)
            {
	            this.WriteComma();
	            this.WriteField("fadeOut", element.fadeOut);
            }
            if (element.update)
            {
	            this.WriteComma();
	            this.WriteField("update", true);
            }
            if (!element.activeSelf)
            {
	            this.WriteComma();
	            this.WriteField("activeSelf", false);
            }
            if (element.luiComponents.Count > 0)
            {
	            this.WriteComma();
	            this.WriteStartArray("components");
                int componentCount = element.luiComponents.Count;
                int found = 0;
                foreach (LuiCompBase component in element.luiComponents)
                {
	                this.WriteStartObject();
	                this.WriteField("type", GetFieldName(component.type));
	                if (!component.enabled)
	                {
		                this.WriteComma();
		                this.WriteField("enabled", false);
	                }
	                if (component.fadeIn > 0)
	                {
		                this.WriteComma();
		                this.WriteField("fadeIn", component.fadeIn);
	                }
	                if (component.placeholderParentId != null)
	                {
		                this.WriteComma();
		                this.WriteField("placeholderParentId", component.placeholderParentId);
	                }
                    switch (component.type)
                    {
	                    case LuiCompType.Text:
		                    LuiTextComp text = component as LuiTextComp;
		                    found++;
		                    if (text.text != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("text", text.text);
		                    }
		                    if (text.fontSize > 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("fontSize", text.fontSize);
		                    }
		                    if (text.font != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("font", text.font);
		                    }
		                    if (text.align != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("align", text.align);
		                    }
		                    if (text.color != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("color", text.color);
		                    }
		                    if (text.verticalOverflow != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("verticalOverflow", text.verticalOverflow);
		                    }
		                    break;
	                    case LuiCompType.Image:
		                    LuiImageComp image = component as LuiImageComp;
		                    found++;
		                    if (image.sprite != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("sprite", image.sprite);
		                    }
		                    if (image.material != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("material", image.material);
		                    }
		                    if (image.color != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("color", image.color);
		                    }
		                    if (image.imageType != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("imagetype", image.imageType);
		                    }
		                    if (image.fillCenter)
		                    {
			                    this.WriteComma();
			                    this.WriteField("fillCenter", true);
		                    }
		                    if (image.png != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("png", image.png);
		                    }
		                    if (image.slice != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("slice", image.slice);
		                    }
		                    if (image.itemid != 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("itemid", image.itemid);
		                    }
		                    if (image.skinid != 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("skinid", image.skinid);
		                    }
		                    break;
	                    case LuiCompType.RawImage:
		                    LuiRawImageComp rawImage = component as LuiRawImageComp;
		                    found++;
		                    if (rawImage.sprite != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("sprite", rawImage.sprite);
		                    }
		                    if (rawImage.color != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("color", rawImage.color);
		                    }
		                    if (rawImage.material != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("material", rawImage.material);
		                    }
		                    if (rawImage.url != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("url", rawImage.url);
		                    }
		                    if (rawImage.png != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("png", rawImage.png);
		                    }
		                    if (rawImage.steamid != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("steamid", rawImage.steamid);
		                    }
		                    break;
	                    case LuiCompType.Button:
		                    LuiButtonComp button = component as LuiButtonComp;
		                    found++;
		                    if (button.command != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("command", button.command);
		                    }
		                    if (button.close != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("close", button.close);
		                    }
		                    if (button.sprite != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("sprite", button.sprite);
		                    }
		                    if (button.material != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("material", button.material);
		                    }
		                    if (button.color != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("color", button.color);
		                    }
		                    if (button.imageType != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("imagetype", button.imageType);
		                    }
		                    if (button.normalColor != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("normalColor", button.normalColor);
		                    }
		                    if (button.highlightedColor != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("highlightedColor", button.highlightedColor);
		                    }
		                    if (button.pressedColor != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("pressedColor", button.pressedColor);
		                    }
		                    if (button.selectedColor != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("selectedColor", button.selectedColor);
		                    }
		                    if (button.disabledColor != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("disabledColor", button.disabledColor);
		                    }
		                    if (button.colorMultiplier != -1)
		                    {
			                    this.WriteComma();
			                    this.WriteField("colorMultiplier", button.colorMultiplier);
		                    }
		                    if (button.fadeDuration != -1)
		                    {
			                    this.WriteComma();
			                    this.WriteField("fadeDuration", button.fadeDuration);
		                    }
		                    break;
	                    case LuiCompType.Outline:
		                    LuiOutlineComp outline = component as LuiOutlineComp;
		                    found++;
		                    if (outline.color != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("color", outline.color);
		                    }
		                    if (outline.distance != default)
		                    {
			                    this.WriteComma();
			                    this.WriteField("distance", outline.distance);
		                    }
		                    if (outline.useGraphicAlpha)
		                    {
			                    this.WriteComma();
			                    this.WriteField("useGraphicAlpha", true);
		                    }
		                    break;
	                    case LuiCompType.InputField:
		                    LuiInputComp input = component as LuiInputComp;
		                    found++;
		                    if (input.fontSize > 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("fontSize", input.fontSize);
		                    }
		                    if (input.font != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("font", input.font);
		                    }
		                    if (input.align != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("align", input.align);
		                    }
		                    if (input.color != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("color", input.color);
		                    }
		                    if (input.characterLimit > 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("characterLimit", input.characterLimit);
		                    }
		                    if (input.command != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("command", input.command);
		                    }
		                    if (input.lineType != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("lineType", input.lineType);
		                    }
		                    if (input.text != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("text", input.text);
		                    }
		                    if (input.readOnly)
		                    {
			                    this.WriteComma();
			                    this.WriteField("readOnly", true);
		                    }
		                    if (input.placeholderId != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("placeholderId", input.placeholderId);
		                    }
		                    if (input.password)
		                    {
			                    this.WriteComma();
			                    this.WriteField("password", true);
		                    }
		                    if (input.needsKeyboard)
		                    {
			                    this.WriteComma();
			                    this.WriteField("needsKeyboard", true);
		                    }
		                    if (input.hudMenuInput)
		                    {
			                    this.WriteComma();
			                    this.WriteField("hudMenuInput", true);
		                    }
		                    if (input.autofocus)
		                    {
			                    this.WriteComma();
			                    this.WriteField("autofocus", true);
		                    }
		                    break;
	                    case LuiCompType.NeedsCursor:
		                    found++;
		                    break;
                        case LuiCompType.RectTransform:
	                        LuiRectTransformComp rect = component as LuiRectTransformComp;
                            found++;
	                        if (rect.anchor != LuiPosition.Full)
	                        {
		                        this.WriteComma();
		                        this.WriteField("anchormin", rect.anchor.anchorMin);
		                        this.WriteComma();
		                        this.WriteField("anchormax", rect.anchor.anchorMax);
	                        }
	                        this.WriteComma(); //Always adding offset, as RUST UI have weird one pixel offset by default, idk who came to this idea lol.
                            this.WriteField("offsetmin", rect.offset.offsetMin);
	                        this.WriteComma();
                            this.WriteField("offsetmax", rect.offset.offsetMax);
	                        if (rect.rotation != 0)
	                        {
		                        this.WriteComma();
		                        this.WriteField("rotation", rect.rotation);
	                        }
	                        if (rect.setParent != null)
	                        {
		                        this.WriteComma();
		                        this.WriteField("setParent", rect.setParent);
	                        }
	                        if (rect.setTransformIndex != -1)
	                        {
		                        this.WriteComma();
		                        this.WriteField("setTransformIndex", rect.setTransformIndex);
	                        }
                            break;
	                    case LuiCompType.Countdown:
		                    LuiCountdownComp countdown = component as LuiCountdownComp;
		                    found++;
		                    if (countdown.endTime != -1)
		                    {
			                    this.WriteComma();
			                    this.WriteField("endTime", countdown.endTime);
		                    }
		                    if (countdown.startTime != -1)
		                    {
			                    this.WriteComma();
			                    this.WriteField("startTime", countdown.startTime);
		                    }
		                    if (countdown.step > 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("step", countdown.step);
		                    }
		                    if (countdown.interval > 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("interval", countdown.interval);
		                    }
		                    if (countdown.timerFormat != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("timerFormat", countdown.timerFormat);
		                    }
		                    if (countdown.numberFormat != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("numberFormat", countdown.numberFormat);
		                    }
		                    if (!countdown.destroyIfDone)
		                    {
			                    this.WriteComma();
			                    this.WriteField("destroyIfDone", false);
		                    }
		                    if (countdown.command != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("command", countdown.command);
		                    }
		                    break;
	                    case LuiCompType.HorizontalLayoutGroup:
		                    LuiHorizontalLayoutGroupComp horizontalLayoutGroup = component as LuiHorizontalLayoutGroupComp;
		                    found++;
		                    if (horizontalLayoutGroup.spacing != 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("spacing", horizontalLayoutGroup.spacing);
		                    }
		                    if (horizontalLayoutGroup.childAlignment != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("childAlignment", horizontalLayoutGroup.childAlignment);
		                    }
		                    if (!horizontalLayoutGroup.childForceExpandWidth)
		                    {
			                    this.WriteComma();
			                    this.WriteField("childForceExpandWidth", false);
		                    }
		                    if (!horizontalLayoutGroup.childForceExpandHeight)
		                    {
			                    this.WriteComma();
			                    this.WriteField("childForceExpandHeight", false);
		                    }
		                    if (horizontalLayoutGroup.childControlWidth)
		                    {
			                    this.WriteComma();
			                    this.WriteField("childControlWidth", true);
		                    }
		                    if (horizontalLayoutGroup.childControlHeight)
		                    {
			                    this.WriteComma();
			                    this.WriteField("childControlHeight", true);
		                    }
		                    if (horizontalLayoutGroup.childScaleWidth)
		                    {
			                    this.WriteComma();
			                    this.WriteField("childScaleWidth", true);
		                    }
		                    if (horizontalLayoutGroup.childScaleHeight)
		                    {
			                    this.WriteComma();
			                    this.WriteField("childScaleHeight", true);
		                    }
		                    if (horizontalLayoutGroup.padding != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("padding", horizontalLayoutGroup.padding);
		                    }
		                    break;
	                    case LuiCompType.VerticalLayoutGroup:
		                    LuiVerticalLayoutGroupComp verticalLayoutGroup = component as LuiVerticalLayoutGroupComp;
		                    found++;
		                    if (verticalLayoutGroup.spacing != 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("spacing", verticalLayoutGroup.spacing);
		                    }
		                    if (verticalLayoutGroup.childAlignment != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("childAlignment", verticalLayoutGroup.childAlignment);
		                    }
		                    if (!verticalLayoutGroup.childForceExpandWidth)
		                    {
			                    this.WriteComma();
			                    this.WriteField("childForceExpandWidth", false);
		                    }
		                    if (!verticalLayoutGroup.childForceExpandHeight)
		                    {
			                    this.WriteComma();
			                    this.WriteField("childForceExpandHeight", false);
		                    }
		                    if (verticalLayoutGroup.childControlWidth)
		                    {
			                    this.WriteComma();
			                    this.WriteField("childControlWidth", true);
		                    }
		                    if (verticalLayoutGroup.childControlHeight)
		                    {
			                    this.WriteComma();
			                    this.WriteField("childControlHeight", true);
		                    }
		                    if (verticalLayoutGroup.childScaleWidth)
		                    {
			                    this.WriteComma();
			                    this.WriteField("childScaleWidth", true);
		                    }
		                    if (verticalLayoutGroup.childScaleHeight)
		                    {
			                    this.WriteComma();
			                    this.WriteField("childScaleHeight", true);
		                    }
		                    if (verticalLayoutGroup.padding != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("padding", verticalLayoutGroup.padding);
		                    }
		                    break;
	                    case LuiCompType.GridLayoutGroup:
		                    LuiGridLayoutGroupComp gridLayoutGroup = component as LuiGridLayoutGroupComp;
		                    found++;
		                    if (gridLayoutGroup.cellSize != LUI.defaultCellSize)
		                    {
			                    this.WriteComma();
			                    this.WriteField("cellSize", gridLayoutGroup.cellSize);
		                    }
		                    if (gridLayoutGroup.spacing != default)
		                    {
			                    this.WriteComma();
			                    this.WriteField("spacing", gridLayoutGroup.spacing);
		                    }
		                    if (gridLayoutGroup.startCorner != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("startCorner", gridLayoutGroup.startCorner);
		                    }
		                    if (gridLayoutGroup.startAxis != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("startAxis", gridLayoutGroup.startAxis);
		                    }
		                    if (gridLayoutGroup.childAlignment != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("childAlignment", gridLayoutGroup.childAlignment);
		                    }
		                    if (gridLayoutGroup.constraint != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("constraint", gridLayoutGroup.constraint);
		                    }
		                    if (gridLayoutGroup.constraintCount != 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("constraintCount", gridLayoutGroup.constraintCount);
		                    }
		                    if (gridLayoutGroup.padding != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("padding", gridLayoutGroup.padding);
		                    }
		                    break;
	                    case LuiCompType.ContentSizeFitter:
		                    LuiContentSizeFitterComp contentSizeFitter = component as LuiContentSizeFitterComp;
		                    found++;
		                    if (contentSizeFitter.horizontalFit != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("horizontalFit", contentSizeFitter.horizontalFit);
		                    }
		                    if (contentSizeFitter.verticalFit != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("verticalFit", contentSizeFitter.verticalFit);
		                    }
		                    break;
	                    case LuiCompType.LayoutElement:
		                    LuiLayoutElementComp layoutElement = component as LuiLayoutElementComp;
		                    found++;
		                    if (layoutElement.preferredWidth != -1)
		                    {
			                    this.WriteComma();
			                    this.WriteField("preferredWidth", layoutElement.preferredWidth);
		                    }
		                    if (layoutElement.preferredHeight != -1)
		                    {
			                    this.WriteComma();
			                    this.WriteField("preferredHeight", layoutElement.preferredHeight);
		                    }
		                    if (layoutElement.minWidth != 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("minWidth", layoutElement.minWidth);
		                    }
		                    if (layoutElement.minHeight != 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("minHeight", layoutElement.minHeight);
		                    }
		                    if (layoutElement.flexibleWidth != 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("flexibleWidth", layoutElement.flexibleWidth);
		                    }
		                    if (layoutElement.flexibleHeight != 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("flexibleHeight", layoutElement.flexibleHeight);
		                    }
		                    if (layoutElement.ignoreLayout)
		                    {
			                    this.WriteComma();
			                    this.WriteField("ignoreLayout", true);
		                    }
		                    break;
	                    case LuiCompType.Draggable:
		                    LuiDraggableComp draggable = component as LuiDraggableComp;
		                    found++;
		                    if (draggable.limitToParent)
		                    {
			                    this.WriteComma();
			                    this.WriteField("limitToParent", true);
		                    }
		                    if (draggable.maxDistance > 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("maxDistance", draggable.maxDistance);
		                    }
		                    if (draggable.allowSwapping)
		                    {
			                    this.WriteComma();
			                    this.WriteField("allowSwapping", true);
		                    }
		                    if (!draggable.dropAnywhere)
		                    {
			                    this.WriteComma();
			                    this.WriteField("dropAnywhere", false);
		                    }
		                    if (draggable.dragAlpha != -1)
		                    {
			                    this.WriteComma();
			                    this.WriteField("dragAlpha", draggable.dragAlpha);
		                    }
		                    if (draggable.parentLimitIndex != -1)
		                    {
			                    this.WriteComma();
			                    this.WriteField("parentLimitIndex", draggable.parentLimitIndex);
		                    }
		                    if (draggable.filter != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("filter", draggable.filter);
		                    }
		                    if (draggable.parentPadding != default)
		                    {
			                    this.WriteComma();
			                    this.WriteField("parentPadding", draggable.parentPadding);
		                    }
		                    if (draggable.anchorOffset != default)
		                    {
			                    this.WriteComma();
			                    this.WriteField("anchorOffset", draggable.anchorOffset);
		                    }
		                    if (draggable.keepOnTop)
		                    {
			                    this.WriteComma();
			                    this.WriteField("keepOnTop", draggable.keepOnTop);
		                    }
		                    if (draggable.positionRPC != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("positionRPC", draggable.positionRPC);
		                    }
		                    if (draggable.moveToAnchor)
		                    {
			                    this.WriteComma();
			                    this.WriteField("moveToAnchor", draggable.moveToAnchor);
		                    }
		                    if (draggable.rebuildAnchor)
		                    {
			                    this.WriteComma();
			                    this.WriteField("rebuildAnchor", draggable.rebuildAnchor);
		                    }
		                    break;
	                    case LuiCompType.Slot:
		                    LuiSlotComp slot = component as LuiSlotComp;
		                    found++;
		                    if (slot.filter != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("filter", slot.filter);
		                    }
		                    break;
	                    case LuiCompType.NeedsKeyboard:
		                    found++;
		                    break;
	                    case LuiCompType.ScrollView:
		                    LuiScrollComp scroll = component as LuiScrollComp;
		                    found++;
		                    bool changeAnchor = scroll.anchor != LuiPosition.Full;
		                    bool changeOffset = scroll.offset != LuiOffset.None;
		                    if (changeAnchor || changeOffset || scroll.pivot != LUI.defaultPivot)
		                    {
			                    this.WriteComma();
			                    this.WriteStartObject("contentTransform");
			                    if (changeAnchor)
			                    {
				                    this.WriteField("anchormin", scroll.anchor.anchorMin);
				                    this.WriteComma();
				                    this.WriteField("anchormax", scroll.anchor.anchorMax);
			                    }
			                    if (changeOffset)
			                    {
				                    if (changeAnchor)
					                    this.WriteComma();
				                    this.WriteField("offsetmin", scroll.offset.offsetMin);
				                    this.WriteComma();
				                    this.WriteField("offsetmax", scroll.offset.offsetMax);
			                    }
			                    if (scroll.pivot != LUI.defaultPivot)
			                    {
				                    if (changeAnchor || changeOffset)
										this.WriteComma();
				                    this.WriteField("pivot", scroll.pivot);
			                    }
			                    this.WriteEndObject();
		                    }
		                    if (scroll.horizontal)
		                    {
			                    this.WriteComma();
			                    this.WriteField("horizontal", true);
		                    }
		                    if (scroll.vertical)
		                    {
			                    this.WriteComma();
			                    this.WriteField("vertical", true);
		                    }
		                    if (scroll.movementType != null)
		                    {
			                    this.WriteComma();
			                    this.WriteField("movementType", scroll.movementType);
		                    }
		                    if (scroll.elasticity != -1)
		                    {
			                    this.WriteComma();
			                    this.WriteField("elasticity", scroll.elasticity);
		                    }
		                    if (scroll.inertia)
		                    {
			                    this.WriteComma();
			                    this.WriteField("inertia", true);
		                    }
		                    if (scroll.decelerationRate != -1)
		                    {
			                    this.WriteComma();
			                    this.WriteField("decelerationRate", scroll.decelerationRate);
		                    }
		                    if (scroll.scrollSensitivity != -1)
		                    {
			                    this.WriteComma();
			                    this.WriteField("scrollSensitivity", scroll.scrollSensitivity);
		                    }
		                    if (scroll.horizontal)
		                    {
			                    this.WriteComma();
			                    this.WriteStartObject("horizontalScrollbar");
			                    WriteScrollBar(scroll.horizontalScrollbar);
			                    this.WriteEndObject();

		                    }
		                    if (scroll.vertical)
		                    {
			                    this.WriteComma();
			                    this.WriteStartObject("verticalScrollbar");
			                    WriteScrollBar(scroll.verticalScrollbar);
			                    this.WriteEndObject();
		                    }
		                    if (scroll.horizontalNormalizedPosition != 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("horizontalNormalizedPosition", scroll.horizontalNormalizedPosition);
		                    }
		                    if (scroll.verticalNormalizedPosition != 0)
		                    {
			                    this.WriteComma();
			                    this.WriteField("verticalNormalizedPosition", scroll.verticalNormalizedPosition);
		                    }
		                    break;
                    }
                    this.WriteEndObject();
                    if (found < componentCount)
	                    this.WriteComma();
                }
                this.WriteEndArray();
            }
            this.WriteEndObject();
            if (elementCounter < elementCount)
	            this.WriteComma();
        }
        this.WriteEndArray();
    }

	private void WriteScrollBar(LuiScrollbar scroll)
	{
		this.WriteField("enabled", !scroll.disabled); //Adding so I don't need to check for first coma.
		if (scroll.invert)
		{
			this.WriteComma();
			this.WriteField("invert", true);
		}
		if (scroll.autoHide)
		{
			this.WriteComma();
			this.WriteField("autoHide", true);
		}
		if (scroll.handleSprite != null)
		{
			this.WriteComma();
			this.WriteField("handleSprite", scroll.handleSprite);
		}
		if (scroll.size != 0)
		{
			this.WriteComma();
			this.WriteField("size", scroll.size);
		}
		if (scroll.handleColor != null)
		{
			this.WriteComma();
			this.WriteField("handleColor", scroll.handleColor);
		}
		if (scroll.highlightColor != null)
		{
			this.WriteComma();
			this.WriteField("highlightColor", scroll.highlightColor);
		}
		if (scroll.pressedColor != null)
		{
			this.WriteComma();
			this.WriteField("pressedColor", scroll.pressedColor);
		}
		if (scroll.trackSprite != null)
		{
			this.WriteComma();
			this.WriteField("trackSprite", scroll.trackSprite);
		}
		if (scroll.trackColor != null)
		{
			this.WriteComma();
			this.WriteField("trackColor", scroll.trackColor);
		}
	}

    public string GetJsonString()
    {
	    byte[] buffer = GetMergedBytes();
	    string jsonString = Encoding.UTF8.GetString(buffer);
	    //string jsonString = "";
	    //ArrayPool<byte>.Shared.Return(buffer);
	    return jsonString;
    }

    public byte[] GetMergedBytes()
    {
	    Flush();
	    int totalSize = 0;
	    for (int i = 0; i < _segmentCount; i++)
		    totalSize += _segments[i].size;
	    byte[] buffer = new byte[totalSize];
	    int offset = 0;

	    for (int i = 0; i < _segmentCount; i++)
	    {
		    LUIBuilder.WriteArray writeArray = _segments[i];
		    writeArray.values.AsSpan(0, writeArray.size).CopyTo(buffer.AsSpan(offset));
		    offset += writeArray.size;
	    }
	    return buffer;
    }

    public void Flush()
    {
	    if (_charIndex == 0) return;
	    byte[] segment = ArrayPool<byte>.Shared.Rent(maxSegmentSize);
	    int size = Encoding.UTF8.GetBytes(_charBuffer, 0, _charIndex, segment, 0);
	    if (_segmentCount == _segmentLimit)
	    {
		    _segmentLimit *= 2;
		    var _tempArray = ArrayPool<LUIBuilder.WriteArray>.Shared.Rent(_segmentLimit);
		    Array.Copy(_segments, _tempArray, _segmentCount);
		    ArrayPool<LUIBuilder.WriteArray>.Shared.Return(_segments, clearArray: true);
		    _segments = _tempArray;
		    _segmentCount = 0;
	    }
	    _segments[_segmentCount++] = new LUIBuilder.WriteArray(segment, size);
	    _charIndex = 0;
    }

    public void Dispose()
    {
	    _charIndex = 0;
	    for (int i = 0; i < _segmentCount; i++)
		    ArrayPool<byte>.Shared.Return(_segments[i].values);
	    ArrayPool<LUIBuilder.WriteArray>.Shared.Return(_segments, clearArray: true);
    }
}
