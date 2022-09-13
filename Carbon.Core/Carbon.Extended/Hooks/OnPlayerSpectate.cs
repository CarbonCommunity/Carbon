using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "StartSpectating" )]
    public class OnPlayerSpectate
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerSpectate" );
        }
    }
}