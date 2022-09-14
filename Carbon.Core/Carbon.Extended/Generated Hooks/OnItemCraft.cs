using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemCrafter ), "CraftItem" )]
    public class OnItemCraft
    {
        public static bool Postfix ( ItemBlueprint bp, BasePlayer owner, ProtoBuf.Item.InstanceData instanceData, System.Int32 amount, System.Int32 skinID, Item fromTempBlueprint, System.Boolean free, ref System.Boolean __result )
        {
            CarbonCore.Log ( $"Postfix OnItemCraft" );

            var result = HookExecutor.CallStaticHook ( "OnItemCraft", l0, a1, a5 );
            
            if ( result != null )
            {
                __result = ( System.Boolean ) result;
                return false;
            }

            return true;
        }
    }
}