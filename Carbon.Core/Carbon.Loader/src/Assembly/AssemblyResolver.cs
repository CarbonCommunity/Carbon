extern alias MonoCecilStandalone;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Carbon.LoaderEx.Common;
using Carbon.LoaderEx.Context;
using Carbon.LoaderEx.Utility;
using MonoCecilStandalone::Mono.Cecil;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.LoaderEx.ASM;

internal sealed class AssemblyResolver : Singleton<AssemblyResolver>, IDisposable
{
	private AppDomain _domain;
	private DefaultAssemblyResolver _resolver;
	private Dictionary<string, byte[]> _cache;

	private static readonly string[] lookupLocations =
	{
		Context.Directories.CarbonManaged,
		Context.Directories.CarbonModules,
		Context.Directories.CarbonLib,
		Context.Directories.GameManaged,
	};

	private static readonly string[] dynamicAssemblies =
	{
		"Carbon", "Carbon.Hooks"
	};

	static AssemblyResolver() { }

	internal AssemblyResolver()
	{
		_cache = new Dictionary<string, byte[]>();
		_resolver = new DefaultAssemblyResolver();
		foreach (string item in lookupLocations) _resolver.AddSearchDirectory(item);
	}

	public void Dispose()
	{
		_domain.AssemblyResolve -= ResolveAssembly;

		_resolver.Dispose();
		_resolver = default;

		_cache.Clear();
		_cache = default;
	}

	internal bool IsPath(string file)
		=> file.Contains(Path.DirectorySeparatorChar);

	internal bool IsDynamicModule(AssemblyName assemblyName)
		=> (dynamicAssemblies.Contains(assemblyName.Name));

	internal AssemblyName NormalizeModuleName(string name)
	{
		// deals with full paths /foo/bar/foobar.dll
		if (IsPath(name)) name = Path.GetFileNameWithoutExtension(name);

		// deals with short paths foobar.dll
		name = name.Replace(".dll", string.Empty);

		// deals with aliases foo and foo_xxxxx
		Match match = Regex.Match(name, Patterns.RenamedAssembly);
		if (match.Success) name = match.Groups[1].Value;

		return new AssemblyName(name);
	}

	internal void RegisterDomain(AppDomain domain)
	{
		_domain = domain;
		_domain.AssemblyResolve += ResolveAssembly;
	}

	private Assembly ResolveAssembly(object sender, ResolveEventArgs args)
		=> ResolveAssembly(sender, args, false);

	private Assembly ResolveAssembly(object sender, ResolveEventArgs args, bool forced)
	{
		Assembly assembly;
		AssemblyName request = NormalizeModuleName(args.Name);
		AssemblyName requester = NormalizeModuleName(args.RequestingAssembly.FullName);

		try
		{
			// the assembly is not expected to be re-loaded during runtime, thus
			// we can use the LoadFile() method that will lock the file on disk.
			if (!IsDynamicModule(request))
			{
				string location = FindFile(request);
				if (location == null) throw new Exception("File not found");

				assembly = Assembly.LoadFile(location);
				Logger.Debug($"Resolved: '{request.Name}' type:static domain:'{_domain.FriendlyName}' requestedBy:'{requester.Name}'");
				return assembly;
			}

			// after a dynamic assembly is loaded to app domain we still expect
			// to receive resolve requests to it's original name. this happens
			// because we have renamed the orignal name to a random one; we need
			// to make sure the returned assembly is the same.
			IReadOnlyList<Assembly> loaded = _domain.GetAssemblies().Reverse().ToList();
			assembly = loaded.FirstOrDefault(x => x.GetName().Name.StartsWith($"{request.Name}_")) ?? null;

			if (assembly != null && !forced)
			{
				Logger.Debug($"Resolved: '{request.Name}' type:dynamic domain:'{_domain.FriendlyName}' requestedBy:'{requester.Name}'");
				return assembly;
			}

			// loads a dynamic assembly for the first time, we respect the cache
			// version of it.
			byte[] raw = ReadCache(request);

			if (raw == null || forced)
			{
				string location = FindFile(request);
				if (location == null) throw new Exception("File not found");

				raw = ReadFile(location);
				if (raw == null) throw new Exception("Unable to read file");

				string nickname = $"{request.Name}_{Guid.NewGuid():N}";
				SetAssemblyName(ref raw, nickname, Path.GetFileName(location).Equals("Carbon.dll") ? location : null);
			}

			_cache[request.Name] = raw;
			Logger.Debug($" - Added to in-memory cache '{request.Name}'");

			assembly = Assembly.Load(raw);
			Logger.Debug($"Resolved: '{request.Name}' type:dynamic domain:'{_domain.FriendlyName}' requestedBy:'{requester.Name}'");
			return assembly;
		}
		catch (System.Exception)
		{
			Logger.Debug($"Unresolved: '{request.Name}' domain:'{_domain.FriendlyName}' requestedBy:'{requester.Name}'");
			return null;
		}
	}

	private string FindFile(AssemblyName assemblyName, string[] locations = null)
	{
		try
		{
			// special case for Carbon
			if (assemblyName.Name.Equals("Carbon", Patterns.IgnoreCase))
				locations = new string[] { Context.Directories.CarbonManaged };

			// special case for Carbon.Loader
			if (assemblyName.Name.Equals("Carbon.Loader", Patterns.IgnoreCase))
				locations = new string[] { Context.Directories.GameHarmony };

			// special case for Carbon.*
			else if (assemblyName.Name.StartsWith("Carbon.", Patterns.IgnoreCase))
				locations = new string[] { Context.Directories.CarbonManaged };

			else if (locations == null)
				locations = lookupLocations;

			foreach (string location in locations)
			{
				string retval = Path.Combine(location, $"{assemblyName.Name}.dll");
				if (File.Exists(retval)) return retval;
			}
			return null;
		}
		catch (System.Exception e)
		{
			Logger.Error($"Error while searching for '{assemblyName.Name}.dll'", e);
			return null;
		}
	}

	private byte[] ReadCache(AssemblyName assemblyName)
	{
		try
		{
			string needle = assemblyName.Name;
			//if (_aliases.TryGetValue(assemblyName.Name, out string nickname)) needle = nickname;

			byte[] raw = _cache.SingleOrDefault(x => x.Key.Equals(needle)).Value ?? null;
			Logger.Debug($" - ReadCache request:{assemblyName} needle:{needle} result:{(raw == null ? false : true)}");
			return raw;
		}
		catch (System.Exception e)
		{
			Logger.Error($"Unable to read from in-memory cache '{assemblyName}'", e);
			return default;
		}
	}

	private byte[] ReadFile(string file)
	{
		Logger.Debug($" - ReadFile {file}");

		try
		{
			if (!File.Exists(file)) throw new FileNotFoundException();
			byte[] raw = File.ReadAllBytes(file);

			if (IndexOf(raw, new byte[4] { 0x01, 0xdc, 0x7f, 0x01 }) == 0)
			{
				byte[] tmp = raw, sha1 = new byte[20];
				Buffer.BlockCopy(tmp, 4, sha1, 0, 20);
				raw = Package(sha1, tmp, 24);
			}

			/*
						// special case asm files we expect to be reloaded during runtime
						if (dynamicAssemblies.Contains(Path.GetFileNameWithoutExtension(file)))
						{
							raw = RenameAssembly(raw, // update carbon.dll on disk with new asm name
								(Path.GetFileName(file).Equals("Carbon.dll")) ? file : null);
						}
			*/

			Logger.Debug($" - Loading file '{Path.GetFileName(file)}', read {raw.Length} bytes from disk");
			return raw;
		}
		catch (System.Exception e)
		{
			Logger.Error($"Unable to load file '{Path.GetFileName(file)}' [{e.GetType()}]");
			return default;
		}
	}

	private static byte[] Package(IReadOnlyList<byte> a, IReadOnlyList<byte> b, int c = 0)
	{
		byte[] retvar = new byte[b.Count - c];
		for (int i = c; i < b.Count; i++)
			retvar[i - c] = (byte)(b[i] ^ a[(i - c) % a.Count]);
		return retvar;
	}

	private static int IndexOf(IReadOnlyList<byte> haystack, IReadOnlyList<byte> needle)
	{
		int len = needle.Count;
		int limit = haystack.Count - len;

		for (int i = 0; i <= limit; i++)
		{
			int k = 0;
			for (; k < len; k++)
				if (needle[k] != haystack[i + k]) break;
			if (k == len) return i;
		}
		return -1;
	}

	private AssemblyName GetAssemblyName(string file)
	{
		if (!File.Exists(file)) throw new Exception("File not found");
		byte[] raw = ReadFile(file);

		using MemoryStream input = new MemoryStream(raw);
		AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(
			input, parameters: new ReaderParameters { AssemblyResolver = _resolver });

		return new AssemblyName(assemblyDefinition.FullName);
	}

	private AssemblyName GetAssemblyName(byte[] raw)
	{
		using MemoryStream input = new MemoryStream(raw);
		AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(
			input, parameters: new ReaderParameters { AssemblyResolver = _resolver });

		return new AssemblyName(assemblyDefinition.FullName);
	}

	private void SetAssemblyName(ref byte[] raw, string nickname, string file = null)
	{
		using MemoryStream input = new MemoryStream(raw);
		AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(
			input, parameters: new ReaderParameters { AssemblyResolver = _resolver });

		if (!Regex.Match(assemblyDefinition.Name.Name, Patterns.RenamedAssembly).Success)
		{
			Logger.Debug($" - Assembly name is '{assemblyDefinition.Name.Name}', rename to '{nickname}'");
			assemblyDefinition.Name = new AssemblyNameDefinition(nickname, assemblyDefinition.Name.Version);
		}

		if (file != null && File.Exists(file))
		{
			Logger.Debug($" - Saved assembly to disk'");
			using FileStream disk = new FileStream(file, FileMode.Truncate);
			assemblyDefinition.Write(disk);
		}

		using MemoryStream output = new MemoryStream();
		assemblyDefinition.Write(output);
		raw = output.ToArray();
	}

	/// <summary>
	/// Gets the array of bytes of a loaded type from AppDomain.<br/>
	/// If the assembly is not found at the registered App Domain, an exception
	/// will be thrown.
	/// </summary>
	///
	/// <param name="name">Assembly name, short of full name</param>
	internal byte[] ReadAssembly(string name)
	{
		// input name can be short or long form of asm name
		// this makes is predictable.
		AssemblyName assemblyName = NormalizeModuleName(name);

		try
		{
			byte[] raw = ReadCache(assemblyName) ?? null;
			if (raw != null) return raw;

			string location = FindFile(assemblyName);
			if (string.IsNullOrEmpty(location)) return null;

			raw = ReadFile(location);
			if (raw == null) throw new Exception();

			return raw;
		}
		catch (System.Exception e)
		{
			Logger.Error($"GetAssemblyBytes failed", e);
			throw;
		}
	}

	/// <summary>
	/// Loads into the AppDomain the assembly from the provided file path on disk.
	/// </summary>
	///
	/// <param name="file">The full file path to the assembly file on disk</param>
	/// <param name="forced">Forces the file to be re-read and re-loaded into the App Domain</param>
	internal Assembly LoadAssembly(string file, bool forced = false)
		=> ResolveAssembly(this, args: new ResolveEventArgs(file, Assembly.GetExecutingAssembly()), forced);
}