using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingPrivlidge ), "AddSelfAuthorize" )]
    public class OnCupboardAuthorize [BuildingPrivlidge]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnCupboardAuthorize [BuildingPrivlidge]" );
        }
    }
}