using System;
using System.Collections.Generic;
using System.IO;
using Carbon.Core;
using Carbon.Extensions;

namespace Carbon
{
	internal class FileLogger
	{
		internal static bool _hasInit;
		internal static List<string> _buffer = new List<string>();
		internal static StreamWriter _file;
		internal static TimeSince _timeSinceFlush;
		internal static int _splitSize = (int)(2.5f * 1000000f);

		internal static void _init(bool archive = false)
		{
			if (_hasInit) return;

			try
			{
				File.Delete(Harmony.FileLog.logPath);
				File.Delete(HarmonyLib.FileLog.LogPath);
			}
			catch { }

			_hasInit = true;

			var path = Path.Combine(Defines.GetLogsFolder(), "carbon_log.txt");

			if (archive)
			{
				if (OsEx.File.Exists(path))
				{
					OsEx.File.Move(path, Path.Combine(Defines.GetLogsFolder(), "archive", $"carbon_log_{DateTime.Now:yyyy.MM.dd.HHmmss}.txt"));
				}
			}

			_file = new StreamWriter(path, append: true);
		}
		internal static void _dispose()
		{
			_file.Flush();
			_file.Close();
			_file.Dispose();

			_hasInit = false;
		}
		internal static void _flush()
		{
			foreach (var line in _buffer)
			{
				_file?.WriteLine(line);
			}

			_file.Flush();
			_buffer.Clear();
			_timeSinceFlush = 0;

			if (_file.BaseStream.Length > _splitSize)
			{
				_dispose();
				_init(archive: true);
			}
		}
		internal static void _queueLog(string message)
		{
			if (Community.IsConfigReady && Community.Runtime.Config.LogFileMode == 0) return;

			_buffer.Add($"[{Logger.GetDate()}] {message}");
			if (Community.IsConfigReady && Community.Runtime.Config.LogFileMode == 2) _flush();
		}
	}
}
