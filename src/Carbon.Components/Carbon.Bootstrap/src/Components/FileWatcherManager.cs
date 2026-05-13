using System;
using System.Collections.Generic;
using System.IO;
using API.Abstracts;
using API.Assembly;
using Carbon.Extensions;
using UnityEngine;

namespace Components;

internal sealed class FileWatcherManager : CarbonBehaviour, IFileWatcherManager, IDisposable
{
	private readonly Dictionary<FileSystemWatcher, WatchFolder> _byHandler = new();

	internal void Awake()
	{
		enabled = false; // by default disable the behaviour
	}

	internal void OnEnable()
	{
		foreach (WatchFolder item in _byHandler.Values)
		{
			item.TriggerAll(WatcherChangeTypes.Created);
			item.InitialEvent = false;
		}
	}

	internal void OnDisable()
	{
		foreach (WatchFolder item in _byHandler.Values)
			item.Handler.EnableRaisingEvents = false;
	}

	internal void OnDestroy()
		=> Dispose();

	internal void FileSystemEvent(object sender, FileSystemEventArgs e)
	{
		if (sender is not FileSystemWatcher handler ||
			!_byHandler.TryGetValue(handler, out WatchFolder item))
		{
			return;
		}

		try
		{
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
		catch (System.Exception ex)
		{
			Utility.Logger.Error($"Watcher dispatch failed for '{e.FullPath}'", ex);
		}
	}

	public void Watch(WatchFolder item)
	{
		try
		{
			if (string.IsNullOrEmpty(item.Extension))
				throw new ArgumentException("No file extension defined");

			if (string.IsNullOrEmpty(item.Directory) || !Directory.Exists(item.Directory))
				throw new Exception($"Unable to watch '{item.Directory}'");

			var normalizedDirectory = PathEx.NormalizePath(item.Directory);

			foreach (WatchFolder existing in _byHandler.Values)
			{
				if (PathEx.Equals(PathEx.NormalizePath(existing.Directory), normalizedDirectory) &&
					PathEx.Equals(existing.Extension, item.Extension))
				{
					throw new InvalidOperationException(
						$"Already watching '{item.Directory}' with filter '{item.Extension}'");
				}
			}

			if (item.Handler is null)
			{
				item.Handler = new FileSystemWatcher(item.Directory)
				{
					Filter = item.Extension,
					NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
					IncludeSubdirectories = item.IncludeSubFolders,
					InternalBufferSize = 64 * 1024,
					EnableRaisingEvents = false
				};

				item.Handler.Changed += FileSystemEvent;
				item.Handler.Created += FileSystemEvent;
				item.Handler.Deleted += FileSystemEvent;
				item.Handler.Error += OnWatcherError;
			}
		}
		catch (System.Exception e)
		{
			Utility.Logger.Error($"Unable to instantiate a new folder watcher", e);
			throw;
		}

		_byHandler[item.Handler] = item;
	}

	public void Unwatch(string directory)
	{
		WatchFolder match = null;

		foreach (WatchFolder item in _byHandler.Values)
		{
			if (item.Directory == directory)
			{
				match = item;
				break;
			}
		}

		if (match == null) return;

		var handler = match.Handler;
		UnwatchInternal(match);
		if (handler != null) _byHandler.Remove(handler);
	}

	private void UnwatchInternal(WatchFolder item)
	{
		if (item?.Handler is null) return;

		item.Handler.Changed -= FileSystemEvent;
		item.Handler.Created -= FileSystemEvent;
		item.Handler.Deleted -= FileSystemEvent;
		item.Handler.Error -= OnWatcherError;
		item.Handler.EnableRaisingEvents = false;
		item.Handler.Dispose();
	}

	private void OnWatcherError(object sender, ErrorEventArgs e)
	{
		var directory = sender is FileSystemWatcher h ? h.Path : "?";
		var ex = e.GetException();
		Utility.Logger.Error($"FileSystemWatcher error in '{directory}': {ex?.Message}", ex);
	}

	private bool _disposing;

	private void Dispose(bool disposing)
	{
		if (!_disposing)
		{
			if (disposing)
			{
				foreach (WatchFolder item in _byHandler.Values)
					UnwatchInternal(item);
				_byHandler.Clear();
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
