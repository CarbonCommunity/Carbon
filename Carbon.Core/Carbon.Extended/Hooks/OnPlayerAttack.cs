using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "OnProjectileAttack" )]
    public class OnPlayerAttack
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerAttack" );
        }
    }
}