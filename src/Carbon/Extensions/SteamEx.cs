public static class SteamEx
{
	public static bool IsSteamId(this string id)
	{
		return ulong.TryParse(id, out var num) && num.IsSteamId();
	}

	public static bool IsSteamId(this ulong id)
	{
		return id > 76561197960265728UL;
	}

	public static bool IsSteamId(this EncryptedValue<ulong> id)
	{
		return id > 76561197960265728UL;
	}
}
