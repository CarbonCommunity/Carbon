using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( DieselEngine ), "EngineOn" )]
    public class OnDieselEngineToggled [on]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnDieselEngineToggled [on]" );
        }
    }
}