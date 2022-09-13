using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Recycler ), "HasRecyclable" )]
    public class CanRecycle
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanRecycle" );
        }
    }
}