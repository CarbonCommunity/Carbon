namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	internal static object IOnNpcTarget(BaseNpc npc, BaseEntity target)
	{
		// OnNpcTarget
		if (HookCaller.CallStaticHook(1066895325, npc, target) == null)
		{
			return null;
		}

		npc.SetFact(BaseNpc.Facts.HasEnemy, 0);
		npc.SetFact(BaseNpc.Facts.EnemyRange, 3);
		npc.SetFact(BaseNpc.Facts.AfraidRange, 1);

		npc.playerTargetDecisionStartTime = 0f;
		return 0f;
	}
}
