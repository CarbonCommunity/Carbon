namespace Carbon.Extensions;

public static class TypeEx
{
	internal static Type _object = typeof(object);
	internal static Type _bool = typeof(bool);
	internal static Type _char = typeof(char);
	internal static Type _sbyte = typeof(sbyte);
	internal static Type _byte = typeof(byte);
	internal static Type _short = typeof(short);
	internal static Type _ushort = typeof(ushort);
	internal static Type _int = typeof(int);
	internal static Type _uint = typeof(uint);
	internal static Type _long = typeof(long);
	internal static Type _ulong = typeof(ulong);
	internal static Type _float = typeof(float);
	internal static Type _double = typeof(double);
	internal static Type _decimal = typeof(Decimal);
	internal static Type _dateTime = typeof(DateTime);
	internal static Type _string = typeof(string);

	internal static IFormatProvider _provider = Thread.CurrentThread.CurrentCulture;

	public static object ConvertType<T>(object value)
	{
		var conversionType = typeof(T);

		if (conversionType == null)
		{
			throw new ArgumentNullException(nameof(conversionType));
		}

		if (value == null)
		{
			if (conversionType.IsValueType)
			{
				throw new InvalidCastException("Value is null (value type)");
			}

			return default;
		}

		if (value is not IConvertible convertible)
		{
			return value.GetType() == conversionType ? (T)value : throw new InvalidCastException("Value is not convertible");
		}

		if (conversionType == _bool)
			return convertible.ToBoolean(_provider);

		if (conversionType == _string)
			return convertible.ToString(_provider);

		if (conversionType == _int)
			return convertible.ToInt32(_provider);

		if (conversionType == _float)
			return convertible.ToSingle(_provider);

		if (conversionType == _double)
			return convertible.ToDouble(_provider);

		if (conversionType == _decimal)
			return convertible.ToDecimal(_provider);

		if (conversionType == _long)
			return convertible.ToInt64(_provider);

		if (conversionType == _dateTime)
			return convertible.ToDateTime(_provider);

		if (conversionType == _char)
			return convertible.ToChar(_provider);

		if (conversionType == _byte)
			return convertible.ToByte(_provider);

		if (conversionType == _uint)
			return convertible.ToUInt32(_provider);

		if (conversionType == _ulong)
			return convertible.ToUInt64(_provider);

		if (conversionType == _short)
			return convertible.ToInt16(_provider);

		if (conversionType == _ushort)
			return convertible.ToUInt16(_provider);

		if (conversionType == _sbyte)
			return convertible.ToSByte(_provider);

		return conversionType == _object ? value : convertible.ToType(conversionType, _provider);
	}
}
