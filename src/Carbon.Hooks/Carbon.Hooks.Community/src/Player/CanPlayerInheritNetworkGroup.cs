using API.Hooks;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Player
{
	public partial class BasePlayer_Player
	{
		[HookAttribute.Patch("CanPlayerInheritNetworkGroup", "CanPlayerInheritNetworkGroup", typeof(BasePlayer), "ShouldInheritNetworkGroup", new System.Type[] { })]

		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Info("Overrides the IsSpectating check, overriding the result.")]
		[MetadataAttribute.Return(typeof(bool))]

		public class CanPlayerInheritNetworkGroup : Patch
		{
			public static bool Prefix(ref BasePlayer __instance, ref bool __result)
			{
				if (HookCaller.CallStaticHook(617273774, __instance) is not bool hookValue) return true;
				__result = hookValue;
				return false;

			}
		}
	}
}
