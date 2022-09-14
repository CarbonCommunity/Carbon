using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Hammer ), "DoAttackShared" )]
    public class OnHammerHit
    {
        public static void Postfix ( HitInfo info )
        {
            HookExecutor.CallStaticHook ( "OnHammerHit" );
        }
    }
}