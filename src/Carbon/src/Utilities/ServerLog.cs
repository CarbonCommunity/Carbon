using System;
using System.IO;
using Carbon.Core;
using Carbon.Extensions;
using UnityEngine;

namespace Carbon;

public class ServerLog : ILogHandler, IDisposable
{
	internal FileStream _stream;
	internal StreamWriter _writer;
	internal ILogHandler _default = Debug.unityLogger.logHandler;

	public ServerLog()
	{
		_stream = new FileStream(CommandLineEx.GetArgumentResult("-logfile", Path.Combine(Defines.GetLogsFolder(), $"Server.{ConVar.Server.identity}.txt")), FileMode.OpenOrCreate, FileAccess.ReadWrite);
		_writer = new StreamWriter(_stream);

		Debug.unityLogger.logHandler = this;
	}

	public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
	{
		_writer.WriteLine(string.Format(format, args));
		_writer.Flush();
		_default.LogFormat(logType, context, format, args);
	}

	public void LogException(Exception exception, UnityEngine.Object context)
	{
		_default.LogException(exception, context);
	}

	public void Dispose()
	{
		Debug.unityLogger.logHandler = _default;
	}
}
