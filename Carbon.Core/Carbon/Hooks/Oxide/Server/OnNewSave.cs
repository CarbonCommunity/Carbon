///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System.IO;
using Carbon.Core;

namespace Carbon.Extended
{
	[OxideHook("OnNewSave"), OxideHook.Category(Hook.Category.Enum.Server)]
	[OxideHook.Info("Called when a new savefile is created (usually when map has wiped)")]
	[OxideHook.Parameter("strFilename", typeof(string))]
	[OxideHook.Patch(typeof(SaveRestore), "Load")]
	public class SaveRestore_Load
	{
		public static void Prefix(string strFilename = "", bool allowOutOfDateSaves = false)
		{
			if (strFilename == "")
			{
				strFilename = World.SaveFolderName + "/" + World.SaveFileName;
			}

			if (!File.Exists(strFilename))
			{
				HookExecutor.CallStaticHook("OnNewSave", strFilename);
			}
		}
	}
}
