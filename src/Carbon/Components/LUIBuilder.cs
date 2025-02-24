using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace Carbon.Common.Carbon.Components;

public static class LUIBuilder
{

	#region Old CUI Improvements
	private static readonly char[] charPreset = new char[32]; //Max 4 chars with 8 chars as no ui x/y should be bigger than 1280

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
		char[] charBuffer = new char[elementId.Length + 12];
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
    public static void WriteStartObject(this LuiBuilderInstance inst) => inst.Write(startObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteEndObject(this LuiBuilderInstance inst) => inst.Write(endObject);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteStartArray(this LuiBuilderInstance inst) => inst.Write(startArray);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteEndArray(this LuiBuilderInstance inst) => inst.Write(endArray);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteComma(this LuiBuilderInstance inst) => inst.Write(coma);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteMark(this LuiBuilderInstance inst) => inst.Write(mark);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteDots(this LuiBuilderInstance inst) => inst.Write(dots);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSpace(this LuiBuilderInstance inst) => inst.Write(whitespace);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteStartObject(this LuiBuilderInstance inst, string key)
    {
	    inst.WriteMark();
	    inst.Write(key);
	    inst.WriteMark();
	    inst.WriteDots();
	    inst.WriteStartObject();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteStartArray(this LuiBuilderInstance inst, string key)
    {
	    inst.WriteMark();
	    inst.Write(key);
	    inst.WriteMark();
	    inst.WriteDots();
	    inst.WriteStartArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteField(this LuiBuilderInstance inst, string key, int value)
    {
	    inst.WriteMark();
	    inst.Write(key);
	    inst.WriteMark();
	    inst.WriteDots();
	    inst.Write(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteField(this LuiBuilderInstance inst, string key, Vector2 value)
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
    public static void WriteField(this LuiBuilderInstance inst, string key, float value)
    {
	    inst.WriteMark();
	    inst.Write(key);
	    inst.WriteMark();
	    inst.WriteDots();
	    inst.Write(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteField(this LuiBuilderInstance inst, string key, bool value)
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
    public static void WriteField(this LuiBuilderInstance inst, string key, string value)
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

	private const int segmentCheck = 4000;

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
    private static void Write(LuiBuilderInstance inst, char character)
    {
	    inst._charBuffer[inst._charIndex] = character;
	    inst._charIndex++;
	    if (inst._charIndex >= segmentCheck)
		    inst.Flush();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Write(this LuiBuilderInstance inst, int value)
    {
        Span<char> tempBuffer = stackalloc char[11];
        int written = WriteIntDigits(value, tempBuffer);
        tempBuffer.Slice(0, written).CopyTo(inst._charBuffer.AsSpan(inst._charIndex));
        inst._charIndex += written;
        if (inst._charIndex >= segmentCheck)
	        inst.Flush();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Write(this LuiBuilderInstance inst, float value)
    {
        Span<char> tempBuffer = stackalloc char[16];
        int written = WriteFloatDigits(value, tempBuffer, 3);
        tempBuffer.Slice(0, written).CopyTo(inst._charBuffer.AsSpan(inst._charIndex));
        inst._charIndex += written;
        if (inst._charIndex >= segmentCheck)
	        inst.Flush();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Write(this LuiBuilderInstance inst, string text)
    {
        int length = text.Length;
        Span<char> buffer = inst._charBuffer.AsSpan();
        int charIndex = inst._charIndex;
        for (int i = 0; i < length; i++)
            buffer[charIndex + i] = text[i];
        inst._charIndex += length;
        if (inst._charIndex >= segmentCheck)
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
		index += WriteIntDigits(intPart, buffer.Slice(index));
		buffer[index++] = dot;
		float fractional = value - intPart;
		long factor = _pow10[precision];
		long fracAsInt = (long)(fractional * factor + 0.5f);
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
	private static int WriteIntDigits(int value, Span<char> buffer)
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

	#endregion
}

static class NamedFields
{
	public const string nameName = "name";
	public const string parentName = "parent";
	public const string componentsName = "components";

	public const string typeName = "type";

	public const string componentRectTransform = "RectTransform";
	public const string anchorMin = "anchormin";
	public const string anchorMax = "anchormax";
	public const string offsetMin = "offsetmin";
	public const string offsetMax = "offsetmax";

	public const string componentImage = "UnityEngine.UI.Image";
	public const string color = "color";
}

public struct LuiBuilderInstance : IDisposable
{
	private LUIBuilder.WriteArray[] _segments = ArrayPool<LUIBuilder.WriteArray>.Shared.Rent(100);
	private int _segmentCount = 0;

	private const int maxSegmentSize = 5000;

	public readonly char[] _charBuffer = new char[maxSegmentSize];

	public int _charIndex = 0;

	private string GetFieldName(LuiCompType type)
	{
		return type switch
		{
			LuiCompType.RectTransform => NamedFields.componentRectTransform,
			LuiCompType.Image => NamedFields.componentImage,
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
            bool hasComponents = element.luiComponents.Count > 0;
            bool hasParent = !string.IsNullOrEmpty(element.parent);
            if (!string.IsNullOrEmpty(element.name))
            {
	            this.WriteField(NamedFields.nameName, element.name);
                if (hasComponents || hasParent)
	                this.WriteComma();
            }

            if (hasParent)
            {
	            this.WriteField(NamedFields.parentName, element.parent);
                if (hasComponents)
	                this.WriteComma();

            }
            if (hasComponents)
            {
	            this.WriteStartArray(NamedFields.componentsName);
                int componentCount = element.luiComponents.Count;
                int found = 0;
                foreach (LuiCompBase component in element.luiComponents)
                {
	                this.WriteStartObject();
	                this.WriteField(NamedFields.typeName, GetFieldName(component.type));
	                this.WriteComma();
                    switch (component.type)
                    {
                        case LuiCompType.RectTransform:
	                        LuiRectTransform rect = component as LuiRectTransform;
                            found++;
                            this.WriteField(NamedFields.anchorMin, rect.anchor.anchorMin);
                            this.WriteComma();
                            this.WriteField(NamedFields.anchorMax, rect.anchor.anchorMax);
                            this.WriteComma();
                            this.WriteField(NamedFields.offsetMin, rect.offset.offsetMin);
                            this.WriteComma();
                            this.WriteField(NamedFields.offsetMax, rect.offset.offsetMax);
                            break;
                        case LuiCompType.Image:
	                        LuiImagePanel img = component as LuiImagePanel;
                            found++;
                            this.WriteField(NamedFields.color, img.color);
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

    public string GetJsonString()
    {
	    byte[] buffer = GetMergedBytes();

	    string jsonString = Encoding.UTF8.GetString(buffer);
	    //string jsonString = "";
	    ArrayPool<byte>.Shared.Return(buffer);
	    return jsonString;
    }

    public byte[] GetMergedBytes()
    {
	    Flush();
	    int totalSize = 0;
	    for (int i = 0; i < _segmentCount; i++)
		    totalSize += _segments[i].size;
	    byte[] buffer = ArrayPool<byte>.Shared.Rent(totalSize);
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
	    _segments[_segmentCount++] = new LUIBuilder.WriteArray(segment, size);
	    _charIndex = 0;
    }

    public void Dispose()
    {
	    _charIndex = 0;
	    for (int i = 0; i < _segmentCount; i++)
		    ArrayPool<byte>.Shared.Return(_segments[i].values);
	    ArrayPool<LUIBuilder.WriteArray>.Shared.Return(_segments);
    }
}
