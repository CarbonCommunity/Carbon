using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SamSite ), "WeaponTick" )]
    public class CanSamSiteShoot
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanSamSiteShoot" );
        }
    }
}