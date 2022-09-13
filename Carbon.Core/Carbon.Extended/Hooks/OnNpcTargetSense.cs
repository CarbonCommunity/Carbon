using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Rust.AI.SimpleAIMemory ), "SetKnown" )]
    public class OnNpcTargetSense
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnNpcTargetSense" );
        }
    }
}