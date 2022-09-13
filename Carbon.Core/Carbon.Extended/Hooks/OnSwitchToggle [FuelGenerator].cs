using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( FuelGenerator ), "RPC_EngineSwitch" )]
    public class OnSwitchToggle [FuelGenerator]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnSwitchToggle [FuelGenerator]" );
        }
    }
}