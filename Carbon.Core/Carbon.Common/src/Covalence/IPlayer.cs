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
	// --- Attributes ----------------------------------------------------------
	bool IsAdmin { get; }
	bool IsBanned { get; }
	bool IsConnected { get; }
	bool IsServer { get; }
	bool IsSleeping { get; }
	CommandType LastCommand { get; set; }
	CultureInfo Language { get; }
	float Health { get; set; }
	float MaxHealth { get; set; }
	int Ping { get; }
	object Object { get; }
	string Address { get; }
	string Id { get; }
	string Name { get; set; }
	TimeSpan BanTimeRemaining { get; }

	// --- Methods -------------------------------------------------------------
	bool BelongsToGroup(string group);
	bool HasPermission(string perm);
	GenericPosition Position();
	void AddToGroup(string group);
	void Ban(string reason, TimeSpan duration = default);
	void Command(string command, params object[] args);
	void GrantPermission(string perm);
	void Heal(float amount);
	void Hurt(float amount);
	void Kick(string reason);
	void Kill();
	void Message(string message, string prefix, params object[] args);
	void Message(string message);
	void Position(out float x, out float y, out float z);
	void RemoveFromGroup(string group);
	void Rename(string name);
	void Reply(string message, string prefix, params object[] args);
	void Reply(string message);
	void RevokePermission(string perm);
	void Teleport(float x, float y, float z);
	void Teleport(GenericPosition position);
	void Unban();
}
