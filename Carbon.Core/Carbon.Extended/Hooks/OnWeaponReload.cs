using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseProjectile ), "StartReload" )]
    public class OnWeaponReload
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnWeaponReload" );
        }
    }
}