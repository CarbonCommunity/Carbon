namespace Carbon;

public partial class Community
{
	public static void LogCommand(object message, BasePlayer player = null)
	{
		if (player == null)
		{
			Carbon.Logger.Log(message);
			return;
		}

		player.SendConsoleCommand($"echo {message}");
	}
}
