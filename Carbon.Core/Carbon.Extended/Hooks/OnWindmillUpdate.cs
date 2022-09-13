using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ElectricWindmill ), "WindUpdate" )]
    public class OnWindmillUpdate
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnWindmillUpdate" );
        }
    }
}