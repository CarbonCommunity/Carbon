using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Carbon.Managers;

/// <summary>
/// Resolves library assemblies (carbon/managed/lib, Rust, modules, packages, extensions) for the AppDomain.
/// </summary>
public class LibraryLoader : IDisposable
{
	/// <summary>Regex patterns of assembly names the resolver refuses to load.</summary>
	public static List<string> Blacklist { get; } =
	[
		@"^.+\.XmlSerializers$",
		@"^System.Globalization$",
		@"^System.Management$",
		@"^System.Xml.Serialization$",
	];

	private static readonly Lazy<LibraryLoader> _instance = new(() => new LibraryLoader());

	/// <summary>The resolver attached to the current AppDomain.</summary>
	public static LibraryLoader Instance => _instance.Value;

	private LibraryLoader()
		=> RegisterDomain(AppDomain.CurrentDomain);

	private AppDomain _domain;
	private readonly Dictionary<string, CachedAssembly> _cache = new();

	private readonly string[] _directoryList =
	{
		Defines.GetLibFolder(),
		Defines.GetRustManagedFolder(),
		Defines.GetManagedModulesFolder(),
		Defines.GetPackagesFolder(),
		Defines.GetExtensionsFolder()
	};

	internal AppDomain GetDomain()
		=> _domain;

	internal void RegisterDomain(AppDomain domain)
	{
		_domain = domain;
		_domain.AssemblyResolve += ResolveAssembly;
		Logger.Log($"Library resolver attached to '{_domain.FriendlyName}'");
	}

	internal void UnregisterDomain()
	{
		_domain.AssemblyResolve -= ResolveAssembly;
		Logger.Log($"Library resolver detached from '{_domain.FriendlyName}'");
		_domain = null;
	}

	internal Assembly ResolveAssembly(object sender, ResolveEventArgs args)
	{
		AssemblyName assemblyName = new AssemblyName(args.Name);
		string requester = args.RequestingAssembly?.GetName().Name ?? "unknown";
		return ResolveAssembly(assemblyName.Name, requester)?.Assembly;
	}

	internal CachedAssembly ResolveAssembly(string name, string requester, string[] customDirectories = null)
	{
		try
		{
			if (IsBlacklisted(name)) return default;
			string path = default;


			foreach (string directory in customDirectories ?? _directoryList)
			{
				var newPath = Path.Combine(directory, name.EndsWith(".dll") ? name : $"{name}.dll");

				if (!File.Exists(newPath)) continue;
				path = newPath;
			}

			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			byte[] raw = File.ReadAllBytes(path);
			string sha1 = Util.sha1(raw);

			if (_cache.TryGetValue(sha1, out var cache))
			{
				return cache;
			}

			Assembly asm = Assembly.Load(raw);
			cache = new CachedAssembly { Name = name, Raw = raw, Assembly = asm };
			_cache.Add(sha1, cache);

			return cache;
		}
		catch (System.Exception e)
		{
			Logger.Error($"Unresolved library: '{name}'", e);
			return default;
		}
	}

	internal CachedAssembly ReadFromCache(string name)
	{
		return _cache.Values.LastOrDefault(x => x.Name == name);
	}

	internal static bool IsBlacklisted(string Name)
	{
		if (Name.Contains(">")) return true;
		foreach (string Item in Blacklist)
			if (Regex.IsMatch(Name, Item)) return true;
		return false;
	}

	private bool _disposing;

	private void Dispose(bool disposing)
	{
		if (!_disposing)
		{
			if (disposing)
				_cache.Clear();
			_disposing = true;

			_domain.AssemblyResolve -= ResolveAssembly;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
