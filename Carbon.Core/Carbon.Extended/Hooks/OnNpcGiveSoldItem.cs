using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( NPCVendingMachine ), "GiveSoldItem" )]
    public class OnNpcGiveSoldItem
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnNpcGiveSoldItem" );
        }
    }
}