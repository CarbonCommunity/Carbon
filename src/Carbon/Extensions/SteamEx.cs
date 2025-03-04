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

	public static bool IsSteamId(this BasePlayer.EncryptedValue<ulong> id)
	{
		return id > 76561197960265728UL;
	}
}
