using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerInventory ), "OnClothingChanged" )]
    public class OnClothingItemChanged
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnClothingItemChanged" );
        }
    }
}