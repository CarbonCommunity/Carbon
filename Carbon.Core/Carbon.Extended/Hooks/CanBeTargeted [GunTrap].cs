using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( GunTrap ), "CheckTrigger" )]
    public class CanBeTargeted [GunTrap]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanBeTargeted [GunTrap]" );
        }
    }
}