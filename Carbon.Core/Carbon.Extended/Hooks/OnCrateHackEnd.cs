using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HackableLockedCrate ), "HackProgress" )]
    public class OnCrateHackEnd
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnCrateHackEnd" );
        }
    }
}