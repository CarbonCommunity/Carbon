using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "StartSpectating" )]
    public class OnPlayerSpectate
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerSpectate" );
        }
    }
}