using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "OnProjectileRicochet" )]
    public class OnProjectileRicochet
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnProjectileRicochet" );
        }
    }
}