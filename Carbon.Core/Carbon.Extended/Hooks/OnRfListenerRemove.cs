using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RFManager ), "RemoveListener" )]
    public class OnRfListenerRemove
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnRfListenerRemove" );
        }
    }
}