using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ServerMgr ), "OnValidateAuthTicketResponse" )]
    public class IOnPlayerBanned
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "IOnPlayerBanned" );
        }
    }
}