using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( GestureConfig ), "IsOwnedBy" )]
    public class CanUseGesture
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "CanUseGesture" );
        }
    }
}