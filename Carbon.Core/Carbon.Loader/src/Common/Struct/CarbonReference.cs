///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///
using System;
using System.IO;
using System.Reflection;

namespace Carbon.Common;

public class CarbonReference
{
	public string name, fullName, location;
	public Version version;

	public byte[] raw;
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
			raw = File.ReadAllBytes(path);
			assembly = Assembly.Load(raw);
			location = path;
			return assembly;
		}
		catch (System.Exception e)
		{
			Utility.Logger.Error($"Unable to load assembly from file '{path}'", e);
			return null;
		}
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