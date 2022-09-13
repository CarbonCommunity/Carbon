using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ExcavatorArm ), "BeginMining" )]
    public class OnExcavatorMiningToggled [start]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnExcavatorMiningToggled [start]" );
        }
    }
}