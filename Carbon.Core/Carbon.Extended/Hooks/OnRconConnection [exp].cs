using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Facepunch.RCon/RConListener ), "ProcessConnections" )]
    public class OnRconConnection [exp]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnRconConnection [exp]" );
        }
    }
}