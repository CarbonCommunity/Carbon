using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( SpinnerWheel ), "RPC_Spin" )]
    public class OnSpinWheel
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnSpinWheel" );
        }
    }
}