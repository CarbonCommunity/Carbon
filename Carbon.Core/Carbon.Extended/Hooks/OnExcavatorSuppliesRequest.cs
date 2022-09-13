using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ExcavatorSignalComputer ), "RequestSupplies" )]
    public class OnExcavatorSuppliesRequest
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnExcavatorSuppliesRequest" );
        }
    }
}