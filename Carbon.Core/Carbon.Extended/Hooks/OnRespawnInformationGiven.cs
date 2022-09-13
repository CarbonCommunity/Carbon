using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "SendRespawnOptions" )]
    public class OnRespawnInformationGiven
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnRespawnInformationGiven" );
        }
    }
}