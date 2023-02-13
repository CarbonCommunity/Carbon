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
		var rustPlayer = Permission._iPlayerField.GetValue(player) as RustPlayer;

		if (rustPlayer == null) Permission._iPlayerField.SetValue(player, rustPlayer = new RustPlayer { Object = player });

		return rustPlayer;
	}
}
