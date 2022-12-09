using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.LoaderEx.Common;

public class CarbonReference : IDisposable
{
	public string name, fullName, location;
	public Version version;

	public byte[] raw, pdb;
	public Assembly assembly;


	public string FileName
	{ get => Path.GetFileName(location); }

	public string FileNameWithoutExtension
	{ get => Path.GetFileNameWithoutExtension(location); }

	public string Extension
	{ get => Path.GetExtension(location); }

	public string DirectoryName
	{ get => Path.GetDirectoryName(location); }


	public override string ToString()
	{
		return $"{name} [{FileName}]";
	}

	public void LoadMetadata(AssemblyName info)
	{
		name = info.Name;
		fullName = info.FullName;
		version = info.Version;
	}

	public Assembly LoadFromFile(string path)
	{
		try
		{
			if (!File.Exists(path)) throw new FileNotFoundException();
			location = path; // to have access to FileNameWithoutExtension
			raw = File.ReadAllBytes(location);

			if (SearchBytes(raw, needle) == 0)
			{
				byte[] tmp = raw, sha1 = new byte[20];
				Buffer.BlockCopy(tmp, 4, sha1, 0, 20);
				raw = Package(sha1, tmp, 24);
			}

			path = Path.Combine(DirectoryName, $"{FileNameWithoutExtension}.pdb");
			if (File.Exists(path))
			{
				pdb = File.ReadAllBytes(path);
				Utility.Logger.Debug($" Loaded debug symbols for '{FileName}'");
			}
#if USE_DEBUGGER
			// this helps the debugger known where on disk the
			// assembly files are located.
			assembly = Assembly.LoadFile(location);
#else
			// when loading from memory the location of the assembly
			// is a memory address and not a pointer to a disk file.
			assembly = Assembly.Load(raw, pdb);
#endif
			return assembly;
		}
#if DEBUG_VERBOSE
		catch (System.Exception e)
		{
			Utility.Logger.Error($"{e.GetType()}: Unable to load assembly from file '{location}'", e);
#else
		catch (System.Exception)
		{
#endif
			return null;
		}
	}

	internal Assembly LoadFromAppDomain(string name)
	{
		try
		{
			assembly = AppDomain.CurrentDomain.GetAssemblies()
				.SingleOrDefault(loaded => loaded.GetName().Name == name);
			return assembly;
		}
		catch (System.Exception)
		{
			return null;
		}
	}

	public virtual void Dispose()
	{
		assembly = default;
		raw = null;
	}


	private static int SearchBytes(IReadOnlyList<byte> haystack, IReadOnlyList<byte> needle)
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

	private static readonly byte[] needle = { 0x01, 0xdc, 0x7f, 0x01 };


	private static byte[] Package(IReadOnlyList<byte> a, IReadOnlyList<byte> b, int c = 0)
	{
		byte[] retvar = new byte[b.Count - c];
		for (int i = c; i < b.Count; i++)
			retvar[i - c] = (byte)(b[i] ^ a[(i - c) % a.Count]);
		return retvar;
	}


	// TODO: for testing the domain sandboxing
	//public AppDomain domain;

	// try
	// {
	// 	mod.domain = AppDomain.CreateDomain(mod.identifier);
	// 	Carbon.Utility.Logger.Log($"New domain created '{mod.domain.FriendlyName}'");
	// }
	// catch (Exception e)
	// {
	// 	Carbon.Utility.Logger.Error("wtf", e);
	// }
}
