using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( PlayerMetabolism ), "RunMetabolism" )]
    public class OnRunPlayerMetabolism
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnRunPlayerMetabolism" );
        }
    }
}