using System;
using System.Collections.Generic;
using System.IO;
using Carbon.Core;
using Carbon.Extensions;
using Facepunch;

namespace Carbon
{
	internal class FileLogger
	{
		internal static bool _hasInit;
		internal static List<string> _buffer = new List<string>();
		internal static StreamWriter _file;
		internal static int _splitSize = (int)(2.5f * 1000000f);

		internal static void _init(bool archive = false)
		{
			if (_hasInit) return;

			var path = Path.Combine(Defines.GetLogsFolder(), "Carbon.Core.log");

			try
			{
				File.Delete(path);
				File.Delete(Harmony.FileLog.logPath);
				File.Delete(HarmonyLib.FileLog.LogPath);
			}
			catch { }

			_hasInit = true;

			if (archive)
			{
				if (OsEx.File.Exists(path))
				{
					OsEx.File.Move(path, Path.Combine(Defines.GetLogsFolder(), "archive", $"Carbon.Core.{DateTime.Now:yyyy.MM.dd.HHmmss}.log"));
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
			var buffer = Pool.GetList<string>();
			buffer.AddRange(_buffer);

			foreach (var line in buffer)
			{
				_file?.WriteLine(line);
			}

			_file.Flush();
			_buffer.Clear();
			Pool.FreeList(ref buffer);

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
