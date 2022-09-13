using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BasePlayer ), "ShouldDropActiveItem" )]
    public class CanDropActiveItem
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanDropActiveItem" );
        }
    }
}