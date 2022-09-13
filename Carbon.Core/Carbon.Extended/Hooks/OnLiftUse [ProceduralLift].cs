using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( ProceduralLift ), "RPC_UseLift" )]
    public class OnLiftUse [ProceduralLift]
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnLiftUse [ProceduralLift]" );
        }
    }
}