using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Signage ), "UpdateSign" )]
    public class OnSignUpdated [Signage]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnSignUpdated [Signage]" );
        }
    }
}