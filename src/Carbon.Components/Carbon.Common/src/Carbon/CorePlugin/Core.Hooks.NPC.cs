using Rust.Ai.Gen2;

namespace Carbon.Core;

#pragma warning disable IDE0051

public partial class CorePlugin
{
	internal static object IOnNpcTarget(object source, BaseEntity target)
	{
		switch (source)
		{
			case SenseComponent sense:
			{
				if (!sense || !target)
				{
					return null;
				}

				var baseEntity = sense.baseEntity;

				if (baseEntity == null)
				{
					return null;
				}

				if (HookCaller.CallStaticHook(1066895325, baseEntity, target) != null)
				{
					return Cache.False;
				}

				return null;
			}

			case WildlifeHazard:
			case BoatAI:
			{
				if (HookCaller.CallStaticHook(1066895325, source, target) != null)
				{
					return Cache.False;
				}

				return null;
			}

			case BaseNpc npc:
			{
				if (HookCaller.CallStaticHook(1066895325, npc, target) != null)
				{
					npc.SetFact(BaseNpc.Facts.HasEnemy, 0);
					npc.SetFact(BaseNpc.Facts.EnemyRange, 3);
					npc.SetFact(BaseNpc.Facts.AfraidRange, 1);
					npc.playerTargetDecisionStartTime = 0f;
					return Cache.DefaultSingle;
				}

				return null;
			}
		}

		return null;
	}
}
