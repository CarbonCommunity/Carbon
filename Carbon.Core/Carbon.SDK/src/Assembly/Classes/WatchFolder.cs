using System.IO;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace API.Assembly;

public class WatchFolder
{
	public bool IncludeSubFolders { get; set; }
	public FileSystemWatcher Handler { get; set; }
	public string Directory { get; set; }
	public string Extension { get; set; }

	public OnFileChangedEvent OnFileChanged { get; set; }
	public delegate void OnFileChangedEvent(object sender, string path);

	public OnFileCreatedEvent OnFileCreated { get; set; }
	public delegate void OnFileCreatedEvent(object sender, string path);

	public OnFileDeletedEvent OnFileDeleted { get; set; }
	public delegate void OnFileDeletedEvent(object sender, string path);
}
