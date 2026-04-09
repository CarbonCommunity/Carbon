using API.Hooks;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Resources
{
	public partial class Resources_GrowableEntity
	{
		[HookAttribute.Patch("OnGrowableUpdate", "OnGrowableUpdate", typeof(GrowableEntity), "RunUpdate", new System.Type[] { })]

		[MetadataAttribute.Parameter("growable", typeof(GrowableEntity))]
		[MetadataAttribute.Info("Called right before the growable entity is updated.")]

		public class OnGrowableUpdate : Patch
		{
			public static void Prefix(ref GrowableEntity __instance)
				=> HookCaller.CallStaticHook(719742115, __instance);
		}
	}
}
