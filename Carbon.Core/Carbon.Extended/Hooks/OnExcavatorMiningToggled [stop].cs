using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ExcavatorArm ), "StopMining" )]
    public class OnExcavatorMiningToggled [stop]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnExcavatorMiningToggled [stop]" );
        }
    }
}