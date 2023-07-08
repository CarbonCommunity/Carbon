/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Core.Libraries.Covalence;

public class SaveInfo
{
	public string SaveName { get; private set; }
	public DateTime CreationTime { get; private set; }
	public uint CreationTimeUnix { get; private set; }

	private readonly Time time = Interface.Oxide.GetLibrary<Time>();
	private readonly string FullPath;

	private SaveInfo(string filepath)
	{
		FullPath = filepath;
		SaveName = Utility.GetFileNameWithoutExtension(filepath);
		Refresh();
	}
	public static SaveInfo Create(string filepath)
	{
		if (!File.Exists(filepath))
		{
			return null;
		}
		return new SaveInfo(filepath);
	}

	public void Refresh()
	{
		if (!File.Exists(FullPath))
		{
			return;
		}
		CreationTime = File.GetCreationTime(FullPath);
		CreationTimeUnix = time.GetUnixFromDateTime(CreationTime);
	}
}
