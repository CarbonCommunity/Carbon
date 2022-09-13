using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseEntity/RPC_Server/IsVisible ), "Test" )]
    public class OnEntityVisibilityCheck
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityVisibilityCheck" );
        }
    }
}