using UnityEngine;

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
	public partial class Entity_BaseEntity
	{
		// [HookAttribute.Patch("OnInvalidPositionCheck", "OnInvalidPositionCheck", typeof(ValidBounds), "Test", new System.Type[] { typeof(Vector3) })]
		// [HookAttribute.Identifier("3ced6bf35f8f48a7ab43f972219abbf6")]
		// 
		// [MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		// [MetadataAttribute.Info("Checks if player that connected is Jackie Chan.")]

		// public class Entity_BaseEntity_OnInvalidPosition_3ced6bf35f8f48a7ab43f972219abbf6 : Patch
		// {
		public static bool Prefix(Vector3 vPos, ref bool __result)
		{
			if (HookCaller.CallStaticHook(388536476, vPos) is bool hookValue)
			{
				__result = !hookValue;
				return false;
			}

			return true;
		}
		// }
	}
}
