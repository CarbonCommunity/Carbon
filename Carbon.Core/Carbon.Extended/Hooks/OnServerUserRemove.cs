using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ServerUsers ), "Remove" )]
    public class OnServerUserRemove
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnServerUserRemove" );
        }
    }
}