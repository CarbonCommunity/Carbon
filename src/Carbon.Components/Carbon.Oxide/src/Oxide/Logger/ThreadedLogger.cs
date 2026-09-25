namespace Oxide.Core.Logging;

public enum LogType
{
	Chat,
	Error,
	Info,
	Warning,
	Debug
}

public abstract class Logger
{
	public struct LogMessage
	{
		public LogType Type;
		public string ConsoleMessage;
		public string LogfileMessage;
	}

	protected LogMessage CreateLogMessage(LogType type, string format, object[] args)
	{
		var msg = new LogMessage
		{
			Type = type,
			ConsoleMessage = $"[Carbon] {DateTime.Now.ToShortTimeString()} [{type}] {format}",
			LogfileMessage = $"{DateTime.Now.ToShortTimeString()} [{type}] {format}"
		};

		if (Interface.Oxide.Config.Console.MinimalistMode)
		{
			msg.ConsoleMessage = format;
		}

		if (args.Length == 0) return msg;
		msg.ConsoleMessage = string.Format(msg.ConsoleMessage, args);
		msg.LogfileMessage = string.Format(msg.LogfileMessage, args);
		return msg;
	}

	public virtual void Write(LogType type, string format, params object[] args)
	{
		Write(CreateLogMessage(type, format, args));
	}

	internal virtual void Write(LogMessage message)
	{
		switch (message.Type)
		{
			case LogType.Chat:
				break;

			case LogType.Info:
			case LogType.Debug:
				Carbon.Logger.Log(message.ConsoleMessage);
				break;
			case LogType.Warning:
				Carbon.Logger.Warn(message.ConsoleMessage);
				break;
			case LogType.Error:
				Carbon.Logger.Error(message.ConsoleMessage);
				break;
		}
	}
}

public class ThreadedLogger : Logger;

public class CompoundLogger : Logger;
