using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseMelee ), "DoAttackShared" )]
    public class OnPlayerAttack [Melee]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerAttack [Melee]" );
        }
    }
}