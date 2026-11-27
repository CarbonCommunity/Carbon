using System;
using System.IO;
using System.Collections.Generic;
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

		private List<string> _cachedIgnoreSource;
		private int _cachedIgnoreCount;
		private Regex[] _cachedIgnores;

		public bool IsMemberIgnored(string name)
		{
			var ignores = GetIgnoreRegexes();
			for (int i = 0; i < ignores.Length; i++)
			{
				if (ignores[i].IsMatch(name))
				{
					return true;
				}
			}
			return false;
		}

		private Regex[] GetIgnoreRegexes()
		{
			var source = PublicizerMemberIgnores;
			if (_cachedIgnores != null && ReferenceEquals(_cachedIgnoreSource, source) && _cachedIgnoreCount == (source?.Count ?? 0))
			{
				return _cachedIgnores;
			}

			var regexes = new Regex[source?.Count ?? 0];
			for (int i = 0; i < regexes.Length; i++)
			{
				regexes[i] = new Regex(source[i]);
			}

			_cachedIgnoreSource = source;
			_cachedIgnoreCount = regexes.Length;
			return _cachedIgnores = regexes;
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
		Singleton.ForceEnsurePublicizedAssembly("Facepunch.Ping.dll");
		Singleton.ForceEnsurePublicizedAssembly("Facepunch.Unity.dll");
		Singleton.ForceEnsurePublicizedAssembly("Facepunch.Rcon.dll");
		Singleton.ForceEnsurePublicizedAssembly("Rust.Localization.dll");
		Singleton.ForceEnsurePublicizedAssembly("Rust.Clans.Local.dll");
		Singleton.ForceEnsurePublicizedAssembly("Rust.FileSystem.dll");
		Singleton.ForceEnsurePublicizedAssembly("Rust.Harmony.dll");
		Singleton.ForceEnsurePublicizedAssembly("Rust.Global.dll");
		Singleton.ForceEnsurePublicizedAssembly("Rust.Data.dll");
		Singleton.ForceEnsurePublicizedAssembly("Fleck.dll");
	}
}
