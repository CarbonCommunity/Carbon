using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( DroppedItem ), "OnDroppedOn" )]
    public class CanCombineDroppedItem
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanCombineDroppedItem" );
        }
    }
}