using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseProjectile ), "SwitchAmmoTo" )]
    public class OnAmmoSwitch
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnAmmoSwitch" );
        }
    }
}