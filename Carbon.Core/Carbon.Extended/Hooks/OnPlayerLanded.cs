using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "ApplyFallDamageFromVelocity" )]
    public class OnPlayerLanded
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerLanded" );
        }
    }
}