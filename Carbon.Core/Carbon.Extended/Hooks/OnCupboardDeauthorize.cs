using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingPrivlidge ), "RemoveSelfAuthorize" )]
    public class OnCupboardDeauthorize
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnCupboardDeauthorize" );
        }
    }
}