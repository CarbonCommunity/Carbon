///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///
using System.IO;
using Carbon.Common;

namespace Carbon.Components;

internal sealed class HarmonyWatcher : FileWatcherBehaviour
{
	internal override void Awake()
	{
		base.Awake();
		Carbon.Utility.Logger.Log("HarmonyWatcher:Initialize()");

		directory = Path.Combine(Context.Directory.Carbon, "harmony");
		includeSubdirectories = true;
		extension = "*.dll";
	}

	internal void Start()
	{
		Carbon.Utility.Logger.Log("HarmonyWatcher:Start()");

		foreach (string f in Directory.EnumerateFiles(directory, extension, SearchOption.AllDirectories))
			HarmonyLoader.GetInstance().Load(f);
	}

	internal override void OnFileChangedEvent(object sender, FileSystemEventArgs e)
	{
		Carbon.Utility.Logger.Log("HarmonyWatcher:OnFileChangedEvent()");
		HarmonyLoader.GetInstance().Load(e.FullPath);
	}

	internal override void OnFileCreatedEvent(object sender, FileSystemEventArgs e)
	{
		Carbon.Utility.Logger.Log("HarmonyWatcher:OnFileCreatedEvent()");
		HarmonyLoader.GetInstance().Load(e.FullPath);
	}

	internal override void OnFileDeletedEvent(object sender, FileSystemEventArgs e)
	{
		Carbon.Utility.Logger.Log("HarmonyWatcher:OnFileDeletedEvent()");
	}

	internal override void OnFileRenamedEvent(object sender, RenamedEventArgs e)
	{
		Carbon.Utility.Logger.Log("HarmonyWatcher:OnFileRenamedEvent()");
	}
}
