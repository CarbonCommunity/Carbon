using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PatternFirework ), "ServerSetFireworkDesign" )]
    public class OnFireworkDesignChange
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnFireworkDesignChange" );
        }
    }
}