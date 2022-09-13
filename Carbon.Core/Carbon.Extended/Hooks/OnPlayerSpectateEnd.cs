using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "StopSpectating" )]
    public class OnPlayerSpectateEnd
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerSpectateEnd" );
        }
    }
}