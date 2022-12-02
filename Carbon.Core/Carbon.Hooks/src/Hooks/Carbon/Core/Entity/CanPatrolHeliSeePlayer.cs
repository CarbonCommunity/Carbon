using Oxide.Core;

/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Entity
{
	public partial class Entity_PatrolHelicopterAI
	{
		/*
		[CarbonHook("CanPatrolHeliSeePlayer", typeof(bool)), CarbonHook.Category(Hook.Category.Enum.Entity)]
		[CarbonHook.Parameter("this", typeof(PatrolHelicopterAI))]
		[CarbonHook.Parameter("player", typeof(BasePlayer))]
		[CarbonHook.Info("Can the Patrol Helicopter see the player.")]
		[CarbonHook.Patch(typeof(PatrolHelicopterAI), "PlayerVisible")]
		*/

		public class Entity_PatrolHelicopterAI_PlayerVisible_d3849557c0c84c4c9248b5252f4c7db2
		{
			public static Metadata metadata = new Metadata("CanPatrolHeliSeePlayer",
				typeof(PatrolHelicopterAI), "PlayerVisible", new System.Type[] { typeof(BasePlayer) });

			static Entity_PatrolHelicopterAI_PlayerVisible_d3849557c0c84c4c9248b5252f4c7db2()
			{
				metadata.SetIdentifier("d3849557c0c84c4c9248b5252f4c7db2");
			}

			public static bool Prefix(BasePlayer ply, ref PatrolHelicopterAI __instance, out bool __result)
			{
				var obj = Interface.CallHook("CanPatrolHeliSeePlayer", __instance, ply);
				if (obj is bool)
				{
					__result = (bool)obj;
					return false;
				}

				__result = default;
				return true;
			}
		}
	}
}
