using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseProjectile ), "UnloadAmmo" )]
    public class OnAmmoUnload
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnAmmoUnload" );
        }
    }
}