using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HackableLockedCrate ), "HackProgress" )]
    public class OnCrateHackEnd
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnCrateHackEnd" );
        }
    }
}