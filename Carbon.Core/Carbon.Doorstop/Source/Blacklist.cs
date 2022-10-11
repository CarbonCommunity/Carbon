///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System.Text.RegularExpressions;

namespace Carbon.Utility
{
	internal static class Blacklist
	{
		private readonly static string[] Items =
		{
			@"^Item.OnDirty$",
		};

		internal static bool IsBlacklisted(string Name)
		{
			foreach (string Item in Items)
				if (Regex.IsMatch(Name, Item)) return true;
			return false;
		}
	}
}