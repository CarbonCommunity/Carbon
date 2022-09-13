using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( RFManager ), "AddListener" )]
    public class OnRfListenerAdd
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnRfListenerAdd" );
        }
    }
}