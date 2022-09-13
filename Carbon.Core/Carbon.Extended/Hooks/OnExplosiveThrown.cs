using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ThrownWeapon ), "DoThrow" )]
    public class OnExplosiveThrown
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnExplosiveThrown" );
        }
    }
}