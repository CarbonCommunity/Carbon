using HarmonyLib;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	internal const string _blankZero = "0";
	internal const string _blankUnnamed = "Unnamed";

	private void OnServerUserSet(ulong steamId, ServerUsers.UserGroup group, string playerName, string reason, long expiry)
	{
		if (Community.IsServerInitialized && group == ServerUsers.UserGroup.Banned)
		{
			var address = GetAddress(BasePlayer.FindByID(steamId));

			// OnPlayerBanned
			HookCaller.CallStaticHook(140408349, playerName, steamId, address, reason, expiry);
		}
	}
	private void OnServerUserRemove(ulong steamId)
	{
		if (Community.IsServerInitialized &&
		    ServerUsers.users.ContainsKey(steamId) &&
		    ServerUsers.users[steamId].group == ServerUsers.UserGroup.Banned)
		{
			var player = BasePlayer.FindByID(steamId);
			var name = player == null || string.IsNullOrEmpty(player.displayName) ? _blankUnnamed : player.displayName;

			// OnPlayerUnbanned
			HookCaller.CallStaticHook(1455743240, name, steamId, GetAddress(player));
		}
	}
	/// <summary>IP (without port) of a connected player, "0" otherwise.</summary>
	internal static string GetAddress(BasePlayer player) => GetAddress(player?.net?.connection?.ipaddress);

	/// <summary>Strips the port off an "ip:port" address. "0" when empty.</summary>
	internal static string GetAddress(string address)
	{
		if (string.IsNullOrEmpty(address))
		{
			return _blankZero;
		}

		var portIndex = address.LastIndexOf(':');
		return portIndex > 0 ? address.Substring(0, portIndex) : address;
	}

	private void OnSaveLoad()
	{
		StoredModifiers.Load();
	}

	[AutoPatch(Silent = true), HarmonyPatch(typeof(SaveRestore), "ShiftSaveBackups", typeof(string))]
	public class Save
	{
		public static void Prefix(string fileName)
		{
			StoredModifiers.Save();
		}
	}
}
