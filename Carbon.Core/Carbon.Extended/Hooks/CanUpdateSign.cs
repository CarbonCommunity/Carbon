using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CarvablePumpkin ), "CanUpdateSign" )]
    public class CanUpdateSign
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanUpdateSign" );
        }
    }
}