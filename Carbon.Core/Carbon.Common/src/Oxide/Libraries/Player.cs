using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Carbon;
using Network;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Game.Rust.Libraries;

public class Player : Library
{
	internal static readonly string ipPattern = ":{1}[0-9]{1}\\d*";
	internal Permission permission => Community.Runtime.CorePlugin.permission;

	public CultureInfo Language(BasePlayer player)
	{
		try
		{
			return CultureInfo.GetCultureInfo(player.net.connection.info.GetString("global.language", "en"));
		}
		catch (CultureNotFoundException)
		{
			return CultureInfo.GetCultureInfo("en");
		}
	}

	public string Address(Connection connection)
	{
		return Regex.Replace(connection.ipaddress, ipPattern, "");
	}
	public string Address(BasePlayer player)
	{
		bool flag;

		if (player == null)
		{
			flag = (null != null);
		}
		else
		{
			var net = player.net;
			flag = (((net != null) ? net.connection : null) != null);
		}

		if (!flag)
		{
			return null;
		}

		return Address(player.net.connection);
	}

	public int Ping(Connection connection)
	{
		return Net.sv.GetAveragePing(connection);
	}
	public int Ping(BasePlayer player)
	{
		return Ping(player.net.connection);
	}

	public bool IsAdmin(ulong id)
	{
		return ServerUsers.Is(id, ServerUsers.UserGroup.Owner) || DeveloperList.Contains(id);
	}
	public bool IsAdmin(string id)
	{
		return IsAdmin(Convert.ToUInt64(id));
	}
	public bool IsAdmin(BasePlayer player)
	{
		return IsAdmin(player.userID);
	}

	public bool IsBanned(ulong id)
	{
		return ServerUsers.Is(id, ServerUsers.UserGroup.Banned);
	}
	public bool IsBanned(string id)
	{
		return IsBanned(Convert.ToUInt64(id));
	}
	public bool IsBanned(BasePlayer player)
	{
		return IsBanned(player.userID);
	}
	public bool IsConnected(BasePlayer player)
	{
		return player.IsConnected;
	}

	public bool IsSleeping(ulong id)
	{
		return BasePlayer.FindSleeping(id);
	}
	public bool IsSleeping(string id)
	{
		return IsSleeping(Convert.ToUInt64(id));
	}
	public bool IsSleeping(BasePlayer player)
	{
		return IsSleeping(player.userID);
	}

	public void Ban(ulong id, string reason = "")
	{
		if (IsBanned(id))
		{
			return;
		}

		var basePlayer = FindById(id);

		ServerUsers.Set(id, ServerUsers.UserGroup.Banned, ((basePlayer != null) ? basePlayer.displayName : null) ?? "Unknown", reason, -1L);
		ServerUsers.Save();

		if (basePlayer != null && IsConnected(basePlayer))
		{
			Kick(basePlayer, reason);
		}
	}
	public void Ban(string id, string reason = "")
	{
		Ban(Convert.ToUInt64(id), reason);
	}
	public void Ban(BasePlayer player, string reason = "")
	{
		Ban(player.UserIDString, reason);
	}
	public void Heal(BasePlayer player, float amount)
	{
		player.Heal(amount);
	}
	public void Hurt(BasePlayer player, float amount)
	{
		player.Hurt(amount);
	}
	public void Kick(BasePlayer player, string reason = "")
	{
		player.Kick(reason);
	}
	public void Kill(BasePlayer player)
	{
		player.Die(null);
	}

	public void Rename(BasePlayer player, string name)
	{
		name = (string.IsNullOrEmpty(name.Trim()) ? player.displayName : name);

		SingletonComponent<ServerMgr>.Instance.persistance.SetPlayerName(player.userID, name);
		player.net.connection.username = name;
		player.displayName = name;
		player._name = name;
		player.SendNetworkUpdateImmediate(false);
		permission.UpdateNickname(player.UserIDString, name);
		Teleport(player, player.transform.position);
	}

	public void Teleport(BasePlayer player, Vector3 destination)
	{
		if (player.IsAlive() && !player.IsSpectating())
		{
			try
			{
				player.EnsureDismounted();
				player.SetParent(null, true, true);
				player.SetServerFall(true);
				player.MovePosition(destination);
				player.ClientRPCPlayer<Vector3>(null, player, "ForcePositionTo", destination);
			}
			finally
			{
				player.SetServerFall(false);
			}
		}
	}
	public void Teleport(BasePlayer player, BasePlayer target)
	{
		Teleport(player, Position(target));
	}
	public void Teleport(BasePlayer player, float x, float y, float z)
	{
		Teleport(player, new Vector3(x, y, z));
	}

	public void Unban(ulong id)
	{
		if (!this.IsBanned(id))
		{
			return;
		}
		ServerUsers.Remove(id);
		ServerUsers.Save();
	}
	public void Unban(string id)
	{
		Unban(Convert.ToUInt64(id));
	}
	public void Unban(BasePlayer player)
	{
		Unban(player.userID);
	}

	public Vector3 Position(BasePlayer player)
	{
		return player.transform.position;
	}

	public BasePlayer Find(string nameOrIdOrIp)
	{
		foreach (BasePlayer basePlayer in this.Players)
		{
			if (nameOrIdOrIp.Equals(basePlayer.displayName, StringComparison.OrdinalIgnoreCase) || nameOrIdOrIp.Equals(basePlayer.UserIDString) || nameOrIdOrIp.Equals(basePlayer.net.connection.ipaddress))
			{
				return basePlayer;
			}
		}

		return null;
	}

	public BasePlayer FindById(string id)
	{
		foreach (BasePlayer basePlayer in this.Players)
		{
			if (id.Equals(basePlayer.UserIDString))
			{
				return basePlayer;
			}
		}

		return null;
	}
	public BasePlayer FindById(ulong id)
	{
		foreach (BasePlayer basePlayer in this.Players)
		{
			if (id.Equals(basePlayer.userID))
			{
				return basePlayer;
			}
		}
		return null;
	}

	public ListHashSet<BasePlayer> Players => BasePlayer.activePlayerList;
	public ListHashSet<BasePlayer> Sleepers => BasePlayer.sleepingPlayerList;

	public void Message(BasePlayer player, string message, string prefix, ulong userId = 0UL, params object[] args)
	{
		if (string.IsNullOrEmpty(message))
		{
			return;
		}

		message = ((args.Length != 0) ? string.Format(Formatter.ToUnity(message), args) : Formatter.ToUnity(message));
		var text = (prefix != null) ? (prefix + " " + message) : message;

		player.SendConsoleCommand("chat.add", 2, userId, text);
	}
	public void Message(BasePlayer player, string message, ulong userId = 0UL)
	{
		Message(player, message, null, userId);
	}

	public void Reply(BasePlayer player, string message, string prefix, ulong userId = 0UL, params object[] args)
	{
		Message(player, message, prefix, userId, args);
	}
	public void Reply(BasePlayer player, string message, ulong userId = 0UL)
	{
		Message(player, message, null, userId);
	}

	public void Command(BasePlayer player, string command, params object[] args)
	{
		player.SendConsoleCommand(command, args);
	}

	public void DropItem(BasePlayer player, int itemId)
	{
		var position = player.transform.position;
		var playerInventory = Inventory(player);

		for (int i = 0; i < playerInventory.containerMain.capacity; i++)
		{
			var slot = playerInventory.containerMain.GetSlot(i);

			if (slot.info.itemid == itemId)
			{
				slot.Drop(position + new Vector3(0f, 1f, 0f) + position / 2f, (position + new Vector3(0f, 0.2f, 0f)) * 8f, default);
			}
		}
		for (int j = 0; j < playerInventory.containerBelt.capacity; j++)
		{
			var slot2 = playerInventory.containerBelt.GetSlot(j);

			if (slot2.info.itemid == itemId)
			{
				slot2.Drop(position + new Vector3(0f, 1f, 0f) + position / 2f, (position + new Vector3(0f, 0.2f, 0f)) * 8f, default);
			}
		}
		for (int k = 0; k < playerInventory.containerWear.capacity; k++)
		{
			var slot3 = playerInventory.containerWear.GetSlot(k);

			if (slot3.info.itemid == itemId)
			{
				slot3.Drop(position + new Vector3(0f, 1f, 0f) + position / 2f, (position + new Vector3(0f, 0.2f, 0f)) * 8f, default);
			}
		}
	}
	public void DropItem(BasePlayer player, global::Item item)
	{
		var position = player.transform.position;
		var playerInventory = this.Inventory(player);

		for (int i = 0; i < playerInventory.containerMain.capacity; i++)
		{
			var slot = playerInventory.containerMain.GetSlot(i);

			if (slot == item)
			{
				slot.Drop(position + new Vector3(0f, 1f, 0f) + position / 2f, (position + new Vector3(0f, 0.2f, 0f)) * 8f, default);
			}
		}
		for (int i = 0; i < playerInventory.containerBelt.capacity; i++)
		{
			var slot2 = playerInventory.containerBelt.GetSlot(i);

			if (slot2 == item)
			{
				slot2.Drop(position + new Vector3(0f, 1f, 0f) + position / 2f, (position + new Vector3(0f, 0.2f, 0f)) * 8f, default);
			}
		}
		for (int i = 0; i < playerInventory.containerWear.capacity; i++)
		{
			var slot3 = playerInventory.containerWear.GetSlot(i);

			if (slot3 == item)
			{
				slot3.Drop(position + new Vector3(0f, 1f, 0f) + position / 2f, (position + new Vector3(0f, 0.2f, 0f)) * 8f, default);
			}
		}
	}

	public void GiveItem(BasePlayer player, int itemId, int quantity = 1)
	{
		GiveItem(player, Item.GetItem(itemId), quantity);
	}
	public void GiveItem(BasePlayer player, global::Item item, int quantity = 1)
	{
		player.inventory.GiveItem(ItemManager.CreateByItemID(item.info.itemid, quantity, 0UL), null);
	}

	public PlayerInventory Inventory(BasePlayer player)
	{
		return player.inventory;
	}
	public void ClearInventory(BasePlayer player)
	{
		var playerInventory = this.Inventory(player);

		if (playerInventory == null)
		{
			return;
		}
		playerInventory.Strip();
	}
	public void ResetInventory(BasePlayer player)
	{
		PlayerInventory playerInventory = this.Inventory(player);
		if (playerInventory != null)
		{
			playerInventory.DoDestroy();
			playerInventory.ServerInit(player);
		}
	}
}
