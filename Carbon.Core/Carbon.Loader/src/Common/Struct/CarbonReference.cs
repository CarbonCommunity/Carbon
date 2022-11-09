///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///
using System;
using System.IO;
using System.Reflection;

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
		catch (System.Exception e)
		{
			Utility.Logger.Error($"{e.GetType()}: Unable to load assembly from file '{location}'", e);
			return null;
		}
	}

	public virtual void Dispose()
	{
		assembly = default;
		raw = null;
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