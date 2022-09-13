using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PatrolHelicopterAI ), "CanUseNapalm" )]
    public class CanHelicopterUseNapalm
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanHelicopterUseNapalm" );
        }
    }
}