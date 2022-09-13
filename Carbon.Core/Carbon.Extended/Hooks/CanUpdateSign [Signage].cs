using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Signage ), "CanUpdateSign" )]
    public class CanUpdateSign [Signage]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanUpdateSign [Signage]" );
        }
    }
}