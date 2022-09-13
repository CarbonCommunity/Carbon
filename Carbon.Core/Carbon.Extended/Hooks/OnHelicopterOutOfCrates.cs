using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CH47HelicopterAIController ), "OutOfCrates" )]
    public class OnHelicopterOutOfCrates
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnHelicopterOutOfCrates" );
        }
    }
}