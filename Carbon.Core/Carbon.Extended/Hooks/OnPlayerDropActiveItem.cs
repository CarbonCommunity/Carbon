using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerBelt ), "DropActive" )]
    public class OnPlayerDropActiveItem
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnPlayerDropActiveItem" );
        }
    }
}