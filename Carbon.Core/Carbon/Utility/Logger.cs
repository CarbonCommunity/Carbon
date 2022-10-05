///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Carbon.Core;
using Carbon.Patterns;
using Facepunch;

namespace Carbon
{
	public class Logger
	{
		public enum Severity
		{
			Critical, Error, Warning, Notice, Informational, Debug
		}

		internal static void Write(Severity severity, object message, Exception ex = null)
		{
			Severity threshold = CarbonCore.Instance?.Config.LogVerbosity ?? Severity.Notice;
			if (severity > threshold) return; // ^ FIXME: logger is init before config

			switch (severity)
			{
				case Severity.Critical:
				case Severity.Error:
					Exception dex = ex?.Demystify() ?? ex;
					if (dex != null) UnityEngine.Debug.LogError($"{message}{dex?.Message}\n{dex?.StackTrace}");
					else UnityEngine.Debug.LogError($"{message}");
					break;

				case Severity.Warning:
					UnityEngine.Debug.LogWarning($"{message}");
					break;

				case Severity.Notice:
				case Severity.Informational:
				case Severity.Debug:
					UnityEngine.Debug.Log($"{message}");
					break;

				default:
					throw new Exception($"Severity {severity} not implemented.");

			}
		}

#if DEBUG
		internal static string GetFileNameEx(string Input)
		{
			// For some reason Path.GetFileName() is not working with
			// [CallerFilePath]. Trying to be OS agnostic..
			string[] arr = Input.Split((Input.Contains("/") ? '/' : '\\'));
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
		public static void Debug(object header, object message)
			=> Write(Logger.Severity.Debug, $"[CRBN.{header}] {message}");

		/// <summary>
		/// Outputs to the game's console a message with severity level 'DEBUG'.
		/// </summary>
		/// <param name="message"></param>
		public static void Debug(object message)
			=> Write(Logger.Severity.Debug, $"[CRBN] {message}");

		/// <summary>
		/// Outputs to the game's console a message with severity level 'INFO'.
		/// </summary>
		/// <param name="header"></param>
		/// <param name="message"></param>
		public static void Info(object header, object message)
			=> Write(Logger.Severity.Informational, $"[CRBN.{header}] {message}");

		/// <summary>
		/// Outputs to the game's console a message with severity level 'INFO'.
		/// </summary>
		/// <param name="header"></param>
		/// <param name="message"></param>
		public static void Info(object message)
			=> Write(Logger.Severity.Informational, $"[CRBN] {message}");

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
			message = $"{message}" + Environment.NewLine +
					  $"(file: {GetFileNameEx(path)}, method: {method}, line: {line})" + Environment.NewLine;
			Write(Logger.Severity.Error, message, ex);
		}
#else
		public static void Error(object message, Exception ex = null)
			=> Write(Logger.Severity.Error, message, ex);
#endif
	}
}