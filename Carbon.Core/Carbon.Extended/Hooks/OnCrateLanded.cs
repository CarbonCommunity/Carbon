using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HackableLockedCrate ), "LandCheck" )]
    public class OnCrateLanded
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnCrateLanded" );
        }
    }
}