using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ConnectionQueue ), "Cycle" )]
    public class OnQueueCycle
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnQueueCycle" );
        }
    }
}