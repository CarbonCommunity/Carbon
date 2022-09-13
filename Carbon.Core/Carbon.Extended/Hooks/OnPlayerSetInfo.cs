using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "SetInfo" )]
    public class OnPlayerSetInfo
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerSetInfo" );
        }
    }
}