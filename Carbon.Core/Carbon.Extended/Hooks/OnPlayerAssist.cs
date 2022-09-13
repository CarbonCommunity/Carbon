using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "RPC_Assist" )]
    public class OnPlayerAssist
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerAssist" );
        }
    }
}