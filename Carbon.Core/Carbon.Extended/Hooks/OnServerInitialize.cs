using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ServerMgr ), "Initialize" )]
    public class OnServerInitialize
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnServerInitialize" );
        }
    }
}