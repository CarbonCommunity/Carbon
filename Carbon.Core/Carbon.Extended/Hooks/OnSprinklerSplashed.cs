using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Sprinkler ), "DoSplash" )]
    public class OnSprinklerSplashed
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnSprinklerSplashed" );
        }
    }
}