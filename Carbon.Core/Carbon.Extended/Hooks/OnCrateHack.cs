using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HackableLockedCrate ), "StartHacking" )]
    public class OnCrateHack
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnCrateHack" );
        }
    }
}