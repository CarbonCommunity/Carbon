using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SamSite ), "ToggleDefenderMode" )]
    public class OnSamSiteModeToggle
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnSamSiteModeToggle" );
        }
    }
}