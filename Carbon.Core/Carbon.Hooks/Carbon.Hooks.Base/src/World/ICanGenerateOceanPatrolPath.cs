using System.Collections.Generic;
using System.Threading;
using API.Hooks;
using UnityEngine;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_World
{
	public partial class World_BaseBoat
	{
		[HookAttribute.Patch("ICanGenerateOceanPatrolPath", "ICanGenerateOceanPatrolPath", typeof(BaseBoat), "GenerateOceanPatrolPath", new System.Type[] { typeof(float), typeof(float) })]
		[HookAttribute.Identifier("402b9bada85a40f1959da145d58a14a5")]
		[HookAttribute.Options(HookFlags.Hidden)]

		public class World_BaseBoat_GenerateOceanPatrolPath_402b9bada85a40f1959da145d58a14a5 : API.Hooks.Patch
		{
			public static bool Prefix(ref List<Vector3> __result)
			{
				var hook = HookCaller.CallStaticHook("ICanGenerateOceanPatrolPath");

				if(hook is List<Vector3> result)
				{
					__result = result;
					return false;
				}

				return true;
			}
		}
	}
}
