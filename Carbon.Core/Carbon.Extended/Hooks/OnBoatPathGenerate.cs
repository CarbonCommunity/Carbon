using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( BaseBoat ), "GenerateOceanPatrolPath" )]
    public class OnBoatPathGenerate
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnBoatPathGenerate" );
        }
    }
}