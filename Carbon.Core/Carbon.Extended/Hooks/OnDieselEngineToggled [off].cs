using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( DieselEngine ), "EngineOff" )]
    public class OnDieselEngineToggled [off]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnDieselEngineToggled [off]" );
        }
    }
}