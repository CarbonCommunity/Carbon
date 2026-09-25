namespace Oxide.Compatibility;

#pragma warning disable IDE0051

/// <summary>
/// Bridges Carbon's native player hooks to Oxide's covalence (IPlayer) hooks.
/// Carbon only calls the BasePlayer variants, this plugin re-emits them for Oxide plugins.
/// </summary>
[Info("OxideCore", "Carbon Community", "1.0.0")]
[Description("Oxide compatibility: covalence hooks.")]
public partial class OxideCore : CarbonPlugin
{
	private static readonly uint OnUserConnected = HookStringPool.GetOrAdd(nameof(OnUserConnected));
	private static readonly uint OnUserDisconnected = HookStringPool.GetOrAdd(nameof(OnUserDisconnected));
	private static readonly uint OnUserKicked = HookStringPool.GetOrAdd(nameof(OnUserKicked));
	private static readonly uint OnUserRespawn = HookStringPool.GetOrAdd(nameof(OnUserRespawn));
	private static readonly uint OnUserRespawned = HookStringPool.GetOrAdd(nameof(OnUserRespawned));
	private static readonly uint OnUserChat = HookStringPool.GetOrAdd(nameof(OnUserChat));
	private static readonly uint OnUserCommand = HookStringPool.GetOrAdd(nameof(OnUserCommand));
	private static readonly uint OnUserBanned = HookStringPool.GetOrAdd(nameof(OnUserBanned));
	private static readonly uint OnUserUnbanned = HookStringPool.GetOrAdd(nameof(OnUserUnbanned));
	private static readonly uint OnPlayerLanguageChangedHook = HookStringPool.GetOrAdd("OnPlayerLanguageChanged");

	private void OnServerInitialized()
	{
		// Make sure everyone online has a covalence player attached
		foreach (var player in BasePlayer.allPlayerList)
		{
			if (!player.IsNpc)
			{
				player.AsIPlayer();
			}
		}
	}

	private void OnServerShutdown()
	{
		Interface.Oxide.OnShutdown();
	}

	private void OnPlayerConnected(BasePlayer player)
	{
		HookCaller.CallStaticHook(OnUserConnected, player.AsIPlayer());
	}

	private void OnPlayerDisconnected(BasePlayer player, string reason)
	{
		HookCaller.CallStaticHook(OnUserDisconnected, player?.AsIPlayer(), reason);
	}

	private void OnPlayerKicked(BasePlayer player, string reason)
	{
		HookCaller.CallStaticHook(OnUserKicked, player.AsIPlayer(), reason);
	}

	private object OnPlayerRespawn(BasePlayer player)
	{
		return HookCaller.CallStaticHook(OnUserRespawn, player.AsIPlayer());
	}

	private void OnPlayerRespawned(BasePlayer player)
	{
		HookCaller.CallStaticHook(OnUserRespawned, player.AsIPlayer());
	}

	private object OnPlayerChat(BasePlayer player, string message, ConVar.Chat.ChatChannel channel)
	{
		return HookCaller.CallStaticHook(OnUserChat, player.AsIPlayer(), message);
	}

	private object OnPlayerCommand(BasePlayer player, string command, string[] args)
	{
		return HookCaller.CallStaticHook(OnUserCommand, player.AsIPlayer(), command, args);
	}

	private void OnPlayerLanguageChanged(BasePlayer player, string language)
	{
		// Same hook name, IPlayer flavour. Our own BasePlayer overload won't match the IPlayer arguments.
		HookCaller.CallStaticHook(OnPlayerLanguageChangedHook, player.AsIPlayer(), language);
	}

	private void OnPlayerBanned(string name, ulong id, string address, string reason, long expiry)
	{
		HookCaller.CallStaticHook(OnUserBanned, name, id.ToString(), address, reason, expiry);
	}

	private void OnPlayerUnbanned(string name, ulong id, string address)
	{
		HookCaller.CallStaticHook(OnUserUnbanned, name, id.ToString(), address);
	}
}
