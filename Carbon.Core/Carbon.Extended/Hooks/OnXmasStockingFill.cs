using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Stocking ), "SpawnLoot" )]
    public class OnXmasStockingFill
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnXmasStockingFill" );
        }
    }
}