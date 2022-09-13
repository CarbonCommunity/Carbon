using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( TriggerBase ), "OnEntityEnter" )]
    public class OnEntityEnter
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityEnter" );
        }
    }
}