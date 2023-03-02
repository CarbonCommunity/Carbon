/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Oxide.Game.Rust;

public class RustCore
{
	public static BasePlayer FindPlayer(string nameOrIdOrIp)
	{
		return BasePlayer.FindAwakeOrSleeping(nameOrIdOrIp);
	}
	public static BasePlayer FindPlayerByName(string name)
	{
		return BasePlayer.FindAwakeOrSleeping(name);
	}
	public static BasePlayer FindPlayerById(ulong id)
	{
		return BasePlayer.FindByID(id);
	}
	public static BasePlayer FindPlayerByIdString(string id)
	{
		return BasePlayer.FindAwakeOrSleeping(id);
	}
}
