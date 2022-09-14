using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( AutoTurret ), "SetTarget" )]
    public class OnTurretTarget
    {
        public static bool Prefix ( BaseCombatEntity targ )
        {
            return HookExecutor.CallStaticHook ( "OnTurretTarget" ) == null;
        }
    }
}