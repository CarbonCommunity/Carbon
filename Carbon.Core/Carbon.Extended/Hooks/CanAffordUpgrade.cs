using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BuildingBlock ), "CanAffordUpgrade" )]
    public class CanAffordUpgrade
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanAffordUpgrade" );
        }
    }
}