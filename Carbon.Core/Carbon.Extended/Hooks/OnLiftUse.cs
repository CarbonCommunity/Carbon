using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( Lift ), "RPC_UseLift" )]
    public class OnLiftUse
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnLiftUse" );
        }
    }
}