///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 
using System;
using System.Collections.Generic;
using System.IO;
using Carbon.Interfaces;

namespace Carbon.Patterns;

internal abstract class FileWatcherBehaviour : CarbonBehaviour, IBase, IDisposable
{
	internal string directory;
	internal string extension;
	internal bool includeSubdirectories;

	internal List<string> Blacklist;

	private FileSystemWatcher Handler;

	internal virtual void Awake()
	{
		Carbon.Utility.Logger.Log("FileWatcherBehaviour:Awake()");

		directory = string.Empty;
		extension = string.Empty;
		includeSubdirectories = false;
		Blacklist = new List<string>();

		Initialize();
	}

	public abstract void Initialize();

	internal void OnEnable()
	{
		Carbon.Utility.Logger.Log("FileWatcherBehaviour:OnEnable()");

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
					NotifyFilters.LastWrite |
					NotifyFilters.FileName |
					NotifyFilters.LastAccess |
					NotifyFilters.FileName
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
		Carbon.Utility.Logger.Log("FileWatcherBehaviour:OnDisable()");

		if (Handler == null) return;
		Handler.EnableRaisingEvents = false;
	}

	internal virtual void OnDestroy()
	{
		Carbon.Utility.Logger.Log("FileWatcherBehaviour:OnDestroy()");

		Handler?.Dispose();
		Handler = default;
		Dispose();
	}

	public abstract void Dispose();
}