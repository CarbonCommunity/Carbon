using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( WaterBall ), "DoSplash" )]
    public class CanWaterBallSplash
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanWaterBallSplash" );
        }
    }
}