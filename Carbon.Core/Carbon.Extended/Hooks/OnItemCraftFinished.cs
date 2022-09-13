using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemCrafter ), "FinishCrafting" )]
    public class OnItemCraftFinished
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnItemCraftFinished" );
        }
    }
}