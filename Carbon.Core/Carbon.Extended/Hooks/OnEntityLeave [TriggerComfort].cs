using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( TriggerComfort ), "OnEntityLeave" )]
    public class OnEntityLeave [TriggerComfort]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityLeave [TriggerComfort]" );
        }
    }
}