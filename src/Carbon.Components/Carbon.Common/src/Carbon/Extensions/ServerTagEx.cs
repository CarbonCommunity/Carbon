namespace Carbon.Extensions;

public static class ServerTagEx
{
	public static void SetRequiredTag(string tag, bool compact)
	{
		var tags = Steamworks.SteamServer.GameTags;

		if (tags.Contains($",{tag}"))
		{
			return;
		}

		if (compact)
		{
			var indexOf = tags.IndexOf('^');
			Steamworks.SteamServer.GameTags = indexOf > 0 ? tags.Insert(indexOf, tag + ",") : $"{tags}{(tags.EndsWith(",") ? string.Empty : ",")}{tag}";
			return;
		}


		Steamworks.SteamServer.GameTags = $"{tags},{tag}";
	}

	public static void UnsetRequiredTag(string tag, bool compact)
	{
		var tags = Steamworks.SteamServer.GameTags;

		if (compact)
		{
			if (!tags.Contains(tag)) return;
			Steamworks.SteamServer.GameTags = tags.Replace(tag, string.Empty);
			return;
		}

		if (!tags.Contains($",{tag}")) return;
		Steamworks.SteamServer.GameTags = tags.Replace($",{tag}", string.Empty);
	}
}
