using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RFManager ), "AddBroadcaster" )]
    public class OnRfBroadcasterAdd
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnRfBroadcasterAdd" );
        }
    }
}