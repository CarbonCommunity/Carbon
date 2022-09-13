using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CH47HelicopterAIController ), "SetDropDoorOpen" )]
    public class OnHelicopterDropDoorOpen
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnHelicopterDropDoorOpen" );
        }
    }
}