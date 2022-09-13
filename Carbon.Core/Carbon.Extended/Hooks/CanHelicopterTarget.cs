using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PatrolHelicopterAI ), "PlayerVisible" )]
    public class CanHelicopterTarget
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanHelicopterTarget" );
        }
    }
}