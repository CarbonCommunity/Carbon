using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SupplySignal ), "Explode" )]
    public class OnCargoPlaneSignaled
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnCargoPlaneSignaled" );
        }
    }
}