using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HotAirBalloon ), "EngineSwitch" )]
    public class OnHotAirBalloonToggled
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnHotAirBalloonToggled" );
        }
    }
}