using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingPrivlidge ), "GetProtectedMinutes" )]
    public class OnCupboardProtectionCalculated
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnCupboardProtectionCalculated" );
        }
    }
}