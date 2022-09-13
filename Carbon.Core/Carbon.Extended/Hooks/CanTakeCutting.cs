using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( GrowableEntity ), "TakeClones" )]
    public class CanTakeCutting
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanTakeCutting" );
        }
    }
}