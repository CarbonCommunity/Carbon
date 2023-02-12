using System;
using System.Globalization;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Core.Libraries.Covalence;

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
