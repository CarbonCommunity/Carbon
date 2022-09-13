using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ProceduralLift ), "RPC_UseLift" )]
    public class OnLiftUse
    {
        public static void Postfix ()
        {
            HookExecutor.CallStaticHook ( "OnLiftUse" );
        }
    }
}