using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CH47HelicopterAIController ), "DropCrate" )]
    public class OnHelicopterDropCrate
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnHelicopterDropCrate" );
        }
    }
}