using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( TriggerComfort ), "OnEntityEnter" )]
    public class OnEntityEnter [TriggerComfort]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityEnter [TriggerComfort]" );
        }
    }
}