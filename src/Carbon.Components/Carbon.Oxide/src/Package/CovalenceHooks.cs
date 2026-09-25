namespace Oxide.Compatibility;

#pragma warning disable IDE0051

/// <summary>
/// Metadata of the covalence hooks emitted by <see cref="OxideCore"/>, so they show up in hook listings.
/// </summary>
public class CovalenceHooks
{
	[HookAttribute.Patch("OnUserConnected", "OnUserConnected", typeof(OxideCore), "OnPlayerConnected")]
	[HookAttribute.Options(HookFlags.MetadataOnly)]
	[MetadataAttribute.Category("Player")]
	[MetadataAttribute.Info("Gets called when a covalence player connects.")]
	[MetadataAttribute.Parameter("player", typeof(IPlayer))]
	[MetadataAttribute.Return(typeof(void), Discarded = true)]
	public class OnUserConnected : Patch;

	[HookAttribute.Patch("OnUserDisconnected", "OnUserDisconnected", typeof(OxideCore), "OnPlayerDisconnected")]
	[HookAttribute.Options(HookFlags.MetadataOnly)]
	[MetadataAttribute.Category("Player")]
	[MetadataAttribute.Info("Gets called when a covalence player disconnects.")]
	[MetadataAttribute.Parameter("player", typeof(IPlayer))]
	[MetadataAttribute.Parameter("reason", typeof(string))]
	[MetadataAttribute.Return(typeof(void), Discarded = true)]
	public class OnUserDisconnected : Patch;

	[HookAttribute.Patch("OnUserKicked", "OnUserKicked", typeof(OxideCore), "OnPlayerKicked")]
	[HookAttribute.Options(HookFlags.MetadataOnly)]
	[MetadataAttribute.Category("Player")]
	[MetadataAttribute.Info("Gets called when a covalence player gets kicked.")]
	[MetadataAttribute.Parameter("player", typeof(IPlayer))]
	[MetadataAttribute.Parameter("reason", typeof(string))]
	[MetadataAttribute.Return(typeof(void), Discarded = true)]
	public class OnUserKicked : Patch;

	[HookAttribute.Patch("OnUserRespawn", "OnUserRespawn", typeof(OxideCore), "OnPlayerRespawn")]
	[HookAttribute.Options(HookFlags.MetadataOnly)]
	[MetadataAttribute.Category("Player")]
	[MetadataAttribute.Info("Gets called when a covalence player respawns.")]
	[MetadataAttribute.Info("Return a BasePlayer.SpawnPoint, or a SleepingBag when respawning at a bag, to change where the player respawns.")]
	[MetadataAttribute.Parameter("player", typeof(IPlayer))]
	[MetadataAttribute.Return(typeof(object))]
	public class OnUserRespawn : Patch;

	[HookAttribute.Patch("OnUserRespawned", "OnUserRespawned", typeof(OxideCore), "OnPlayerRespawned")]
	[HookAttribute.Options(HookFlags.MetadataOnly)]
	[MetadataAttribute.Category("Player")]
	[MetadataAttribute.Info("Gets called when a covalence player fully respawned.")]
	[MetadataAttribute.Parameter("player", typeof(IPlayer))]
	[MetadataAttribute.Return(typeof(void), Discarded = true)]
	public class OnUserRespawned : Patch;

	[HookAttribute.Patch("OnUserChat", "OnUserChat", typeof(OxideCore), "OnPlayerChat")]
	[HookAttribute.Options(HookFlags.MetadataOnly)]
	[MetadataAttribute.Category("Player")]
	[MetadataAttribute.Info("Gets called when a player sends a chat message.")]
	[MetadataAttribute.Info("Return a bool to block the chat message.")]
	[MetadataAttribute.Parameter("player", typeof(IPlayer))]
	[MetadataAttribute.Parameter("message", typeof(string))]
	[MetadataAttribute.Return(typeof(bool))]
	public class OnUserChat : Patch;

	[HookAttribute.Patch("OnUserCommand", "OnUserCommand [IPlayer]", typeof(OxideCore), "OnPlayerCommand")]
	[HookAttribute.Options(HookFlags.MetadataOnly)]
	[MetadataAttribute.Category("Player")]
	[MetadataAttribute.Info("Gets called when a player executes command.")]
	[MetadataAttribute.Parameter("player", typeof(IPlayer))]
	[MetadataAttribute.Parameter("command", typeof(string))]
	[MetadataAttribute.Parameter("args", typeof(string[]))]
	[MetadataAttribute.Return(typeof(void))]
	public class OnUserCommand_IPlayer : Patch;

	[HookAttribute.Patch("OnPlayerLanguageChanged", "OnPlayerLanguageChanged [IPlayer]", typeof(OxideCore), "OnPlayerLanguageChanged")]
	[HookAttribute.Options(HookFlags.MetadataOnly)]
	[MetadataAttribute.Category("Player")]
	[MetadataAttribute.Info("Gets called when a player's language gets changed.")]
	[MetadataAttribute.Parameter("player", typeof(IPlayer))]
	[MetadataAttribute.Parameter("var", typeof(string))]
	[MetadataAttribute.Return(typeof(void), Discarded = true)]
	public class OnPlayerLanguageChanged_IPlayer : Patch;

	[HookAttribute.Patch("OnUserBanned", "OnUserBanned", typeof(OxideCore), "OnPlayerBanned")]
	[HookAttribute.Options(HookFlags.MetadataOnly)]
	[MetadataAttribute.Category("Player")]
	[MetadataAttribute.Info("Gets called when a player gets banned.")]
	[MetadataAttribute.Parameter("playerName", typeof(string))]
	[MetadataAttribute.Parameter("playerId", typeof(string))]
	[MetadataAttribute.Parameter("address", typeof(string))]
	[MetadataAttribute.Parameter("reason", typeof(string))]
	[MetadataAttribute.Parameter("expiry", typeof(long))]
	[MetadataAttribute.Return(typeof(void), Discarded = true)]
	public class OnUserBanned : Patch;

	[HookAttribute.Patch("OnUserUnbanned", "OnUserUnbanned", typeof(OxideCore), "OnPlayerUnbanned")]
	[HookAttribute.Options(HookFlags.MetadataOnly)]
	[MetadataAttribute.Category("Player")]
	[MetadataAttribute.Info("Gets called when a player gets unbanned.")]
	[MetadataAttribute.Parameter("playerName", typeof(string))]
	[MetadataAttribute.Parameter("playerId", typeof(string))]
	[MetadataAttribute.Parameter("address", typeof(string))]
	[MetadataAttribute.Return(typeof(void), Discarded = true)]
	public class OnUserUnbanned : Patch;
}
