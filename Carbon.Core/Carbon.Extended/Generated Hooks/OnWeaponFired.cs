using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseProjectile ), "CLProject" )]
    public class OnWeaponFired
    {
        public static void Postfix ( BaseEntity.RPCMessage msg , ref BaseProjectile __instance )
        {
            HookExecutor.CallStaticHook ( "OnWeaponFired" );
        }
    }
}