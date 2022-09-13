using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( DroppedItem ), "OnCollision" )]
    public class CanCombineDroppedItem [patch 2]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanCombineDroppedItem [patch 2]" );
        }
    }
}