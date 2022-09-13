using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerMetabolism ), "ServerUpdate" )]
    public class OnPlayerMetabolize
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerMetabolize" );
        }
    }
}