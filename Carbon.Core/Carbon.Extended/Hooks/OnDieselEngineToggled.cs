using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( DieselEngine ), "EngineOn" )]
    public class OnDieselEngineToggled
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnDieselEngineToggled" );
        }
    }
}