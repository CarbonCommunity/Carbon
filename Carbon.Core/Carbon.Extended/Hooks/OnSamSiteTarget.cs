using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SamSite ), "TargetScan" )]
    public class OnSamSiteTarget
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnSamSiteTarget" );
        }
    }
}