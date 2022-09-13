using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ServerMgr ), "RestartServer" )]
    public class OnServerRestartInterrupt
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnServerRestartInterrupt" );
        }
    }
}