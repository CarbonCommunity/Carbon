using API.Hooks;
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
	public partial class Fixes_Recycler
	{
		[HookAttribute.Patch("IRecyclerThinkSpeed", "IRecyclerThinkSpeed", typeof(Recycler), "StartRecycling", new System.Type[] { })]
		[HookAttribute.Identifier("a07a3f25546d49d2b140d6cbf6453aa0")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class Fixes_Recycler_StartRecycling_a07a3f25546d49d2b140d6cbf6453aa0 : Patch
		{
			private static bool Prefix(Recycler __instance)
			{
				var hook = HookCaller.CallStaticHook(880503512, __instance);

				if (hook is float value)
				{
					if (__instance.IsOn())
					{
						return false;
					}

					__instance.InvokeRepeating(__instance.RecycleThink, value, value);
					Effect.server.Run(__instance.startSound.resourcePath, __instance, 0U, Vector3.zero, Vector3.zero, null, false);
					__instance.SetFlag(BaseEntity.Flags.On, true, false, true);
					__instance.SendNetworkUpdateImmediate(false);
					return false;
				}

				return true;
			}
		}
	}
}
