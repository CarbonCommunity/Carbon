using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "ApplyFallDamageFromVelocity" )]
    public class OnPlayerLand
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerLand" );
        }
    }
}