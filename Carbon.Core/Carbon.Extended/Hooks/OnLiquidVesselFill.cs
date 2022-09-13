using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseLiquidVessel ), "FillCheck" )]
    public class OnLiquidVesselFill
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnLiquidVesselFill" );
        }
    }
}