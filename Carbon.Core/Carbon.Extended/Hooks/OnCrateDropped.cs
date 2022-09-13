using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HackableLockedCrate ), "SetWasDropped" )]
    public class OnCrateDropped
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnCrateDropped" );
        }
    }
}