using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseNpc ), "StartAttack" )]
    public class OnNpcAttack
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnNpcAttack" );
        }
    }
}