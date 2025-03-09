using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Carbon.Publicizer;

#pragma warning disable

[Serializable]
public class Config
{
	public static Config Singleton;

	public bool DeveloperMode { get; set; } = false;
	public PublicizerConfig Publicizer { get; set; } = new();

	public void ForceEnsurePublicizedAssembly(string value)
	{
		if (Publicizer.PublicizedAssemblies.Contains(value))
		{
			return;
		}
		Publicizer.PublicizedAssemblies.Add(value);
	}

	public class PublicizerConfig
	{
		public List<string> PublicizedAssemblies { get; set; } = new();
		public List<string> PublicizerMemberIgnores { get; set; } =
		[
			@"^HiddenValueBase$",
			@"^HiddenValue`1$",
			@"^Pool$"
		];

		public bool IsMemberIgnored(string name)
		{
			foreach (var item in PublicizerMemberIgnores)
			{
				if (Regex.IsMatch(name, item))
				{
					return true;
				}
			}
			return false;
		}
	}

	public static void Init(string configFile)
	{
		if (Singleton != null)
		{
			return;
		}

		if (!File.Exists(configFile))
		{
			Singleton = new();
		}
		else
		{
			Singleton = JsonConvert.DeserializeObject<Config>(File.ReadAllText(configFile));
		}

		Singleton.ForceEnsurePublicizedAssembly("Assembly-CSharp.dll");
		Singleton.ForceEnsurePublicizedAssembly("Facepunch.Console.dll");
		Singleton.ForceEnsurePublicizedAssembly("Facepunch.Network.dll");
		Singleton.ForceEnsurePublicizedAssembly("Facepunch.Nexus.dll");
		Singleton.ForceEnsurePublicizedAssembly("Rust.Clans.Local.dll");
		Singleton.ForceEnsurePublicizedAssembly("Rust.Harmony.dll");
		Singleton.ForceEnsurePublicizedAssembly("Rust.Global.dll");
		Singleton.ForceEnsurePublicizedAssembly("Rust.Data.dll");
	}
}
