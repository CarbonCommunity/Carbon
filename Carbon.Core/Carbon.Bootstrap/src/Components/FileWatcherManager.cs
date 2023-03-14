using System;
using System.Collections.Generic;
using System.IO;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;

internal abstract class FileWatcherManager : IDisposable
{
	private string _directory;
	private string _extension;
	internal bool includeSubdirectories;

	internal List<string> Blacklist;

	private FileSystemWatcher Handler;

	internal virtual void Awake()
	{
		//Carbon.LoaderEx.Utility.Logger.Log("FileWatcherBehaviour:Awake()");

		_directory = string.Empty;
		_extension = string.Empty;
		includeSubdirectories = false;
		Blacklist = new List<string>();
	}

	internal void OnEnable()
	{
		//Carbon.LoaderEx.Utility.Logger.Log("FileWatcherBehaviour:OnEnable()");

		if (string.IsNullOrEmpty(_extension))
			throw new ArgumentException("No file extension defined");

		if (string.IsNullOrEmpty(_directory) && !Directory.Exists(_directory))
			throw new Exception($"Unable to watch '{_directory}");

		if (Handler is null)
		{
			Handler = new FileSystemWatcher(_directory)
			{
				Filter =
					_extension,

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
		//Carbon.LoaderEx.Utility.Logger.Log("FileWatcherBehaviour:OnDisable()");

		if (Handler == null) return;
		Handler.EnableRaisingEvents = false;
	}

	internal virtual void OnDestroy()
	{
		//Carbon.LoaderEx.Utility.Logger.Log("FileWatcherBehaviour:OnDestroy()");
		Dispose();
	}

	public virtual void Dispose()
	{
		Handler?.Dispose();
		Handler = default;
	}
}