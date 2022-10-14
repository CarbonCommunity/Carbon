///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Carbon.Core;
using Carbon.Extensions;
using Facepunch;

namespace Carbon
{
	public class Logger
	{
		public enum Severity
		{
			Error, Warning, Notice, Debug
		}

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

			_buffer.Add($"[{_getDate()}] {message}");
			if (Community.IsConfigReady && Community.Runtime.Config.LogFileMode == 2) _flush();
		}

		internal static string _getDate()
		{
			return DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
		}
		internal static void _write(Severity severity, object message, Exception ex = null, int verbosity = 1)
		{
			_init();

			if (severity != Severity.Debug)
			{
				Severity minSeverity = Community.Runtime?.Config?.LogSeverity ?? Severity.Notice;
				if (severity > minSeverity) return;
			}

			switch (severity)
			{
				case Severity.Error:
					var dex = ex?.Demystify() ?? ex;

					if (dex != null)
					{
						UnityEngine.Debug.LogError($"{message} ({dex?.Message})\n{dex?.StackTrace}");
						_queueLog($"[ERRO] {message} ({dex?.Message})\n{dex?.StackTrace}");
					}
					else
					{
						UnityEngine.Debug.LogError(message);
						_queueLog($"[ERRO] {message}");
					}
					break;

				case Severity.Warning:
					UnityEngine.Debug.LogWarning($"{message}");
					_queueLog($"[WARN] {message}");
					break;

				case Severity.Notice:
					UnityEngine.Debug.Log($"{message}");
					_queueLog($"[INFO] {message}");
					break;

				case Severity.Debug:
					int minVerbosity = Community.Runtime?.Config?.LogVerbosity ?? -1;
					if (verbosity > minVerbosity) break;
					UnityEngine.Debug.Log($"{message}");
					_queueLog($"[INFO] {message}");
					break;

				default:
					throw new Exception($"Severity {severity} not implemented.");
			}
		}

#if DEBUG
		internal static string _getFileNameEx(string input)
		{
			// For some reason Path.GetFileName() is not working with
			// [CallerFilePath]. Trying to be OS agnostic..
			string[] arr = input.Split((input.Contains("/") ? '/' : '\\'));
			string ret = arr[arr.Length - 1];

			Array.Clear(arr, 0, arr.Length);
			Pool.Free(ref arr);
			return ret;
		}
#endif

		/// <summary>
		/// Outputs to the game's console a message with severity level 'DEBUG'.
		/// </summary>
		/// <param name="header"></param>
		/// <param name="message"></param>
		/// <param name="verbosity"></param>
		public static void Debug(object header, object message, int verbosity)
			=> _write(Logger.Severity.Debug, $"[CRBN.{header}] {message}", null, verbosity);

		/// <summary>
		/// Outputs to the game's console a message with severity level 'DEBUG'.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="verbosity"></param>
		public static void Debug(object message, int verbosity)
			=> _write(Logger.Severity.Debug, $"[CRBN] {message}", null, verbosity);

		/// <summary>
		/// Outputs to the game's console a message with severity level 'DEBUG'.
		/// </summary>
		/// <param name="header"></param>
		/// <param name="message"></param>
		public static void Debug(object header, object message)
			=> _write(Logger.Severity.Debug, $"[CRBN.{header}] {message}");

		/// <summary>
		/// Outputs to the game's console a message with severity level 'DEBUG'.
		/// </summary>
		/// <param name="message"></param>
		public static void Debug(object message)
			=> _write(Logger.Severity.Debug, $"[CRBN] {message}");

		/// <summary>
		/// Outputs to the game's console a message with severity level 'NOTICE'.
		/// </summary>
		/// <param name="message"></param>
		public static void Log(object message)
			=> _write(Logger.Severity.Notice, message);

		/// <summary>
		/// Outputs to the game's console a message with severity level 'WARNING'.
		/// DEBUG IS ENABLED FOR THIS METHOD.
		/// </summary>
		/// <param name="message"></param>
		public static void Warn(object message)
			=> _write(Logger.Severity.Warning, message);

		/// <summary>
		/// Outputs to the game's console a message with severity level 'ERROR'.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="ex"></param>
#if DEBUG
		public static void Error(object message, Exception ex = null,
			[CallerLineNumber] int line = 0,
			[CallerFilePath] string path = null,
			[CallerMemberName] string method = null)
		{
			// allows the usage of Error() before config has been init
			int minVerbosity = Community.Runtime?.Config?.LogVerbosity ?? -1;

			if (minVerbosity > 0)
				message = $"{message}\n" +
						  $" [file: {_getFileNameEx(path)}, method: {method}, line: {line}]";

			_write(Logger.Severity.Error, message, ex);
		}
#else
		public static void Error(object message, Exception ex = null)
			=> _write(Logger.Severity.Error, message, ex);
#endif
	}
}
