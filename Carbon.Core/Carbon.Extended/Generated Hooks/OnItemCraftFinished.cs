using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemCrafter ), "FinishCrafting" )]
    public class OnItemCraftFinished
    {
        public static void Postfix ( ItemCraftTask task )
        {
            HookExecutor.CallStaticHook ( "OnItemCraftFinished" );
        }
    }
}