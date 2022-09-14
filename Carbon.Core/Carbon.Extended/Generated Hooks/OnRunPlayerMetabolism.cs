using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerMetabolism ), "RunMetabolism" )]
    public class OnRunPlayerMetabolism
    {
        public static bool Prefix ( BaseCombatEntity ownerEntity, System.Single delta )
        {
            return HookExecutor.CallStaticHook ( "OnRunPlayerMetabolism" ) == null;
        }
    }
}