using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "OnProjectileAttack" )]
    public class OnPlayerAttack
    {
        public static void Postfix ( BaseEntity.RPCMessage msg , ref BasePlayer __instance )
        {
            HookExecutor.CallStaticHook ( "OnPlayerAttack" );
        }
    }
}