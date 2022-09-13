using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( WireTool ), "CanPlayerUseWires" )]
    public class CanUseWires
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanUseWires" );
        }
    }
}