using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RFManager ), "RemoveBroadcaster" )]
    public class OnRfBroadcasterRemove
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnRfBroadcasterRemove" );
        }
    }
}