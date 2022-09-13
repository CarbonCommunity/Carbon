using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "PlayerInit" )]
    public class IOnPlayerConnected
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "IOnPlayerConnected" );
        }
    }
}