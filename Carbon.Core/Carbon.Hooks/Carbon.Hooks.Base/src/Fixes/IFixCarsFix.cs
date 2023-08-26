using System.Collections.Generic;
using System.Linq;
using API.Hooks;
using Facepunch;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Fixes
{
	public partial class Fixes_FixCars
	{
		[HookAttribute.Patch("IFixCarsFix", "IFixCarsFix", typeof(ConVar.vehicle), "fixcars", new System.Type[] { typeof(ConsoleSystem.Arg) })]
		[HookAttribute.Identifier("b0136143c913407191e74f71dae7b675")]
		[HookAttribute.Options(HookFlags.Hidden | HookFlags.Static | HookFlags.IgnoreChecksum)]

		public class Fixes_ItemContainer_b0136143c913407191e74f71dae7b675 : Patch
		{
			public static bool Prefix(ConsoleSystem.Arg arg)
			{
				BasePlayer player = arg.Player();

				if (player == null)
				{
					arg.ReplyWith("Null player.");
					return false;
				}

				if (!player.IsAdmin)
				{
					arg.ReplyWith("Must be an admin to use fixcars.");
					return false;
				}

				var tier = Mathf.Clamp(arg.GetInt(0, 2), 1, 3);
				var count = 0;

				var entities = Pool.GetList<BaseEntity>();
				Vis.Entities(player.transform.position, 10f, entities);

				foreach(var entity in entities.Distinct())
				{
					switch (entity)
					{
						case BaseVehicle vehicle:
							vehicle.AdminFixUp(tier);
							count++;
							break;

						case MLRS mlrs:
							mlrs.AdminFixUp();
							count++;
							break;
					}
				}

				Pool.FreeList(ref entities);

				arg.ReplyWith($"Fixed up {count} vehicles.");

				return false;
			}
		}
	}
}
