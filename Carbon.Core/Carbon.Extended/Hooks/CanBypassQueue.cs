using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ConnectionQueue ), "CanJumpQueue" )]
    public class CanBypassQueue
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanBypassQueue" );
        }
    }
}