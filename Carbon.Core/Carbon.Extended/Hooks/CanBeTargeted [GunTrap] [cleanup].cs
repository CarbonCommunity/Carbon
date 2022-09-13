using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( GunTrap ), "CheckTrigger" )]
    public class CanBeTargeted [GunTrap] [cleanup]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanBeTargeted [GunTrap] [cleanup]" );
        }
    }
}