using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( DroppedItem ), "OnDroppedOn" )]
    public class CanCombineDroppedItem [patch 1]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanCombineDroppedItem [patch 1]" );
        }
    }
}