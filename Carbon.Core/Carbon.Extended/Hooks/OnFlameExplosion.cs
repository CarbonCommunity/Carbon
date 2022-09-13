using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( FlameExplosive ), "FlameExplode" )]
    public class OnFlameExplosion
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnFlameExplosion" );
        }
    }
}