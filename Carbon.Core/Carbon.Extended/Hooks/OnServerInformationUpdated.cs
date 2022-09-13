using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ServerMgr ), "UpdateServerInformation" )]
    public class OnServerInformationUpdated
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnServerInformationUpdated" );
        }
    }
}