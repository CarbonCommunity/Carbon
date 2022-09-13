using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( WaterPump ), "CreateWater" )]
    public class OnWaterCollect
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnWaterCollect" );
        }
    }
}