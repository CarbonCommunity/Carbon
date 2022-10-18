///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
///
using System.IO;
using Carbon.Utility;

namespace Carbon.Patterns;

internal class HarmonyWatcher : FileWatcherBehaviour
{
	public override void Initialize()
	{
		Carbon.Utility.Logger.Log("HarmonyWatcher:Initialize()");

		directory = Path.Combine(Context.CarbonDirectory, "harmony");
		extension = "*.dll";
	}

	public override void Dispose()
	{
		Carbon.Utility.Logger.Log("HarmonyWatcher:Dispose()");
	}

	internal override void OnFileChangedEvent(object sender, FileSystemEventArgs e)
	{
		Carbon.Utility.Logger.Log("HarmonyWatcher:OnFileChangedEvent()");
	}

	internal override void OnFileCreatedEvent(object sender, FileSystemEventArgs e)
	{
		Carbon.Utility.Logger.Log("HarmonyWatcher:OnFileCreatedEvent()");
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
