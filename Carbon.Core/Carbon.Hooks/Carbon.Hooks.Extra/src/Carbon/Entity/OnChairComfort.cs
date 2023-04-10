using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Entity
{
	public partial class BasePlayer_Entity
	{
		[HookAttribute.Patch("OnChairComfort", "OnChairComfort", typeof(BaseChair), "GetComfort", new System.Type[] { })]
		[HookAttribute.Identifier("738d3070f2414abb9edec591cdef6538")]

		[MetadataAttribute.Info("Overrides the amount of comfort chairs give to players.")]
		[MetadataAttribute.Parameter("chair", typeof(BaseChair))]

		public class Entity_BaseChair_738d3070f2414abb9edec591cdef6538 : Patch
		{
			public static bool Prefix(ref BaseChair __instance, ref float __result)
			{
				if (HookCaller.CallStaticHook("OnChairComfort", __instance) is float hookValue)
				{
					__result = hookValue;
					return false;
				}

				return true;
			}
		}
	}
}
