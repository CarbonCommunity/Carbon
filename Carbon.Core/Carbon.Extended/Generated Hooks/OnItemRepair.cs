using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RepairBench ), "RepairAnItem" )]
    public class OnItemRepair
    {
        public static void Postfix ( Item itemToRepair, BasePlayer player, BaseEntity repairBenchEntity, System.Single maxConditionLostOnRepair, System.Boolean mustKnowBlueprint )
        {
            HookExecutor.CallStaticHook ( "OnItemRepair" );
        }
    }
}