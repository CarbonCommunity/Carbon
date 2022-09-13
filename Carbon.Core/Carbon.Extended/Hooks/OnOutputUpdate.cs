using Carbon.Core;
using Harmony;

namespace Carbon.Extended
{
    [HarmonyPatch ( typeof ( IOEntity ), "UpdateOutputs" )]
    public class OnOutputUpdate
    {
        public static void Prefix ()
        {
            HookExecutor.CallStaticHook ( "OnOutputUpdate" );
        }
    }
}