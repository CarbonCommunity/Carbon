using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerInventory ), "CanWearItem" )]
    public class CanWearItem
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "CanWearItem" );
        }
    }
}