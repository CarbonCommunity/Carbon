using API.Hooks;

namespace Carbon.Hooks;

#pragma warning disable IDE0051

public partial class Category_Entity
{
	public partial class Entity_PatrolHelicopterAI
	{
		[HookAttribute.Patch("CanPatrolHeliSeePlayer", "CanPatrolHeliSeePlayer", typeof(PatrolHelicopterAI), "PlayerVisible", new System.Type[] { typeof(BasePlayer) })]

		[MetadataAttribute.Info("Can the Patrol Helicopter see the player.")]
		[MetadataAttribute.Parameter("heli", typeof(PatrolHelicopterAI))]
		[MetadataAttribute.Parameter("player", typeof(BasePlayer))]
		[MetadataAttribute.Return(typeof(bool))]

		public class CanPatrolHeliSeePlayer : Patch
		{
			public static bool Prefix(BasePlayer ply, ref PatrolHelicopterAI __instance, out bool __result)
			{
				if (HookCaller.CallStaticHook(2827558490, __instance, ply) is bool boolean)
				{
					__result = boolean;
					return false;
				}

				__result = default;
				return true;
			}
		}
	}
}
