using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "StartSleeping" )]
    public class OnPlayerSleep
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerSleep" );
        }
    }
}