using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( VendingMachine ), "GiveSoldItem" )]
    public class OnGiveSoldItem
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnGiveSoldItem" );
        }
    }
}