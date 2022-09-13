using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseProjectile ), "ReloadMagazine" )]
    public class OnMagazineReload
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnMagazineReload" );
        }
    }
}