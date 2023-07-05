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
	public partial class Entity_BaseNetworkable
	{
		[HookAttribute.Patch("OnTugboatMaxSpeedOverride", "OnTugboatMaxSpeedOverride", typeof(Tugboat), "MaxVelocity", new System.Type[] { })]
		[HookAttribute.Identifier("909d2230cb4d43f1901cd2902feb929e")]

		[MetadataAttribute.Info("Overrides Tugboat default maximum speed (15mp/h).")]
		[MetadataAttribute.Parameter("tugboat", typeof(Tugboat))]
		[MetadataAttribute.Return(typeof(float))]

		public class Entity_BaseNetworkable_909d2230cb4d43f1901cd2902feb929e : Patch
		{
			public static bool Prefix(ref Tugboat __instance, ref float __result)
			{
				if (HookCaller.CallStaticHook(2455781143, __instance) is float value)
				{
					__result = value;
					return false;
				}

				return true;
			}
		}
	}
}
