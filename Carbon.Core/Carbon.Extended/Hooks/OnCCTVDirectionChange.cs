using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CCTV_RC ), "Server_SetDir" )]
    public class OnCCTVDirectionChange
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnCCTVDirectionChange" );
        }
    }
}