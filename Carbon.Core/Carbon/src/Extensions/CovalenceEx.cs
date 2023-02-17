using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Game.Rust.Libraries.Covalence;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

public static class CovalenceEx
{
	public static RustPlayer AsIPlayer(this BasePlayer player)
	{
		if (Permission.iPlayerField.GetValue(player) is not RustPlayer rustPlayer)
			Permission.iPlayerField.SetValue(player, rustPlayer = new RustPlayer(player));

		return rustPlayer;
	}
}
