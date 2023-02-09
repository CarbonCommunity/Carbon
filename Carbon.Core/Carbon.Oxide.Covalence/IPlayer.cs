using System;
using System.Globalization;
using Epic.OnlineServices.UI;
using System.Runtime.Serialization;
using Oxide.Core.Libraries.Covalence;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Core.Libraries.Covalence
{
	public interface IPlayer
	{
		object Object { get; }
		CommandType LastCommand { get; set; }
		string Name { get; set; }
		string Id { get; }
		string Address { get; }
		int Ping { get; }
		CultureInfo Language { get; }
		bool IsConnected { get; }
		bool IsSleeping { get; }
		bool IsServer { get; }
		bool IsAdmin { get; }
		bool IsBanned { get; }
		void Ban(string reason, TimeSpan duration = default(TimeSpan));
		TimeSpan BanTimeRemaining { get; }
		void Heal(float amount);
		float Health { get; set; }
		void Hurt(float amount);
		void Kick(string reason);
		void Kill();
		float MaxHealth { get; set; }
		void Rename(string name);
		void Teleport(float x, float y, float z);
		void Unban();
		void Position(out float x, out float y, out float z);
		GenericPosition Position();
		void Message(string message, string prefix, params object[] args);
		void Message(string message);
		void Reply(string message, string prefix, params object[] args);
		void Reply(string message);
		void Command(string command, params object[] args);
		bool HasPermission(string perm);
		void GrantPermission(string perm);
		void RevokePermission(string perm);
		bool BelongsToGroup(string group);
		void AddToGroup(string group);
		void RemoveFromGroup(string group);
	}
}

namespace Oxide.Game.Rust.Libraries.Covalence
{
	public struct RustPlayer : IPlayer
	{
		public object Object { get; set; }

		public BasePlayer BasePlayer => Object as BasePlayer;

		public CommandType LastCommand { get; set; }
		public string Name { get; set; }

		public string Id => BasePlayer?.UserIDString;

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

		}

		public void Ban(string reason, TimeSpan duration = default)
		{

		}

		public bool BelongsToGroup(string group)
		{
			return false;
		}

		public void Command(string command, params object[] args)
		{

		}

		public void GrantPermission(string perm)
		{

		}

		public bool HasPermission(string perm)
		{
			return false;
		}

		public void Heal(float amount)
		{

		}

		public void Hurt(float amount)
		{

		}

		public void Kick(string reason)
		{

		}

		public void Kill()
		{

		}

		public void Message(string message, string prefix, params object[] args)
		{

		}

		public void Message(string message)
		{

		}

		public void Position(out float x, out float y, out float z)
		{
			x = y = z = 0;
		}

		public GenericPosition Position()
		{
			var position = BasePlayer.transform.position;
			return new GenericPosition(position.x, position.y, position.z);
		}

		public void RemoveFromGroup(string group)
		{

		}

		public void Rename(string name)
		{

		}

		public void Reply(string message, string prefix, params object[] args)
		{

		}

		public void Reply(string message)
		{

		}

		public void RevokePermission(string perm)
		{

		}

		public void Teleport(float x, float y, float z)
		{

		}

		public void Unban()
		{

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
