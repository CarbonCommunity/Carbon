using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HelicopterTurret ), "SetTarget" )]
    public class OnHelicopterTarget
    {
        public static bool Prefix ( BaseCombatEntity newTarget )
        {
            return HookExecutor.CallStaticHook ( "OnHelicopterTarget" ) == null;
        }
    }
}