using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseMelee ), "DoAttackShared" )]
    public class OnPlayerAttack [melee, patch]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerAttack [melee, patch]" );
        }
    }
}