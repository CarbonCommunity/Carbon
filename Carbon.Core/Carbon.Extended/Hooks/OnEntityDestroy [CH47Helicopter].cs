using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( CH47HelicopterAIController ), "OnKilled" )]
    public class OnEntityDestroy [CH47Helicopter]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnEntityDestroy [CH47Helicopter]" );
        }
    }
}