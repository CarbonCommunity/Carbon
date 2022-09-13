using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Signage ), "LockSign" )]
    public class OnSignLocked [Signage]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnSignLocked [Signage]" );
        }
    }
}