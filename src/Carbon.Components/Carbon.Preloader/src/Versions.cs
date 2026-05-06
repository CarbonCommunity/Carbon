using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Doorstop.Utility;

namespace Doorstop;

public class Versions
{
	public static VersionValue[] Values;

	public static bool IsValid => Values != null;

	public static string CurrentVersion
	{
		get
		{
			var version = Assembly.GetExecutingAssembly().GetName().Version;

			return $"{version.Major}.{version.Minor}.{version.Build}";
		}
	}

	public static bool Init(string data)
	{
		if (string.IsNullOrEmpty(data))
		{
			return false;
		}

		try
		{
			Values = JsonConvert.DeserializeObject<VersionValue[]>(data);
			return true;
		}
		catch (Exception ex)
		{
			Logger.Error($"Failed fetching Carbon versions. Invalid JSON?", ex);
		}

		return false;
	}

	public static VersionValue GetVersion(string name)
	{
		return !IsValid ? default : Values.FirstOrDefault(x => x.Name == name);
	}

	public class VersionValue
	{
		[JsonProperty("name")] public string Name;
		[JsonProperty("date")] public string Date;
		[JsonProperty("protocol")] public string Protocol;
		[JsonProperty("prerelease")] public bool Prerelease;
		[JsonProperty("version")] public string Version;
	}
}
