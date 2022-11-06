///
/// Copyright (c) 2022 Carbon Community 
/// All rights reserved
/// 

using Oxide.Core.Libraries.Covalence;

public static class CovalenceEx
{
	public static RustPlayer AsIPlayer(this BasePlayer player)
	{
		var iplayer = default(RustPlayer);
		iplayer.Object = player;

		return iplayer;
	}
}
