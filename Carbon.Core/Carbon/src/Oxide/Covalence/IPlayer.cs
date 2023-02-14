using System;
using System.Globalization;
using Oxide.Core;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Game.Rust.Libraries.Covalence
{
	public struct RustPlayer : IPlayer
	{
		public object Object { get; set; }

		public BasePlayer BasePlayer => Object as BasePlayer;

		public RustPlayer(BasePlayer player)
		{
			Object = player;
			Id = player.UserIDString;
			Name = player.displayName.Sanitize();
			LastCommand = 0;
			perms = Interface.Oxide.GetLibrary<Permission>();
		}

		private static Permission perms;

		public CommandType LastCommand { get; set; }

		public string Name { get; set; }

		public string Id { get; set; }

		public string Address => BasePlayer?.Connection?.ipaddress;

		public int Ping => BasePlayer == null ? 0 : Network.Net.sv.GetAveragePing(BasePlayer.Connection);

		public CultureInfo Language => CultureInfo.GetCultureInfo(BasePlayer.net.connection.info.GetString("global.language", "") ?? "en");

		public bool IsConnected => BasePlayer.IsConnected;

		public bool IsSleeping => BasePlayer.IsSleeping();

		public bool IsServer => true;

		public bool IsAdmin => !ulong.TryParse(Id, out var id) ? false : ServerUsers.Is(id, ServerUsers.UserGroup.Owner);

		public bool IsBanned => !ulong.TryParse(Id, out var id) ? false : ServerUsers.Is(id, ServerUsers.UserGroup.Banned);

		public TimeSpan BanTimeRemaining
		{
			get
			{
				if (!IsBanned)
				{
					return TimeSpan.Zero;
				}

				return TimeSpan.MaxValue;
			}
		}

		public float Health
		{
			get
			{
				return BasePlayer.health;
			}
			set
			{
				BasePlayer.health = value;
			}
		}

		public float MaxHealth
		{
			get
			{
				return BasePlayer.MaxHealth();
			}
			set
			{
				BasePlayer.SetMaxHealth(value);
			}
		}

		public void AddToGroup(string group)
		{
			if (!perms.GroupExists(group))
			{
				return;
			}
			if (!perms.GetUserData(Id).Groups.Add(group))
			{
				return;
			}

			Interface.Call("OnUserGroupAdded", Id, group);
		}

		public void Ban(string reason, TimeSpan duration = default)
		{
			if (IsBanned)
			{
				return;
			}

			ServerUsers.Set(BasePlayer.userID, ServerUsers.UserGroup.Banned, ((BasePlayer != null) ? BasePlayer.displayName : null) ?? "Unknown", reason, -1L);
			ServerUsers.Save();

			if (IsConnected)
			{
				Kick(reason);
			}
		}

		public bool BelongsToGroup(string group)
		{
			return perms.UserHasGroup(Id, group);
		}

		public void Command(string command, params object[] args)
		{
			BasePlayer.SendConsoleCommand(command, args);
		}

		public void GrantPermission(string perm)
		{
			perms.GrantUserPermission(Id, perm, null);
		}

		public bool HasPermission(string perm)
		{
			return perms.UserHasPermission(Id, perm);
		}

		public void Heal(float amount)
		{
			BasePlayer.Heal(amount);
		}

		public void Hurt(float amount)
		{
			BasePlayer.Hurt(amount);
		}

		public void Kick(string reason)
		{
			BasePlayer.Kick(reason);
		}

		public void Kill()
		{
			BasePlayer.Die(null);
		}

		public void Message(string message, string prefix, params object[] args)
		{
			if (string.IsNullOrEmpty(message))
			{
				return;
			}

			message = ((args.Length != 0) ? string.Format(Formatter.ToUnity(message), args) : Formatter.ToUnity(message));
			var text = (prefix != null) ? (prefix + " " + message) : message;
			BasePlayer.SendConsoleCommand("chat.add", 2, Id, text);
		}

		public void Message(string message)
		{
			Message(message, null, Array.Empty<string>());
		}

		public void Position(out float x, out float y, out float z)
		{
			var vector = BasePlayer.transform.position;
			x = vector.x;
			y = vector.y;
			z = vector.z;
		}

		public GenericPosition Position()
		{
			var position = BasePlayer.transform.position;
			return new GenericPosition(position.x, position.y, position.z);
		}

		public void RemoveFromGroup(string name)
		{
			if (!perms.GroupExists(name))
			{
				return;
			}

			var userData = perms.GetUserData(Id);
			if (name.Equals("*"))
			{
				if (userData.Groups.Count <= 0)
				{
					return;
				}
				userData.Groups.Clear();
				return;
			}
			else
			{
				if (!userData.Groups.Remove(name))
				{
					return;
				}

				Interface.Call("OnUserGroupRemoved", Id, name);
				return;
			}
		}

		public void Rename(string name)
		{
			name = (string.IsNullOrEmpty(name.Trim()) ? BasePlayer.displayName : name);
			SingletonComponent<ServerMgr>.Instance.persistance.SetPlayerName(BasePlayer.userID, name);
			BasePlayer.net.connection.username = name;
			BasePlayer.displayName = name;
			BasePlayer._name = name;
			BasePlayer.SendNetworkUpdateImmediate(false);
			var iPlayer = BasePlayer.AsIPlayer();
			iPlayer.Name = name;
			perms.UpdateNickname(BasePlayer.UserIDString, name);
			var position = BasePlayer.transform.position;
			Teleport(position.x, position.y, position.z);
		}

		public void Reply(string message, string prefix, params object[] args)
		{
			Message(message, prefix, args);
		}

		public void Reply(string message)
		{
			Message(message);
		}

		public void RevokePermission(string permission)
		{
			if (string.IsNullOrEmpty(permission))
			{
				return;
			}
			var userData = perms.GetUserData(Id);
			if (permission.EndsWith("*"))
			{
				if (!permission.Equals("*"))
				{
					userData.Perms.RemoveWhere((string p) => p.StartsWith(permission.TrimEnd('*'), StringComparison.OrdinalIgnoreCase));
					return;
				}
				if (userData.Perms.Count <= 0)
				{
					return;
				}
				userData.Perms.Clear();
				return;
			}
			else
			{
				if (!userData.Perms.Remove(permission))
				{
					return;
				}

				Interface.Call("OnUserPermissionRevoked", Id, permission);
				return;
			}
		}

		public void Teleport(float x, float y, float z)
		{
			if (BasePlayer.IsAlive() && !BasePlayer.IsSpectating())
			{
				try
				{
					var position = new Vector3(x, y, z);

					BasePlayer.EnsureDismounted();
					BasePlayer.SetParent(null, true, true);
					BasePlayer.SetServerFall(true);
					BasePlayer.MovePosition(position);
					BasePlayer.ClientRPCPlayer<Vector3>(null, BasePlayer, "ForcePositionTo", position);
				}
				finally
				{
					BasePlayer.SetServerFall(false);
				}
			}
		}

		public void Unban()
		{
			if (!IsBanned)
			{
				return;
			}
			ServerUsers.Remove(BasePlayer.userID);
			ServerUsers.Save();
		}
	}

	public struct RustConsolePlayer : IPlayer
	{
		public object Object => null;

		public CommandType LastCommand { get => CommandType.Console; set { } }

		public string Name { get => "Server Console"; set { } }
		public string Id => "server_console";

		public CultureInfo Language => CultureInfo.InstalledUICulture;
		public string Address => "127.0.0.1";

		public int Ping => 0;
		public bool IsAdmin => true;
		public bool IsBanned => false;
		public bool IsConnected => true;
		public bool IsSleeping => false;
		public bool IsServer => true;

		public void Ban(string reason, TimeSpan duration) { }
		public TimeSpan BanTimeRemaining => TimeSpan.Zero;

		public void Heal(float amount) { }
		public float Health { get; set; }
		public void Hurt(float amount) { }

		public void Kick(string reason) { }
		public void Kill() { }

		public float MaxHealth { get; set; }

		public void Rename(string name) { }

		public void Teleport(float x, float y, float z) { }

		public void Teleport(GenericPosition pos)
		{
			Teleport(pos.X, pos.Y, pos.Z);
		}

		public void Unban() { }
		public void Position(out float x, out float y, out float z)
		{
			x = 0f;
			y = 0f;
			z = 0f;
		}

		public GenericPosition Position()
		{
			return new GenericPosition(0f, 0f, 0f);
		}

		public void Message(string message, string prefix, params object[] args)
		{
			message = (args.Length != 0) ? string.Format(message, args) : message;
			var format = (prefix != null) ? (prefix + " " + message) : message;
			Message(format);
		}
		public void Message(string message)
		{
			Message(message, null, null);
		}
		public void Reply(string message, string prefix, params object[] args)
		{
			Message(message, prefix, args);
		}
		public void Reply(string message)
		{
			Message(message, null, Array.Empty<object>());
		}
		public void Command(string command, params object[] args)
		{
			ConsoleSystem.Run(ConsoleSystem.Option.Server, command, args);
		}
		public bool HasPermission(string perm)
		{
			return true;
		}
		public void GrantPermission(string perm) { }
		public void RevokePermission(string perm) { }
		public bool BelongsToGroup(string group)
		{
			return false;
		}
		public void AddToGroup(string group) { }
		public void RemoveFromGroup(string group) { }
	}
}
