using System;
using System.IO;

namespace Carbon;

/// <summary>
/// A folder watched by FileWatcherManager.
/// </summary>
public sealed class WatchFolder
{
	public string Directory { get; set; }
	public string Filter { get; set; }
	public bool IncludeSubFolders { get; set; }

	public Action<WatchFileEvent> OnEvent { get; set; }
}

public readonly struct WatchFileEvent
{
	public readonly WatcherChangeTypes Type;
	public readonly string Path;
	public readonly string OldPath;
	public readonly bool IsInitial;

	public WatchFileEvent(WatcherChangeTypes type, string path, string oldPath, bool isInitial)
	{
		Type = type;
		Path = path;
		OldPath = oldPath;
		IsInitial = isInitial;
	}
}