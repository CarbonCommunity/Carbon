namespace Carbon;

/// <summary>
/// Base file logger class used by most logging systems of Carbon.
/// </summary>
public class FileLogger : IDisposable
{
	public string Name { get; set; } = "default";

	private readonly object _sync = new();

	/// <summary>
	/// By default, each log file gets split when it reaches exactly 2.5MB in file size and sent in the archive folder.
	/// </summary>
	public int SplitSize { get; set; } = (int)(5f * 1000000f);

	public bool HasInit { get; private set; }
	private int _isFlushing;

	private readonly List<string> _buffer = [];
	private StreamWriter _file;

	public int PendingCount
	{
		get
		{
			lock (_sync)
			{
				return _buffer.Count;
			}
		}
	}

	public FileLogger() { }
	public FileLogger(string name)
	{
		Name = name;
	}

	/// <summary>
	/// Initializes current file logger.
	/// </summary>
	/// <param name="archive">If true, it'll archive the existent log file before initializing this new logger instance.</param>
	/// <param name="backup">If true, it'll back up the existent log file.</param>
	public virtual void Init(bool archive = false, bool backup = false)
	{
		lock (_sync)
		{
			if (HasInit && !archive)
			{
				return;
			}

			var path = Path.Combine(Defines.GetLogsFolder(), $"{Name}.log");
			var archiveFolder = Path.Combine(Defines.GetLogsFolder(), "archive");
			var backupFailed = false;
			OsEx.Folder.Create(archiveFolder);

			if (backup && OsEx.File.Exists(path))
			{
				try
				{
					var backupPath = Path.Combine(archiveFolder, $"{Name}.backup.{DateTime.Now:yyyy.MM.dd}.log");
					var logContent = OsEx.File.ReadText(path);

					if (OsEx.File.Exists(backupPath))
					{
						File.AppendAllText(backupPath, logContent);
					}
					else
					{
						OsEx.File.Create(backupPath, logContent);
					}
				}
				catch (Exception ex)
				{
					backupFailed = true;
					Debug.LogError($"Failed backing up the current log file. Most likely because it's in use. ({ex.Message})\n{ex.StackTrace}");
				}
			}

			if (archive && !backupFailed)
			{
				if (OsEx.File.Exists(path))
				{
					OsEx.File.Move(path, Path.Combine(archiveFolder, $"{Name}.{DateTime.Now:yyyy.MM.dd.HHmmss}.log"));
				}
			}

			if (!backupFailed)
			{
				try
				{
					File.Delete(path);
				}
				catch { }
			}
			else
			{
				path = Path.Combine(Defines.GetLogsFolder(), $"{Name}_locked.log");
			}

			HasInit = true;

			_file = new StreamWriter(path, append: true);
		}
	}

	/// <summary>
	/// File logger disposal. Flushes all changes to file, closes and terminates the file stream.
	/// </summary>
	public virtual void Dispose()
	{
		while (Interlocked.CompareExchange(ref _isFlushing, 1, 0) != 0)
		{
			Thread.Yield();
		}

		try
		{
			lock (_sync)
			{
				if (_file != null)
				{
					_file.Flush();
					_file.Close();
					_file.Dispose();
					_file = null;
				}

				HasInit = false;
			}
		}
		finally
		{
			Interlocked.Exchange(ref _isFlushing, 0);
		}
	}

	/// <summary>
	/// Forcefully writes all queued logs to file.
	/// </summary>
	public virtual void Flush()
	{
		if (Interlocked.CompareExchange(ref _isFlushing, 1, 0) != 0)
		{
			return;
		}

		try
		{
			while (true)
			{
				lock (_sync)
				{
					if (_file == null || _buffer.Count == 0)
					{
						return;
					}

					var count = _buffer.Count;
					for (var i = 0; i < count; i++)
					{
						_file.WriteLine(_buffer[i]);
					}

					_file.Flush();

					if (_buffer.Count == count)
					{
						_buffer.Clear();
					}
					else
					{
						_buffer.RemoveRange(0, count);
					}

					if (_file.BaseStream.Length > SplitSize)
					{
						_file.Flush();
						_file.Close();
						_file.Dispose();
						_file = null;

						HasInit = false;
						Init(archive: true);
					}
				}

				if (!(Community.IsConfigReady && Community.Runtime.Config.Logging.LogFileMode == 2))
				{
					return;
				}

				Thread.Yield();
			}
		}
		finally
		{
			Interlocked.Exchange(ref _isFlushing, 0);
		}
	}

	/// <summary>
	/// Queues a log to the file logger buffer, pending flushing, unless the configuration specifies immediate flushing.
	/// </summary>
	/// <param name="message"></param>
	public virtual void QueueLog(object message)
	{
		// Logging is disabled
		if (Community.IsConfigReady && Community.Runtime.Config.Logging.LogFileMode == 0)
		{
			return;
		}

		var shouldFlush = false;

		lock (_sync)
		{
			_buffer.Add($"[{Logger.GetDate()}] {message}");

			// If logging allows immediate flushing, flush
			if (Community.IsConfigReady && Community.Runtime.Config.Logging.LogFileMode == 2)
			{
				shouldFlush = true;
			}
		}

		if (shouldFlush)
		{
			Flush();
		}
	}
}
