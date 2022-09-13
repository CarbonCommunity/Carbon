using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ServerMgr ), "OnGiveUserInformation" )]
    public class OnClientAuth
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnClientAuth" );
        }
    }
}