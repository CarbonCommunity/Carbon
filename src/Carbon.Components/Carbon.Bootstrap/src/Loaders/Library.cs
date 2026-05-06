using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using API.Abstracts;
using API.Assembly;
using Utility;

namespace Loaders;

internal sealed class LibraryLoader : Singleton<LibraryLoader>, IDisposable
{
	private static readonly string[] _blacklist = {
		@"^.+\.XmlSerializers$",
		@"^Oxide\..+$",
		@"^System.Globalization$",
		@"^System.Management$",
		@"^System.Xml.Serialization$",
	};

	// Singleton pattern needs a private ctor
	private LibraryLoader()
		=> RegisterDomain(AppDomain.CurrentDomain);

	private class Item : IAssemblyCache
	{
		public string Name { get; internal set; }
		public byte[] Raw { get; internal set; }
		public Assembly Assembly { get; internal set; }
	}

	private AppDomain _domain;
	private readonly Dictionary<string, Item> _cache = new();

	private readonly string[] _directoryList =
	{
		Context.CarbonLib,
		Context.GameManaged,
		Context.CarbonModules,
		Context.CarbonExtensions
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
		_domain = null;
		_domain.AssemblyResolve -= ResolveAssembly;
		Logger.Log($"Library resolver detached from '{_domain.FriendlyName}'");
	}

	internal Assembly ResolveAssembly(object sender, ResolveEventArgs args)
	{
		AssemblyName assemblyName = new AssemblyName(args.Name);
		string requester = args.RequestingAssembly?.GetName().Name ?? "unknown";
		return ResolveAssembly(assemblyName.Name, requester)?.Assembly;
	}

	internal IAssemblyCache ResolveAssembly(string name, string requester, string[] customDirectories = null)
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

			if (_cache.TryGetValue(sha1, out Item cache))
			{
				return cache;
			}

			Assembly asm = Assembly.Load(raw);
			cache = new Item { Name = name, Raw = raw, Assembly = asm };
			_cache.Add(sha1, cache);

			return cache;
		}
		catch (System.Exception e)
		{
			Logger.Error($"Unresolved library: '{name}'", e);
			return default;
		}
	}

	internal IAssemblyCache ReadFromCache(string name)
	{
		Item item = _cache.Select(x => x.Value).Last(x => x.Name == name);
		return item ?? default;
	}

	internal static bool IsBlacklisted(string Name)
	{
		if (Name.Contains(">")) return true;
		foreach (string Item in _blacklist)
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
