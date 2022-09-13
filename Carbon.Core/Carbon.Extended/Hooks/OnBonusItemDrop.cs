using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( LootContainer ), "DropBonusItems" )]
    public class OnBonusItemDrop
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnBonusItemDrop" );
        }
    }
}