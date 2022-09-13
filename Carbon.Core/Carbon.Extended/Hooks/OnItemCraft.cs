using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemCrafter ), "CraftItem" )]
    public class OnItemCraft
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnItemCraft" );
        }
    }
}