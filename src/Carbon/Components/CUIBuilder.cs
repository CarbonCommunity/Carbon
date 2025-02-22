using System.Runtime.CompilerServices;

namespace Carbon.Common.Carbon.Components;

public static class CUIBuilder
{

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

	private const char under = '_';
	private const char minus = '-';
	private const char zero = '0';
	private const char dot = '.';
	private const char whitespace = ' ';
	private const float zeroFloat = 0;

	private static readonly long[] _pow10 = {
		1L, 10L, 100L, 1_000L, 10_000L, 100_000L,
		1_000_000L, 10_000_000L, 100_000_000L, 1_000_000_000L
	};


	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static int WriteFloatDigits(float value, Span<char> buffer, int startIndex = 0, int precision = 3)
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
}
