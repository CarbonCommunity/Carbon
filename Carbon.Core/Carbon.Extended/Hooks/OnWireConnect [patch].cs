using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( WireTool ), "MakeConnection" )]
    public class OnWireConnect [patch]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnWireConnect [patch]" );
        }
    }
}