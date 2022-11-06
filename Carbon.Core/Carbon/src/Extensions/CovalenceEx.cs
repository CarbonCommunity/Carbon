///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core.Libraries.Covalence;

public static class CovalenceEx
{
	public static Player AsIPlayer(this BasePlayer player)
	{
		var iplayer = default(Player);
		iplayer.Object = player;

		return iplayer;
	}
}
