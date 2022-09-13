using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseProjectile ), "DelayedModsChanged" )]
    public class OnWeaponModChange
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnWeaponModChange" );
        }
    }
}