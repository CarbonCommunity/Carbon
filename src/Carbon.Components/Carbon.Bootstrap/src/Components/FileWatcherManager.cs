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
	private sealed class WatchEntry
	{
		public WatchFolder Config;
		public FileSystemWatcher Handler;
	}

	private readonly Dictionary<FileSystemWatcher, WatchEntry> _byHandler = new();

	internal void Awake()
	{
		enabled = false; // by default disable the behaviour
	}

	internal void OnEnable()
	{
		foreach (WatchEntry entry in _byHandler.Values)
		{
			DispatchInitialEvents(entry);
			entry.Handler.EnableRaisingEvents = true;
		}
	}

	internal void OnDisable()
	{
		foreach (WatchEntry entry in _byHandler.Values)
			entry.Handler.EnableRaisingEvents = false;
	}

	internal void OnDestroy()
		=> Dispose();

	private void DispatchInitialEvents(WatchEntry entry)
	{
		var callback = entry.Config.OnEvent;
		if (callback == null) return;

		foreach (string file in Directory.EnumerateFiles(entry.Handler.Path, entry.Handler.Filter))
		{
			try
			{
				callback(new WatchFileEvent(WatcherChangeTypes.Created, file, null, isInitial: true));
			}
			catch (System.Exception ex)
			{
				Utility.Logger.Error($"Initial event dispatch failed for '{file}'", ex);
			}
		}
	}

	internal void FileSystemEvent(object sender, FileSystemEventArgs e)
	{
		if (sender is not FileSystemWatcher handler ||
			!_byHandler.TryGetValue(handler, out WatchEntry entry))
		{
			return;
		}

		var callback = entry.Config.OnEvent;
		if (callback == null) return;

		try
		{
			string oldPath = e is RenamedEventArgs renamed ? renamed.OldFullPath : null;
			callback(new WatchFileEvent(e.ChangeType, e.FullPath, oldPath, isInitial: false));
		}
		catch (System.Exception ex)
		{
			Utility.Logger.Error($"Watcher dispatch failed for '{e.FullPath}'", ex);
		}
	}

	public void Watch(WatchFolder item)
	{
		FileSystemWatcher handler;

		try
		{
			if (string.IsNullOrEmpty(item.Filter))
				throw new ArgumentException("No filter defined");

			if (string.IsNullOrEmpty(item.Directory) || !Directory.Exists(item.Directory))
				throw new Exception($"Unable to watch '{item.Directory}'");

			var normalizedDirectory = PathEx.NormalizePath(item.Directory);

			foreach (WatchEntry existing in _byHandler.Values)
			{
				if (PathEx.Equals(PathEx.NormalizePath(existing.Config.Directory), normalizedDirectory) &&
					PathEx.Equals(existing.Config.Filter, item.Filter))
				{
					throw new InvalidOperationException(
						$"Already watching '{item.Directory}' with filter '{item.Filter}'");
				}
			}

			handler = new FileSystemWatcher(item.Directory)
			{
				Filter = item.Filter,
				NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
				IncludeSubdirectories = item.IncludeSubFolders,
				InternalBufferSize = 64 * 1024,
				EnableRaisingEvents = false
			};

			handler.Changed += FileSystemEvent;
			handler.Created += FileSystemEvent;
			handler.Renamed += FileSystemEvent;
			handler.Deleted += FileSystemEvent;
			handler.Error += OnWatcherError;
		}
		catch (System.Exception e)
		{
			Utility.Logger.Error("Unable to instantiate a new folder watcher", e);
			throw;
		}

		_byHandler[handler] = new WatchEntry { Config = item, Handler = handler };
	}

	public void Unwatch(WatchFolder item)
	{
		if (item == null) return;

		FileSystemWatcher handler = null;
		foreach (var pair in _byHandler)
		{
			if (ReferenceEquals(pair.Value.Config, item))
			{
				handler = pair.Key;
				break;
			}
		}

		if (handler == null) return;

		UnwatchInternal(handler);
		_byHandler.Remove(handler);
	}

	public void Unwatch(string directory)
	{
		if (string.IsNullOrEmpty(directory)) return;

		var normalizedDirectory = PathEx.NormalizePath(directory);

		FileSystemWatcher handler = null;
		foreach (var pair in _byHandler)
		{
			if (PathEx.Equals(PathEx.NormalizePath(pair.Value.Config.Directory), normalizedDirectory))
			{
				handler = pair.Key;
				break;
			}
		}

		if (handler == null) return;

		UnwatchInternal(handler);
		_byHandler.Remove(handler);
	}

	private void UnwatchInternal(FileSystemWatcher handler)
	{
		if (handler == null) return;

		handler.Changed -= FileSystemEvent;
		handler.Created -= FileSystemEvent;
		handler.Renamed -= FileSystemEvent;
		handler.Deleted -= FileSystemEvent;
		handler.Error -= OnWatcherError;
		handler.EnableRaisingEvents = false;
		handler.Dispose();
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
				foreach (var handler in _byHandler.Keys)
					UnwatchInternal(handler);
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