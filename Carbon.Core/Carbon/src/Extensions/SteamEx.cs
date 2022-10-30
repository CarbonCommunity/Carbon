using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class SteamEx
{
	public static bool IsSteamId(this string id)
	{
		return ulong.TryParse(id, out var num) && num > 76561197960265728UL;
	}

	public static bool IsSteamId(this ulong id)
	{
		return id > 76561197960265728UL;
	}
}
