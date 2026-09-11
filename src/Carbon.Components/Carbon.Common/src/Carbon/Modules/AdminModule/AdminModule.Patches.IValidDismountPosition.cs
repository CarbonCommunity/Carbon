#if !MINIMAL

using API.Hooks;

namespace Carbon.Modules;

public partial class AdminModule
{
	[HookAttribute.Patch("IValidDismountPosition", "IValidDismountPosition", typeof(BaseMountable), "ValidDismountPosition", new System.Type[] { typeof(BasePlayer), typeof(Vector3) })]
	[HookAttribute.Options(HookFlags.Hidden)]

	public class BaseMountable_ValidDismountPosition : API.Hooks.Patch
	{
		public static bool Prefix(BasePlayer player, Vector3 disPos, BaseMountable __instance, ref bool __result)
		{
			switch (__instance.skinID)
			{
				case 69696:
					__result = true;
					return false;
				default:
					return true;
			}
		}
	}
}

#endif
