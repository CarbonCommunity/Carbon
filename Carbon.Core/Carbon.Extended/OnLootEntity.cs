using Carbon.Core;
using Carbon.Core.Harmony;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerLoot ), "StartLootingEntity" )]
    public class PlayerLoot_StartLootingEntity
    {
        public static void Postfix ( BaseEntity targetEntity, bool doPositionChecks, ref bool __result, ref PlayerLoot __instance )
        {
            CarbonCore.Log ( $"OnLootEntity: {targetEntity}" );
            HookExecutor.CallStaticHook ( "OnLootEntity", __instance.GetComponent<BasePlayer>(), targetEntity );
        }
    }
}
