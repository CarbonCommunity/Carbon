using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ResourceDispenser ), "AssignFinishBonus" )]
    public class OnDispenserBonus
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnDispenserBonus" );
        }
    }
}