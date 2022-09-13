using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ItemCrafter ), "CanCraft" )]
    public class CanCraft [ItemCrafter]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanCraft [ItemCrafter]" );
        }
    }
}