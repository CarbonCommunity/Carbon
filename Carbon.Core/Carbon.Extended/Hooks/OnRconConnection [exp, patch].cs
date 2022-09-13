using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Facepunch.RCon/RConListener ), "ProcessConnections" )]
    public class OnRconConnection [exp, patch]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnRconConnection [exp, patch]" );
        }
    }
}