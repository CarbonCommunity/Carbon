using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( EACServer ), "HandleClientUpdate" )]
    public class OnPlayerBanned [EAC]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerBanned [EAC]" );
        }
    }
}