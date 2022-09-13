using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HotAirBalloon ), "EngineSwitch" )]
    public class OnHotAirBalloonToggle
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnHotAirBalloonToggle" );
        }
    }
}