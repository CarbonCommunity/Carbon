using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( HBHFSensor ), "UpdatePassthroughAmount" )]
    public class OnSensorDetect
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnSensorDetect" );
        }
    }
}