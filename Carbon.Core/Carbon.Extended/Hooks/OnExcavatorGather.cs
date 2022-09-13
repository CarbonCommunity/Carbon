using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ExcavatorArm ), "ProduceResources" )]
    public class OnExcavatorGather
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnExcavatorGather" );
        }
    }
}