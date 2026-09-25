namespace Carbon.Plugins;

/// <summary>
/// The standard plugin base for Rust. Adds CUI, chat/console printing and command cooldowns on top of <see cref="Plugin"/>.
/// </summary>
public class CarbonPlugin : Plugin
{
	public CUI.Handler CuiHandler { get; set; }

	public override void Setup(string name, string author, VersionNumber version, string description)
	{
		base.Setup(name, author, version, description);

		CuiHandler = new CUI.Handler();
	}

	#region CUI

	public CUI CreateCUI()
	{
		return new CUI(CuiHandler);
	}

	#endregion

	#region Printing

	private static string Format(string format, object[] args) => args == null || args.Length == 0 ? format : string.Format(format, args);

	private static long ChatId =>
#if MINIMAL
		0;
#else
		Community.Runtime.Core.DefaultServerChatId;
#endif

	protected void PrintToConsole(BasePlayer player, string format, params object[] args)
	{
		if (player == null || player.net == null)
		{
			return;
		}

		player.SendConsoleCommand("echo " + Format(format, args));
	}

	protected void PrintToConsole(string format, params object[] args)
	{
		if (BasePlayer.activePlayerList.Count >= 1)
		{
			ConsoleNetwork.BroadcastToAllClients("echo " + Format(format, args));
		}
	}

	protected void PrintToChat(BasePlayer player, string format, params object[] args)
	{
		PrintToChat(player, format, ChatId, args);
	}

	protected void PrintToChat(string format, params object[] args)
	{
		PrintToChat(format, ChatId, args);
	}

	protected void PrintToChat(BasePlayer player, string format, long chatId, params object[] args)
	{
		if (player == null || player.net == null)
		{
			return;
		}

		player.SendConsoleCommand("chat.add", 2, chatId, Format(format, args));
	}

	protected void PrintToChat(string format, long chatId, params object[] args)
	{
		if (BasePlayer.activePlayerList.Count > 0)
		{
			ConsoleNetwork.BroadcastToAllClients("chat.add", 2, chatId, Format(format, args));
		}
	}

	/// <summary>Replies to whoever ran a console command: the player's console, or the server log.</summary>
	protected void SendReply(ConsoleSystem.Arg arg, string format, params object[] args)
	{
		var basePlayer = arg?.Connection?.player as BasePlayer;

		if (basePlayer != null && basePlayer.net != null)
		{
			basePlayer.SendConsoleCommand($"echo {Format(format, args)}");
			return;
		}

		Puts(format, args);
	}

	protected void SendReply(BasePlayer player, string format, params object[] args)
	{
		PrintToChat(player, format, args);
	}

	protected void SendWarning(ConsoleSystem.Arg arg, string format, params object[] args)
	{
		var basePlayer = arg?.Connection?.player as BasePlayer;

		if (basePlayer != null && basePlayer.net != null)
		{
			basePlayer.SendConsoleCommand($"echo {Format(format, args)}");
			return;
		}

		PrintWarning(format, args);
	}

	protected void SendError(ConsoleSystem.Arg arg, string format, params object[] args)
	{
		var basePlayer = arg?.Connection?.player as BasePlayer;

		if (basePlayer != null && basePlayer.net != null)
		{
			basePlayer.SendConsoleCommand($"echo {Format(format, args)}");
			return;
		}

		PrintError(format, args);
	}

	#endregion

	protected void ForcePlayerPosition(BasePlayer player, Vector3 destination)
	{
		player.MovePosition(destination);

		if (!player.IsSpectating() || Vector3.Distance(player.transform.position, destination) > 25.0)
		{
			player.ClientRPC(RpcTarget.Player("ForcePositionTo", player), destination);
			return;
		}

		player.SendNetworkUpdate(BasePlayer.NetworkQueue.UpdateDistance);
	}

	#region Command Cooldown

	internal static Dictionary<BasePlayer, List<CooldownInstance>> CommandCooldownBuffer = [];

	/// <summary>
	/// Checks (and by default starts) a per-player cooldown for <paramref name="command"/>.
	/// Returns true while the player is still cooling down.
	/// </summary>
	public static bool IsCommandCooledDown(
		BasePlayer player, string command,
		int time,
		out float timeLeft,
		bool doCooldownIfNot = true,
		float appendMultiplier = 0.5f,
		bool doCooldownPenalty = false)
	{
		timeLeft = -1;
		if (time == 0 || player == null)
		{
			return false;
		}

		if (!CommandCooldownBuffer.TryGetValue(player, out var pairs))
		{
			CommandCooldownBuffer.Add(player, pairs = []);
		}

		var lookupCommand = pairs.FirstOrDefault(x => x.Command == command);
		if (lookupCommand == null)
		{
			pairs.Add(lookupCommand = new CooldownInstance { Command = command });
		}

		var timePassed = DateTime.Now - lookupCommand.LastCall;
		if (timePassed.TotalMilliseconds >= time)
		{
			if (doCooldownIfNot)
			{
				lookupCommand.LastCall = DateTime.Now;
			}

			return false;
		}

		timeLeft = (float)((time - timePassed.TotalMilliseconds) * 0.001f);

		if (doCooldownPenalty)
		{
			lookupCommand.LastCall = lookupCommand.LastCall.AddMilliseconds(time * appendMultiplier);
		}

		return true;
	}

	internal sealed class CooldownInstance
	{
		public string Command;
		public DateTime LastCall;
	}

	#endregion
}
