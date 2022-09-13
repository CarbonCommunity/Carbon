using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ElectricSwitch ), "SVSwitch" )]
    public class OnSwitchToggle [ElectricSwitch]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnSwitchToggle [ElectricSwitch]" );
        }
    }
}