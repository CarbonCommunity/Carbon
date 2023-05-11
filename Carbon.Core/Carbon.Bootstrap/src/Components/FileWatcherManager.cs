using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using API.Abstracts;
using API.Assembly;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;

internal sealed class FileWatcherManager : CarbonBehaviour, IFileWatcherManager, IDisposable
{
	private readonly List<WatchFolder> _watchlist = new();

	internal void Awake()
	{
		enabled = false; // by default disable the behaviour
	}

	internal void OnEnable()
	{
		foreach (WatchFolder item in _watchlist)
		{
			foreach (string file in Directory.GetFiles(item.Handler.Path, item.Handler.Filter))
			{
				FileSystemEventArgs args = new FileSystemEventArgs(
					WatcherChangeTypes.Created, item.Handler.Path, Path.GetFileName(file));
				FileSystemEvent(item.Handler, args);
			}
			item.Handler.EnableRaisingEvents = true;
		}
	}

	internal void OnDisable()
	{
		foreach (WatchFolder item in _watchlist)
			item.Handler.EnableRaisingEvents = false;
	}

	internal void OnDestroy()
		=> Dispose();

	internal void FileSystemEvent(object sender, FileSystemEventArgs e)
	{
		Utility.Logger.Debug($"New file system event '{e.ChangeType}' triggered for file '{e.FullPath}'");

		try
		{
			WatchFolder item = _watchlist.Single(x => x.Directory == Path.GetDirectoryName(e.FullPath));

			switch (e.ChangeType)
			{
				case WatcherChangeTypes.Changed:
					item.OnFileChanged?.Invoke(sender, e.FullPath);
					break;

				case WatcherChangeTypes.Created:
					item.OnFileCreated?.Invoke(sender, e.FullPath);
					break;

				case WatcherChangeTypes.Deleted:
					item.OnFileDeleted?.Invoke(sender, e.FullPath);
					break;
			}
		}
		catch (System.Exception)
		{
			Utility.Logger.Error($"Unable to find watcher item for '{e.FullPath}");
		}
	}

	public void Watch(WatchFolder item)
	{
		try
		{
			if (string.IsNullOrEmpty(item.Extension))
				throw new ArgumentException("No file extension defined");

			if (string.IsNullOrEmpty(item.Directory) && !Directory.Exists(item.Directory))
				throw new Exception($"Unable to watch '{item.Directory}");

			if (item.Handler is null)
			{
				item.Handler = new FileSystemWatcher(item.Directory)
				{
					Filter =
						item.Extension,

					NotifyFilter =
						NotifyFilters.FileName | NotifyFilters.LastWrite
				};

				item.Handler.Changed += FileSystemEvent;
				item.Handler.Created += FileSystemEvent;
				item.Handler.Deleted += FileSystemEvent;

				item.Handler.EnableRaisingEvents = false;
				item.Handler.IncludeSubdirectories = item.IncludeSubFolders;
			}
		}
		catch (System.Exception e)
		{
			Utility.Logger.Error($"Unable to instantiate a new folder watcher", e);
			throw;
		}

		_watchlist.Add(item);
	}

	public void Unwatch(string directory)
	{
		WatchFolder item = _watchlist.Single(x => x.Directory == directory) ?? default;

		Unwatch(item);
		_watchlist.Remove(item);
	}

	internal void Unwatch(WatchFolder item)
	{
		if (item is null) return;

		item.Handler.Changed -= FileSystemEvent;
		item.Handler.Created -= FileSystemEvent;
		item.Handler.Deleted -= FileSystemEvent;
		item.Handler.EnableRaisingEvents = false;
		item.Handler.Dispose();
	}

	private bool _disposing;

	private void Dispose(bool disposing)
	{
		if (!_disposing)
		{
			if (disposing)
			{
				foreach (WatchFolder item in _watchlist)
					Unwatch(item);
				_watchlist.Clear();
			}
			_disposing = true;
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}
