using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingBlock ), "DoUpgradeToGrade" )]
    public class OnStructureUpgrade
    {
        public static void Postfix ( BaseEntity.RPCMessage msg , ref BuildingBlock __instance )
        {
            HookExecutor.CallStaticHook ( "OnStructureUpgrade" );
        }
    }
}