using API.Hooks;

/*
 *
 * Copyright (c) 2022-2023 Carbon Community 
 * All rights reserved.
 *
 */

namespace Carbon.Hooks;
#pragma warning disable IDE0051

public partial class Category_Resources
{
	public partial class Resources_GrowableEntity
	{
		[HookAttribute.Patch("OnGrowableUpdate", "OnGrowableUpdate", typeof(GrowableEntity), "RunUpdate", new System.Type[] { })]
		[HookAttribute.Identifier("0122c151c6dd4865a324d79d05155a57")]

		[MetadataAttribute.Parameter("growable", typeof(GrowableEntity))]
		[MetadataAttribute.Info("Called right before the growable entity is updated.")]

		public class Resources_GrowableEntity_0122c151c6dd4865a324d79d05155a57 : Patch
		{
			public static void Prefix(ref GrowableEntity __instance)
				=> HookCaller.CallStaticHook("OnGrowableUpdate", __instance);
		}
	}
}
