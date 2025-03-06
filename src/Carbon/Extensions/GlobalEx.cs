public static class GlobalEx
{
	public static bool IsSteamId(this string id)
	{
		return ulong.TryParse(id, out var num) && num.IsSteamId();
	}

	public static bool IsSteamId(this ulong id)
	{
		return id > 76561197960265728UL;
	}

	[Obsolete("This method is deprecated! Use effect.Clear() instead.")]
	public static void Clear(this Effect effect, bool _)
	{
		effect.Clear();
	}
}
