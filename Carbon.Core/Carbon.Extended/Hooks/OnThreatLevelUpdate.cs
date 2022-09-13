using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "EnsureUpdated" )]
    public class OnThreatLevelUpdate
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnThreatLevelUpdate" );
        }
    }
}