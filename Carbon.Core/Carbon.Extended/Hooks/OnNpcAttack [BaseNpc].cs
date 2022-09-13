using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseNpc ), "StartAttack" )]
    public class OnNpcAttack [BaseNpc]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnNpcAttack [BaseNpc]" );
        }
    }
}