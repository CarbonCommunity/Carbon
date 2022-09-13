using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CH47HelicopterAIController ), "CanDropCrate" )]
    public class CanHelicopterDropCrate
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanHelicopterDropCrate" );
        }
    }
}