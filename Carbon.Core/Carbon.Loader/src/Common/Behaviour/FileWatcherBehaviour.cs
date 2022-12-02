///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using System;
using System.Collections.Generic;
using System.IO;

namespace Carbon.LoaderEx.Common;

internal abstract class FileWatcherBehaviour : CarbonBehaviour, IDisposable
{
	internal string directory;
	internal string extension;
	internal bool includeSubdirectories;

	internal List<string> Blacklist;

	private FileSystemWatcher Handler;

	internal virtual void Awake()
	{
		Carbon.LoaderEx.Utility.Logger.Log("FileWatcherBehaviour:Awake()");

		directory = string.Empty;
		extension = string.Empty;
		includeSubdirectories = false;
		Blacklist = new List<string>();
	}

	internal void OnEnable()
	{
		Carbon.LoaderEx.Utility.Logger.Log("FileWatcherBehaviour:OnEnable()");

		if (string.IsNullOrEmpty(extension))
			throw new ArgumentException("No file extension defined");

		if (string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
			throw new Exception($"Unable to watch '{directory}");

		if (Handler is null)
		{
			Handler = new FileSystemWatcher(directory)
			{
				Filter =
					extension,

				NotifyFilter =
					NotifyFilters.FileName |
					NotifyFilters.LastWrite |
					NotifyFilters.LastAccess
			};

			Handler.Changed += OnFileChangedEvent;
			Handler.Created += OnFileCreatedEvent;
			Handler.Deleted += OnFileDeletedEvent;
			Handler.Renamed += OnFileRenamedEvent;

			Handler.IncludeSubdirectories = includeSubdirectories;
		}

		Handler.EnableRaisingEvents = true;
	}

	internal abstract void OnFileChangedEvent(object sender, FileSystemEventArgs e);
	internal abstract void OnFileCreatedEvent(object sender, FileSystemEventArgs e);
	internal abstract void OnFileDeletedEvent(object sender, FileSystemEventArgs e);
	internal abstract void OnFileRenamedEvent(object sender, RenamedEventArgs e);

	internal void OnDisable()
	{
		Carbon.LoaderEx.Utility.Logger.Log("FileWatcherBehaviour:OnDisable()");

		if (Handler == null) return;
		Handler.EnableRaisingEvents = false;
	}

	internal virtual void OnDestroy()
	{
		Carbon.LoaderEx.Utility.Logger.Log("FileWatcherBehaviour:OnDestroy()");
		Dispose();
	}

	public virtual void Dispose()
	{
		Handler?.Dispose();
		Handler = default;
	}
}
