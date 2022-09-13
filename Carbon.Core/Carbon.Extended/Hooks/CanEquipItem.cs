using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerInventory ), "CanEquipItem" )]
    public class CanEquipItem
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanEquipItem" );
        }
    }
}