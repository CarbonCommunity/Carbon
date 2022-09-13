using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ServerMgr ), "Shutdown" )]
    public class IOnServerShutdown
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "IOnServerShutdown" );
        }
    }
}