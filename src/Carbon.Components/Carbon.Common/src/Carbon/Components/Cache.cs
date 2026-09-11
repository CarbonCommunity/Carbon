namespace Carbon;

/// <summary>
/// Boxing cache of unmanaged values. Very helpful for reducing memory allocations for hooks returning an object.
/// </summary>
public class Cache
{
	public static readonly object False = false;
	public static readonly object True = true;
	public static readonly object EmptyString = string.Empty;
	public static readonly object SpaceString = " ";

	public static readonly object DefaultSByte = default(sbyte);
	public static readonly object DefaultChar = default(char);
	public static readonly object DefaultInt16 = default(short);
	public static readonly object DefaultInt64 = default(long);
	public static readonly object DefaultByte = default(byte);
	public static readonly object DefaultUInt16 = default(ushort);
	public static readonly object DefaultUInt32 = default(uint);
	public static readonly object DefaultUInt64 = default(ulong);
	public static readonly object DefaultSingle = default(float);
	public static readonly object DefaultDouble = default(double);
	public static readonly object DefaultDecimal = default(decimal);
	public static readonly object DefaultDateTime = default(DateTime);

	public class CUI
	{
		public static readonly string BlankColor = "0 0 0 0";
		public static readonly string BlackColor = "0 0 0 1";
		public static readonly string WhiteColor = "1 1 1 1";
	}
}
