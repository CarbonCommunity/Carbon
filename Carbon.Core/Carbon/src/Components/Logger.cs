///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Facepunch;

namespace Carbon
{
	public class Logger
	{
		public enum Severity
		{
			Error, Warning, Notice, Debug
		}

		internal static string GetDate()
		{
			return DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
		}
		internal static void Write(Severity severity, object message, Exception ex = null, int verbosity = 1)
		{
			FileLogger._init();

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
						FileLogger._queueLog($"[ERRO] {message} ({dex?.Message})\n{dex?.StackTrace}");
					}
					else
					{
						UnityEngine.Debug.LogError(message);
						FileLogger._queueLog($"[ERRO] {message}");
					}
					break;

				case Severity.Warning:
					UnityEngine.Debug.LogWarning($"{message}");
					FileLogger._queueLog($"[WARN] {message}");
					break;

				case Severity.Notice:
					UnityEngine.Debug.Log($"{message}");
					FileLogger._queueLog($"[INFO] {message}");
					break;

				case Severity.Debug:
					int minVerbosity = Community.Runtime?.Config?.LogVerbosity ?? -1;
					if (verbosity > minVerbosity) break;
					UnityEngine.Debug.Log($"{message}");
					FileLogger._queueLog($"[INFO] {message}");
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
			=> Write(Logger.Severity.Debug, $"[CRBN.{header}] {message}", null, verbosity);

		/// <summary>
		/// Outputs to the game's console a message with severity level 'DEBUG'.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="verbosity"></param>
		public static void Debug(object message, int verbosity)
			=> Write(Logger.Severity.Debug, $"[CRBN] {message}", null, verbosity);

		/// <summary>
		/// Outputs to the game's console a message with severity level 'DEBUG'.
		/// </summary>
		/// <param name="header"></param>
		/// <param name="message"></param>
		public static void Debug(object header, object message)
			=> Write(Logger.Severity.Debug, $"[CRBN.{header}] {message}");

		/// <summary>
		/// Outputs to the game's console a message with severity level 'DEBUG'.
		/// </summary>
		/// <param name="message"></param>
		public static void Debug(object message)
			=> Write(Logger.Severity.Debug, $"[CRBN] {message}");

		/// <summary>
		/// Outputs to the game's console a message with severity level 'NOTICE'.
		/// </summary>
		/// <param name="message"></param>
		public static void Log(object message)
			=> Write(Logger.Severity.Notice, message);

		/// <summary>
		/// Outputs to the game's console a message with severity level 'WARNING'.
		/// DEBUG IS ENABLED FOR THIS METHOD.
		/// </summary>
		/// <param name="message"></param>
		public static void Warn(object message)
			=> Write(Logger.Severity.Warning, message);

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

			Write(Logger.Severity.Error, message, ex);
		}
#else
		public static void Error(object message, Exception ex = null)
			=> Write(Logger.Severity.Error, message, ex);
#endif
	}
}
