using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( TriggeredEventPrefab ), "RunEvent" )]
    public class OnEventTrigger
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEventTrigger" );
        }
    }
}