using API.Hooks;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Entity
{
	public partial class BasePlayer_Entity
	{
		[HookAttribute.Patch("OnChairComfort", "OnChairComfort", typeof(BaseChair), "GetComfort", new System.Type[] { })]

		[MetadataAttribute.Info("Overrides the amount of comfort chairs give to players.")]
		[MetadataAttribute.Parameter("chair", typeof(BaseChair))]
		[MetadataAttribute.Return(typeof(float))]

		public class OnChairComfort : Patch
		{
			public static bool Prefix(ref BaseChair __instance, ref float __result)
			{
				if (HookCaller.CallStaticHook(3306666476, __instance) is float hookValue)
				{
					__result = hookValue;
					return false;
				}

				return true;
			}
		}
	}
}
