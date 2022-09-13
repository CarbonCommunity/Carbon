using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SolarPanel ), "SunUpdate" )]
    public class OnSolarPanelSunUpdate
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnSolarPanelSunUpdate" );
        }
    }
}