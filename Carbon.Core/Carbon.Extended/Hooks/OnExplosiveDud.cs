using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( DudTimedExplosive ), "Explode" )]
    public class OnExplosiveDud
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnExplosiveDud" );
        }
    }
}