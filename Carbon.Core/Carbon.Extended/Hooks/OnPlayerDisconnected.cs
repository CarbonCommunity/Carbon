using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ServerMgr ), "OnDisconnected" )]
    public class OnPlayerDisconnected
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerDisconnected" );
        }
    }
}