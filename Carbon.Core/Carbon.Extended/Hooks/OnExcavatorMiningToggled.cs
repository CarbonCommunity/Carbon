using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ExcavatorArm ), "StopMining" )]
    public class OnExcavatorMiningToggled
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnExcavatorMiningToggled" );
        }
    }
}