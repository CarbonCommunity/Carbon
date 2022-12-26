using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Carbon.LoaderEx.Context;
using Carbon.LoaderEx.Utility;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.LoaderEx.ASM;

internal sealed class ResolverEx : IDisposable
{
	private AppDomain _domain;

	public void Dispose()
	{
		_domain.AssemblyResolve -= ResolveAssembly;
	}

	internal void RegisterDomain(AppDomain domain)
	{
		_domain = domain;
		_domain.AssemblyResolve += ResolveAssembly;
		Logger.Log($"Resolver attached to '{_domain.FriendlyName}'");
	}

	internal Assembly ResolveAssembly(object sender, ResolveEventArgs args)
	{
		string name = args.Name;

#if DEBUG
		string requester = args.RequestingAssembly?.GetName().Name ?? "unknown";
		Logger.Debug($"Resolve assembly '{name}' requested by '{requester}'");
#endif

		// deals with aliases foo and foo_xxxxx
		Match match = Regex.Match(args.Name, @"(?i)^((?:\w+)(?:\.(?:\w+))?)_([0-9a-f]+)\.dll$");
		if (match.Success) name = match.Groups[1].Value;

		Assembly retval = ResolveFromDisk(name);

		if (retval == null)
		{
			Logger.Debug($"Unresolved: {name}");
			return default;
		}
		else
		{
			Logger.Debug($"Resolved: {retval.FullName}");
			return retval;
		}
	}

	private Assembly ResolveFromDisk(string name)
	{
		AssemblyName assemblyName = new AssemblyName(name);

#if DEBUG
		Logger.Debug($"ResolveFromDisk: {name}");
#endif

		try
		{
			string fullPath = FindFile($"{assemblyName.Name}.dll");
			return Assembly.Load(ReadFile(fullPath));
		}
		catch (System.Exception)
		{
			return null;
		}
	}

	private static string FindFile(string file)
	{
#if DEBUG
		Logger.Debug($"FindFile: {file}");
#endif

		string location = file switch
		{
			"Carbon.dll" => Directories.CarbonManaged,
			"Carbon.Hooks.dll" => Directories.CarbonManaged,
			"Carbon.Loader.dll" => Directories.CarbonManaged,
			"Carbon.Doorstop.dll" => Directories.CarbonManaged,
			_ => null
		};

		if (location != null)
			return Path.Combine(location, file);

		if (location == null) // Module search
		{
			string needle = Path.Combine(Directories.CarbonModules, file);
			if (File.Exists(needle)) return needle;
		}

		if (location == null) // Carbon reference search
		{
			string needle = Path.Combine(Directories.CarbonLib, file);
			if (File.Exists(needle)) return needle;
		}

		if (location == null) // Game reference search
		{
			string needle = Path.Combine(Directories.GameManaged, file);
			if (File.Exists(needle)) return needle;
		}

		throw new FileNotFoundException(file);
	}

	private static byte[] ReadFile(string file)
	{
		byte[] raw = default;

#if DEBUG
		Logger.Debug($"ReadFile: {file}");
#endif

		try
		{
			if (!File.Exists(file)) throw new FileNotFoundException();
			raw = File.ReadAllBytes(file);

			if (IndexOf(raw, new byte[4] { 0x01, 0xdc, 0x7f, 0x01 }) == 0)
			{
				byte[] tmp = raw, sha1 = new byte[20];
				Buffer.BlockCopy(tmp, 4, sha1, 0, 20);
				raw = Package(sha1, tmp, 24);
			}
		}
		catch (System.Exception e)
		{
			Logger.Error($"Unable to load file '{Path.GetFileName(file)}' [{e.GetType()}]");
		}
		finally
		{
#if DEBUG
			Logger.Debug($" - Loading file '{Path.GetFileName(file)}', read {raw.Length} bytes from disk");
#endif
		}

		return raw;
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
}
