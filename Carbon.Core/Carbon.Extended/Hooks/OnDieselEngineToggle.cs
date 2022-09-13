using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( DieselEngine ), "EngineSwitch" )]
    public class OnDieselEngineToggle
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnDieselEngineToggle" );
        }
    }
}