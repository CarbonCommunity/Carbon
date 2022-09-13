using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemCrafter ), "CancelTask" )]
    public class OnItemCraftCancelled
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnItemCraftCancelled" );
        }
    }
}