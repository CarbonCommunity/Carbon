using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( WireTool ), "RequestClear" )]
    public class OnWireClear
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnWireClear" );
        }
    }
}