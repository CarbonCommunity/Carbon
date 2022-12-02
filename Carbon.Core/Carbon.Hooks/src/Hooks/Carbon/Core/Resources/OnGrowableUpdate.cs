
/*
 *
 * Copyright (c) 2022 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;

public partial class Category_Resources
{
	public partial class Resources_GrowableEntity
	{
		/*
		[CarbonHook("OnGrowableUpdate"), CarbonHook.Category(CarbonHook.Category.Enum.Resources)]
		[CarbonHook.Parameter("this", typeof(GrowableEntity))]
		[CarbonHook.Info("Called before growable entity will be updated.")]
		[CarbonHook.Patch(typeof(GrowableEntity), "RunUpdate")]
		*/

		public class Resources_GrowableEntity_RunUpdate_0122c151c6dd4865a324d79d05155a57
		{
			public static Metadata metadata = new Metadata("OnGrowableUpdate",
				typeof(GrowableEntity), "RunUpdate", new System.Type[] { });

			static Resources_GrowableEntity_RunUpdate_0122c151c6dd4865a324d79d05155a57()
			{
				metadata.SetIdentifier("0122c151c6dd4865a324d79d05155a57");
			}

			public static void Prefix(ref GrowableEntity __instance)
			{
				HookCaller.CallStaticHook("OnGrowableUpdate", __instance);
			}
		}
	}
}
