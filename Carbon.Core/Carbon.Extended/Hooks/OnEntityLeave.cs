using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( TriggerComfort ), "OnEntityLeave" )]
    public class OnEntityLeave
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityLeave" );
        }
    }
}