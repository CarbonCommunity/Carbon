using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( TriggerComfort ), "OnEntityEnter" )]
    public class OnEntityEnter
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityEnter" );
        }
    }
}