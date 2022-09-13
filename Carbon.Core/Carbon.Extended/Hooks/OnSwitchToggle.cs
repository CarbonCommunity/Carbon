using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( FuelGenerator ), "RPC_EngineSwitch" )]
    public class OnSwitchToggle
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnSwitchToggle" );
        }
    }
}