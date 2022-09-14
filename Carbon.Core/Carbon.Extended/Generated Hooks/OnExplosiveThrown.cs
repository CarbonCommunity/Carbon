using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ThrownWeapon ), "DoThrow" )]
    public class OnExplosiveThrown
    {
        public static void Postfix ( BaseEntity.RPCMessage msg , ref ThrownWeapon __instance )
        {
            HookExecutor.CallStaticHook ( "OnExplosiveThrown" );
        }
    }
}