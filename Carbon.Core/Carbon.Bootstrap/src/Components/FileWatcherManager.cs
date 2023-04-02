using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Components;

internal sealed class FileWatcherManager : MonoBehaviour, IDisposable
{
	private class Item
	{
		public bool IncludeSubFolders { get; internal set; }
		public FileSystemWatcher Handler { get; internal set; }
		public string Directory { get; internal set; }
		public string Extension { get; internal set; }

		public OnFileChangedEvent OnFileChanged { get; internal set; }
		public delegate void OnFileChangedEvent(object sender, string path);

		public OnFileCreatedEvent OnFileCreated { get; internal set; }
		public delegate void OnFileCreatedEvent(object sender, string path);

		public OnFileDeletedEvent OnFileDeleted { get; internal set; }
		public delegate void OnFileDeletedEvent(object sender, string path);
	}

	private readonly List<Item> _watchlist = new()
	{
		new Item
		{
			Extension = "*.dll",
			IncludeSubFolders = false,
			Directory = Utility.Context.CarbonExtensions,

			OnFileCreated = (sender, file) =>
			{
				Carbon.Bootstrap.AssemblyEx.LoadExtension(
					Path.GetFileName(file), $"{typeof(FileWatcherManager)}");
			},
		},
#if EXPERIMENTAL
		new Item
		{
			Extension = "*.dll",
			IncludeSubFolders = false,
			Directory = Utility.Context.CarbonPlugins,

			OnFileCreated = (sender, file) =>
			{
				Carbon.Bootstrap.AssemblyEx.LoadPlugin(
					Path.GetFileName(file), $"{typeof(FileWatcherManager)}");
			},
		}
#endif
	};

	internal void Awake()
	{
		// by default disable the behaviour
		enabled = false;

		foreach (Item item in _watchlist)
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
		}
	}

	internal void OnEnable()
	{
		foreach (Item item in _watchlist)
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

		foreach (Item item in _watchlist)
		{
			foreach (string file in Directory.GetFiles(item.Handler.Path, item.Handler.Filter))
			{
				FileSystemEventArgs args = new FileSystemEventArgs(
					WatcherChangeTypes.Created, item.Handler.Path, Path.GetFileName(file));
				FileSystemEvent(item.Handler, args);
			}
			item.Handler.EnableRaisingEvents = false;
		}
	}

	internal void OnDestroy()
		=> Dispose();

	internal void FileSystemEvent(object sender, FileSystemEventArgs e)
	{
		Utility.Logger.Debug($"New file system event '{e.ChangeType}' triggered for file '{e.FullPath}'");

		try
		{
			Item item = _watchlist.Single(x => x.Directory == Path.GetDirectoryName(e.FullPath));

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

	private bool _disposing;

	private void Dispose(bool disposing)
	{
		if (!_disposing)
		{
			if (disposing)
			{
				foreach (Item item in _watchlist)
				{
					item.Handler.Changed -= FileSystemEvent;
					item.Handler.Created -= FileSystemEvent;
					item.Handler.Deleted -= FileSystemEvent;
					item.Handler.EnableRaisingEvents = false;
					item.Handler.Dispose();
				}
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